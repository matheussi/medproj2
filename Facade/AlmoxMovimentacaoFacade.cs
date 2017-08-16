namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public sealed class AlmoxMovimentacaoFacade
    {
        #region Singleton 

        static AlmoxMovimentacaoFacade _instance;
        public static AlmoxMovimentacaoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new AlmoxMovimentacaoFacade(); }
                return _instance;
            }
        }
        #endregion

        private AlmoxMovimentacaoFacade() { }

        public Boolean SalvarSaida(AlmoxMovimentacao movimentacao, ref String msgRetorno)
        {
            movimentacao.TipoDeMovimentacao = (int)AlmoxMovimentacao.TipoMovimentacao.Saida;
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            AlmoxProduto prod = new AlmoxProduto();
            prod.ID = movimentacao.ProdutoID;
            pm.Load(prod);

            AlmoxTipoProduto tipo = new AlmoxTipoProduto();
            tipo.ID = prod.TipoProdutoID;
            pm.Load(tipo);

            //checa se o intervalo a ser retirado é válido
            Boolean result = false;
            if (tipo.Numerado)
            {
                result = AlmoxMovimentacao.ExisteIntervalo(movimentacao.NumeracaoInicial,
                    movimentacao.NumeracaoFinal, prod.OperadoraID, movimentacao.QtdZerosAEsquerda, movimentacao.Letra);

                if (!result)
                {
                    msgRetorno = "Intervalo inválido.";
                    pm.Rollback();
                    pm = null;
                    return false;
                }

                result = AlmoxContratoImpresso.ExisteIntervaloRetirado(movimentacao.NumeracaoInicial,
                movimentacao.NumeracaoFinal, movimentacao.QtdZerosAEsquerda, movimentacao.Letra, prod.OperadoraID, pm);

                if (result)
                {
                    msgRetorno = "Intervalo já retirado do estoque.";
                    pm.Rollback();
                    pm = null;
                    return false;
                }
            }
            else
            {
                //checa se tem em estoque
                if(prod.QTD < movimentacao.QTD)
                {
                    msgRetorno = "Quantidade indisponível no estoque.";
                    pm.Rollback();
                    pm = null;
                    return false;
                }
            }

            try
            {
                pm.Save(movimentacao);

                //altera os dados flutuantes da movimentacao de entrada
                if (movimentacao.MovimentacaoID != null)
                {
                    AlmoxMovimentacao fonte = new AlmoxMovimentacao();
                    fonte.ID = movimentacao.MovimentacaoID;
                    pm.Load(fonte);
                    fonte.QTDFlutuante -= movimentacao.QTD;

                    if(tipo.Numerado)
                        fonte.NumeracaoInicialFlutuante += movimentacao.NumeracaoFinal;
                    else
                        fonte.NumeracaoInicialFlutuante += movimentacao.QTD;

                    pm.Save(fonte);

                    if (movimentacao.SubTipoDeMovimentacao == (int)AlmoxMovimentacao.SubTipoMovimentacao.Normal)
                    {
                        //seta o relacionamento entre corretor e a proposta
                        String numero = "";
                        for (int i = movimentacao.NumeracaoInicial; i <= movimentacao.NumeracaoFinal; i++)
                        {
                            ProdutoCorretor pc = new ProdutoCorretor();
                            pc.AgenteID = movimentacao.UsuarioRetiradaID;
                            pc.ProdutoID = movimentacao.ProdutoID;
                            pc.ProdutoNumero = i;
                            pc.ProdutoQTD = movimentacao.QTD;
                            pc.EntradaID = movimentacao.MovimentacaoID;
                            pm.Save(pc);

                            if (tipo.Numerado)
                            {
                                #region gera numero do contrato impresso

                                //if (movimentacao.QtdZerosAEsquerda > 0)
                                //{
                                //    String mascara = new String('0', movimentacao.QtdZerosAEsquerda);
                                //    numero = String.Format("{0:" + mascara + "}", i);
                                //}
                                //else
                                //    numero = i.ToString();

                                //if (!String.IsNullOrEmpty(movimentacao.Letra))
                                //    numero = movimentacao.Letra + numero;
                                numero = i.ToString();
                                #endregion

                                AlmoxContratoImpresso.SetaProdutor(
                                    movimentacao.UsuarioRetiradaID, movimentacao.ProdutoID, 
                                    numero, movimentacao.Letra, movimentacao.QtdZerosAEsquerda, pm);

                                ContratoStatusHistorico.Salvar(i, movimentacao.QtdZerosAEsquerda,
                                    movimentacao.Letra, prod.OperadoraID, ContratoStatusHistorico.eStatus.ComCorretor, pm);
                            }
                        }
                    }
                }

                //altera a quantidade disponível em estoque
                prod.QTD -= movimentacao.QTD;
                pm.Save(prod);

                pm.Commit();  //commit!
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }

            return true;
        }

        public Boolean SalvarEntrada(AlmoxMovimentacao movimentacao, ref String msgRetorno, Object operadoraId)
        {
            AlmoxProduto prod = new AlmoxProduto();
            prod.ID = movimentacao.ProdutoID;
            prod.Carregar();

            
            AlmoxTipoProduto tipo = new AlmoxTipoProduto();
            tipo.ID = prod.TipoProdutoID;
            tipo.Carregar();

            if ((AlmoxMovimentacao.SubTipoMovimentacao)movimentacao.SubTipoDeMovimentacao ==
                  AlmoxMovimentacao.SubTipoMovimentacao.Normal)
            {
                if (tipo.Numerado)
                {
                    //checa se existe
                    Boolean existe = AlmoxMovimentacao.ExisteIntervalo(
                        movimentacao.NumeracaoInicial, movimentacao.NumeracaoFinal, operadoraId, 
                        movimentacao.QtdZerosAEsquerda, movimentacao.Letra);

                    if (existe)
                    {
                        msgRetorno = "Intervalo de numeração inválido.";
                        return false;
                    }
                }
            }
            else if ((AlmoxMovimentacao.SubTipoMovimentacao)movimentacao.SubTipoDeMovimentacao ==
                  AlmoxMovimentacao.SubTipoMovimentacao.Devolucao)
            {
                //checa se existe
                Boolean existe = AlmoxMovimentacao.ExisteIntervalo(
                    movimentacao.NumeracaoInicial, movimentacao.NumeracaoFinal, operadoraId,
                        movimentacao.QtdZerosAEsquerda, movimentacao.Letra);

                if (!existe)
                {
                    msgRetorno = "Intervalo de numeração inálido.";
                    return false;
                }

                //checa se foi retirado do estoque. se nao foi, nao deixa devolver
                existe = AlmoxMovimentacao.ExisteIntervaloRetirado(movimentacao.NumeracaoInicial, 
                    movimentacao.NumeracaoFinal, prod.ID, movimentacao.QtdZerosAEsquerda, movimentacao.Letra);

                if (!existe)
                {
                    msgRetorno = "Impossível gravar esse intervalo de numeração.<br>Ele está em estoque integral ou parcialmente.";
                    return false;
                }
            }

            movimentacao.TipoDeMovimentacao = (int)AlmoxMovimentacao.TipoMovimentacao.Entrada;
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                pm.Save(movimentacao);

                prod.QTD += movimentacao.QTD;
                pm.Save(prod);

                //cria os contratos impressos
                if ((AlmoxMovimentacao.SubTipoMovimentacao)movimentacao.SubTipoDeMovimentacao ==
                    AlmoxMovimentacao.SubTipoMovimentacao.Normal && tipo.Numerado)
                {
                    for (int i = movimentacao.NumeracaoInicial; i <= movimentacao.NumeracaoFinal; i++)
                    {
                        AlmoxContratoImpresso aci   = new AlmoxContratoImpresso();
                        aci.Numero = i;
                        aci.QtdZerosAEsquerda = movimentacao.QtdZerosAEsquerda;

                        if (!String.IsNullOrEmpty(movimentacao.Letra))
                            aci.Letra = movimentacao.Letra;

                        #region gera numero do contrato impresso 

                        //if (movimentacao.QtdZerosAEsquerda > 0)
                        //{
                            //String mascara = new String('0', movimentacao.QtdZerosAEsquerda);
                        //    aci.Numero = String.Format("{0:" + mascara + "}", i);
                        //}
                        //else
                        //    aci.Numero = i;

                        //if (!String.IsNullOrEmpty(movimentacao.Letra))
                        //    aci.Numero = movimentacao.Letra + aci.Numero;
                        #endregion

                        aci.MovID       = movimentacao.ID;
                        aci.ProdutoID   = movimentacao.ProdutoID;
                        pm.Save(aci);

                        //gera log historico de alteracao de status
                        ContratoStatusHistorico.Salvar(i, movimentacao.QtdZerosAEsquerda, movimentacao.Letra, operadoraId, ContratoStatusHistorico.eStatus.NoEstoque, pm);
                    }
                }
                //remove impresso do corretor
                else if (tipo.Numerado && (AlmoxMovimentacao.SubTipoMovimentacao)movimentacao.SubTipoDeMovimentacao == AlmoxMovimentacao.SubTipoMovimentacao.Devolucao)
                {
                    String numero = "";
                    for (int i = movimentacao.NumeracaoInicial; i <= movimentacao.NumeracaoFinal; i++)
                    {
                        #region gera numero do contrato impresso 

                        //numero = EntityBase.GeraNumeroDeContrato(i, movimentacao.QtdZerosAEsquerda, movimentacao.Letra);
                        ////if (movimentacao.QtdZerosAEsquerda > 0)
                        ////{
                        ////    String mascara = new String('0', movimentacao.QtdZerosAEsquerda);
                        ////    numero = String.Format("{0:" + mascara + "}", i);
                        ////}
                        ////else
                        ////    numero = i.ToString();

                        ////if (!String.IsNullOrEmpty(movimentacao.Letra))
                        ////    numero = movimentacao.Letra + numero;
                        #endregion

                        //checa se foi usando em contrato. se foi, nao deixa devolver
                        if (Contrato.NumeroDeContratoEmUso(i.ToString(), movimentacao.Letra, movimentacao.QtdZerosAEsquerda, operadoraId, pm))
                        {
                            pm.Rollback();
                            msgRetorno = "O contrato de número " + numero + " está em uso."; ;
                            return false;
                        }

                        //remove do produtor
                        AlmoxContratoImpresso.RetiraDeProdutor(movimentacao.ProdutoID, i, movimentacao.Letra, movimentacao.QtdZerosAEsquerda, pm);

                        //gera log historico de alteracao de status
                        ContratoStatusHistorico.Salvar(i, movimentacao.QtdZerosAEsquerda, movimentacao.Letra, operadoraId, ContratoStatusHistorico.eStatus.NoEstoque, pm);
                    }
                }

                if ((AlmoxMovimentacao.SubTipoMovimentacao)movimentacao.SubTipoDeMovimentacao ==
                     AlmoxMovimentacao.SubTipoMovimentacao.Devolucao)
                {
                    //remove do corretor
                    if (tipo.Numerado)
                    {
                        Boolean result = false;
                        for (int i = movimentacao.NumeracaoInicial; i <= movimentacao.NumeracaoFinal; i++)
                        {
                            result = ProdutoCorretor.Remove(
                                movimentacao.UsuarioRetiradaID, prod.ID, i, 0, pm);

                            if (!result)
                            {
                                pm.Rollback();
                                msgRetorno = "Não foi possível efetivar a operação.\\nCertifique-se de ter informados corretamente os parâmetros.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Boolean result = ProdutoCorretor.Remove(
                            movimentacao.UsuarioRetiradaID, prod.ID, 0, movimentacao.QTD, pm);

                        if (!result)
                        {
                            pm.Rollback();
                            msgRetorno = "Não foi possível efetivar a operação.\\nCertifique-se de ter informado corretamente os parâmetros.";
                            return false;
                        }
                    }
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }

            return true;
        }
    }
}
namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public sealed class AtendimentoFacade
    {
        #region Singleton

        static AtendimentoFacade _instance;
        public static AtendimentoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new AtendimentoFacade(); }
                return _instance;
            }
        }
        #endregion

        AtendimentoFacade() { }

        /// <summary>
        /// Altera o status dos atendimentos para "concluído" e efetiva as ações.
        /// </summary>
        /// <param name="atendimentoIDs">Lista de ids de atendimentos a serem processados.</param>
        public void EfetivaAtendimento(List<String> atendimentoItemIDs)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Atendimento atendimento;

            try
            {
                IList<AtendimentoItem> itensAtendimento = AtendimentoItem.CarregaAtendimentoItens(atendimentoItemIDs, pm);
                if (itensAtendimento != null)
                {
                    Object contratoId = null;
                    String[] arrAux = null, arrAux2 = null;
                    ContratoBeneficiario cb = null;

                    foreach (AtendimentoItem item in itensAtendimento)
                    {
                        item.Status = (Int32)Atendimento.eStatus.Concluido; //true;
                        pm.Save(item);

                        atendimento = new Atendimento(item.AtendimentoId);
                        pm.Load(atendimento);

                        if (eTipoAtendimentoItem.SegundaViaCartao == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Marca o beneficiário para segunda via de cartao 

                            arrAux = item.BeneficiarioIds.Split(',');
                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);

                            for (int i = 0; i < arrAux.Length; i++)
                            {
                                //TODO: criar uma sobrecarga que recebe um array de beneficiarioIDs e faz a alteração de forma atomica
                                ContratoBeneficiario.AlteraStatusBeneficiario(contratoId, arrAux[i], ContratoBeneficiario.eStatus.SegundaViaCartaoPendente, pm);
                            }

                            #endregion
                        }
                        if (eTipoAtendimentoItem.CancelamentoContrato == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Marca o contrato para cancelamento  

                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);
                            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contratoId, pm);
                            titular.Status = (Int32)ContratoBeneficiario.eStatus.CancelamentoPendente;
                            pm.Save(titular);

                            #endregion

                            #region Cancela o contrato e adiciona histórico 

                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);
                            Contrato.AlteraStatusDeContrato(contratoId, true, pm);

                            #endregion
                        }
                        else if (eTipoAtendimentoItem.AdicionarBeneficiarios == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Adiciona o beneficiario como ContratoBeneficiario NOVO, calcula o novo valor do contrato, a vigencia dele, e atualiza as cobrancas

                            arrAux = item.BeneficiarioIds.Split(',');
                            arrAux2 = item.ParentescoIds.Split(',');
                            DateTime dataAdmissao = atendimento.DataHora;
                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);

                            for (int i = 0; i < arrAux.Length; i++)
                            {
                                cb = new ContratoBeneficiario();
                                cb.Ativo = true;
                                cb.BeneficiarioID = arrAux[i];// bId;
                                cb.ContratoID = contratoId;
                                cb.Data = dataAdmissao;
                                cb.ParentescoID = arrAux2[i]; //obter info da classe atendimento
                                cb.Tipo = (Int32)ContratoBeneficiario.TipoRelacao.Dependente;
                                cb.Status = (Int32)ContratoBeneficiario.eStatus.Novo;
                                cb.NumeroSequencial = ContratoBeneficiario.ProximoNumeroSequencial(contratoId, arrAux[i], pm);
                                pm.Save(cb);
                            }

                            ContratoStatusHistorico.Salvar(atendimento.NumeroContrato, atendimento.OperadoraID, ContratoStatusHistorico.eStatus.BeneficiarioAdicionado, pm);

                            #endregion
                        }
                        else if (eTipoAtendimentoItem.AlteracaoCadastro == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Marca o ContratoBeneficiario como pendente para ENVIAR para a operadora 

                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);

                            arrAux = item.BeneficiarioIds.Split(',');
                            foreach (String benefID in arrAux)
                            {
                                cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(contratoId, benefID, pm);
                                cb.Status = (Int32)ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
                                pm.Save(cb);
                            }

                            ContratoStatusHistorico.Salvar(atendimento.NumeroContrato, atendimento.OperadoraID, ContratoStatusHistorico.eStatus.BeneficiarioAlterado, pm);

                            #endregion
                        }
                        else if (eTipoAtendimentoItem.CancelarBeneficiarios == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Marca os beneficiarios para cancelamento 

                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);
                            arrAux = item.BeneficiarioIds.Split(',');
                            DateTime dataCancelamento = atendimento.DataHora;
                            foreach (String benefID in arrAux)
                            {
                                cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(contratoId, benefID, pm);
                                cb.Status = (Int32)ContratoBeneficiario.eStatus.ExclusaoPendente;
                                cb.Ativo = false;
                                cb.DataInativacao = dataCancelamento;
                                pm.Save(cb);
                            }

                            ContratoStatusHistorico.Salvar(atendimento.NumeroContrato, atendimento.OperadoraID, ContratoStatusHistorico.eStatus.BeneficiarioCancelado, pm);

                            #endregion
                        }
                        else if (eTipoAtendimentoItem.MudancaDePlano == ((eTipoAtendimentoItem)item.Tipo))
                        {
                            #region Marca o titular para mudança de plano do contrato 

                            contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);

                            cb = ContratoBeneficiario.CarregarTitular(contratoId, pm);
                            cb.Status = (Int32)ContratoBeneficiario.eStatus.MudancaPlanoPendente;
                            pm.Save(cb);
                            ContratoStatusHistorico.Salvar(atendimento.NumeroContrato, atendimento.OperadoraID, ContratoStatusHistorico.eStatus.MudancaDePlano, pm);

                            #endregion

                            #region Altera o plano, calcula novo valor, a vigencia dele, modifica as cobranças 

                            //contratoId = Contrato.CarregaContratoID(atendimento.OperadoraID, atendimento.NumeroContrato, pm);
                            Contrato contrato = new Contrato(contratoId);
                            pm.Load(contrato);

                            contrato.PlanoID = item.PlanoId;
                            contrato.TipoAcomodacao = item.AcomodacaoId;
                            pm.Save(contrato);

                            DateTime dataAlteracao = atendimento.DataHora;

                            //Historico de alteracao de planos 
                            ContratoPlano cp = new ContratoPlano();
                            cp.ContratoID = contratoId;
                            cp.PlanoID = item.PlanoId;
                            cp.TipoAcomodacao = item.AcomodacaoId;
                            cp.Data = atendimento.DataHora;
                            pm.Save(cp);

                            //contratoADMid = contrato.ContratoADMID;

                            //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contratoADMid,
                            //    dataAlteracao, out vigencia, out vencimento, out diaSemJuros, out valorDataLimite, pm);

                            //cobrancas = Cobranca.CarregarTodas(contratoId, pm);
                            //if (cobrancas != null && cobrancas.Count > 0)
                            //{
                            //    foreach (Cobranca cobranca in cobrancas)
                            //    {
                            //        if (((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Dupla ||
                            //            ((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Complementar)
                            //        { continue; }

                            //        if (cobranca.DataVencimento >= vigencia)
                            //        {
                            //            cobranca.Valor = Contrato.CalculaValorDaProposta(contratoId, cobranca.DataVencimento, pm);
                            //            pm.Save(cobranca);
                            //        }
                            //    }
                            //}
                            #endregion
                        }
                    }
                }

                pm.Commit();
            }
            catch(Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        /// <summary>
        /// TODO: 
        /// </summary>
        public void CancelaAtendimento(List<String> atendimentoItemIDs)
        {
            //String cmd = "UPDATE atendimento_item SET item_status=" + Convert.ToInt32(Atendimento.eStatus.Cancelado) + " WHERE item_id IN (" + String.Join(",", atendimentoItemIDs.ToArray()) + ")" ;
            //NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
        }
    }
}

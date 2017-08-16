namespace MedProj.Entidades
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Data.OleDb;
    using System.Configuration;
    using System.Collections.Generic;
    using MedProj.Entidades.Enuns;
    using System.Globalization;

    public class AgendaPagamento : EntidadeBase
    {
        public AgendaPagamento()
        {
            Ativa = true;
            Processado = false;
            DataCriacao = DateTime.Now;
            DataProcessamento = DateTime.Now.AddMinutes(5);
        }

        public virtual string Descricao { get; set; }

        public virtual DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data em que deverá ocorrer o processamento
        /// </summary>
        public virtual DateTime DataProcessamento { get; set; }

        /// <summary>
        /// Data em que o processamento foi concluído
        /// </summary>
        public virtual DateTime? DataConclusao { get; set; }

        public virtual DateTime PeriodoDe { get; set; }
        public virtual DateTime PeriodoAte { get; set; }

        public virtual bool Processado { get; set; }

        public virtual bool Ativa { get; set; }

        public virtual PeriodicidadePagto TipoPagto { get; set; }

        public virtual IList<AgendaPagamentoItem> Itens { get; set; }

        /// <summary>
        /// Gera o arquivo CNAB de pagamento
        /// </summary>
        public virtual string GerarArquivo()
        {
            if (this.Itens == null) return string.Empty;

            StringBuilder sb = new StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            #region Header do arquivo  

            sb.Append("34100000      081218136065000124                    09108 000000009660 4JHM ADMINISTRADORA DE BENEFICIBANCO ITAU SA                           1");
            sb.Append(DateTime.Now.ToString("ddMMyyyy"));
            sb.Append(DateTime.Now.ToString("HHmmss"));
            sb.Append("000000000");
            sb.Append("00000");
            sb.Append(" ".PadRight(69, ' '));

            #endregion

            List<AgendaPagamentoItem> ordenados = this.Itens.OrderBy(i => i.Atendimento.Unidade.ID).ToList();

            long id = 0; string aux = "";
            decimal valTotalLote = 0, valTotal = 0;
            int loteId = 0, registroId = 0, qtdRegistros_DoLote = 0, qtdLotes_Tipo1 = 0, qtdRegistros_Total = 0;

            List<long> ids = new List<long>();
            List<long> atendIds = new List<long>();

            int index = 0;
            foreach (AgendaPagamentoItem item in ordenados)
            {
                index++;

                if (ids.Contains(item.ID)) continue;
                if (atendIds.Contains(item.Atendimento.ID)) continue;

                ids.Add(item.ID);
                atendIds.Add(item.Atendimento.ID);

                if (id != 0 && id != item.Atendimento.Unidade.ID)
                {
                    #region Trailler do lote - tipo 5 - pag 18 

                    this.adTraillerDeLote(ref sb, item, ref loteId, ref aux, cinfo, ref registroId, ref valTotalLote, ref qtdRegistros_DoLote, ref qtdRegistros_Total);

                    #region comentado 

                    //sb.Append(Environment.NewLine);

                    //qtdTotalLote++;
                    //qtdRegistros++;

                    //sb.Append("341");
                    //sb.Append(loteId.ToString().PadLeft(4, '0'));
                    //sb.Append("5");  //tipo registro
                    //sb.Append("         "); //brancos
                    //sb.Append(qtdTotalLote.ToString().PadLeft(6, '0'));

                    ////valor total do lote
                    //aux = qtdTotalLote.ToString("N", cinfo);
                    //sb.Append(aux.Split(',')[0].PadLeft(16, '0'));
                    //sb.Append(aux.Split(',')[1]);

                    //sb.Append("0".PadLeft(18, '0')); //zeros
                    //sb.Append(" ".PadRight(171, ' '));
                    //sb.Append("0".PadLeft(10, '0')); //ocorrencias, somente no arquivo de retorno

                    //registroId = 0; //zera o id de registro para o proximo lote
                    //qtdTotalLote = 0; //zera o total de registro 1, 3 e 5
                    //valTotalLote = 0; //zera o valor total do lote
                    #endregion

                    #endregion

                }

                if (id == 0 || id != item.Atendimento.Unidade.ID)
                {
                    id = item.Atendimento.Unidade.ID;

                    #region header do lote - tipo 1 

                    sb.Append(Environment.NewLine);

                    qtdLotes_Tipo1++;
                    loteId++;
                    qtdRegistros_DoLote++;
                    qtdRegistros_Total++;

                    sb.Append("341");
                    sb.Append(loteId.ToString().PadLeft(4, '0')); //nota 3, pag.38
                    sb.Append("1");  //tipo registro
                    sb.Append("C");
                    sb.Append("20"); //tipo pagto - 20 = fornecedores - nota 4, pag.38

                    if(item.Atendimento.Unidade.Banco.Codigo == "341" || item.Atendimento.Unidade.Banco.Codigo=="409")
                        sb.Append("01"); //forma pagto - 01 = cred. em cc itau - ATENCAO AQUI
                    else
                        sb.Append("03"); //forma pagto - 03 = DOC C 

                    sb.Append("040"); //layout do lote
                    sb.Append(" ");

                    //if (item.Atendimento.Unidade.Tipo == TipoPessoa.Juridica)
                        sb.Append("2");
                    //else
                    //    sb.Append("1");

                    //if (item.Atendimento.Unidade.Documento == null) item.Atendimento.Unidade.Documento = "";
                    //sb.Append(item.Atendimento.Unidade.Documento.Replace(".", "").Replace("-", "").Replace("\\", "").PadLeft(14, '0'));
                    sb.Append("18136065000124");

                    sb.Append("".PadRight(20, ' ')); //campos identificacao do pagamento (x04) e brancos x16

                    sb.Append("09108 000000009660 4"); //agencia + branco + conta + branco + DAC (agencia e conta debitadas)
                    sb.Append("JHM ADMINISTRADORA                                                    ");// empresa debitada + finalidade do lote + compl. historico conta debitada
                    sb.Append("RUA DO MERCADO                00011andar 22       RIO DE JANEIRO      20010120RJ"); //endereco empresa debitada
                    sb.Append("        "); //brancos x8
                    sb.Append(" ".PadRight(10, ' ')); //ocorrencias -> somente no arquivo de retorno, informar branco

                    #endregion
                }

                #region Registro Detalhe - Segmento A - tipo 3 

                sb.Append(Environment.NewLine);

                qtdRegistros_DoLote++;
                qtdRegistros_Total++;

                sb.Append("341");
                sb.Append(loteId.ToString().PadLeft(4, '0')); //nota 3, pag.38
                sb.Append("3");  //tipo registro

                registroId++;
                sb.Append(registroId.ToString().PadLeft(5, '0')); //numero do registro, nota 9, pag.43
                sb.Append("A"); //Segmento
                sb.Append("000"); //Tipo de movimento, nota 10
                sb.Append("000"); //Camara, nota 37, pag.55

                sb.Append(item.Atendimento.Unidade.Banco.Codigo.PadLeft(3, '0'));

                if (item.Atendimento.Unidade.Banco.Codigo == "341" || item.Atendimento.Unidade.Banco.Codigo == "409") //se itau ou unibanco
                { 
                    //nota 11 - pag 44
                    sb.Append("0");
                    sb.Append(item.Atendimento.Unidade.Banco.Agencia.PadLeft(4, '0'));
                    sb.Append(" ");
                    sb.Append("000000");
                    sb.Append(item.Atendimento.Unidade.Banco.Conta.PadLeft(6, '0'));
                    sb.Append(" ");
                    sb.Append(item.Atendimento.Unidade.Banco.ContaDAC.Trim());
                }
                else
                {
                    sb.Append(item.Atendimento.Unidade.Banco.Agencia.PadLeft(5, '0'));
                    sb.Append(" ");
                    sb.Append(item.Atendimento.Unidade.Banco.Conta.PadLeft(12, '0'));

                    if (item.Atendimento.Unidade.Banco.ContaDAC.Trim().Length == 2)
                    {
                        sb.Append(item.Atendimento.Unidade.Banco.ContaDAC.Trim());
                    }
                    else
                    {
                        sb.Append(" ");
                        sb.Append(item.Atendimento.Unidade.Banco.ContaDAC.Trim());
                    }
                }

                //return sb.ToString();///////////////////////////////

                if (item.Atendimento.Unidade.Nome.Replace("(","").Length <= 30)
                    sb.Append(item.Atendimento.Unidade.Nome.Replace("(", "").PadRight(30, ' '));
                else
                    sb.Append(item.Atendimento.Unidade.Nome.Replace("(", "").Substring(0, 29).PadRight(30, ' '));

                sb.Append(item.Atendimento.ID.ToString().PadLeft(20, '0')); //Seu Numero

                sb.Append(DateTime.Now.AddDays(3).ToString("ddMMyyyy")); //data prevista para pagto.

                sb.Append("REA"); //tipo de moeda

                sb.Append("00000000"); //codigo ispb 9(8)

                sb.Append("0000000"); //zeros

                //Valor
                valTotalLote += item.Atendimento.Procedimentos.Sum(p => p.Valor);
                valTotal += valTotalLote;

                aux = item.Atendimento.Procedimentos.Sum(p => p.Valor).ToString("N", cinfo);
                sb.Append(aux.Split(',')[0].PadLeft(13, '0'));
                sb.Append(aux.Split(',')[1]);

                sb.Append(" ".PadRight(15, ' ')); //Nosso numero, somente no arquivo de retorno

                sb.Append("     "); //brancos x(5)
                sb.Append("00000000"); //data efetiva do pagto, somente arq. de retorno, 9(8)

                sb.Append("000000000000000"); //valor efetivo do pagto, somente arq. de retorno, 9(13)V9(02)

                sb.Append(" ".PadLeft(18, ' ')); //finalidade detalhe, nota 31, X(18) 

                sb.Append("  "); //brancos 2

                sb.Append("000000"); //num documento, somente arq. de retorno

                //num de inscricao do favorecido
                if (!string.IsNullOrEmpty(item.Atendimento.Unidade.Documento) && item.Atendimento.Unidade.Documento.Replace(".", "").Replace("-", "").Replace("\\", "").Length <= 14)
                    sb.Append(item.Atendimento.Unidade.Documento.Replace(".", "").Replace("-", "").Replace("\\", "").PadLeft(14, '0'));
                else
                    sb.Append("00000000000000");

                ////dentro da documentacao, mas confirmar
                //sb.Append("1"); //tipo de indentificacao, nota 30, pag52: 1 para por cnpj de beneficiario
                //sb.Append("           "); //11 brancos
                //sb.Append("0"); //aviso ao favorecido, nota 16 pag47, 0 para nao emitir aviso
                //sb.Append("          "); //ocorrencias - 10 brancos, utilizado no arquivo de retorno


                //Aparentemente fora da documentação, mas igual ao deles:
                sb.Append("            0          ");

                #endregion

                #region Registro Detalhe - Segmento B - tipo 3 

                qtdRegistros_Total++;
                qtdRegistros_DoLote++;

                sb.Append(Environment.NewLine);

                sb.Append("341");
                sb.Append(loteId.ToString().PadLeft(4, '0'));
                sb.Append("3");  //tipo registro

                registroId++; //O primeiro registro de um lote recebe o nº '00001' e assim consecutivamente. Para o Segmento “J-52”, "B", “C”, “D”, “E”, “F”, “W” e “Z”, por se tratar de complemento de informações, conterá o mesmo número atribuído no Segmento "A", “J” e “N” correspondente.
                sb.Append(registroId.ToString().PadLeft(5, '0')); //numero do registro, nota 9 pag43
                sb.Append("B"); //Segmento
                sb.Append("   "); //COMPLEMENTO DE REGISTRO, pag 12

                if (item.Atendimento.Unidade.Tipo == TipoPessoa.Fisica)
                    sb.Append("1");
                else
                    sb.Append("2");

                //num de inscricao do favorecido, pag 12
                if (!string.IsNullOrEmpty(item.Atendimento.Unidade.Documento) && item.Atendimento.Unidade.Documento.Replace(".", "").Replace("-", "").Replace("\\", "").Length <= 14)
                    sb.Append(item.Atendimento.Unidade.Documento.Replace(".", "").Replace("-", "").Replace("\\", "").PadLeft(14, '0'));
                else
                    sb.Append("00000000000000");

                //Nome da rua, praça, avenida, etc.
                if (item.Atendimento.Unidade.Endereco.Length > 30)
                    sb.Append(item.Atendimento.Unidade.Endereco.Substring(0, 29).PadRight(30, ' '));
                else
                    sb.Append(item.Atendimento.Unidade.Endereco.PadRight(30, ' '));

                //numero
                if (item.Atendimento.Unidade.Numero.Length > 5)
                    sb.Append(item.Atendimento.Unidade.Numero.Substring(0, 5).PadLeft(5, '0'));
                else
                    sb.Append(item.Atendimento.Unidade.Numero.PadLeft(5, '0'));

                //complemento
                if (item.Atendimento.Unidade.Complemento.Length > 15)
                    sb.Append(item.Atendimento.Unidade.Complemento.Replace("º", "o").Substring(0, 15).PadRight(15, ' '));
                else
                    sb.Append(item.Atendimento.Unidade.Complemento.Replace("º", "o").PadRight(15, ' '));

                //bairro
                if (item.Atendimento.Unidade.Bairro.Length > 15)
                    sb.Append(item.Atendimento.Unidade.Bairro.Substring(0, 15).PadRight(15, ' '));
                else
                    sb.Append(item.Atendimento.Unidade.Bairro.PadRight(15, ' '));

                //cidade
                if (item.Atendimento.Unidade.Cidade.Length > 20)
                    sb.Append(item.Atendimento.Unidade.Cidade.Substring(0, 20).PadRight(15, ' '));
                else
                    sb.Append(item.Atendimento.Unidade.Cidade.PadRight(20, ' '));

                //cep
                sb.Append(item.Atendimento.Unidade.CEP.Replace("-", "").PadLeft(8, '0'));

                //uf
                sb.Append(item.Atendimento.Unidade.UF.PadRight(2, ' '));

                //email
                sb.Append(" ".PadRight(100, ' '));

                //brancos
                sb.Append("   ");

                //ocorrencias
                sb.Append(" ".PadRight(10, ' '));

                #endregion

                if (index == ordenados.Count) //tipo 5
                {
                    this.adTraillerDeLote(ref sb, item, ref loteId, ref aux, cinfo, ref registroId, ref valTotalLote, ref qtdRegistros_DoLote, ref qtdRegistros_Total);
                }
            }

            #region Trailler do arquivo - tipo 9 - pag. 37 

            sb.Append(Environment.NewLine);
            sb.Append("341");
            sb.Append("9999");
            sb.Append("9");
            sb.Append("         ");

            sb.Append(qtdLotes_Tipo1.ToString().PadLeft(6, '0'));
            sb.Append((qtdRegistros_Total + 2).ToString().PadLeft(6, '0')); //soma 2 para contar o header e o trailler do arquivo
            sb.Append(" ".PadRight(211, ' '));

            #endregion

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// Trailler do lote - tipo 5 - pag 18 
        /// </summary>
        void adTraillerDeLote(ref StringBuilder sb, AgendaPagamentoItem item, ref int loteId, ref string aux, CultureInfo cinfo, ref int registroId, ref decimal valTotalLote, ref int qtdRegistros_DoLote, ref int qtdRegistros_Total)
        {
            sb.Append(Environment.NewLine);

            qtdRegistros_DoLote++; //soma este registro 5
            qtdRegistros_Total++;  //atualiza a qtd total de registro para o trailler do arquivo

            sb.Append("341");
            sb.Append(loteId.ToString().PadLeft(4, '0'));
            sb.Append("5");  //tipo registro
            sb.Append("         "); //brancos
            sb.Append(qtdRegistros_DoLote.ToString().PadLeft(6, '0'));

            //valor total do lote
            aux = valTotalLote.ToString("N", cinfo);
            sb.Append(aux.Split(',')[0].PadLeft(16, '0'));
            sb.Append(aux.Split(',')[1]);

            sb.Append("0".PadLeft(18, '0')); //zeros
            sb.Append(" ".PadRight(171, ' '));
            sb.Append(" ".PadLeft(10, ' ')); //ocorrencias, somente no arquivo de retorno

            registroId = 0; //zera o id de registro para o proximo lote
            qtdRegistros_DoLote = 0; //zera o total de registro 1, 3 e 5
            valTotalLote = 0; //zera o valor total do lote
        }




























        /*
            base.HasMany<Atendimento>(c => c.Atendimentos)
                .Table("prestador_unidade")
                .KeyColumn("Owner_ID"); ////nome da FK em prestador_unidade
         * 
         * LAYOUT DO LOTE = no arquivo exemplo está 080. na documentacao está 040
         * Camara e codigo ISPB ?
         * finalidade e status ?
         */
    }
}

namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.BusinessLayer;

    [DBTable("relatorio_agendamento")]
    public class AgendaRelatorio : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            public static void FillCombo(DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new ListItem("Contas a Receber - Pago", "0"));
                cbo.Items.Add(new ListItem("Contas a Receber - Em aberto", "1"));
                cbo.Items.Add(new ListItem("Controladoria - Detalhe", "2"));
                cbo.Items.Add(new ListItem("Controladoria - Taxa", "3"));
            }

            public static String TrataNomeColuna(String nomeOriginal)
            {
                return nomeOriginal
                    .Replace("estipulante_carteira", "Carteira")
                    .Replace("tabelavalor_vencimentoInicio", "TabVenctoInicio")
                    .Replace("tabelavalor_vencimentoFim", "TabVenctoFim")
                    .Replace("cobranca_dataCriacao", "Criação")
                    .Replace("filial", "Filial")
                    .Replace("operadora_nome", "Operadora")
                    .Replace("estipulante_descricao", "Estipulante")
                    .Replace("contrato_numero", "NumeroContrato")
                    .Replace("contratobeneficiario_numeroSequencia", "Seq")
                    .Replace("beneficiario_nome", "NomeBeneficiario")
                    .Replace("beneficiario_cpf", "CPFbeneficiario")
                    .Replace("contrato_codcobranca", "CodCobranca")
                    .Replace("cobranca_parcela", "Parcela")
                    .Replace("plano_descricao", "Plano")
                    .Replace("contrato_desconto", "Desconto")
                    .Replace("beneficiario_dataNascimento", "DataNascimento")
                    .Replace("contrato_vigencia", "DataVigencia")
                    .Replace("contrato_datacancelamento", "DataCancelInativo")
                    .Replace("cobranca_dataVencimento", "DataVencto")
                    .Replace("cobranca_dataPagto", "DataPagto")
                    .Replace("cobranca_valor", "Valor")
                    .Replace("cobranca_valorPagto", "Pagto")
                    .Replace("motivobaixa_descricao", "MotivoBaixa");
                ;
            }
        }

        public enum eTipo : int
        {
            CReceberPago,
            CReceberAberto,
            ControladoriaDetalhe,
            ControladoriaTaxa
        }

        #region fields 

        Object _id;
        Object _usuarioId;
        DateTime _data;
        Int32 _tipo;
        String _estipulanteIds;
        String _operadoraIds;
        DateTime _dataDe;
        DateTime _dataAte;
        DateTime _processarEm;
        Boolean _processado;
        String _arquivo;

        #endregion

        #region properties 

        [DBFieldInfo("agenda_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("agenda_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        [DBFieldInfo("agenda_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [DBFieldInfo("agenda_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("agenda_estipulantes", FieldType.Single)]
        public String EstipulanteIDs
        {
            get { return _estipulanteIds; }
            set { _estipulanteIds = value; }
        }

        [DBFieldInfo("agenda_operadoras", FieldType.Single)]
        public String OperadoraIDs
        {
            get { return _operadoraIds; }
            set { _operadoraIds = value; }
        }

        [DBFieldInfo("agenda_dataDe", FieldType.Single)]
        public DateTime DataDe
        {
            get { return _dataDe; }
            set { _dataDe = value; }
        }

        [DBFieldInfo("agenda_dataAte", FieldType.Single)]
        public DateTime DataAte
        {
            get { return _dataAte; }
            set { _dataAte = value; }
        }

        [DBFieldInfo("agenda_processarEm", FieldType.Single)]
        public DateTime ProcessarEm
        {
            get { return _processarEm; }
            set { _processarEm = value; }
        }

        [DBFieldInfo("agenda_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }

        [DBFieldInfo("agenda_arquivo", FieldType.Single)]
        public String Arquivo
        {
            get { return _arquivo; }
            set { _arquivo= value; }
        }

        public String strTipo
        {
            get
            {
                switch (_tipo)
                {
                    case 0:
                    {
                        return "Contas a Receber - Pago";
                    }
                    case 1:
                    {
                        return "Contas a Receber - Em aberto";
                    }
                    case 2:
                    {
                        return "Controladoria - Detalhe";
                    }
                    case 3:
                    {
                        return "Controladoria - Taxa";
                    }
                    default:
                    {
                        return String.Empty;
                    }
                }
            }
        }

        #endregion

        public AgendaRelatorio()
        {
            _tipo = (int)eTipo.CReceberPago;
            _data = DateTime.Now;
        }

        public AgendaRelatorio(Object id) : this() { _id = id; }

        #region EntityBase methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AgendaRelatorio>CarregarPorUsuario(Object usuarioId, eTipo tipo)
        {
            String qry = String.Concat("* from relatorio_agendamento where agenda_usuarioId=", usuarioId,
                " and agenda_tipo=", Convert.ToInt32(tipo));

            return LocatorHelper.Instance.ExecuteQuery<AgendaRelatorio>(qry, typeof(AgendaRelatorio));
        }

        public static String ControladoriaTaxaQUERY(DateTime dtFrom, DateTime dtTo, String[] oper, String[] estp)
        {
            String qry = String.Concat(
                "select distinct CONVERT(varchar(7), cobranca_dataVencimento, 111) as CompeVencto,operadora_nome as Operadora,estipulante_descricao as Estipulante,estipulante_carteira,contrato_numero as NumeroContrato,contratobeneficiario_numeroSequencia as Seq,beneficiario_nome as NomeBeneficiario,beneficiario_cpf as CPFbeneficiario,datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365 as IDADE,CONVERT(varchar(14), beneficiario.beneficiario_dataNascimento, 103) as DataNascimento,CONVERT(varchar(14), contrato_vigencia, 103) as DataVigencia,CONVERT(varchar(14), contrato_datacancelamento, 103) as DataCancelInativo,CONVERT(varchar(14), cobranca_dataVencimento, 103) as DataVencto,CONVERT(varchar(14), cobranca_dataPagto, 103) as DataPagto,contrato.contrato_codcobranca as CodCobranca,cobranca.cobranca_parcela as Parcela,plano.plano_descricao as Plano,contrato.contrato_desconto as Desconto,tabela_valor.tabelavalor_vencimentoInicio,tabela_valor.tabelavalor_vencimentoFim,CONVERT(varchar(14), cobranca_dataCriacao, 103) as cobranca_dataCriacao,",
                "   case ",
                "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                "       WHEN  operadora_id = 18 THEN 'DF' ",
                "       WHEN  operadora_id = 19 THEN 'PR' ",
                "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                "       WHEN  operadora_id = 24 THEN 'RN' ",
                "       WHEN  operadora_id = 25 THEN 'PE' ",
                "       WHEN  operadora_id = 26 THEN 'MG' ",
                "       WHEN  operadora_id = 27 THEN 'GO' ",
                "   end as filial, ",
                "   case ",
                "       WHEN  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  < 19) THEN '0  a 18 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 18) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  24) THEN '19 a 23 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 23) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  29) THEN '24 a 28 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 28) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  34) THEN '29 a 33 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 33) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  39) THEN '34 a 38 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 38) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  44) THEN '39 a 43 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 43) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  49) THEN '44 a 48 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 48) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  54) THEN '49 a 53 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 53) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  59) THEN '54 a 58 anos' ",
                "       ELSE '59 anos ou mais' ",
                "   end as FaixaEtaria, ",
                "   case ",
                "       WHEN contratobeneficiario_numeroSequencia = 0 THEN 'TITULAR' ",
                "       ELSE contratoAdmparentescoagregado_parentescoDescricao  ",
                "   end as Parentesco, ",

                "   case ",
                "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 0 then 'MANUAL' ",
                "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 1 then 'PROVISORIA' ",
                "       else 'BANCARIA' ",
                "   end as TipoBaixa, ",

                "   case ",
                "       WHEN contratobeneficiario_numeroSequencia = 0 and cobranca_composicao.cobrancacomp_tipo = 0 THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valor), ',',''), '.',',') ",
                "       ELSE NULL ",
                "   end as Valor_Boleto, ",

                "   case ",
                "       WHEN contratobeneficiario_numeroSequencia = 0 and cobranca_composicao.cobrancacomp_tipo = 0 THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valorPagto), ',',''), '.',',') ",
                "       ELSE NULL ",
                "   end as Valor_Pago, ",

                "   case ",
                "       WHEN cobranca.cobranca_tipo = 0 then 'NORMAL' ",
                "       WHEN cobranca.cobranca_tipo = 1 then 'COMPLEMENTAR' ",
                "       WHEN cobranca.cobranca_tipo = 2 then 'DUPLA' ",
                "       WHEN cobranca.cobranca_tipo = 4 then 'NEGOCIACAO' ",
                "       ELSE 'NAO IDENTIFICADO' ",
                "   end as Tipo_Boleto, ",

                "   case ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 1 then 'TAXA ASSOCIATIVA' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 3 then 'ADICONAL' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 0 then 'VALOR MEDICO' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 4 then 'DESCONTO' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 2 then 'TARIFA BANCARIA' ",
                "       ELSE 'NAO IDENTIFICADO' ",
                "   end as TIPO, ",

                "   case ",
                "       WHEN contrato.contrato_tipoAcomodacao = 0 then 'COLETIVO' ",
                "       ELSE 'PRIVATIVO' ",
                "   end as Acomodacao, ",

                "   case ",
                "       WHEN cobranca.cobranca_pago = 0 then 'Em aberto' ",
                "       ELSE 'Pago' ",
                "   end as StatusBoleto, ",

                "   case ",
                "       when cobranca_composicao.cobrancacomp_tipo = 1 AND contrato_beneficiario.contratobeneficiario_numeroSequencia > 0 then NULL ",
                "       ELSE REPLACE(REPLACE(CONVERT(varchar(100), cobranca_composicao.cobrancacomp_valor), ',',''), '.',',') ",
                "   end as ValorDetalhe, ",

                "   case ",
                "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 0 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qComumPagamento), ',',''), '.',',') ",
                "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 1 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qParticularPagamento), ',',''), '.',',') ",
                "       ELSE NULL ",
                "   end as Valor_Operadora, ",

                "   case ",
                "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 0 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qComum), ',',''), '.',',') ",
                "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 1 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qParticular), ',',''), '.',',') ",
                "       ELSE NULL ",
                "   end as Valor_TabCliente, ",

                "   case ",
                "       WHEN contrato_vigencia < '2013-07-01 00:00:00' then 'PSPADRAO' ",
                "       WHEN operadora_id = 16 then 'PSPADRAO' ",
                "       ELSE 'QUALICORP' ",
                "   end as Cedente ",

                "	from contrato ",
                "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id and contratobeneficiario_tipo=0 ",
                "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                "		inner join operadora on operadora_id = contrato_operadoraId ",
                "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                "       inner join cobranca on cobranca_propostaId=contrato_id ",
                "       inner join plano on  contrato.contrato_planoId = plano_Id and contrato.contrato_contratoAdmId = plano.plano_contratoId ",
                "       inner join dbo.cobranca_composicao on cobranca.cobranca_id = cobranca_composicao.cobrancacomp_cobranaId and cobranca_composicao.cobrancacomp_tipo = 1 ",
                "       left  join contratoADM_parentesco_agregado on contratobeneficiario_parentescoId = contratoAdmparentescoagregado_Id ",
                "       left join cobranca_baixa on cobranca_baixa.cobrancabaixa_cobrancaId = cobranca.cobranca_id ",
                "       left join tabela_valor on tabela_valor.tabelavalor_contratoId = contrato.contrato_contratoAdmId and cobranca.cobranca_dataVencimento+1 >= tabela_valor.tabelavalor_vencimentoInicio and cobranca.cobranca_dataVencimento+1 <= tabela_valor.tabelavalor_fim ",
                "       left join tabela_valor_item on tabela_valor_item.tabelavaloritem_tabelaid = tabela_valor.tabelavalor_id and tabela_valor_item.tabelavaloritem_planoId = contrato.contrato_planoId and (datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365) between tabela_valor_item.tabelavaloritem_idadeInicio and tabela_valor_item.tabelavaloritem_idadeFim ",
                "	where ",
                "		cobranca_parcela <> 1 and cobranca_cancelada <> 1 and cobranca_tipo=0 ",
                "       and (contrato_datacancelamento > cobranca_dataVencimento or contrato_datacancelamento is null) ",
                "		and cobranca_dataVencimento between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                "       and contrato_estipulanteId IN (", String.Join(",", estp), ")",
                "	order by 5,6");

            return qry;
        }

        public static String ControladoriaDetalheQUERY(DateTime dtFrom, DateTime dtTo, String[] oper, String[] estp)
        {
            String qry = String.Concat(
                    "select distinct operadora_nome as Operadora,estipulante_descricao as Estipulante,estipulante_carteira,contrato_numero as NumeroContrato,contratobeneficiario_numeroSequencia as Seq, CONVERT(varchar(7), cobranca_dataVencimento, 111) as CompeVencto, beneficiario_Id as IdBenef,CONVERT(varchar(14), contratobeneficiario_vigencia, 103) as InicioBenef,beneficiario_nome as NomeBeneficiario,beneficiario_cpf as CPFbeneficiario,datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365 as IDADE,CONVERT(varchar(14), beneficiario.beneficiario_dataNascimento, 103) as DataNascimento,CONVERT(varchar(14), contrato_vigencia, 103) as DataVigência,CONVERT(varchar(14), contrato_datacancelamento, 103) as DataCancelInativo,CONVERT(varchar(14), cobranca_dataVencimento, 103) as DataVencto,CONVERT(varchar(14), cobranca_dataPagto, 103) as DataPagto,contrato.contrato_codcobranca as CodCobranca,cobranca.cobranca_parcela as Parcela,cobranca.cobranca_id, cobranca_composicao.cobrancacomp_id,plano.plano_descricao as Plano,contrato.contrato_desconto as Desconto,tabela_valor.tabelavalor_id, cobranca.cobranca_dataVencimento,tabela_valor.tabelavalor_vencimentoInicio, tabela_valor.tabelavalor_vencimentoFim,CONVERT(varchar(14), cobranca_dataCriacao, 103) as cobranca_dataCriacao,",
                    "   case ",
                    "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                    "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                    "       WHEN  operadora_id = 18 THEN 'DF' ",
                    "       WHEN  operadora_id = 19 THEN 'PR' ",
                    "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                    "       WHEN  operadora_id = 24 THEN 'RN' ",
                    "       WHEN  operadora_id = 25 THEN 'PE' ",
                    "       WHEN  operadora_id = 26 THEN 'MG' ",
                    "       WHEN  operadora_id = 27 THEN 'GO' ",
                    "   end as filial, ",
                    "   case ",
                    "       WHEN  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  < 19) THEN '0  a 18 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 18) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  24) THEN '19 a 23 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 23) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  29) THEN '24 a 28 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 28) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  34) THEN '29 a 33 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 33) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  39) THEN '34 a 38 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 38) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  44) THEN '39 a 43 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 43) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  49) THEN '44 a 48 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 48) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  54) THEN '49 a 53 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 53) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  59) THEN '54 a 58 anos' ",
                    "       ELSE '59 anos ou mais' ",
                    "   end as FaixaEtaria, ",
                    "   case ",
                    "       WHEN contratobeneficiario_numeroSequencia = 0 THEN 'TITULAR' ",
                    "       ELSE contratoAdmparentescoagregado_parentescoDescricao  ",
                    "   end as Parentesco, ",

                    //"   case ",
                //"       WHEN cobranca_baixa.cobrancabaixa_id is not null then 'MANUAL' ",
                //"       ELSE 'BANCARIA' ",
                //"   end as TipoBaixa, ",
                    "   case ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 0 then 'MANUAL' ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 1 then 'PROVISORIA' ",
                    "       else 'BANCARIA' ",
                    "   end as TipoBaixa, ",

                    "   case ",
                    "       WHEN contratobeneficiario_numeroSequencia = 0 and cobranca_composicao.cobrancacomp_tipo = 0 THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valor), ',',''), '.',',') ",
                    "       ELSE NULL ",
                    "   end as Valor_Boleto, ",

                    "   case ",
                    "       WHEN contratobeneficiario_numeroSequencia = 0 and cobranca_composicao.cobrancacomp_tipo = 0 THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valorPagto), ',',''), '.',',') ",
                    "       ELSE NULL ",
                    "   end as Valor_Pago, ",

                    "   case ",
                    "       WHEN cobranca.cobranca_tipo = 0 then 'NORMAL' ",
                    "       WHEN cobranca.cobranca_tipo = 1 then 'COMPLEMENTAR' ",
                    "       WHEN cobranca.cobranca_tipo = 2 then 'DUPLA' ",
                    "       WHEN cobranca.cobranca_tipo = 4 then 'NEGOCIACAO' ",
                    "       ELSE 'NAO IDENTIFICADO' ",
                    "   end as Tipo_Boleto, ",

                    "   case ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 1 then 'TAXA ASSOCIATIVA' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 3 then 'ADICONAL' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 0 then 'VALOR MEDICO' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 4 then 'DESCONTO' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 2 then 'TARIFA BANCARIA' ",
                    "       ELSE 'NAO IDENTIFICADO' ",
                    "   end as TIPO, ",

                    "   case ",
                    "       WHEN contrato.contrato_tipoAcomodacao = 0 then 'COLETIVO' ",
                    "       ELSE 'PRIVATIVO' ",
                    "   end as Acomodacao, ",

                    "   case ",
                    "       WHEN cobranca.cobranca_pago = 0 then 'Em aberto' ",
                    "       ELSE 'Pago' ",
                    "   end as StatusBoleto, ",

                    "   case ",
                    "       when cobranca_composicao.cobrancacomp_tipo = 1 AND contrato_beneficiario.contratobeneficiario_numeroSequencia > 0 then NULL ",
                    "       ELSE REPLACE(REPLACE(CONVERT(varchar(100), cobranca_composicao.cobrancacomp_valor), ',',''), '.',',') ",
                    "   end as ValorDetalhe, ",

                    "   case ",
                    "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 0 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qComumPagamento), ',',''), '.',',') ",
                    "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 1 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qParticularPagamento), ',',''), '.',',') ",
                    "       ELSE NULL ",
                    "   end as Valor_Operadora, ",

                    "   case ",
                    "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 0 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qComum), ',',''), '.',',') ",
                    "       when cobranca_composicao.cobrancacomp_tipo = 0  and contrato.contrato_tipoAcomodacao = 1 then REPLACE(REPLACE(CONVERT(varchar(100), tabela_valor_item.tabelavaloritem_qParticular), ',',''), '.',',') ",
                    "       ELSE NULL ",
                    "   end as Valor_TabCliente, ",

                    "   case ",
                    "       WHEN contrato_vigencia < '2013-07-01 00:00:00' then 'PSPADRAO' ",
                    "       WHEN operadora_id = 16 then 'PSPADRAO' ",
                    "       ELSE 'QUALICORP' ",
                    "   end as Cedente ",

                    "	from contrato ",
                    "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id ",
                    "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                    "		inner join operadora on operadora_id = contrato_operadoraId ",
                    "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                    "       inner join cobranca on cobranca_propostaId=contrato_id ",
                    "       inner join plano on  contrato.contrato_planoId = plano_Id and contrato.contrato_contratoAdmId = plano.plano_contratoId ",
                    "       inner join cobranca_composicao on cobranca.cobranca_id = cobranca_composicao.cobrancacomp_cobranaId and cobranca_composicao.cobrancacomp_beneficiarioId = beneficiario.beneficiario_id ",
                    "       left  join contratoADM_parentesco_agregado on contratobeneficiario_parentescoId = contratoAdmparentescoagregado_Id ",
                    "       left join cobranca_baixa on cobranca_baixa.cobrancabaixa_cobrancaId = cobranca.cobranca_id ",
                    "       left join tabela_valor on tabela_valor.tabelavalor_contratoId = contrato.contrato_contratoAdmId and cobranca.cobranca_dataVencimento+1 >= tabela_valor.tabelavalor_vencimentoInicio and cobranca.cobranca_dataVencimento+1 <= tabela_valor.tabelavalor_fim ",
                    "       left join tabela_valor_item on tabela_valor_item.tabelavaloritem_tabelaid = tabela_valor.tabelavalor_id and tabela_valor_item.tabelavaloritem_planoId = contrato.contrato_planoId and (datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365) between tabela_valor_item.tabelavaloritem_idadeInicio and tabela_valor_item.tabelavaloritem_idadeFim ",
                    "	where ",
                    "		cobranca_parcela <> 1 and cobranca_cancelada <> 1 ",
                    "       and (contrato_datacancelamento > cobranca.cobranca_dataVencimento or contrato_datacancelamento is null) ",
                    "		and cobranca_dataVencimento between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                    "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                    "       and contrato_estipulanteId IN (", String.Join(",", estp), ")",
                    "	order by 5,6");

            return qry;
        }

        public static String ContasReceberPagoQUERY(DateTime dtFrom, DateTime dtTo, String[] oper, String[] estp)
        {
            String qry = String.Concat(
                    "select distinct CONVERT(varchar(7), cobranca_dataVencimento, 111) as CompeVencto, operadora_nome,estipulante_descricao,estipulante_carteira,contrato_numero,contratobeneficiario_numeroSequencia,beneficiario_nome,beneficiario_cpf,datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365 as IDADE,CONVERT(varchar(14), beneficiario.beneficiario_dataNascimento, 103) as beneficiario_dataNascimento,CONVERT(varchar(14), contrato_vigencia, 103) as contrato_vigencia,CONVERT(varchar(14), contrato_datacancelamento, 103) as contrato_datacancelamento,CONVERT(varchar(14), cobranca_dataVencimento, 103) as cobranca_dataVencimento,CONVERT(varchar(14), cobranca_dataPagto, 103) as cobranca_dataPagto,cobranca_valor,cobranca_valorPagto,contrato_codcobranca,cobranca_parcela,plano_descricao,cobranca_id,CONVERT(varchar(14), cobranca_dataCriacao, 103) as cobranca_dataCriacao,cobrancacomp_id,",
                    "   case ",
                    "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                    "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                    "       WHEN  operadora_id = 18 THEN 'DF' ",
                    "       WHEN  operadora_id = 19 THEN 'PR' ",
                    "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                    "       WHEN  operadora_id = 24 THEN 'RN' ",
                    "       WHEN  operadora_id = 25 THEN 'PE' ",
                    "       WHEN  operadora_id = 26 THEN 'MG' ",
                    "       WHEN  operadora_id = 27 THEN 'GO' ",
                    "   end as filial, ",
                    "   case ",
                    "       WHEN  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  < 19) THEN '0  a 18 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 18) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  24) THEN '19 a 23 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 23) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  29) THEN '24 a 28 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 28) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  34) THEN '29 a 33 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 33) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  39) THEN '34 a 38 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 38) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  44) THEN '39 a 43 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 43) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  49) THEN '44 a 48 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 48) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  54) THEN '49 a 53 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 53) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  59) THEN '54 a 58 anos' ",
                    "       ELSE '59 anos ou mais' ",
                    "   end as FaixaEtaria, ",
                    "   case ",
                    "       WHEN contratobeneficiario_numeroSequencia = 0 THEN 'TITULAR' ",
                    "       ELSE contratoAdmparentescoagregado_parentescoDescricao ", //'BANCARIA'
                    "   end as Parentesco, ",

                    "   case ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 0 then 'MANUAL' ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 1 then 'PROVISORIA' ",
                    "       ELSE 'BANCARIA' ",
                    "   end as TipoBaixa, ",

                    "   cobranca_motivoBaixa.motivobaixa_descricao as MotivoBaixa, ",

                    "   case ",
                    "       WHEN cobranca.cobranca_tipo = 0 then 'NORMAL' ",
                    "       WHEN cobranca.cobranca_tipo = 1 then 'COMPLEMENTAR' ",
                    "       WHEN cobranca.cobranca_tipo = 2 then 'DUPLA' ",
                    "       WHEN cobranca.cobranca_tipo = 4 then 'NEGOCIACAO' ",
                    "       ELSE 'NAO IDENTIFICADO' ",
                    "   end as Tipo_Boleto, ",

                    "   case ",
                    "       when cobranca.cobranca_tipo = 4 then cobranca_empresa.empresa_nome ",
                    "       when parccob_cobrancaId = cobranca_id then cobranca_empresa.empresa_nome ",
                    "       else '' ",
                    "   end as EmpresaCobranca, ",

                    "   case ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 1 then 'TAXA ASSOCIATIVA' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 3 then 'ADICONAL' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 0 then 'VALOR MEDICO' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 4 then 'DESCONTO' ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 2 then 'TARIFA BANCARIA' ",
                    "       ELSE 'NAO IDENTIFICADO' ",
                    "   end as TIPO, ",

                    "   case ",
                    "       WHEN contrato.contrato_tipoAcomodacao = 0 then 'COLETIVO' ",
                    "       ELSE 'PRIVATIVO' ",
                    "   end as Acomodacao, ",

                    "   case ",
                    "       WHEN cobranca.cobranca_pago = 0 then 'Em aberto' ",
                    "       ELSE 'Pago' ",
                    "   end as StatusBoleto, ",

                    "   case ",
                    "       WHEN cobranca_composicao.cobrancacomp_tipo = 1 AND contrato_beneficiario.contratobeneficiario_numeroSequencia > 0 then NULL ",
                    "       ELSE REPLACE(REPLACE(CONVERT(varchar(100), cobranca_composicao.cobrancacomp_valor), ',',''), '.',',') ",
                    "   end as ValorDetalhe, ",

                    "   case ",
                    "       WHEN contrato_vigencia < '2013-07-01 00:00:00' then 'PSPADRAO' ",
                    "       WHEN operadora_id = 16 then 'PSPADRAO' ",
                    "       ELSE 'QUALICORP' ",
                    "   end as Cedente ",

                    "	from contrato ",
                    "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id  and contratobeneficiario_tipo=0 ",
                    "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                    "		inner join operadora on operadora_id = contrato_operadoraId ",
                    "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                    "       inner join cobranca on cobranca_propostaId=contrato_id ",
                    "       inner join plano on  contrato.contrato_planoId = plano_Id and contrato.contrato_contratoAdmId = plano.plano_contratoId ",
                    "       left  join cobranca_composicao on cobranca.cobranca_id = cobranca_composicao.cobrancacomp_cobranaId and cobranca_composicao.cobrancacomp_tipo = 1 ",
                    "       left  join contratoADM_parentesco_agregado on contratobeneficiario_parentescoId = contratoAdmparentescoagregado_Id ",
                    "       left  join cobranca_baixa on cobranca_baixa.cobrancabaixa_cobrancaId = cobranca.cobranca_id ",
                    "       left join cobranca_empresa on cobranca_empresa.empresa_id = contrato.contrato_empresaCobrancaId ",
                    "       left join cobranca_parcelamentoCobrancaOriginal on parccob_cobrancaId = cobranca.cobranca_id ",
                    "       left  join tabela_valor on tabela_valor.tabelavalor_contratoId = contrato.contrato_contratoAdmId and cobranca.cobranca_dataVencimento+1 >= tabela_valor.tabelavalor_vencimentoInicio and cobranca.cobranca_dataVencimento+1 <= tabela_valor.tabelavalor_fim ",
                    "       left join tabela_valor_item on tabela_valor_item.tabelavaloritem_tabelaid = tabela_valor.tabelavalor_id and tabela_valor_item.tabelavaloritem_planoId = contrato.contrato_planoId and (datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365) between tabela_valor_item.tabelavaloritem_idadeInicio and tabela_valor_item.tabelavaloritem_idadeFim ",
                    "       left join dbo.cobranca_motivoBaixa on cobranca_motivoBaixa.motivobaixa_id = cobranca_baixa.cobrancabaixa_motivoId ",
                    "	where ",
                    "		cobranca_parcela <> 1 and cobranca_cancelada <> 1 ",
                    "		and cobranca_dataPagto between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                    "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                    "       and contrato_estipulanteId IN (", String.Join(",", estp), ")",
                    "	order by 5,6 --operadora_nome,contrato_numero,contrato_vigencia,cobranca_parcela");

            return qry;
        }

        public static String ContasReceberAbertoQUERY(DateTime dtFrom, DateTime dtTo, String[] oper, String[] estp)
        {
            String qry = String.Concat(
                    "select operadora_nome,estipulante_descricao,estipulante_carteira,contrato_numero,contratobeneficiario_numeroSequencia,beneficiario_nome,beneficiario_cpf,datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365 as IDADE,CONVERT(varchar(14), beneficiario.beneficiario_dataNascimento, 103) as beneficiario_dataNascimento,CONVERT(varchar(14), contrato_vigencia, 103) as contrato_vigencia,CONVERT(varchar(14), contrato_datacancelamento, 103) as contrato_datacancelamento,CONVERT(varchar(14), cobranca_dataVencimento, 103) as cobranca_dataVencimento,CONVERT(varchar(14), cobranca_dataPagto, 103) as cobranca_dataPagto,cobranca_valor,cobranca_valorPagto,contrato_codcobranca,cobranca_parcela,plano_descricao,CONVERT(varchar(14), cobranca_dataCriacao, 103) as cobranca_dataCriacao,",
                    "   case ",
                    "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                    "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                    "       WHEN  operadora_id = 18 THEN 'DF' ",
                    "       WHEN  operadora_id = 19 THEN 'PR' ",
                    "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                    "       WHEN  operadora_id = 24 THEN 'RN' ",
                    "       WHEN  operadora_id = 25 THEN 'PE' ",
                    "       WHEN  operadora_id = 26 THEN 'MG' ",
                    "       WHEN  operadora_id = 27 THEN 'GO' ",
                    "   end as filial, ",
                    "   case ",
                    "       WHEN  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  < 19) THEN '0  a 18 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 18) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  24) THEN '19 a 23 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 23) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  29) THEN '24 a 28 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 28) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  34) THEN '29 a 33 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 33) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  39) THEN '34 a 38 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 38) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  44) THEN '39 a 43 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 43) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  49) THEN '44 a 48 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 48) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  54) THEN '49 a 53 anos' ",
                    "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 53) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  59) THEN '54 a 58 anos' ",
                    "       ELSE '59 anos ou mais' ",
                    "   end as FaixaEtaria, ",
                    "   case ",
                    "       WHEN contratobeneficiario_numeroSequencia = 0 THEN 'TITULAR' ",
                    "       ELSE contratoAdmparentescoagregado_parentescoDescricao ", //'BANCARIA'
                    "   end as Parentesco, ",
                    "   case ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 0 then 'MANUAL' ",
                    "       when cobranca_baixa.cobrancabaixa_id is not null and cobranca_baixa.cobrancabaixa_baixaProvisoria = 1 then 'PROVISORIA' ",
                    "       else 'BANCARIA' ",
                    "   end as TipoBaixa, ",
                    "   case ",
                    "       WHEN cobranca.cobranca_tipo = 0 then 'NORMAL' ",
                    "       WHEN cobranca.cobranca_tipo = 1 then 'COMPLEMENTAR' ",
                    "       WHEN cobranca.cobranca_tipo = 2 then 'DUPLA' ",
                    "       WHEN cobranca.cobranca_tipo = 4 then 'NEGOCIACAO' ",
                    "       ELSE 'NAO IDENTIFICADO' ",
                    "   end as Tipo_Boleto, ",
                    "   case ",
                    "       WHEN cobranca.cobranca_pago = 0 then 'Em aberto' ",
                    "       ELSE 'Pago' ",
                    "   end as StatusBoleto, ",
                    "   case ",
                    "       WHEN contrato.contrato_tipoAcomodacao = 0 then 'COLETIVO' ",
                    "       ELSE 'PRIVATIVO' ",
                    "   end as Acomodacao, ",

                    "   case ",
                    "       WHEN contrato_vigencia < '2013-07-01 00:00:00' then 'PSPADRAO' ",
                    "       WHEN operadora_id = 16 then 'PSPADRAO' ",
                    "       ELSE 'QUALICORP' ",
                    "   end as Cedente ",

                    "	from contrato ",
                    "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id  and contratobeneficiario_tipo=0 ",
                    "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId ",
                    "		inner join operadora on operadora_id = contrato_operadoraId ",
                    "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join usuario on usuario_id=contrato_donoId ",
                    "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                    "       inner join cobranca on cobranca_propostaId=contrato_id ",
                    "       inner join plano on  contrato.contrato_planoId = plano_Id and contrato.contrato_contratoAdmId = plano.plano_contratoId ",
                    "       left  join contratoADM_parentesco_agregado on contratobeneficiario_parentescoId = contratoAdmparentescoagregado_Id ",
                    "       left  join adicional_beneficiario on adicionalbeneficiario_propostaId =  cobranca_propostaId ",
                    "       left join adicional on adicional.adicional_id = adicional_beneficiario.adicionalbeneficiario_adicionalid ",
                    "       left join cobranca_baixa on cobranca_baixa.cobrancabaixa_cobrancaId = cobranca.cobranca_id  ",
                    "        ",
                    "	where ",
                    "		cobranca_parcela <> 1 and cobranca_cancelada <> 1 and cobranca.cobranca_dataPagto is null and (contrato_datacancelamento > cobranca_dataVencimento or contrato_datacancelamento is null) ",
                    "		and cobranca_dataVencimento between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                    "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                    "       and contrato_estipulanteId IN (", String.Join(",", estp), ")",
                    "	order by operadora_nome,contrato_numero,contrato_vigencia,cobranca_parcela");

            return qry;
        }

        public static IList<AgendaRelatorio> CarregarTodos(Boolean somenteNaoProcessados, DateTime? dataProcessamento)
        {
            String cond = "";

            if (somenteNaoProcessados)
                cond = " WHERE agenda_processado=0 ";

            if (dataProcessamento != null)
            {
                if (cond.Length == 0)
                    cond = " WHERE ";
                else
                    cond += " AND ";

                cond += " GETDATE() >= agenda_processarEm ";
            }

            String qry = " * FROM relatorio_agendamento " + cond + " ORDER BY agenda_processarEm";

            qry = " * FROM relatorio_agendamento where agenda_id=78";

            return LocatorHelper.Instance.ExecuteQuery<AgendaRelatorio>(qry, typeof(AgendaRelatorio));
        }
    }
}
using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

using LC.Framework.Phantom;

namespace LC.Web.PadraoSeguros.Entity
{
    [DBTable("cobranca_restore")]
    public class CobrancaTemp : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            public static void FillComboCarteira(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Unibanco", "0"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Itau sem registro", "1"));
            }

            public static void FillComboFormato(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato antigo", "0"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato novo (Carnê)", "1"));
            }
        }

        public enum eTipoBanco : int
        {
            Unibanco,
            Itau
        }

        public enum eTipo : int 
        {
            Normal,
            Complementar,
            Dupla,
            Indefinido
        }

        public enum eCarteira : int 
        {
            /// <summary>
            /// 0
            /// </summary>
            Unibanco,
            /// <summary>
            /// 1
            /// </summary>
            ItauSemRegistro
        }

        #region Fields 

        Object _id;
        Object _propostaId;
        Decimal _valor;
        Decimal _valorNominal;
        DateTime _vencimento;
        DateTime _vencimentoIsencaoJuro;
        DateTime _dataCriacao;
        Boolean _pago;
        DateTime _dataPagto;
        Decimal _valorPagto;
        Object _arquivoIdUltimoEnvio;
        Int32 _parcela;
        Int32 _tipo;
        Int32 _tipoTemp;
        Object _cobrancaRefId;
        Boolean _comissaoPaga;
        Boolean _cancelada;
        Boolean _dataVencimentoForcada;
        String _nossoNumero;

        Int32 _carteira;

        String _contratoCodCobranca;
        Object _operadoraId;
        String _contratoNumero;
        String _contratoTitularNome;
        Object _contratoEnderecoCobrancaId;
        String _filialNome;
        String _operadoraNome;
        String _estipulanteNome;
        Object _contratoBeneficiarioId;
        String _contratoBeneficiarioEmail;

        Object _composicaoBeneficiarioId;
        Int32 _composicaoTipo;
        Decimal _composicaoValor;
        String _composicaoResumo;

        ArquivoRemessaCriterio _criterio;
        #endregion

        #region Properties 

        [DBFieldInfo("cobranca_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("cobranca_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaId; }
            set { _propostaId = value; }
        }

        /// <summary>
        /// Quando cobrança dupla, esta propriedade guardará o ID da cobrança que compões esta cobrança.
        /// </summary>
        [DBFieldInfo("cobranca_cobrancaRefId", FieldType.Single)]
        public Object CobrancaRefID
        {
            get { return _cobrancaRefId; }
            set { _cobrancaRefId= value; }
        }

        [DBFieldInfo("cobranca_arquivoUltimoEnvioId", FieldType.Single)]
        public Object ArquivoIDUltimoEnvio
        {
            get { return _arquivoIdUltimoEnvio; }
            set { _arquivoIdUltimoEnvio= value; }
        }

        [DBFieldInfo("cobranca_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        /// <summary>
        /// Usado em cobranças duplas, qdo necessário, para guardar o valor original da cobrança antes de ser dupla
        /// </summary>
        [DBFieldInfo("cobranca_valorNominal", FieldType.Single)]
        public Decimal ValorNominal
        {
            get { return _valorNominal; }
            set { _valorNominal= value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoIsencaoJuro", FieldType.Single)]
        public DateTime DataVencimentoISENCAOJURO
        {
            get { return _vencimentoIsencaoJuro; }
            set { _vencimentoIsencaoJuro= value; }
        }

        [DBFieldInfo("cobranca_dataVencimento", FieldType.Single)]
        public DateTime DataVencimento
        {
            get { return _vencimento; }
            set { _vencimento= value; }
        }

        [DBFieldInfo("cobranca_dataCriacao", FieldType.Single)]
        public DateTime DataCriacao
        {
            get { return _dataCriacao; }
            set { _dataCriacao= value; }
        }

        [DBFieldInfo("cobranca_pago", FieldType.Single)]
        public Boolean Pago
        {
            get { return _pago; }
            set { _pago= value; }
        }

        [DBFieldInfo("cobranca_dataPagto", FieldType.Single)]
        public DateTime DataPgto
        {
            get { return _dataPagto; }
            set { _dataPagto= value; }
        }

        [DBFieldInfo("cobranca_valorPagto", FieldType.Single)]
        public Decimal ValorPgto
        {
            get { return _valorPagto; }
            set { _valorPagto= value; }
        }

        [DBFieldInfo("cobranca_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("cobranca_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("cobranca_tipoTemp", FieldType.Single)]
        public Int32 TipoTemp
        {
            get { return _tipoTemp; }
            set { _tipoTemp = value; }
        }

        /// <summary>
        /// Indica se a comissao sobre esta cobrança já foi paga.
        /// </summary>
        [DBFieldInfo("cobranca_comissaoPaga", FieldType.Single)]
        public Boolean ComissaoPaga
        {
            get { return _comissaoPaga; }
            set { _comissaoPaga= value; }
        }

        [DBFieldInfo("cobranca_cancelada", FieldType.Single)]
        public Boolean Cancelada
        {
            get { return _cancelada; }
            set { _cancelada= value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoForcada", FieldType.Single)]
        public Boolean DataVencimentoForcada
        {
            get { return _dataVencimentoForcada; }
            set { _dataVencimentoForcada = value; }
        }

        [DBFieldInfo("cobranca_nossoNumero", FieldType.Single)]
        public String NossoNumero
        {
            get { return _nossoNumero; }
            set { _nossoNumero= value; }
        }

        [DBFieldInfo("cobranca_carteira", FieldType.Single)]
        public Int32 Carteira
        {
            get { return _carteira; }
            set { _carteira= value; }
        }

        [Joinned("contrato_codcobranca")]
        public String ContratoCodCobranca
        {
            get { return _contratoCodCobranca; }
            set { _contratoCodCobranca= value; }
        }

        [Joinned("operadora_id")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        /// <summary>
        /// O nome do titular da proposta.
        /// </summary>
        [Joinned("beneficiario_nome")]
        public String ContratoTitularNome
        {
            get { return _contratoTitularNome; }
            set { _contratoTitularNome= value; }
        }

        /// <summary>
        /// O número da proposta, do contrato impresso.
        /// </summary>
        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero= value; }
        }

        [Joinned("contrato_enderecoCobrancaId")]
        public Object ContratoEnderecoCobrancaID
        {
            get { return _contratoEnderecoCobrancaId; }
            set { _contratoEnderecoCobrancaId= value; }
        }

        [Joinned("filial_nome")]
        public String FilialNome
        {
            get { return _filialNome; }
            set { _filialNome= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulanteNome
        {
            get { return _estipulanteNome; }
            set { _estipulanteNome= value; }
        }

        [Joinned("beneficiario_id")]
        public Object BeneficiarioId
        {
            get { return _contratoBeneficiarioId; }
            set { _contratoBeneficiarioId = value; }
        }

        [Joinned("beneficiario_email")]
        public String BeneficiarioEmail
        {
            get { return _contratoBeneficiarioEmail; }
            set { _contratoBeneficiarioEmail = value; }
        }

        [Joinned("cobrancacomp_beneficiarioId")]
        public Object ComposicaoBeneficiarioID
        {
            get { return _composicaoBeneficiarioId; }
            set { _composicaoBeneficiarioId = value; }
        }

        [Joinned("cobrancacomp_tipo")]
        public Int32 ComposicaoTipo
        {
            get { return _composicaoTipo; }
            set { _composicaoTipo= value; }
        }

        [Joinned("cobrancacomp_valor")]
        public Decimal ComposicaoValor
        {
            get { return _composicaoValor; }
            set { _composicaoValor= value; }
        }

        public String ComposicaoResumo
        {
            get { return _composicaoResumo; }
        }

        public String STRNossoNumero
        {
            get
            {
                if (this._dataCriacao.Year == 2013)
                    return geraNossoNumeroItau();
                else
                    return geraNossoNumeroUnibanco();
            }
        }

        public String strPago
        {
            get { if (_pago) { return "Sim"; } else { return "Não"; } }
        }

        public String strDataPago
        {
            get
            {
                if (_dataPagto != DateTime.MinValue)
                    return _dataPagto.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }

        public String strEnviado
        {
            get { if (_arquivoIdUltimoEnvio == null) { return "Não"; } else { return "Sim"; } }
        }

        public static Boolean NossoNumeroITAU
        {
            get
            {
                String tipo = ConfigurationManager.AppSettings["tipoNossoNumero"];
                if (String.IsNullOrEmpty(tipo)) { return false; }
                if (tipo.ToLower() == "itau")
                    return true;
                else
                    return false;
            }
        }

        public ArquivoRemessaCriterio Criterio
        {
            get { return _criterio; }
            set { _criterio= value; }
        }

        /// <summary>
        /// No caso do boleto itaú, essa propriedade retorna o parâmetro necessário para ser enviado 
        /// ao boletomail.
        /// </summary>
        public static String BoletoUrlComp
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["boletoMailUrlParam"]))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Concat("&", ConfigurationManager.AppSettings["boletoMailUrlParam"]);
                }
            }
        }

        #endregion

        public CobrancaTemp(Object id) : this() { _id = id; }
        public CobrancaTemp() { _dataCriacao = DateTime.Now; _pago = false; _cancelada = false; _carteira = (Int32)eCarteira.Unibanco; _dataVencimentoForcada = false; }

        #region EntityBase methods 

        public void Salvar()
        {
            if (((eTipo)this._tipo) == eTipo.Dupla && this._valorNominal == 0)
            {
                //denis - ta vindo null
                if (this._cobrancaRefId != null)
                {
                    Cobranca cobrancaRef = new Cobranca(this._cobrancaRefId);
                    cobrancaRef.Carregar();
                    this._valorNominal = this._valor - cobrancaRef.Valor;
                }
                else
                {
                    Cobranca cobrancaRef = new Cobranca();
                }
            }

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

        /// <summary>
        /// Para o unibanco, gera com DV. Para o itau, gera SEM o DV.
        /// </summary>
        public String GeraNossoNumero()
        {
            if (!Cobranca.NossoNumeroITAU)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this._tipo);
                sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

                String dv = this.CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

                sb.Append(_parcela.ToString().PadLeft(3, '0'));
                sb.Append(dv);

                String nossonumero = sb.ToString();
                sb.Remove(0, sb.Length);
                sb = null;
                return nossonumero;
            }
            else
            {
                String nossonumero = Convert.ToString(this._id).PadLeft(8, '0');
                //nossonumero = String.Concat(nossonumero, CalculaDVMod11(nossonumero));
                return nossonumero;
            }
        }

        String geraNossoNumeroItau()
        {
            return Convert.ToString(this._id).PadLeft(8, '0');
        }
        String geraNossoNumeroUnibanco()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this._tipo);
            sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

            String dv = this.CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

            sb.Append(_parcela.ToString().PadLeft(3, '0'));
            sb.Append(dv);

            String nossonumero = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return String.Format("{0:" +  new String('0', 12) + "}", Convert.ToInt64(nossonumero));
        }

        internal String CalculaDVMod11(Int32 tipo, String contratoCodCobranca, Int32 parcela)
        {
            StringBuilder sb = new StringBuilder();

            if (!Cobranca.NossoNumeroITAU)
            {
                sb.Append(tipo);
                sb.Append(contratoCodCobranca.PadLeft(10, '0'));
            }
            else
            {
                sb.Append(Convert.ToString(this._id).PadLeft(11, '0'));
            }

            return this.CalculaDVMod11(sb.ToString() + parcela.ToString().PadLeft(3, '0'));
        }

        internal String CalculaDVMod11(String nossoNumero)
        {
            Int32 fatorMult = 2;
            Int32 resultado = 0;

            char[] buffer = nossoNumero.ToCharArray();
            Array.Reverse(buffer);
            String nossoNumeroReverso = new String(buffer);

            for (int i = 0; i < nossoNumeroReverso.Length; i++)
            {
                resultado += Convert.ToInt32(nossoNumeroReverso.Substring(i, 1)) * fatorMult;
                fatorMult++;
                if (fatorMult > 9) { fatorMult = 2; }
            }

            resultado *= 10;
            resultado %= 11;
            resultado %= 10;

            return resultado.ToString();
        }

        public void LeNossoNumero(String nossoNumero)
        {
            if (!Cobranca.NossoNumeroITAU)
            {
                Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
            }
            else
            {
                this._id = Convert.ToInt32(nossoNumero);
            }
        }

        public void LeNossoNumeroUNIBANCO(String nossoNumero)
        {
            Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
        }

        public static void LeNossoNumero(String nossoNumero, out Int32 tipo, out String codCobranca, out Int32 parcela)
        {
            //tipo = 0; propostaId = 0; parcela = 0;
            try
            {
                tipo = Convert.ToInt32(nossoNumero.Substring(0, 1));
                codCobranca = Convert.ToInt32(nossoNumero.Substring(1, 10)).ToString();
                parcela = Convert.ToInt32(nossoNumero.Substring(11, 3));
            }
            catch
            {
                tipo = (int)Cobranca.eTipo.Indefinido;
                codCobranca = null;
                parcela = -1;
            }
        }


        static Decimal toDecimal(Object param)
        {
            if (param == DBNull.Value || param == null)
                return Decimal.Zero;
            else
            {
                try
                {
                    return Convert.ToDecimal(param);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }
        }
        static DateTime toDateTime(Object param)
        {
            if (param == DBNull.Value || param == null)
                return DateTime.MinValue;
            else
            {
                try
                {
                    return Convert.ToDateTime(param);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        static Object toObject(Object param)
        {
            if (param == DBNull.Value)
                return null;
            else
                return param;
        }
        static Boolean toBoolean(Object param)
        {
            if (param == DBNull.Value || param == null)
                return false;
            else
            {
                try
                {
                    return Convert.ToBoolean(param);
                }
                catch
                {
                    return false;
                }
            }
        }
        static Int32 toInt(Object param)
        {
            if (param == DBNull.Value || param == null)
                return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(param);
                }
                catch
                {
                    return 0;
                }
            }
        }
        static String toString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return null;
            else
                return Convert.ToString(param);
        }

        public static IList<CobrancaTemp> CarregaTodasQueNaoExistam()
        {
            string qry = "select * from cobranca_restore r where r.cobranca_id not in (select cobranca_id from cobranca where cobranca_dataVencimento > '2013-05-01 00:00:00.000' and cobranca_dataPagto IS NULL) and r.cobranca_dataVencimento > '2013-05-01 00:00:00.000' and r.cobranca_dataPagto IS NULL  AND r.cobranca_propostaId IN (SELECT CONTRATO_ID FROM CONTRATO WHERE CONTRATO.contrato_operadoraId = 10) ";

            qry = "select * from cobranca_restore r where r.cobranca_id not in  (select cobranca_id from cobranca where cobranca_dataVencimento > '2013-05-01 00:00:00.000' and cobranca_dataPagto IS NULL) and r.cobranca_dataVencimento > '2013-05-01 00:00:00.000' and r.cobranca_dataPagto IS NULL AND r.cobranca_propostaId IN (SELECT CONTRATO_ID FROM CONTRATO WHERE CONTRATO.contrato_operadoraId = 10) order by r.cobranca_id desc";

            return LocatorHelper.Instance.ExecuteQuery<CobrancaTemp>(qry, typeof(CobrancaTemp));
        }
    }
}

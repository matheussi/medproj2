namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("contrato")]
    public class Contrato : EntityBase, IPersisteableEntity
    {
        public enum eTipoAcomodacao : int
        {
            quartoComun = 0,
            quartoParticular,
            indefinido
        }
        public enum eStatus : int
        {
            Normal,
            /// <summary>
            /// Proposta importada com o status 'Nao Implantada'.
            /// </summary>
            NaoImplantadoNaImportacao
        }

        #region fields 

        Object _id;
        Object _filialId;
        Object _estipulanteId;
        Object _operadoraId;
        Object _contratoadmId;
        Object _planoId;
        Object _donoId; //corretor que vendeu a proposta
        Object _usuarioId;

        String _corretorTerceiroNome;
        String _corretorTerceiroCPF;
        String _superiorTerceiroNome;
        String _superiorTerceiroCPF;

        Object _operadorTmktId;
        Object _tipoContratoId;
        Object _enderecoReferenciaId;
        Object _enderecoCobrancaId;
        String _numero;
        Object _numeroId; //é o id da proposta, do impresso registrado no almoxarifado.
        String _vingencia;
        String _numeroMatricula;
        Decimal _valorAto;
        Boolean _adimplente;
        Boolean _cobrarTaxaAssociativa;
        DateTime _data;
        DateTime _dataCancelamento;
        String _emailCobranca;

        String _responsavelNome;
        String _responsavelCPF;
        String _responsavelRG;
        DateTime _responsavelDataNascimento;
        String _responsavelSexo;
        Object _responsavelParentescoId;
        Int32 _tipoAcomodacao;

        DateTime _admissao;
        DateTime _vigencia;
        DateTime _vencimento;

        String _empresaAnterior;
        String _empresaAnteriorMatricula;
        Int32 _empresaAnteriorTempo;
        Boolean _rascunho;
        Boolean _cancelado;
        Boolean _inativo;
        Boolean _pendente;
        String _obs;
        DateTime _alteracao;
        Int32 _codigoCobranca;
        Decimal _desconto;
        Int32 _status;

        int _tipo;
        int _vidas;

        string _senha;
        bool _importado;


        String _planoDescricao;
        string _estipulantDescricao;
        String _operadoraDescricao;
        String _beneficiarioTitularNome;
        String _beneficiarioTitularNomeMae;
        DateTime _beneficiarioTitularDataNascimento;
        String _beneficiarioTipo;
        String _beneficiarioCpf;

        Object _contratobeneficiario_beneficiarioId;

        String _empresaCobrancaNome;

        #endregion

        #region propriedades 

        [DBFieldInfo("contrato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contrato_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("contrato_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId= value; }
        }

        [DBFieldInfo("contrato_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("contrato_contratoAdmId", FieldType.Single)]
        public Object ContratoADMID
        {
            get { return _contratoadmId; }
            set { _contratoadmId= value; }
        }

        [DBFieldInfo("contrato_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId= value; }
        }

        [DBFieldInfo("contrato_tipoContratoId", FieldType.Single)]
        public Object TipoContratoID
        {
            get { return _tipoContratoId; }
            set { _tipoContratoId= value; }
        }

        /// <summary>
        /// Corretor que vendeu a proposta
        /// </summary>
        [DBFieldInfo("contrato_donoId", FieldType.Single)]
        public Object DonoID
        {
            get { return _donoId; }
            set { _donoId= value; }
        }

        /// <summary>
        /// Corretor comissionado. Demanda JAN.
        /// </summary>
        //[DBFieldInfo("contrato_corretorComissionadoId", FieldType.Single)]
        //public object CorretorComissionadoId
        //{
        //    get;
        //    set;
        //}

        [DBFieldInfo("contrato_corretorTerceiroNome", FieldType.Single)]
        public String CorretorTerceiroNome
        {
            get { return _corretorTerceiroNome; }
            set { _corretorTerceiroNome= value; }
        }
        [DBFieldInfo("contrato_corretorTerceiroCPF", FieldType.Single)]
        public String CorretorTerceiroCPF
        {
            get { return _corretorTerceiroCPF; }
            set { _corretorTerceiroCPF= value; }
        }

        [DBFieldInfo("contrato_superiorTerceiroNome", FieldType.Single)]
        public String SuperiorTerceiroNome
        {
            get { return _superiorTerceiroNome; }
            set { _superiorTerceiroNome= value; }
        }
        [DBFieldInfo("contrato_superiorTerceiroCPF", FieldType.Single)]
        public String SuperiorTerceiroCPF
        {
            get { return _superiorTerceiroCPF; }
            set { _superiorTerceiroCPF= value; }
        }

        /// <summary>
        /// Operador de telemarketing que participou ou propiciou a venda.
        /// </summary>
        [DBFieldInfo("contrato_operadorTmktId", FieldType.Single)]
        public Object OperadorTmktID
        {
            get { return _operadorTmktId; }
            set { _operadorTmktId= value; }
        }

        [DBFieldInfo("contrato_enderecoReferenciaId", FieldType.Single)]
        public Object EnderecoReferenciaID
        {
            get { return _enderecoReferenciaId; }
            set { _enderecoReferenciaId= value; }
        }

        [DBFieldInfo("contrato_enderecoCobrancaId", FieldType.Single)]
        public Object EnderecoCobrancaID
        {
            get { return _enderecoCobrancaId; }
            set { _enderecoCobrancaId= value; }
        }

        /// <summary>
        /// Número do contrato.
        /// </summary>
        [DBFieldInfo("contrato_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero= value; }
        }

        [DBFieldInfo("contrato_emailCobranca", FieldType.Single)]
        public String EmailCobranca
        {
            get { return _emailCobranca; }
            set { _emailCobranca= value; }
        }

        /// <summary>
        /// É o ID da proposta, do impresso registrado no almoxarifado.
        /// </summary>
        [DBFieldInfo("contrato_numeroId", FieldType.Single)]
        public Object NumeroID
        {
            get { return _numeroId; }
            set { _numeroId= value; }
        }

        [Obsolete("Em desuso.", true)]
        [DBFieldInfo("contrato_vingencia", FieldType.Single)]
        public String Vingencia
        {
            get { return _vingencia; }
            set { _vingencia= value; }
        }

        /// <summary>
        /// Número da matrícula.
        /// </summary>
        [DBFieldInfo("contrato_numeroMatricula", FieldType.Single)]
        public String NumeroMatricula
        {
            get { return _numeroMatricula; }
            set { _numeroMatricula= value; }
        }

        [DBFieldInfo("contrato_valorAto", FieldType.Single)]
        public Decimal ValorAto
        {
            get { return _valorAto; }
            set { _valorAto= value; }
        }

        /// <summary>
        /// Informa se o contrato está adimplente.
        /// </summary>
        [DBFieldInfo("contrato_adimplente", FieldType.Single)]
        public Boolean Adimplente
        {
            get { return _adimplente; }
            set { _adimplente= value; }
        }

        [DBFieldInfo("contrato_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        /// <summary>
        /// Usuário que digitou a proposta.
        /// </summary>
        [DBFieldInfo("contrato_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("contrato_dataCancelamento", FieldType.Single)]
        public DateTime DataCancelamento
        {
            get { return _dataCancelamento; }
            set { _dataCancelamento= value; }
        }

        [DBFieldInfo("contrato_responsavelNome", FieldType.Single)]
        public String ResponsavelNome
        {
            get { return _responsavelNome; }
            set { _responsavelNome= value; }
        }

        [DBFieldInfo("contrato_responsavelCPF", FieldType.Single)]
        public String ResponsavelCPF
        {
            get { return _responsavelCPF; }
            set { _responsavelCPF= value; }
        }

        [DBFieldInfo("contrato_responsavelRG", FieldType.Single)]
        public String ResponsavelRG
        {
            get { return _responsavelRG; }
            set { _responsavelRG= value; }
        }

        [DBFieldInfo("contrato_responsavelDataNascimento", FieldType.Single)]
        public DateTime ResponsavelDataNascimento
        {
            get { return _responsavelDataNascimento; }
            set { _responsavelDataNascimento= value; }
        }

        [DBFieldInfo("contrato_responsavelSexo", FieldType.Single)]
        public String ResponsavelSexo
        {
            get { return _responsavelSexo; }
            set { _responsavelSexo= value; }
        }

        [DBFieldInfo("contrato_responsavelParentescoId", FieldType.Single)]
        public Object ResponsavelParentescoID
        {
            get { return _responsavelParentescoId; }
            set { _responsavelParentescoId= value; }
        }

        [DBFieldInfo("contrato_tipoAcomodacao", FieldType.Single)]
        public Int32 TipoAcomodacao
        {
            get { return _tipoAcomodacao; }
            set { _tipoAcomodacao= value; }
        }

        [DBFieldInfo("contrato_admissao", FieldType.Single)]
        public DateTime Admissao
        {
            get { return _admissao; }
            set { _admissao= value; }
        }

        [DBFieldInfo("contrato_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia= value; }
        }

        [DBFieldInfo("contrato_vencimento", FieldType.Single)]
        public DateTime Vencimento
        {
            get { return _vencimento; }
            set { _vencimento= value; }
        }

        [DBFieldInfo("contrato_empresaAnterior", FieldType.Single)]
        public String EmpresaAnterior
        {
            get { return _empresaAnterior; }
            set { _empresaAnterior= value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorMatricula", FieldType.Single)]
        public String EmpresaAnteriorMatricula
        {
            get { return _empresaAnteriorMatricula; }
            set { _empresaAnteriorMatricula= value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorTempo", FieldType.Single)]
        public Int32 EmpresaAnteriorTempo
        {
            get { return _empresaAnteriorTempo; }
            set { _empresaAnteriorTempo= value; }
        }

        [DBFieldInfo("contrato_rascunho", FieldType.Single)]
        public Boolean Rascunho
        {
            get { return _rascunho; }
            set { _rascunho= value; }
        }

        [DBFieldInfo("contrato_cancelado", FieldType.Single)]
        public Boolean Cancelado
        {
            get { return _cancelado; }
            set { _cancelado= value; }
        }

        [DBFieldInfo("contrato_inativo", FieldType.Single)]
        public Boolean Inativo
        {
            get { return _inativo; }
            set { _inativo= value; }
        }

        /// <summary>
        /// Quando um novo contrato é cadastrado, e um ou mais beneficiários têm algum item de saúde marcado,
        /// a proposta fica pendente, dependendo de análise técnica.
        /// </summary>
        [DBFieldInfo("contrato_pendente", FieldType.Single)]
        public Boolean Pendente
        {
            get { return _pendente; }
            set { _pendente= value; }
        }

        [DBFieldInfo("contrato_cobrarTaxaAssociativa", FieldType.Single)]
        public Boolean CobrarTaxaAssociativa
        {
            get { return _cobrarTaxaAssociativa; }
            set { _cobrarTaxaAssociativa= value; }
        }

        [DBFieldInfo("contrato_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("contrato_codcobranca", FieldType.Single)]
        public Int32 CodCobranca
        {
            get { return _codigoCobranca; }
            set { _codigoCobranca= value; }
        }

        [DBFieldInfo("contrato_alteracao", FieldType.Single)]
        public DateTime Alteracao
        {
            get { return _alteracao; }
            set { _alteracao= value; }
        }

        [DBFieldInfo("contrato_desconto", FieldType.Single)]
        public Decimal Desconto
        {
            get { return _desconto; }
            set { _desconto= value; }
        }

        [DBFieldInfo("contrato_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status= value; }
        }

        /// <summary>
        /// Informa se o contrato pertence a pessoa fisica ou juridica. Enumeracao Entity.eTipoPessoa
        /// </summary>
        [DBFieldInfo("contrato_tipoPessoa", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("contrato_qtdVidas", FieldType.Single)]
        public Int32 Vidas
        {
            get { return _vidas; }
            set { _vidas = value; }
        }

        [DBFieldInfo("contrato_senha", FieldType.Single)]
        public String Senha
        {
            get { return _senha; }
            set { _senha= value; }
        }

        DateTime _dataValidade;
        [DBFieldInfo("contrato_validade", FieldType.Single)]
        public DateTime DataValidade
        {
            get { return _dataValidade; }
            set { _dataValidade= value; }
        }

        [DBFieldInfo("contrato_importado", FieldType.Single)]
        public bool Importado
        {
            get { return _importado; }
            set { _importado = value; }
        }

        /// <summary>
        /// 1 para Acrescimo ; 2 para desconto
        /// </summary>
        [DBFieldInfo("contrato_descascTipo", FieldType.Single)]
        public int DescontoAcrescimoTipo
        {
            get;
            set;
        }

        [DBFieldInfo("contrato_descascValor", FieldType.Single)]
        public decimal DescontoAcrescimoValor
        {
            get;
            set;
        }

        [DBFieldInfo("contrato_descascDataFinal", FieldType.Single)]
        public DateTime DescontoAcrescimoData
        {
            get;
            set;
        }

        [DBFieldInfo("contrato_ramo", FieldType.Single)]
        public string Ramo { get; set; }

        [DBFieldInfo("contrato_numeroApolice", FieldType.Single)]
        public string Apolice { get; set; }


        [DBFieldInfo("contrato_utilizarIugu", FieldType.Single)]
        public bool IuguHabilitado
        {
            get;
            set;
        }

        [DBFieldInfo("contrato_iugu_custumerid", FieldType.Single)]
        public string IuguCustumerId
        { 
            get; 
            set; 
        }

        [DBFieldInfo("contrato_iugu_subscriptionid", FieldType.Single)]
        public string IuguSubscriptionId
        {
            get;
            set;
        }

        [Joinned("plano_descricao")]
        public String PlanoDescricao
        {
            get { return _planoDescricao; }
            set { _planoDescricao= value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulantDescricao
        {
            get { return _estipulantDescricao; }
            set { _estipulantDescricao = value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraDescricao
        {
            get { return _operadoraDescricao; }
            set { _operadoraDescricao= value; }
        }

        [Joinned("contratobeneficiario_beneficiarioId")]
        public Object ContratoBeneficiarioId
        {
            get { return _contratobeneficiario_beneficiarioId; }
            set { _contratobeneficiario_beneficiarioId = value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioTitularNome
        {
            get { return _beneficiarioTitularNome; }
            set { _beneficiarioTitularNome= value; }
        }

        [Joinned("beneficiario_nomeMae")]
        public String BeneficiarioTitularNomeMae
        {
            get { return _beneficiarioTitularNomeMae; }
            set { _beneficiarioTitularNomeMae= value; }
        }

        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioTitularDataNascimento
        {
            get { return _beneficiarioTitularDataNascimento; }
            set { _beneficiarioTitularDataNascimento= value; }
        }

        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf= value; }
        }

        public String BeneficiarioCPFFormatado
        {
            get
            {
                if (String.IsNullOrEmpty(_beneficiarioCpf)) { return _beneficiarioCpf; }
                return String.Format(@"{0:000\.000\.000\-00}", Convert.ToInt64(_beneficiarioCpf));
            }
        }

        /// <summary>
        /// Se o beneficiário é titular ou dependente.
        /// </summary>
        [Joinned("contratobeneficiario_tipo")]
        public String TipoParticipacaoContrato
        {
            get { return _beneficiarioTipo; }
            set { _beneficiarioTipo= value; }
        }

        [Joinned("empresa_nome")]
        public String EmpresaCobranca
        {
            get { return _empresaCobrancaNome; }
            set { _empresaCobrancaNome= value; }
        }

        /// <summary>
        /// Condição para retornar, em uma query sql, apenas contratos não cancelados, ativos e que não sejam rascunhos.
        /// </summary>
        internal static String CondicaoBasicaQuery
        {
            get
            {
                return " contrato_cancelado <> 1 AND contrato_inativo <> 1 AND contrato_rascunho <> 1 "; //contrato_adimplente=1 AND 
            }
        }

        #endregion

        public virtual string GerarSenha()
        {
            Random r1 = new Random(DateTime.Now.Millisecond);

            this.Senha = string.Concat(r1.Next(1, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));

            r1 = null;

            return this.Senha;
        }

        public ContratoBeneficiario ContratoBeneficiario
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Plano Plano 
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Beneficiario Beneficiario 
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Estipulante Estipulante 
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Agente Agente 
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Contrato()
        {
            _tipo = (int)eTipoPessoa.Fisica;
            _importado = false; _inativo = false; _cancelado = false; _adimplente = true; _pendente = false;
            _responsavelDataNascimento = DateTime.MinValue; _alteracao = DateTime.Now; _status = 0;
        }
        public Contrato(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String query = "delete from contrato_beneficiario WHERE contratobeneficiario_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_valor where contratovalor_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_plano_historico where contratoplano_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from cobranca where cobranca_propostaId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_beneficiario where contratobeneficiario_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from adicional_beneficiario where adicionalbeneficiario_propostaid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            pm.Remove(this);

            pm.CloseSingleCommandInstance();
            pm = null;
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public static Contrato CarregarParcial(Object id, PersistenceManager pm)
        {
            String qry = "contrato_id,contrato_numero, contrato_operadoraid, contrato_estipulanteId, contrato_cobrarTaxaAssociativa, contrato_contratoAdmId, contrato_admissao, contrato_vigencia,contrato_codcobranca,contrato_inativo,contrato_cancelado,contrato_dataCancelamento,contrato_responsavelNome,contrato_responsavelCPF FROM contrato WHERE contrato_id=" + id; ;
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId)
        {
            return CarregarParcial(propostaNumero, operadoraId, null);
        }
        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId, PersistenceManager pm)
        {
            String qry = "contrato_id, contrato_numero, contrato_contratoAdmId,contrato_admissao,contrato_planoId,contrato_inativo,contrato_status FROM contrato WHERE contrato_numero=@numero AND contrato_operadoraId=" + operadoraId;

            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { propostaNumero };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, names, value, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcialPorCodCobranca(Object codCobranca, PersistenceManager pm)
        {
            IList<Contrato> lista = LocatorHelper.Instance.ExecuteQuery<Contrato>("contrato_id, contrato_operadoraId, contrato_numero, contrato_contratoAdmId, contrato_admissao, contrato_inativo, contrato_cancelado, contrato_adimplente, contrato_datacancelamento FROM contrato WHERE contrato_codCobranca=" + codCobranca, typeof(Contrato));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        #endregion

        public void AtualizarIuguCustomerId(PersistenceManager pm = null)
        {
            NonQueryHelper.Instance.ExecuteNonQuery(
                string.Concat("update contrato set contrato_iugu_custumerid='", this.IuguCustumerId, "' where contrato_id=", this.ID),
                pm);
        }

        public void AtualizarIuguSubscriptonId(PersistenceManager pm = null)
        {
            NonQueryHelper.Instance.ExecuteNonQuery(
                string.Concat("update contrato set contrato_iugu_subscriptionid='", this.IuguSubscriptionId, "' where contrato_id=", this.ID),
                pm);
        }

        public static Contrato CarregarPorParametros(String numeroContrato, Object operadoraId, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT * ",
                "   FROM contrato ",
                "   WHERE ",
                "       contrato_numero = @NumeroContrato AND ",
                "       contrato_operadoraId = ", operadoraId);

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato), pm);

            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static IList<Contrato> CarregarPorParametros(String numeroContrato, Object operadoraId, DateTime vigencia, String titularCpf, String titularNome)
        {
            String qry = String.Concat("SELECT contrato.*, beneficiario_nome, beneficiario_cpf, beneficiario_nomeMae, beneficiario_dataNascimento ",
                "   FROM contrato ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "   WHERE ",
                "       contrato_operadoraId = ", operadoraId);

            List<String> paramNames  = new List<String>();
            List<String> paramValues = new List<String>();

            String[] pName  = null;
            String[] pValue = null;

            if (!String.IsNullOrEmpty(numeroContrato))
            {
                qry += " AND contrato_numero = @NumeroContrato ";
                paramNames.Add("@NumeroContrato");
                paramValues.Add(numeroContrato);
            }

            if (vigencia != DateTime.MinValue)
            {
                qry = String.Concat(qry, " AND DAY(contrato_vigencia)=", vigencia.Day, " AND MONTH(contrato_vigencia)=", vigencia.Month, " AND YEAR(contrato_vigencia)=", vigencia.Year);
            }

            if (!String.IsNullOrEmpty(titularCpf))
            {
                qry = String.Concat(qry, " AND beneficiario_cpf='", titularCpf,"'");
            }

            if (!String.IsNullOrEmpty(titularNome))
            {
                qry += " AND beneficiario_nome LIKE @TitularNome ";
                paramNames.Add("@TitularNome");
                paramValues.Add(titularNome + "%");
            }

            if (paramNames.Count > 0)
            {
                pName  = paramNames.ToArray();
                pValue = paramValues.ToArray();
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorNumero(String numero, PersistenceManager pm)
        {
            String[] values = null;
            String[] pnames = null;

            pnames = new String[1] { "@contrato_numero" };
            values = new String[1] { numero };

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_numero=@contrato_numero",
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato), pm);
        }

        public static IList<Contrato> CarregarPorCPFTitular(String cpf, PersistenceManager pm)
        {
            String[] values = null;
            String[] pnames = null;

            pnames = new String[1] { "@beneficiario_cpf" };
            values = new String[1] { cpf };

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_cpf=@beneficiario_cpf and contrato_inativo=0 and contrato_cancelado=0 ",
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato), pm);
        }

        public static IList<Contrato> CarregarPorIDContrato(long ID, PersistenceManager pm)
        {
            String[] values = null;
            String[] pnames = null;

            pnames = new String[1] { "@contrato_id" };
            values = new String[1] { ID.ToString() };

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_id=@contrato_id ",
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato), pm);
        }

        public static IList<Contrato> CarregarPorParametros(String benficiarioNome)
        {
            if (string.IsNullOrEmpty(benficiarioNome)) benficiarioNome = "";

            //String[] values = null;
            //String[] pnames = null;

            //pnames = new String[1] { "@beneficiario_nome" };
            //values = new String[1] { "%" + benficiarioNome + "%" };

            //String query = String.Concat("contrato_id,contrato_numero, beneficiario_nome ",
            //    "FROM contrato ",
            //    " inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
            //    " inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
            //    "WHERE beneficiario_nome LIKE @beneficiario_nome or contrato_numero like @beneficiario_nome ", 
            //    " ORDER BY beneficiario_nome ");


            String query = String.Concat("contrato_id,contrato_numero, beneficiario_nome ",
                "FROM contrato ",
                " inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE '%", benficiarioNome, "%'  or contrato_numero like '%", benficiarioNome, "%'",
                " ORDER BY beneficiario_nome ");

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(query, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome)
        {
            String whereAnd = "";
            String[] values = null;
            String[] pnames = null;

            if (!String.IsNullOrEmpty(numero))
            {
                //numero = String.Format("{0:0000000000}", Convert.ToInt32(numero));
                whereAnd = " AND contrato_numero=@contrato_numero ";
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numero, "%" + benficiarioNome + "%" };
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "%" + benficiarioNome + "%" };
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 50 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorParametros(numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId= " + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " and beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE (beneficiario_nome LIKE @beneficiario_nome) ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String cpf, long associadoPjId)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@beneficiario_cpf", "@contrato_estipulanteId" };
            String[] values = { numero, "%" + benficiarioNome + "%", cpf, associadoPjId.ToString() };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (associadoPjId > 0)
            {
                whereAnd += " AND contrato_estipulanteId=@contrato_estipulanteId ";
            }

            //if (somenteInativos)
            //{
            //    whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            //}

            //if (somenteAtivos)
            //{
            //    whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            //}

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " and beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 100 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE (beneficiario_nome LIKE @beneficiario_nome) ", whereAnd,
                " ORDER BY beneficiario_nome");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        public static DataTable DTCarregarPorRegraComissionamento(string regraComissionamentoId)
        {
            String query = String.Concat("select contrato_id as ID,contrato_numero as Numero, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "inner join regraComissao_contrato on contrato_id = regracomissaocontrato_contratoId ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE regracomissaocontrato_regraId= ", regraComissionamentoId,
                " ORDER BY beneficiario_nome");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, null, null).Tables[0];
        }

        /// <summary>
        /// Carrega o contrato e seus beneficiários (todos os beneficiários ativos, titular e dependentes)
        /// </summary>
        /// <param name="operadoraId">ID da operadora à qual pertence o contrato.</param>
        /// <param name="numeroContrato">Número do contrato.</param>
        /// <param name="cpf">TODO: Se fornecido um cpf, somente o beneficiário dono dele será carregado.</param>
        /// <returns></returns>
        public static IList<Contrato> Carregar(Object operadoraId, String numeroContrato, String cpf)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_numero=@NumeroContrato AND ",
                "       contrato_operadoraId=", operadoraId);

            String[] pName  = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> Carregar(Object contratoId)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_id=", contratoId);

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorBeneficiário(Object beneficiarioId)
        {
            String query = String.Concat("contrato_id, contrato_numero, estipulante_descricao, operadora_nome, contrato_data, contratobeneficiario_tipo FROM contrato ",
                "INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN operadora ON contrato_operadoraId=operadora_id ",
                "INNER JOIN estipulante ON contrato_estipulanteId=estipulante_id ",
                "LEFT JOIN cobranca_empresa ON contrato_empresaCobrancaId=empresa_id ",
                "WHERE beneficiario_id=", beneficiarioId, " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(query, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome)
        {
            return CarregarPorOperadoraID(operadoraId, numero, benficiarioNome, null);

            /*
            String whereAnd = "";
            String[] values = null;
            String[] pnames = null;

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numero, "%" + benficiarioNome + "%" };
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "%" + benficiarioNome + "%" };
            }

            String query = String.Concat("TOP 100 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
            */
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            if (codCobranca == null) { codCobranca = ""; }
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = new String[3] { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("TOP 100 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarContratoIUGU()
        {
            //String whereAnd = "";
            //if (codCobranca == null) { codCobranca = ""; }
            //if (benficiarioNome == null) { benficiarioNome = ""; }
            //String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            //String[] values = new String[3] { numero, "%" + benficiarioNome + "%", codCobranca };

            //if (!String.IsNullOrEmpty(numero))
            //{
            //    whereAnd = " AND contrato_numero=@contrato_numero ";
            //}

            //if (!String.IsNullOrEmpty(codCobranca))
            //{
            //    whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            //}

            String query = String.Concat("contrato.*, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "INNER JOIN contratoadm ON contrato_contratoadmid=contratoadm_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_utilizarIugu=1 ",
                " ORDER BY beneficiario_nome DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, null, null, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = new String[4] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 60 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorOperadoraID(operadoraId, numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = new String[5] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId=" + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " AND beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes()
        {
            return BuscarECarregarInadimplentes_PORCOBRANCA(null);
        }
        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <param name="pm">Objeto PersistenceManager participante de uma transação.</param>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes_PORCOBRANCA(PersistenceManager pm)
        {
            String qry = String.Concat("SELECT DISTINCT(cobranca_propostaId) AS ContratoID, contrato_id,contrato_estipulanteId,contrato_operadoraId,contrato_contratoAdmId,contrato_planoId,contrato_tipoContratoId,contrato_donoId,contrato_enderecoReferenciaId,contrato_enderecoCobrancaId,contrato_numero,contrato_numeroId,contrato_adimplente,contrato_cobrarTaxaAssociativa, contrato_tipoAcomodacao,contrato_admissao,contrato_vigencia,contrato_vencimento ",
                "FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "WHERE ",
                "   contrato_cancelado <> 1 AND ",
                "   contrato_rascunho <> 1 AND ",
                "   cobranca_pago=0 AND ",
                "   cobranca_datavencimento IS NOT NULL AND ",
                " cobranca_datavencimento < GETDATE()");

            using (DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0])
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Contrato> lista = new List<Contrato>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Contrato contrato = new Contrato(dr["contrato_id"]);
                        contrato.EstipulanteID = dr["contrato_estipulanteId"];
                        contrato.OperadoraID = dr["contrato_operadoraId"];
                        contrato.ContratoADMID = dr["contrato_contratoAdmId"];
                        contrato.PlanoID = dr["contrato_planoId"];
                        contrato.TipoContratoID = dr["contrato_tipoContratoId"];
                        contrato.DonoID = dr["contrato_donoId"];

                        lista.Add(contrato);
                    }

                    return lista;
                }
                else
                    return null;
            }

            // LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
        }

        public static void SetaUsuarioLiberador(Object contratoId, Object usuarioId, PersistenceManager pm)
        {
            String command = String.Concat(
                "UPDATE contrato SET contrato_usuarioLiberador=", 
                usuarioId, " WHERE contrato_id = ", contratoId);

            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        /// <summary>
        /// Checa se o número de contrato não está sendo usado por outro contrato.
        /// </summary>
        public static Boolean ContratoDisponivel(Object contratoId, Object operadoraId, String numero, ref String msgRetorno)
        {
            String query = "SELECT contrato_id FROM contrato WHERE contrato_rascunho=0 AND contrato_operadoraId=" + operadoraId + " AND contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            if (!valido)
            {
                msgRetorno = "Número de contrato já está em uso para essa operadora.";
            }

            return valido;
        }

        public static Boolean NumeroJaUtilizado(String numero, Object contratoId)
        {
            String query = "SELECT top 1 contrato_id FROM contrato WHERE contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static Boolean NumeroDeContratoEmUso(String numero, String letra, Int32 zerosAEsquerda, Object operadoraId, PersistenceManager pm)
        {
            String _numero = EntityBase.GeraNumeroDeContrato(Convert.ToInt32(numero), zerosAEsquerda, letra);

            String qry = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero=@NUM";
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, new String[] { "@NUM" }, new String[] { _numero }, typeof(Contrato), pm);
            return ret != null;
        }

        public static Boolean CanceladoOuInativo(Object contratoId)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE (contrato_inativo=1 OR contrato_cancelado=1) and contrato_id=" + contratoId;
            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, null);
            return ret != null;
        }

        /// <summary>
        /// Realiza entrada no historico de mudanças do contrato.
        /// </summary>
        public static void AlteraStatusDeContrato(Object contratoId, Boolean cancelado)
        {
            AlteraStatusDeContrato(contratoId, cancelado, null);
        }
        /// <summary>
        /// Realiza entrada no historico de mudanças do contrato.
        /// </summary>
        public static void AlteraStatusDeContrato(Object contratoId, Boolean cancelado, PersistenceManager _pm)
        {
            String valor = "0", dataCancelamentoParam = ", contrato_alteracao=getdate() ";
            if (cancelado) { valor = "1"; dataCancelamentoParam = ", contrato_alteracao=getdate(), contrato_dataCancelamento=GetDate()"; }

            PersistenceManager pm = null;
            if (_pm != null)
                pm = _pm;
            else
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();
            }

            try
            {
                String command = "UPDATE contrato SET contrato_cancelado=" + valor + dataCancelamentoParam + " WHERE contrato_id=" + contratoId;
                NonQueryHelper.Instance.ExecuteNonQuery(command, pm);

                Contrato contrato = new Contrato(contratoId);
                pm.Load(contrato);

                if (cancelado)
                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cancelado, pm);
                else
                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.ReAtivado, pm);

                if (_pm == null) { pm.Commit(); }
            }
            catch (Exception ex)
            {
                if (_pm == null) { pm.Rollback();}
                throw ex;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        /// <summary>
        /// Retorna o id do contrato administrativo de uma proposta.
        /// </summary>
        /// <param name="contratoId">ID da proposta (contrato com o segurado).</param>
        /// <param name="pm">Objeto PersistenceManager participante da transação, caso exista uma, ou null.</param>
        /// <returns>ID do contrato administrativo.</returns>
        public static Object CarregaContratoAdmID(Object contratoId, PersistenceManager pm)
        {
            String qry = "SELECT contrato_contratoAdmId FROM contrato WHERE contrato_id=" + contratoId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Object CarregaContratoID(Object operadoraId, String contratoNumero, PersistenceManager pm)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE contrato_numero='" + contratoNumero + "' AND contrato_operadoraId=" + operadoraId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        /// <summary>
        /// Retorna o último contrato do beneficiário.
        /// </summary>
        /// <param name="cpfTitular">CPF do beneficiário.</param>
        /// <param name="historico">Última entrada no histórico do contrato.</param>
        /// <param name="adimplente">TRUE para adimplente, do contrário FALSE.</param>
        /// <returns>Último contrato, ou null.</returns>
        public static Contrato CarregarUltimoContratoDoBeneficiario(String cpfTitular, ref ContratoStatusHistorico historico, Boolean cancelado)
        {
            String query = String.Concat("SELECT TOP 1 contrato.* FROM  contrato ",
                "INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId ",
                "INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "WHERE contrato_rascunho=0 AND beneficiario_cpf=@CPF");

            if (!cancelado) { query += " AND contrato_cancelado=0"; }
            else { query += " AND contrato_cancelado=1"; }
            
            query += " ORDER BY contrato_data DESC";

            String[] pName = new String[] { "@CPF" };
            String[] pValue = new String[] { cpfTitular };

            IList<Contrato> lista = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(query, pName, pValue, typeof(Contrato));

            if (lista == null) { return null; }

            IList<ContratoStatusHistorico> _historico = ContratoStatusHistorico.Carregar(lista[0].OperadoraID, lista[0].Numero);
            if (_historico != null) { historico = _historico[0]; }

            return lista[0];
            //return Convert.ToDateTime(ret, new System.Globalization.CultureInfo("pt-Br"));
        }

        public static Decimal CalculaValorDaProposta(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite)
        {
            return CalculaValorDaProposta(propostaId, dataReferencia, pm, calculaTaxa, calculaValorEstipulante, ref composite, false);
        }

        /// <summary>
        /// Esse método foi escrito APENAS para calculo de demonstrativo de pagamento.
        /// </summary>
        public static Decimal CalculaValorDaProposta_TODOS(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean forcaTabelaVigente)
        {
            if (pm == null) { pm = new PersistenceManager(); }
            composite = new List<CobrancaComposite>();

            Decimal valorAdicionais = 0, valorPlano = 0, valorPlanoAux = 0, valorAdicional = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            //beneficiarios ativos da proposta
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(propostaId, false, false, pm);
            if (beneficiarios == null) { return 0; }

            //calcula valores de TAXA DE ASSOCIACAO
            Decimal valorEstipulante = 0;
            if (calculaValorEstipulante) { valorEstipulante = CalculaValorDaTaxaAssociativa(contrato, beneficiarios.Count, null, pm); }
            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaAssociacao, valorEstipulante));

            ContratoBeneficiario.eStatus status = ContratoBeneficiario.eStatus.Desconhecido;
            foreach (ContratoBeneficiario contratoBeneficiario in beneficiarios)
            {
                status = (ContratoBeneficiario.eStatus)contratoBeneficiario.Status;

                #region nesses casos não calcula o valor
                if (status == ContratoBeneficiario.eStatus.Cancelado ||
                    status == ContratoBeneficiario.eStatus.CancelamentoDevolvido ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendente ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora ||
                    status == ContratoBeneficiario.eStatus.Excluido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoDevolvido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendente ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora ||
                    !contratoBeneficiario.DMED ||
                    //status == ContratoBeneficiario.eStatus.Novo ||
                    (dataReferencia < contratoBeneficiario.Vigencia && contratoBeneficiario.Tipo != 0)) //Vigencia
                {
                    continue; // nesses casos não calcula o valor
                }

                if (!contratoBeneficiario.Ativo && contratoBeneficiario.DataInativacao != DateTime.MinValue && dataReferencia != DateTime.MinValue)
                {
                    contratoBeneficiario.DataInativacao = new DateTime(contratoBeneficiario.DataInativacao.Year,
                        contratoBeneficiario.DataInativacao.Month, contratoBeneficiario.DataInativacao.Day, 23, 59, 59, 998);
                    if (contratoBeneficiario.DataInativacao < dataReferencia) { continue; }
                }
                else if (!contratoBeneficiario.Ativo) { continue; }

                #endregion

                Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
                beneficiario.Carregar_DataNascimento(pm);
                Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

                //calcula valores de ADICIONAIS
                IList<AdicionalBeneficiario> adicionaisBeneficiario =
                    AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

                #region adicionais

                if (adicionaisBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                    {
                        valorAdicional = Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                            adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);

                        valorAdicionais += valorAdicional;
                        composite.Add(new CobrancaComposite(null, adicionalBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Adicional, valorAdicional));
                    }

                    adicionaisBeneficiario = null;
                }
                #endregion

                //calcula valor do PLANO
                valorPlanoAux = TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia, forcaTabelaVigente);

                if (valorPlanoAux == 0) { return 0; } //não foi localizada uma tabela vigente. interrompe o cálculo

                composite.Add(new CobrancaComposite(null, contratoBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Plano, valorPlanoAux));
                valorPlano += valorPlanoAux;
            }

            beneficiarios = null;
            Decimal valorTaxa = 0;

            #region taxa da tabela de valor

            if (calculaTaxa)
            {
                IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                if (tabela != null && tabela.Count > 0)
                {
                    Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    tabela = null;
                    if (taxa != null && !taxa.Embutido)
                    {
                        valorTaxa = taxa.ValorEmbutido;
                        if (taxa.ValorEmbutido > 0)
                        {
                            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaTabelaValor, valorTaxa));
                        }
                        taxa = null;
                    }
                }
            }
            #endregion

            return valorPlano + valorAdicionais + valorEstipulante + valorTaxa - contrato.Desconto;
        }

        public static Decimal CalculaValorDaProposta(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean forcaTabelaVigente)
        {
            if (pm == null) { pm = new PersistenceManager(); }
            composite = new List<CobrancaComposite>();

            Decimal valorAdicionais = 0, valorPlano = 0, valorPlanoAux = 0, valorAdicional = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            //beneficiarios ativos da proposta
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(propostaId, true, false, pm);
            if (beneficiarios == null) { return 0; }

            //calcula valores de TAXA DE ASSOCIACAO
            Decimal valorEstipulante = 0;
            if (calculaValorEstipulante) { valorEstipulante = CalculaValorDaTaxaAssociativa(contrato, beneficiarios.Count, dataReferencia, pm); }
            if (valorEstipulante > 0)
            {
                composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaAssociacao, valorEstipulante));
            }

            ContratoBeneficiario.eStatus status = ContratoBeneficiario.eStatus.Desconhecido;
            foreach (ContratoBeneficiario contratoBeneficiario in beneficiarios)
            {
                status = (ContratoBeneficiario.eStatus)contratoBeneficiario.Status;

                #region nesses casos não calcula o valor 
                if (status == ContratoBeneficiario.eStatus.Cancelado ||
                    status == ContratoBeneficiario.eStatus.CancelamentoDevolvido ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendente ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora ||
                    status == ContratoBeneficiario.eStatus.Excluido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoDevolvido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendente ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora ||
                    //status == ContratoBeneficiario.eStatus.Novo ||
                    dataReferencia < contratoBeneficiario.Vigencia && contratoBeneficiario.Tipo != (int)ContratoBeneficiario.TipoRelacao.Titular)
                {
                    continue; // nesses casos não calcula o valor
                }

                #endregion

                Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
                beneficiario.Carregar_DataNascimento(pm);
                Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

                //calcula valores de ADICIONAIS
                IList<AdicionalBeneficiario> adicionaisBeneficiario =
                    AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

                #region adicionais 

                if (adicionaisBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                    {
                        valorAdicional = Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                            adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);

                        valorAdicionais += valorAdicional;
                        composite.Add(new CobrancaComposite(null, adicionalBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Adicional, valorAdicional));
                    }

                    adicionaisBeneficiario = null;
                }
                #endregion

                //calcula valor do PLANO
                valorPlanoAux = TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia, forcaTabelaVigente);

                if (valorPlanoAux == 0) { return 0; } //não foi localizada uma tabela vigente. interrompe o cálculo

                composite.Add(new CobrancaComposite(null, contratoBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Plano, valorPlanoAux));
                valorPlano += valorPlanoAux;
            }

            beneficiarios = null;
            Decimal valorTaxa = 0;

            #region taxa da tabela de valor 

            if (calculaTaxa)
            {
                IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                if (tabela != null && tabela.Count > 0)
                {
                    Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    tabela = null;
                    if (taxa != null && !taxa.Embutido)
                    {
                        valorTaxa = taxa.ValorEmbutido;
                        if (taxa.ValorEmbutido > 0)
                        {
                            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaTabelaValor, valorTaxa));
                        }
                        taxa = null;
                    }
                }
            }
            #endregion

            return valorPlano + valorAdicionais + valorEstipulante + valorTaxa - contrato.Desconto;
        }

        public static Decimal CalculaValorDaPropostaSemTaxaAssociativa(Object propostaId, ContratoBeneficiario contratoBeneficiario, DateTime dataReferencia, PersistenceManager pm)
        {
            Decimal valorAdicionais = 0, valorPlano = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
            pm.Load(beneficiario);
            Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

            //calcula valores de ADICIONAIS
            IList<AdicionalBeneficiario> adicionaisBeneficiario =
                AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

            if (adicionaisBeneficiario != null)
            {
                foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                {
                    valorAdicionais += Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                        adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);
                }

                adicionaisBeneficiario = null;
            }

            //calcula valor do PLANO
            if (dataReferencia == null || dataReferencia == DateTime.MinValue)
            {
                valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, null);
            }
            else
            {
                valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia);
            }

            return valorPlano + valorAdicionais - contrato.Desconto;
        }

        public static Decimal CalculaValorDaPropostaSemTaxaAssocSemAdicional(Contrato contrato, ContratoBeneficiario contratoBeneficiario, DateTime dataReferencia, PersistenceManager pm)
        {
            Decimal valorPlano = 0;

            Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
            pm.Load(beneficiario);
            Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

            //calcula valor do PLANO
            valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                contrato.ContratoADMID, contrato.PlanoID,
                (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, null);

            return valorPlano - contrato.Desconto;
        }

        public static Decimal CalculaValorDaTaxaAssociativa(Contrato contrato, Int32 qtdBeneficiarios, DateTime? dataRef, PersistenceManager pm)
        {
            if (contrato.CobrarTaxaAssociativa)
            {
                EstipulanteTaxa estipulanteTaxa = null;
                if(dataRef == null || dataRef.Value == DateTime.MinValue)
                    estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                else
                    estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, dataRef.Value, pm);

                if (estipulanteTaxa != null)
                {
                    if (((EstipulanteTaxa.eTipoTaxa)estipulanteTaxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorProposta)
                    {
                        return estipulanteTaxa.Valor;
                    }
                    else // taxa por vida
                    {
                        if (qtdBeneficiarios == -1)
                        {
                            IList<ContratoBeneficiario> benefs = ContratoBeneficiario.CarregarPorContratoID_Parcial(contrato.ID, true, false, pm);
                            if (benefs != null)
                                qtdBeneficiarios = benefs.Count;
                            else
                                qtdBeneficiarios = 0;
                            benefs = null;
                        }
                        return (estipulanteTaxa.Valor * qtdBeneficiarios);
                    }
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public static Boolean VerificaExistenciaBeneficiarioNoContrato(Object beneficiarioId, Object contratoId)
        {
            String[] pName = new String[0];
            String[] pValue = new String[0];

            String query = " SELECT contrato.* " +
                        " FROM contrato " +
                        " INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId " +
                        " WHERE contratobeneficiario_beneficiarioId = " + beneficiarioId + " AND contrato_id = " + contratoId + " AND contratobeneficiario_ativo = 1 ";

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, pName, pValue).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static void AlterarNumeroDeContrato(Object contratoId, String novoNumero, PersistenceManager pm)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numero=@numero WHERE contrato_id=" + contratoId, names, value, pm);
        }

        public static void AlterarNumeroDeMatricula(Object contratoId, String novoNumero)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numeroMatricula=@numero WHERE contrato_id=" + contratoId, names, value, null);
        }

    }

    /// <summary>
    /// Representa a composição da cobrança.
    /// </summary>
    [DBTable("cobranca_composicao")]
    public class CobrancaComposite : EntityBase, IPersisteableEntity
    {
        public enum eComposicaoTipo : int
        {
            Plano,
            TaxaAssociacao,
            TaxaTabelaValor,
            Adicional,
            Desconto
        }

        #region fields 

        Object _id;
        Object _cobrancaId;
        Object _beneficiarioId;
        Int32 _tipo;
        Decimal _valor;

        String _beneficiarioNome;

        #endregion

        #region properties 

        [DBFieldInfo("cobrancacomp_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("cobrancacomp_cobranaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        [DBFieldInfo("cobrancacomp_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId= value; }
        }

        [DBFieldInfo("cobrancacomp_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("cobrancacomp_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { return _beneficiarioNome; }
            set { _beneficiarioNome= value; }
        }

        public String StrTipo
        {
            get
            {
                if ((eComposicaoTipo)_tipo == eComposicaoTipo.Adicional)
                    return "Adicional";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.Desconto)
                    return "Desconto";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.Plano)
                    return "Plano";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.TaxaAssociacao)
                    return "Taxa associativa";
                else
                    return "Taxa (tabela)";
            }
        }

        #endregion

        public CobrancaComposite() { }
        public CobrancaComposite(Object cobrancaId, Object beneficiarioId, eComposicaoTipo tipo, Decimal valor)
        {
            _cobrancaId = cobrancaId;
            _beneficiarioId = beneficiarioId;
            _tipo = Convert.ToInt32(tipo);
            _valor = valor;
        }

        public static void Salvar(Object cobrancaId, IList<CobrancaComposite> lista, PersistenceManager pm)
        {
            StringBuilder sb = new StringBuilder();

            if (lista == null) { return; }
            foreach (CobrancaComposite compos in lista)
            {
                sb.Append("INSERT INTO cobranca_composicao (cobrancacomp_cobranaId,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor) VALUES (");
                sb.Append(cobrancaId); sb.Append(", ");

                if (compos.BeneficiarioID != null)
                    sb.Append(compos.BeneficiarioID);
                else
                    sb.Append("NULL");
                sb.Append(", ");

                sb.Append(Convert.ToInt32(compos.Tipo));
                sb.Append(", '");
                sb.Append(compos.Valor.ToString(new System.Globalization.CultureInfo("en-US")));
                sb.Append("') ;");
            }

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
            }
            catch
            {
            }

            sb.Remove(0, sb.Length);
            sb = null;
        }

        #region entity base methods 

        public void Salvar(PersistenceManager pm)
        {
            base.Salvar(this);
        }

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

        public static void Remover(Object cobrancaId, PersistenceManager pm)
        {
            String cmd = "delete from cobranca_composicao where cobrancacomp_cobranaId=" + cobrancaId;
            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        public static IList<CobrancaComposite> Carregar(Object cobrancaId)
        {
            return Carregar(cobrancaId, null);
        }

        public static IList<CobrancaComposite> Carregar(Object cobrancaId, PersistenceManager pm)
        {
            String qry = String.Concat("cobranca_composicao.*,beneficiario_nome from cobranca_composicao left join beneficiario on cobrancacomp_beneficiarioId=beneficiario_id where cobrancacomp_cobranaId=", cobrancaId);

            return LocatorHelper.Instance.ExecuteQuery<CobrancaComposite>(qry, typeof(CobrancaComposite), pm);
        }
    }

    [DBTable("contratoStatusHistorico")]
    public class ContratoStatusHistorico : EntityBase, IPersisteableEntity
    {
        public enum eStatus : int
        {
            NoEstoque,
            ComCorretor,            // 1
            Rasurado,               // 2
            EmConferencia,          // 3
            NoCadastro,             // 4
            Cadastrado,             // 5
            Cancelado,              // 6
            ReAtivado,              // 7
            //DevolucaoParaAcerto   //8
            AlteradoNaOperadora,    // 8
            BeneficiarioAdicionado, // 9
            BeneficiarioAlterado,   //10
            BeneficiarioCancelado,  //11
            MudancaDePlano          //12
        }

        Object _id;
        Object _operadoraId;
        String _propostaNumero;
        Int32 _status;
        DateTime _data;

        #region propriedades 

        [DBFieldInfo("contratostatushistorico_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratostatushistorico_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("contratostatushistorico_propostaNumero", FieldType.Single)]
        public String PropostaNumero
        {
            get { return _propostaNumero; }
            set { _propostaNumero= value; }
        }

        [DBFieldInfo("contratostatushistorico_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status= value; }
        }

        [DBFieldInfo("contratostatushistorico_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        public ContratoStatusHistorico() { _status = 0; }
        public ContratoStatusHistorico(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public static ContratoStatusHistorico Salvar(String numero, Object operadoraId, eStatus status, PersistenceManager pm)
        {
            ContratoStatusHistorico csh = new ContratoStatusHistorico();
            csh.Data = DateTime.Now;
            csh.OperadoraID = operadoraId;
            csh.PropostaNumero = numero;
            csh.Status = Convert.ToInt32(status);

            if (pm != null)
                pm.Save(csh);
            else
                csh.Salvar();

            return csh;
        }

        public static ContratoStatusHistorico Salvar(Int32 numero, Int32 qtdZerosEsquerda, String letra, Object operadoraId, eStatus status, PersistenceManager pm)
        {
            String _numero = EntityBase.GeraNumeroDeContrato(numero, qtdZerosEsquerda, letra);
            return ContratoStatusHistorico.Salvar(_numero, operadoraId, status, pm);
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

        public static IList<ContratoStatusHistorico> Carregar(Object operadoraId, String propostaNumero)
        {
            String query = String.Concat("* FROM contratoStatusHistorico WHERE contratostatushistorico_operadoraId = ",
                operadoraId, " AND contratostatushistorico_propostaNumero='", propostaNumero, "' ORDER BY contratostatushistorico_data DESC");

            return LocatorHelper.Instance.ExecuteQuery
                <ContratoStatusHistorico>(query, typeof(ContratoStatusHistorico));
        }
    }

    [DBTable("contrato_valor")]
    public class ContratoValor : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        Decimal _valorTotal;
        Boolean _status;
        DateTime _data;

        #region propriedades 

        [DBFieldInfo("contratovalor_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratovalor_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("contratovalor_valorFinal", FieldType.Single)]
        public Decimal ValorTotal
        {
            get { return _valorTotal; }
            set { _valorTotal= value; }
        }

        [DBFieldInfo("contratovalor_status", FieldType.Single)]
        public Boolean Status
        {
            get { return _status; }
            set { _status= value; }
        }

        [DBFieldInfo("contratovalor_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        public ContratoValor(Object id) : this() { _id = id; }
        public ContratoValor() { _status = true; _data = DateTime.Now; }

        #region métodos EntityBase 

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

        public static ContratoValor CarregarUltimo(Object contratoId, PersistenceManager pm)
        {
            String query = String.Concat(" TOP 1 * FROM contrato_valor WHERE contratovalor_contratoId = ",
                contratoId, " AND contratovalor_status=1 ORDER BY contratovalor_data DESC");

            IList<ContratoValor> ret = LocatorHelper.Instance.
                ExecuteQuery<ContratoValor>(query, typeof(ContratoValor), pm);

            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static void InsereNovoValorSeNecessario(Object contratoId, Decimal valor, PersistenceManager pm)
        {
            ContratoValor cValor = ContratoValor.CarregarUltimo(contratoId, pm);
            if (cValor == null || cValor.ValorTotal != valor)
            {
                cValor = new ContratoValor();
                cValor.ContratoID = contratoId;
                cValor.Data = DateTime.Now;
                cValor.Status = true;
                cValor.ValorTotal = Convert.ToDecimal(valor);
                pm.Save(cValor);
            }
        }
    }
}
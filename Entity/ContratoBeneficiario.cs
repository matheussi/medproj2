namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable]
    [DBTable("contrato_beneficiario")]
    public class ContratoBeneficiario : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            public static void FillDropdownWithStatus(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Novo", "0"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Pendente na operadora", "1"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Incluído", "2"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Devolvido", "3"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração de cadastro pendente no sistema", "4"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão pendente no sistema", "5"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão pendente no sistema", "6"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração de cadastro pendente na operadora", "7"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão pendente na operadora", "8"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão pendente na operadora", "9"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Excluído", "10"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano pendente no sistema", "11"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano pendente na operadora", "12"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato pendente no sistema", "13"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato pendente na operadora", "14"));
                //cbo.Items.Add(new System.Web.UI.WebControls.ListItem("", "15"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração cadastral devolvida pela operadora", "16"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão de beneficiário devolvida pela operadora", "17"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão devolvida pela operadora", "18"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano devolvida pela operadora", "19"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato devolvido pela operadora", "20"));
                //cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Contrato cancelado", "21"));
            }
        }

        public enum TipoRelacao : int
        {
            Titular=0,
            Dependente,
            Agregado
        }

        public enum eStatus : int
        {
            /// <summary>
            /// Novo beneficiário cadastrado. A operadora ainda NÃO foi notificada.
            /// </summary>
            Novo = 0,
            PendenteNaOperadora,
            Incluido, //acatado pela operadora
            /// <summary>
            /// Inclusão de contrato ou beneficiário devolvida pela Operadora.
            /// </summary>
            Devolvido,  //3
            /// <summary>
            /// Alteração de Dados Cadastrais pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            AlteracaoCadastroPendente, //4
            /// <summary>
            /// Exclusão de Beneficiário pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            ExclusaoPendente, //5
            /// <summary>
            /// Segunda Via de Cartão pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            SegundaViaCartaoPendente, //6
            /// <summary>
            /// Alteração de Dados Cadastrais pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            AlteracaoCadastroPendenteNaOperadora, //7
            /// <summary>
            /// Exclusão de Beneficiário pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            ExclusaoPendenteNaOperadora, //8
            /// <summary>
            /// Segunda Via de Cartão pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            SegundaViaCartaoPendenteNaOperadora, //9
            /// <summary>
            /// O beneficiário está cancelado na operadora e no sistema.
            /// </summary>
            Excluido, //10
            /// <summary>
            /// Mudança de Plano pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            MudancaPlanoPendente, //11
            /// <summary>
            /// Mudança de Plano pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            MudancaPlanoPendenteNaOperadora,
            /// <summary>
            /// Cancelamento de Contrato pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            CancelamentoPendente,
            /// <summary>
            /// Cancelamento de Contrato pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            CancelamentoPendenteNaOperadora, //14
            /// <summary>
            /// Desconhecido.
            /// </summary>
            Desconhecido,
            /// <summary>
            /// Alteração de dados cadastrais devolvida pela Operadora.
            /// </summary>
            AlteracaoCadastroDevolvido,
            /// <summary>
            /// Exclusao de beneficiario devolvida pela Operadora.
            /// </summary>
            ExclusaoDevolvido, //17
            /// <summary>
            /// Segunda via de cartão devolvida pela Operadora.
            /// </summary>
            SegundaViaCartaoDevolvido,
            /// <summary>
            /// Mudança de plano devolvida pela Operadora.
            /// </summary>
            MudancaDePlanoDevolvido,
            /// <summary>
            /// Cancelamento de contrato devolvido pela Operadora.
            /// </summary>
            CancelamentoDevolvido, //20
            /// <summary>
            /// Contrato Cancelado.
            /// </summary>
            Cancelado
        }

        #region fields 

        Object _id;
        Object _contratoId;
        Object _beneficiarioId;
        Object _parentescoId;
        Object _estadoCivilId;
        int _tipo;
        DateTime _data;
        DateTime _vigencia;
        DateTime _dataInativacao;
        Boolean _ativo;
        Int32 _status;
        Int32 _numeroSequencial;

        //Object      _estadoCivil;
        DateTime    _dataCasamento;
        Decimal     _peso;
        Decimal     _altura;

        String _carenciaOperadora;
        Object _carenciaOperadoraId;
        String _carenciaOperadoraDescricao;
        String _carenciaMatriculaNumero;
        DateTime _carenciaContratoDe;
        DateTime _carenciaContratoAte;
        Int32 _carenciaContratoTempo; //em meses.
        String _carenciaCodigo;
        bool _dmed;

        Decimal _valor;
        String _portabilidade;

        String _numeroMatriculaSaude;
        String _numeroMatriculaDental;

        String _beneficiarioNome;
        String _beneficiarioNomeMae;
        String _beneficiarioCpf;
        String _beneficiarioSexo;
        String _parentescoDescricao;
        String _parentescoCodigo;
        String _estadoCivilDescricao;
        String _estadoCivilCodigo;

        DateTime _beneficiarioDataNascimento;

        #endregion

        #region propriedades 

        [DBFieldInfo("contratobeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratobeneficiario_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("contratobeneficiario_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId= value; }
        }

        [DBFieldInfo("contratobeneficiario_parentescoId", FieldType.Single)]
        public Object ParentescoID
        {
            get { return _parentescoId; }
            set { _parentescoId= value; }
        }

        [DBFieldInfo("contratobeneficiario_tipo", FieldType.Single)]
        public int Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        /// <summary>
        /// Data de admissão do beneficiário.
        /// </summary>
        [DBFieldInfo("contratobeneficiario_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("contratobeneficiario_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia= value; }
        }

        [DBFieldInfo("contratobeneficiario_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraOrigemId", FieldType.Single)]
        public Object CarenciaOperadoraID
        {
            get { return _carenciaOperadoraId; }
            set { _carenciaOperadoraId= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraId", FieldType.Single)]
        public String CarenciaOperadora
        {
            get { return _carenciaOperadora; }
            set { _carenciaOperadora= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraDescricao", FieldType.Single)]
        public String CarenciaOperadoraDescricao
        {
            get { return _carenciaOperadoraDescricao; }
            set { _carenciaOperadoraDescricao= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaMatriculaNumero", FieldType.Single)]
        public String CarenciaMatriculaNumero
        {
            get { return _carenciaMatriculaNumero; }
            set { _carenciaMatriculaNumero= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaContratoDataDe", FieldType.Single)]
        public DateTime CarenciaContratoDe
        {
            get { return _carenciaContratoDe; }
            set { _carenciaContratoDe= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaContratoDataAte", FieldType.Single)]
        public DateTime CarenciaContratoAte
        {
            get { return _carenciaContratoAte; }
            set { _carenciaContratoAte= value; }
        }

        /// <summary>
        /// Tempo de contrato anterior (em meses)
        /// </summary>
        [DBFieldInfo("contratobeneficiario_carenciaContratoTempo", FieldType.Single)]
        public Int32 CarenciaContratoTempo
        {
            get { return _carenciaContratoTempo; }
            set { _carenciaContratoTempo= value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaCodigo", FieldType.Single)]
        public String CarenciaCodigo
        {
            get { return _carenciaCodigo; }
            set { _carenciaCodigo= value; }
        }

        [DBFieldInfo("contratobeneficiario_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status= value; }
        }

        [DBFieldInfo("contratobeneficiario_numeroSequencia", FieldType.Single)]
        public Int32 NumeroSequencial
        {
            get { return _numeroSequencial; }
            set { _numeroSequencial= value; }
        }

        [DBFieldInfo("contratobeneficiario_estadoCivilId", FieldType.Single)]
        public Object EstadoCivilID
        {
            get { return _estadoCivilId; }
            set { _estadoCivilId= value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        [DBFieldInfo("contratobeneficiario_dataCasamento", FieldType.Single)]
        public DateTime DataCasamento
        {
            get { return _dataCasamento; }
            set { _dataCasamento = value; }
        }

        [DBFieldInfo("contratobeneficiario_peso", FieldType.Single)]
        public Decimal Peso
        {
            get { return _peso; }
            set { _peso= value; }
        }

        [DBFieldInfo("contratobeneficiario_altura", FieldType.Single)]
        public Decimal Altura
        {
            get { return _altura; }
            set { _altura= value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        [DBFieldInfo("contratobeneficiario_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [DBFieldInfo("contratobeneficiario_portabilidade", FieldType.Single)]
        public String Portabilidade
        {
            get { return _portabilidade; }
            set { _portabilidade= value; }
        }

        [DBFieldInfo("contratobeneficiario_numMatriculaSaude", FieldType.Single)]
        public String NumeroMatriculaSaude
        {
            get { return _numeroMatriculaSaude; }
            set { _numeroMatriculaSaude= value; }
        }

        [DBFieldInfo("contratobeneficiario_numMatriculaDental", FieldType.Single)]
        public String NumeroMatriculaDental
        {
            get { return _numeroMatriculaDental; }
            set { _numeroMatriculaDental= value; }
        }

        [DBFieldInfo("contratobeneficiario_dataInativo", FieldType.Single)]
        public DateTime DataInativacao
        {
            get { return _dataInativacao; }
            set { _dataInativacao= value; }
        }

        /// <summary>
        /// Aprovado na DMED?
        /// </summary>
        [Joinned("beneficiario_dmed")]
        public bool DMED
        {
            get { return _dmed; }
            set { _dmed= value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { return _beneficiarioNome; }
            set { _beneficiarioNome= value; }
        }

        [Joinned("beneficiario_nomeMae")]
        public String BeneficiarioNomeMae
        {
            get { return _beneficiarioNomeMae; }
            set { _beneficiarioNomeMae = value; }
        }

        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf = value; }
        }

        [Joinned("beneficiario_sexo")]
        public String BeneficiarioSexo
        {
            get { return _beneficiarioSexo; }
            set { _beneficiarioSexo= value; }
        }

        [Joinned("contratoAdmparentescoagregado_parentescoDescricao")]
        public String ParentescoDescricao
        {
            get { return _parentescoDescricao; }
            set { _parentescoDescricao= value; }
        }

        [Joinned("contratoAdmparentescoagregado_parentescoCodigo")]
        public String ParentescoCodigo
        {
            get { return _parentescoCodigo; }
            set { _parentescoCodigo= value; }
        }

        [Joinned("estadocivil_descricao")]
        public String EstadoCivilDescricao
        {
            get { return _estadoCivilDescricao; }
            set { _estadoCivilDescricao= value; }
        }

        [Joinned("estadocivil_codigo")]
        public String EstadoCivilCodigo
        {
            get { return _estadoCivilCodigo; }
            set { _estadoCivilCodigo= value; }
        }

        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioDataNascimento
        {
            get { return _beneficiarioDataNascimento; }
            set { _beneficiarioDataNascimento = value; }
        }

        string _beneficiarioEmail;
        [Joinned("beneficiario_email")]
        public string BeneficiarioEmail
        {
            get { return _beneficiarioEmail; }
            set { _beneficiarioEmail= value; }
        }

        #endregion

        public ItemDeclaracaoSaudeINSTANCIA ItemDeclaracaoSaudeINSTANCIA
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ContratoBeneficiario() { _status = 0; _data = DateTime.Now; _ativo = true; _numeroSequencial = -1; _dmed = true; }

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

        public static ContratoBeneficiario CarregarPorContratoEBeneficiario(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, contratoAdmparentescoagregado_parentescoDescricao,beneficiario_dmed",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contratobeneficiario_contratoId=", contratoId, " AND contratobeneficiario_beneficiarioId=", beneficiarioId);

            IList<ContratoBeneficiario> ret = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean semTitular)
        {
            return CarregarPorContratoID(contratoId, true, semTitular);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean apenasAtivos, Boolean semTitular)
        {
            return CarregarPorContratoID(contratoId, apenasAtivos, semTitular, null);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean apenasAtivos, Boolean semTitular, PersistenceManager pm)
        {
            String semTitularCondition = "";

            if (semTitular)
            {
                semTitularCondition = " AND contratobeneficiario_tipo <> " + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
            }

            String apenasAtivosCondition = "";
            if (apenasAtivos) { apenasAtivosCondition = "contratobeneficiario_ativo=1 AND "; }

            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_nomeMae, beneficiario_cpf, beneficiario_sexo, beneficiario_dataNascimento, contratoAdmparentescoagregado_parentescoDescricao, contratoAdmparentescoagregado_parentescoCodigo, estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ", apenasAtivosCondition, " contratobeneficiario_contratoId=", contratoId, semTitularCondition,
                " ORDER BY contratobeneficiario_numeroSequencia");

            //query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, estadocivil_id,estadocivil_descricao,estadocivil_codigo ",
            //    " FROM contrato_beneficiario",
            //    "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
            //    "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
            //    " WHERE ",
            //    "   contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular), " AND contratobeneficiario_contratoId=", contratoId);

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        public static IList<ContratoBeneficiario> Carregar(string[] ids, PersistenceManager pm)
        {
            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_nomeMae, beneficiario_cpf, beneficiario_sexo, beneficiario_dataNascimento, contratoAdmparentescoagregado_parentescoDescricao, contratoAdmparentescoagregado_parentescoCodigo, estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE contratobeneficiario_id in (", string.Join(",", ids), ") ",
                " ORDER BY contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID_Parcial(Object contratoId, Boolean apenasAtivos, Boolean semTitular, PersistenceManager pm)
        {
            String semTitularCondition = "";

            if (semTitular)
            {
                semTitularCondition = " AND contratobeneficiario_tipo <> " + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
            }

            String apenasAtivosCondition = "";
            if (apenasAtivos) { apenasAtivosCondition = "contratobeneficiario_ativo=1 AND "; }

            String query = String.Concat("contratobeneficiario_id,contratobeneficiario_beneficiarioId, contratobeneficiario_contratoId, contratobeneficiario_tipo, beneficiario_dataNascimento,contratobeneficiario_ativo,contratobeneficiario_dataInativo,contratobeneficiario_data,contratobeneficiario_vigencia,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE ", apenasAtivosCondition, " contratobeneficiario_contratoId=", contratoId, semTitularCondition,
                " ORDER BY contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        /// <summary>
        /// ID do beneficiario titular
        /// </summary>
        public static Object CarregaTitularID(Object contratoId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_beneficiarioid FROM contrato_beneficiario WHERE contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_tipo=" + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular).ToString(), paramNm, paramVl, pm);
        }
        /// <summary>
        /// ID do beneficiario titular
        /// </summary>
        public static Object CarregaTitularID(String contratoNumero, Object operadoraId, PersistenceManager pm)
        {
            Object contratoId = Contrato.CarregaContratoID(operadoraId, contratoNumero, pm);
            if (contratoId == null) { return null; }
            return CarregaTitularID(contratoId, pm);
        }

        public static Object CarregaID(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId", "@beneficiarioId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId), Convert.ToString(beneficiarioId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_id FROM contrato_beneficiario WHERE contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_beneficiarioid=@beneficiarioId", paramNm, paramVl, pm);
        }

        /// <summary>
        /// ID do ContratoBeneficiario para o titular
        /// </summary>
        public static Object CarregaID_ParaTitular(Object contratoId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_id FROM contrato_beneficiario WHERE contratobeneficiario_ativo=1 AND contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_tipo=" + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular).ToString(), paramNm, paramVl, pm);
        }

        /// <summary>
        /// 
        /// </summary>
        public static IList<ContratoBeneficiario> CarregarPorContratoNumero(String contratoNumero, Object operadoraId, Boolean somenteAtivos)
        {
            String[] paramNm = new String[] { "@Numero" };
            String[] paramVl = new String[] { contratoNumero };

            String ativoCond = "";
            if (somenteAtivos)
            {
                ativoCond = " AND contratobeneficiario_ativo=1 ";
            }

            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, contratoAdmparentescoagregado_parentescoDescricao",
                " FROM contrato_beneficiario",
                " INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ", ativoCond,
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id", ativoCond,
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contrato_numero=@Numero AND contrato_operadoraId=", operadoraId,
                " ORDER BY contrato_id, contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoBeneficiario>(query, paramNm, paramVl, typeof(ContratoBeneficiario));
        }

        public static ContratoBeneficiario CarregarTitularPorContratoNumero(String contratoNumero, String operadoraNome, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@Numero", "@OperadoraNome" };
            String[] paramVl = new String[] { contratoNumero, operadoraNome };

            String query = String.Concat("top 1 beneficiario_id,beneficiario_nome, beneficiario_cpf ",
                " FROM contrato_beneficiario",
                " INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ", 
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contrato_numero=@Numero AND contrato_operadoraNome=@operadoraNome",
                " ORDER BY contratobeneficiario_numeroSequencia");

            IList<ContratoBeneficiario> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoBeneficiario>(query, paramNm, paramVl, typeof(ContratoBeneficiario), pm);

            if (ret == null || ret.Count == 0)
                return null;
            else
                return ret[0];
        }

        public static ContratoBeneficiario CarregarTitular(Object contratoId, PersistenceManager pm)
        {
            String query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome,beneficiario_email, beneficiario_cpf, beneficiario_dataNascimento, estadocivil_id,estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ",
                "   contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular), " AND contratobeneficiario_contratoId=", contratoId);

            IList<ContratoBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static ContratoBeneficiario CarregarPorIDContratoBeneficiario(Object id, PersistenceManager pm)
        {
            String query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, estadocivil_id,estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ",
                "   contratobeneficiario_id=", id);

            IList<ContratoBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        /// <summary>
        /// Método para pegar o CPF do Titular de contrato (proposta para cliente final).
        /// </summary>
        /// <param name="contratoId">ID do Contrato.</param>
        /// <returns>Retorna o CPF do Titular do Contrato.</returns>
        public static String GetCPFTitular(Object contratoId)
        {
            return GetCPFTitular(contratoId, null);
        }

        /// <summary>
        /// Método para pegar o CPF do Titular de contrato (proposta para cliente final).
        /// </summary>
        /// <param name="contratoId">ID do Contrato.</param>
        /// <returns>Retorna o CPF do Titular do Contrato.</returns>
        public static String GetCPFTitular(Object contratoId, PersistenceManager PM)
        {
            if (contratoId != null)
            {
                String[] strParam = new String[1];
                String[] strValue = new String[1];

                strParam[0] = "@contrato_id";
                strValue[0] = contratoId.ToString();

                String strSQL = String.Concat("SELECT ",
                                              "      Ben.beneficiario_cpf ",
                                              "  FROM contrato_beneficiario cBen ",
                                              "  INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ",
                                              "  WHERE contratobeneficiario_tipo = 0 AND contratobeneficiario_contratoId = @contrato_id");

                Object retVal = null;

                if (PM == null) PM = new PersistenceManager();

                try
                {
                    retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strValue, PM);
                }
                catch (Exception) { throw; }

                if (retVal != null && !(retVal is DBNull))
                    return retVal.ToString().Trim();
                else
                    return null;
            }
            else
                throw new ArgumentNullException("O ID do contrato está nulo.");
        }

        public static String GetNomeTitular(Object contratoId, PersistenceManager PM)
        {
            if (contratoId != null)
            {
                String[] strParam = new String[1];
                String[] strValue = new String[1];

                strParam[0] = "@contrato_id";
                strValue[0] = contratoId.ToString();

                String strSQL = String.Concat("SELECT ",
                                              "      Ben.beneficiario_nome ",
                                              "  FROM contrato_beneficiario cBen ",
                                              "  INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ",
                                              "  WHERE contratobeneficiario_tipo = 0 AND contratobeneficiario_contratoId = @contrato_id");
                Object retVal = null;

                try
                {
                    retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strValue, PM);
                }
                catch (Exception) { throw; }

                if (retVal != null && !(retVal is DBNull))
                    return retVal.ToString().Trim();
                else
                    return null;
            }
            else
                throw new ArgumentNullException("O ID do contrato está nulo.");
        }

        /// <summary>
        /// Retorna o próximo número sequencial para beneficiário de um contrato.
        /// </summary>
        /// <param name="contratoId">ID do contrato (proposta)</param>
        /// <returns>Próximo número sequencial para beneficiário de um contrato</returns>
        public static Int32 ProximoNumeroSequencial(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            Object ret = null;
            if (beneficiarioId != null)
            {
                ret = LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_numeroSequencia FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=" + contratoId + " AND contratobeneficiario_beneficiarioId=" + beneficiarioId, null, null, pm);
                if (ret != null)
                    return Convert.ToInt32(ret);
            }

            ret = LocatorHelper.Instance.ExecuteScalar("SELECT MAX(contratobeneficiario_numeroSequencia) FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=" + contratoId, null, null, pm);

            if (ret == null)
                return 1;
            else
                return (Convert.ToInt32(ret) + 1);
        }

        /// <summary>
        /// Retorna o próximo Status de acordo com a Movimentação.
        /// </summary>
        /// <param name="Movimentacao">Inclusão de Beneficiário, Alteração de Dados Cadastrais, Mudança de Plano, etc.</param>
        /// <returns>Retorna o próximo Status do Workflow.</returns>
        public static eStatus ProximoStatusPorMovimentacao(String Movimentacao)
        {
            switch (Movimentacao)
            {
                case ArqTransacionalUnimed.Movimentacao.InclusaoBeneficiario:
                    return eStatus.PendenteNaOperadora;
                case ArqTransacionalUnimed.Movimentacao.AlteracaoBeneficiario:
                    return eStatus.AlteracaoCadastroPendenteNaOperadora;
                case ArqTransacionalUnimed.Movimentacao.ExclusaoBeneficiario:
                    return eStatus.ExclusaoPendenteNaOperadora;
                case ArqTransacionalUnimed.Movimentacao.SegundaViaCartaoBeneficiario:
                    return eStatus.SegundaViaCartaoPendenteNaOperadora;
                case ArqTransacionalUnimed.Movimentacao.MudancaDePlano:
                    return eStatus.MudancaPlanoPendenteNaOperadora;
                case ArqTransacionalUnimed.Movimentacao.CancelamentoContrato:
                    return eStatus.CancelamentoPendenteNaOperadora;
                default:
                    return eStatus.Desconhecido;
            }
        }

        /// <summary>
        /// Ao desfazer o envio de um lote da operadora, o beneficiário precisa reassumir seu status de pendência no sistema.
        /// Este método calcula esse status com base no status assumido quando enviado à operadora.
        /// </summary>
        internal static ContratoBeneficiario.eStatus StatusAntesDeDesfazerEnvio(ContratoBeneficiario.eStatus statusAtual)
        {
            switch (statusAtual)
            {
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.Novo;
                }
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
                }
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.ExclusaoPendente;
                }
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
                }
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.MudancaPlanoPendente;
                }
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                {
                    return ContratoBeneficiario.eStatus.CancelamentoPendente;
                }
            }

            return ContratoBeneficiario.eStatus.Desconhecido;
        }

        internal static void SetaStatusDevolvidoParaContratoBeneficiario(ContratoBeneficiario.eStatus statusAtual, Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            ContratoBeneficiario.eStatus novoStatus = ContratoBeneficiario.eStatus.Desconhecido;

            #region obtém próximo status

            switch (statusAtual)
            {
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido;
                    break;
                }
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.CancelamentoDevolvido;
                    break;
                }
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.ExclusaoDevolvido;
                    break;
                }
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido;
                    break;
                }
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.Devolvido;
                    break;
                }
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                {
                    novoStatus = ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido;
                    break;
                }
            }
            #endregion

            if (novoStatus != ContratoBeneficiario.eStatus.Desconhecido)
            {
                ContratoBeneficiario.AlteraStatusBeneficiario(contratoId, beneficiarioId, novoStatus, pm);
                //ContratoStatusHistorico csh = new ContratoStatusHistorico();
                //csh.Data = DateTime.Now;
                //csh.OperadoraID = null;
                //csh.PropostaNumero = "";
                //csh.Status = ContratoStatusHistorico.eStatus.
            }
        }


        /// <summary>
        /// Método par aAlterar o Status de um Beneficiário dentro de um Contrato.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiário.</param>
        /// <param name="Status">Status do Beneficiário.</param>
        public static void AlteraStatusBeneficiario(Object ContratoID, Object BeneficiarioID, eStatus Status)
        {
            AlteraStatusBeneficiario(ContratoID, BeneficiarioID, Status, new PersistenceManager());
        }

        /// <summary>
        /// Método par aAlterar o Status de um Beneficiário dentro de um Contrato.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiário.</param>
        /// <param name="Status">Status do Beneficiário.</param>
        public static void AlteraStatusBeneficiario(Object ContratoID, Object BeneficiarioID, eStatus Status, PersistenceManager PM)
        {
            if (BeneficiarioID != null && ContratoID != null)
            {
                String[] strParam = new String[3];
                String[] strVaule = new String[3];

                strParam[0] = "@status";
                strParam[1] = "@contrato_id";
                strParam[2] = "@beneficiario_id";

                strVaule[0] = ((Int32)Status).ToString();
                strVaule[1] = ContratoID.ToString();
                strVaule[2] = BeneficiarioID.ToString();

                String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_status = @status WHERE contratobeneficiario_contratoId = @contrato_id AND contratobeneficiario_beneficiarioId = @beneficiario_id";

                try
                {
                    if (PM == null) PM = new PersistenceManager();

                    LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strVaule, PM);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("O ID do beneficiario ou do contrato não foi informado.");
        }

        public static void AlteraStatusBeneficiario(Object ContratoBeneficiarioID, eStatus Status, PersistenceManager PM)
        {
            String[] strParam = new String[2];
            String[] strVaule = new String[2];

            strParam[0] = "@status";
            strParam[1] = "@contratobeneficiario_id";

            strVaule[0] = ((Int32)Status).ToString();
            strVaule[1] = ContratoBeneficiarioID.ToString();

            String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_status = @status WHERE contratobeneficiario_id = @contratobeneficiario_id";

            try
            {
                if (PM == null) PM = new PersistenceManager();

                NonQueryHelper.Instance.ExecuteNonQuery(strSQL, strParam, strVaule, PM);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Inativa o Beneficiário no contrato
        /// </summary>
        /// <param name="ContratoID">Id do contrato</param>
        /// <param name="BeneficiarioId">Id do beneficiário</param>
        public static void InativaBeneficiario(Object ContratoID, Object BeneficiarioId, PersistenceManager PM)
        {
            if (BeneficiarioId != null && ContratoID != null)
            {
                String[] strParam = new String[2];
                String[] strVaule = new String[2];

                strParam[0] = "@contrato_id";
                strParam[1] = "@beneficiario_id";

                strVaule[0] = ContratoID.ToString();
                strVaule[1] = BeneficiarioId.ToString();

                String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_ativo = 0, contratobeneficiario_dataInativo = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE contratobeneficiario_contratoId = @contrato_id AND contratobeneficiario_beneficiarioId = @beneficiario_id ";

                try
                {
                    if (PM == null) PM = new PersistenceManager();

                    LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strVaule, PM);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("O ID do beneficiario ou do contrato não foi informado.");
        }
        
    }
}
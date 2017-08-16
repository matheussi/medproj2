namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using System.Text;
    using System.Data;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("_atendimento")]
    public class AtendimentoTemp : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            private UI() { }

            public static void FillCombo(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Informação".ToUpper(), "1"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Reclamação".ToUpper(), "2"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Solicitação".ToUpper(), "3"));
            }
        }

        #region fields 

        Object _id;
        Object _propostaId;
        Object _usuarioId;
        Object _tipoId;
        Object _subTipoId;
        String _titulo;
        String _texto;
        DateTime _dataInicio;
        DateTime _dataFim;
        DateTime _dataPrevisao;
        DateTime _data;

        String _iniciadoPor;
        String _resolvidoPor;

        String _atendimentoTipo;
        String _contratoNumero;
        String _operadoraNome;

        #endregion

        #region propriedades 

        [DBFieldInfo("atendimento_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("atendimento_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaId; }
            set { _propostaId = value; }
        }

        [DBFieldInfo("atendimento_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("atendimento_tipoId", FieldType.Single)]
        public Object TipoID
        {
            get { return _tipoId; }
            set { _tipoId= value; }
        }

        [DBFieldInfo("atendimento_subTipoId", FieldType.Single)]
        public Object SubTipoID
        {
            get { return _subTipoId; }
            set { _subTipoId= value; }
        }

        [DBFieldInfo("atendimento_titulo", FieldType.Single)]
        public String Titulo
        {
            get { return _titulo; }
            set { _titulo= value; }
        }

        [DBFieldInfo("atendimento_texto", FieldType.Single)]
        public String Texto
        {
            get { return _texto; }
            set { _texto= value; }
        }

        [DBFieldInfo("atendimento_dataInicio", FieldType.Single)]
        public DateTime DataInicio
        {
            get { return _dataInicio; }
            set { _dataInicio= value; }
        }

        [DBFieldInfo("atendimento_dataPrevisao", FieldType.Single)]
        public DateTime DataPrevisao
        {
            get { return _dataPrevisao; }
            set { _dataPrevisao= value; }
        }

        [DBFieldInfo("atendimento_dataTermino", FieldType.Single)]
        public DateTime DataFim
        {
            get { return _dataFim; }
            set { _dataFim= value; }
        }

        [DBFieldInfo("atendimento_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("atendimento_cadastrado", FieldType.Single)]
        public String IniciadoPor
        {
            get { return _iniciadoPor; }
            set { _iniciadoPor= value; }
        }

        [DBFieldInfo("atendimento_resolvido", FieldType.Single)]
        public String ResolvidoPor
        {
            get { return _resolvidoPor; }
            set { _resolvidoPor= value; }
        }

        [Joinned("atendimentotipo_descricao")]
        public String AtendimentoTipoDescricao
        {
            get { return _atendimentoTipo; }
            set { _atendimentoTipo= value; }
        }

        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        public String TituloOuCategoria
        {
            get
            {
                if (!String.IsNullOrEmpty(_atendimentoTipo))
                    return _atendimentoTipo;
                else
                    return _titulo;
            }
        }

        public String strDataFim
        {
            get
            {
                if (_dataFim == DateTime.MinValue)
                    return String.Empty;
                else
                    return _dataFim.ToString("dd/MM/yyy");
            }
        }

        #endregion

        public AtendimentoTemp(Object id) : this() { _id = id; }
        public AtendimentoTemp() { _data = DateTime.Now; _dataInicio = DateTime.Now; }

        #region métodos EntityBase 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AtendimentoTemp> CarregarPorProposta(Object propostaId)
        {
            String qry = "_atendimento.*, atendimentotipo_descricao FROM _atendimento left join atendimentoTipo on atendimento_tipoId = atendimentotipo_id WHERE atendimento_propostaId=" + propostaId + " ORDER BY atendimento_id DESC";

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTemp>(qry, typeof(AtendimentoTemp));
        }

        public static IList<AtendimentoTemp> CarregarPorProposta(Object propostaId, AtendimentoTipo.eTipo tipo)
        {
            String tipoCond = "";
            if (tipo != AtendimentoTipo.eTipo.Indefinido)
            {
                tipoCond = " and atendimento_tipoId in (select atendimentotipo_id from atendimentoTipo where atendimentotipo_tipo=" + Convert.ToInt32(tipo).ToString() + ") ";
            }

            String qry = String.Concat("_atendimento.*, atendimentotipo_descricao ",
                "   FROM _atendimento ",
                "       left join atendimentoTipo on atendimento_tipoId = atendimentotipo_id ",
                "   WHERE atendimento_propostaId=", propostaId, tipoCond,
                "   ORDER BY atendimento_id DESC");

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTemp>(qry, typeof(AtendimentoTemp));
        }

        public static IList<AtendimentoTemp> CarregarPorProposta(Object propostaId, Object tipoId)
        {
            String qry = "_atendimento.*, atendimentotipo_descricao FROM _atendimento left join atendimentoTipo on atendimento_tipoId = atendimentotipo_id WHERE atendimento_propostaId=" + propostaId + " and atendimento_tipoId = " + tipoId + " ORDER BY atendimento_id DESC";

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTemp>(qry, typeof(AtendimentoTemp));
        }

        public static IList<AtendimentoTemp> Carregar(Object categoriaId, Object subcategoriaId, String[] operadoraIds, String abertoPor, DateTime de, DateTime ate, Int32 status)
        {
            String qry = String.Concat(
                "select operadora_nome, atendimento_id,atendimento_propostaId,atendimento_tipoId,atendimento_titulo,atendimento_dataInicio,atendimento_dataPrevisao,atendimento_dataTermino,atendimento_data,atendimento_cadastrado,atendimento_resolvido,atendimentotipo_descricao,contrato_numero ",
                "   from _atendimento ",
                "       inner join contrato on contrato_id=atendimento_propostaId ",
                "       inner join operadora on operadora_id=contrato_operadoraId ",
                "       left join atendimentoTipo on atendimentotipo_id=atendimento_tipoId ",
                "   where ",
                "       atendimento_dataInicio between '", de.ToString("yyyy-MM-dd"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:990"), "' ",
                "       and operadora_id in (", String.Join(",", operadoraIds), ") ");

            if (categoriaId != null)
                qry += " and atendimento_tipoId=" + categoriaId;
            else
                qry += " and atendimento_tipoId is not null ";

            if (subcategoriaId != null)
                qry += " and atendimento_subtipoId=" + subcategoriaId;

            if (abertoPor != null)
                qry += " and atendimento_cadastrado='" + abertoPor + "'";

            if(status == 1)
                qry += " and atendimento_resolvido is not null ";
            else if(status == 2)
                qry += " and atendimento_resolvido is null ";

            qry += " order by operadora_id, atendimento_dataInicio";

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTemp>(qry, typeof(AtendimentoTemp));
        }

        public static DataTable CarregarDataTable(Object categoriaId, Object subcategoriaId, String[] operadoraIds, String abertoPor, DateTime de, DateTime ate, Int32 status)
        {
            String qry = String.Concat(
                "select operadora_nome as OperadoraNome, plano_descricao as PlanoDescricao, atendimento_id as ID,atendimento_propostaId,atendimento_tipoId,atendimento_titulo,atendimento_dataInicio as DataInicio,atendimento_dataPrevisao as DataPrevisao,atendimento_dataTermino as strDataFim,atendimento_data,atendimento_cadastrado as IniciadoPor,atendimento_resolvido as ResolvidoPor,atendimentotipo_descricao as AtendimentoTipo,contrato_numero as ContratoNumero, contrato_vigencia as ContratoVigencia, beneficiario_nome as TitularNome, beneficiario_cpf as TitularCPF, contratobeneficiario_numMatriculaDental as Dental, contratobeneficiario_numMatriculaSaude as Saude, contrato_datacancelamento as DataCancelamento ",
                "   from _atendimento ",
                "       inner join contrato on contrato_id=atendimento_propostaId ",
                "       inner join operadora on operadora_id=contrato_operadoraId ",
                "       inner join plano on plano_id=contrato_planoId ",
                "       inner join contrato_beneficiario on contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 and contratobeneficiario_contratoId=contrato_id ",
                "       inner join beneficiario on contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 and contratobeneficiario_beneficiarioId = beneficiario_id ",
                "       left join atendimentoTipo on atendimentotipo_id=atendimento_tipoId ",
                "   where ",
                "       atendimento_dataInicio between '", de.ToString("yyyy-MM-dd"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:990"), "' ",
                "       and operadora_id in (", String.Join(",", operadoraIds), ") ");

            if (categoriaId != null)
                qry += " and atendimento_tipoId=" + categoriaId;
            else
                qry += " and atendimento_tipoId is not null ";

            if (subcategoriaId != null)
                qry += " and atendimento_subtipoId=" + subcategoriaId;

            if (abertoPor != null)
                qry += " and atendimento_cadastrado='" + abertoPor + "'";

            if (status == 1)
                qry += " and atendimento_resolvido is not null ";
            else if (status == 2)
                qry += " and atendimento_resolvido is null ";

            qry += " order by operadora_id, atendimento_dataInicio";

            return LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
        }
    }

    [DBTable("_atendimentoItem")]
    public class AtendimentoTempItem : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _atendimentoId;
        String _texto;
        DateTime _data;
        Object _tipoId;
        Object _subTipoId;
        Object _usuarioId;

        #endregion

        #region properties 

        [DBFieldInfo("atendimentoitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("atendimentoitem_atendimentoId", FieldType.Single)]
        public Object AtendimentoID
        {
            get { return _atendimentoId; }
            set { _atendimentoId= value; }
        }

        [DBFieldInfo("atendimentoitem_texto", FieldType.Single)]
        public String Texto
        {
            get { return _texto; }
            set { _texto = value; }
        }

        [DBFieldInfo("atendimentoitem_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [DBFieldInfo("atendimentoitem_tipoId", FieldType.Single)]
        public Object TipoID
        {
            get { return _tipoId; }
            set { _tipoId = value; }
        }

        [DBFieldInfo("atendimentoitem_subTipoId", FieldType.Single)]
        public Object SubTipoID
        {
            get { return _subTipoId; }
            set { _subTipoId= value; }
        }

        [DBFieldInfo("atendimentoitem_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        #endregion

        public AtendimentoTempItem() { }
        public AtendimentoTempItem(Object id) { _id = id; }

        #region métodos EntityBase 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AtendimentoTempItem> CarregarPorProposta(Object atendimentoId)
        {
            String qry = "* FROM _atendimentoItem where atendimentoitem_atendimentoId=" + atendimentoId;

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTempItem>(qry, typeof(AtendimentoTempItem));
        }
    }

    [DBTable("atendimento")]
    public class Atendimento : EntityBase, IPersisteableEntity
    {
        public enum eStatus : int
        {
            Pendente,
            Concluido,
            Cancelado
        }

        #region fields 

        Object _id;
        Int32 _operadora_id;
        Object _atendenteId;
        String _numero_contrato;
        String _nome;
        String _email;
        String _telefone;
        String _cpf;
        DateTime _datahora;
        //Int32 _status;
        String _protocolo;

        Object _item_id;
        String _item_texto;
        DateTime _item_prazo;
        String _item_beneficiario_ids;
        String _item_parentesco_ids;
        String _atendente_apelido;

        #endregion

        public Atendimento() { /*_status = (Int32)eStatus.Pendente;*/ }
        public Atendimento(Object atendimento_id) { _id = atendimento_id; }

        #region propriedades 

        [DBFieldInfo("atendimento_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("atendimento_operadora_id", FieldType.Single)]
        public Int32 OperadoraID
        {
            get { return _operadora_id; }
            set { _operadora_id = value; }
        }

        [DBFieldInfo("atendimento_atendente_id", FieldType.Single)]
        public Object AtendenteID
        {
            get { return _atendenteId; }
            set { _atendenteId= value; }
        }

        [DBFieldInfo("atendimento_numero_contrato", FieldType.Single)]
        public String NumeroContrato
        {
            get { return _numero_contrato; }
            set { _numero_contrato = value; }
        }

        [DBFieldInfo("atendimento_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        [DBFieldInfo("atendimento_email", FieldType.Single)]
        public String Email
        {
            get { return _email; }
            set { _email = value; }
        }

        [DBFieldInfo("atendimento_telefone", FieldType.Single)]
        public String Telefone
        {
            get { return _telefone; }
            set { _telefone = value; }
        }

        public String FTelefone
        {
            get
            {
                return base.FormataTelefone(_telefone);
            }
        }

        [DBFieldInfo("atendimento_cpf", FieldType.Single)]
        public String CPF
        {
            get { return _cpf; }
            set { _cpf = value; }
        }

        [DBFieldInfo("atendimento_datahora", FieldType.Single)]
        public DateTime DataHora
        {
            get { return _datahora; }
            set { _datahora = value; }
        }

        //[DBFieldInfo("atendimento_status", FieldType.Single)]
        //public Int32 Status
        //{
        //    get { return _status; }
        //    set { _status= value; }
        //}

        [DBFieldInfo("atendimento_protocolo", FieldType.Single)]
        public String Protocolo
        {
            get { return _protocolo; }
            set { _protocolo = value; }
        }

        [Joinned("usuario_apelido")]
        public String AtendenteApelido
        {
            get { return _atendente_apelido; }
            set { _atendente_apelido= value; }
        }

        [Joinned("item_id")]
        public Object ItemId
        {
            get { return _item_id; }
            set { _item_id = value; }
        }

        [Joinned("item_texto")]
        public String ItemTexto
        {
            get { return _item_texto; }
            set { _item_texto = value; }
        }

        [Joinned("item_prazo")]
        public DateTime ItemPrazo
        {
            get { return _item_prazo; }
            set { _item_prazo = value; }
        }

        [Joinned("item_beneficiario_ids")]
        public String ItemBeneficiarioIds
        {
            get { return _item_beneficiario_ids; }
            set { _item_beneficiario_ids = value; }
        }

        [Joinned("item_parentesco_ids")]
        public String ItemParentescoIds
        {
            get { return _item_parentesco_ids; }
            set { _item_parentesco_ids = value; }
        }
        #endregion

        #region métodos EntityBase 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<Atendimento> ConsultaAtendimentos(Object operadoraId, Object atendimentoTipo, String dataDe, String dataAte, Object atendenteId, eStatus status)
        {
            String strWhere = "";
            String[] values = null;
            String[] pnames = null;
            DateTime dtDe = DateTime.MinValue, dtAte = DateTime.MaxValue;

            if (operadoraId != null)
                strWhere += " AND atendimento_operadora_id = " + operadoraId;

            if (atendimentoTipo != null)
                strWhere += " AND item_tipo = " + atendimentoTipo;

            if (dataDe.ToString().Trim() != "" && dataAte.ToString().Trim() != "")
            {
                dtDe = Convert.ToDateTime(dataDe);
                dataDe = dtDe.ToString("yyyy-MM-dd");

                dtAte = Convert.ToDateTime(dataAte);
                dataAte = dtAte.ToString("yyyy-MM-dd");

                strWhere += " AND atendimento_datahora >= '" + dataDe + " 00:00:00' ";
                strWhere += " AND atendimento_datahora <= '" + dataAte + " 23:59:59' ";
            }

            if (atendenteId != null)
            {
                strWhere += " AND atendimento_atendente_id=" + atendenteId;
            }

            //if (status == eStatus.Pendente)
            //    strWhere += " AND item_status = 0 ";
            //else if (status == eStatus.Concluido)
            //    strWhere += " AND item_status = 1 ";
            //else
            //    strWhere += " AND item_status = 2 ";
            strWhere += " AND item_status = " + Convert.ToInt32(status);

            String query = String.Concat("",
                "SELECT atendimento.*, usuario_apelido, item_id, item_texto, item_beneficiario_ids, item_parentesco_ids, item_prazo ",
                "   FROM atendimento ",
                "   INNER JOIN atendimento_item ON atendimento_id = item_atendimento_id ",
                "   LEFT JOIN usuario ON atendimento_atendente_id = usuario_id ",
                "   WHERE atendimento_id > 0 ",
                    strWhere,
                "   ORDER BY atendimento_datahora DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Atendimento>
                (query, pnames, values, typeof(Atendimento));
        }

        public static IList<Atendimento> CarregaAtendimentos(List<String> atendimentoIDs)
        {
            return CarregaAtendimentos(atendimentoIDs, null);
        }
        public static IList<Atendimento> CarregaAtendimentos(List<String> atendimentoIDs, PersistenceManager pm)
        {
            String qry = String.Concat("* FROM atendimento INNER JOIN atendimento_item ON atendimento_id = item_atendimento_id WHERE atendimento_id IN (",
                String.Join(",", atendimentoIDs.ToArray()), ")");

            return LocatorHelper.Instance.ExecuteQuery<Atendimento>(qry, typeof(Atendimento), pm);
        }
    }

    [DBTable("atendimentoTipo")]
    public class AtendimentoTipo : EntityBase, IPersisteableEntity
    {
        public enum eTipo
        {
            Indefinido,
            EmpresaCobranca
        }

        Object _id;
        String _descricao;
        String _email;
        Int32 _tipo;
        Int32 _prazoConclusao;

        [DBFieldInfo("atendimentotipo_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("atendimentotipo_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("atendimentotipo_email", FieldType.Single)]
        public String Email
        {
            get { return _email; }
            set { _email= value; }
        }

        [DBFieldInfo("atendimentotipo_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("atendimentotipo_prazoConclusao", FieldType.Single)]
        public Int32 PrazoConclusao
        {
            get { return _prazoConclusao; }
            set { _prazoConclusao= value; }
        }

        public AtendimentoTipo() { }
        public AtendimentoTipo(Object id) { _id = id; }

        #region métodos EntityBase 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AtendimentoTipo> CarregarTodos()
        {
            String qry = "* from atendimentoTipo order by atendimentotipo_descricao";

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTipo>(qry, typeof(AtendimentoTipo));
        }

        public static IList<AtendimentoTipo> CarregarTodos(eTipo tipo)
        {
            String cond = "";

            if (tipo != eTipo.Indefinido)
                cond = " where atendimentotipo_tipo=" + Convert.ToInt32(tipo);

            String qry = "* from atendimentoTipo" + cond + " order by atendimentotipo_descricao";

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoTipo>(qry, typeof(AtendimentoTipo));
        }

        public class UI
        {
            public static void FillComboWithTypes(DropDownList combo)
            {
                combo.Items.Clear();
                combo.Items.Add(new ListItem("indefinido", "0"));
                combo.Items.Add(new ListItem("Empresa de cobrança", "1"));
            }
        }
    }
}
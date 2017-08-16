namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("contratoStatus")]
    public class ContratoStatus : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            Indefinido,
            Inativacao,
            Cancelamento,
            Reativacao
        }

        #region fields 

        Object _id;
        Int32 _tipo;
        String _descricao;

        #endregion

        #region properties 

        [DBFieldInfo("contratostatus_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratostatus_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        public String TipoTRADUZIDO
        {
            get
            {
                return ContratoStatus.TraduzTipo((eTipo)_tipo);
            }
        }

        [DBFieldInfo("contratostatus_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        #endregion

        public static String TraduzTipo(eTipo tipo)
        {
            switch (tipo)
            {
                case eTipo.Cancelamento:
                {
                    return "Cancelamento";
                }
                case eTipo.Inativacao:
                {
                    return "Inativação";
                }
                case eTipo.Reativacao:
                {
                    return "Reativação";
                }
                default:
                {
                    return "Indefinido";
                }
            }
        }

        public class UI
        {
            public static void FillComboWithTypes(System.Web.UI.WebControls.DropDownList combo)
            {
                combo.Items.Clear();
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento", "2"));
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Inativação", "1"));
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Reativação", "3"));
            }
        }

        public ContratoStatus() { }
        public ContratoStatus(Object id) { _id = id; }

        #region entitybase methods 

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

        public static IList<ContratoStatus> Carregar(eTipo tipo)
        {
            String qry = "* from contratoStatus ";
            if (tipo != eTipo.Indefinido) { qry += " where contratostatus_tipo=" + (int)tipo; }
            qry += " order by contratostatus_descricao";

            return LocatorHelper.Instance.ExecuteQuery<ContratoStatus>(qry, typeof(ContratoStatus));
        }
    }

    [DBTable("contratoStatusInstancia")]
    public class ContratoStatusInstancia : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _statusId;
        Object _contratoId;
        DateTime _data;
        DateTime _dataSistema;
        Object _usuarioId;
        String _obs;

        Int32 _statusTipo;
        String _statusDescricao;

        #endregion

        #region properties 

        [DBFieldInfo("contratostatusinst_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratostatusinst_statusId", FieldType.Single)]
        public Object StatusID
        {
            get { return _statusId; }
            set { _statusId= value; }
        }

        [DBFieldInfo("contratostatusinst_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("contratostatusinst_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("contratostatusinst_dataSistema", FieldType.Single)]
        public DateTime DataSistema
        {
            get { if (_dataSistema == DateTime.MinValue) { _dataSistema = DateTime.Now; } return _dataSistema; }
            set { _dataSistema = value; }
        }

        [DBFieldInfo("contratostatusinst_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("contratostatusinst_obs", FieldType.Single)]
        public String OBS
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [Joinned("contratostatus_descricao")]
        public String StatusDescricao
        {
            get { return _statusDescricao; }
            set { _statusDescricao= value; }
        }

        [Joinned("contratostatus_tipo")]
        public Int32 StatusTipo
        {
            get { return _statusTipo; }
            set { _statusTipo= value; }
        }

        public String StatusTipoTRADUZIDO
        {
            get { return ContratoStatus.TraduzTipo((ContratoStatus.eTipo)_statusTipo); }
        }

        #endregion

        public ContratoStatusInstancia() { }
        public ContratoStatusInstancia(Object id) { _id = id; }

        #region entitybase methods 

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

        public static IList<ContratoStatusInstancia> Carregar(Object contratoId)
        {
            String qry = String.Concat(
                "contratoStatusInstancia.*, contratostatus_descricao, contratostatus_tipo ",
                "   from contratoStatusInstancia ",
                "       inner join contratoStatus on contratostatusinst_statusId = contratostatus_id ",
                "   where contratostatusinst_contratoId=", contratoId,
                "   order by contratostatusinst_data desc, contratostatusinst_id desc");

            return LocatorHelper.Instance.ExecuteQuery
                <ContratoStatusInstancia>(qry, typeof(ContratoStatusInstancia));
        }

        public static IList<ContratoStatusInstancia> Carregar_SemObs(Object contratoId)
        {
            String qry = String.Concat(
                "contratostatusinst_id,contratostatusinst_statusId,contratostatusinst_contratoId,contratostatusinst_data,contratostatusinst_usuarioId,contratostatusinst_dataSistema, contratostatus_descricao, contratostatus_tipo ",
                "   from contratoStatusInstancia ",
                "       inner join contratoStatus on contratostatusinst_statusId = contratostatus_id ",
                "   where contratostatusinst_contratoId=", contratoId,
                "   order by contratostatusinst_data desc, contratostatusinst_id desc");

            return LocatorHelper.Instance.ExecuteQuery
                <ContratoStatusInstancia>(qry, typeof(ContratoStatusInstancia));
        }

        public static ContratoStatusInstancia CarregarUltima(Object contratoId)
        {
            String qry = String.Concat(
                "top 1 contratoStatusInstancia.*, contratostatus_descricao, contratostatus_tipo ",
                "   from contratoStatusInstancia ",
                "       inner join contratoStatus on contratostatusinst_statusId = contratostatus_id ",
                "   where contratostatusinst_contratoId=", contratoId,
                "   order by contratostatusinst_data desc, contratostatusinst_id desc");

            IList<ContratoStatusInstancia> list = LocatorHelper.Instance.
                ExecuteQuery<ContratoStatusInstancia>(qry, typeof(ContratoStatusInstancia));

            if (list == null)
                return null;
            else
                return list[0];
        }
    }
}
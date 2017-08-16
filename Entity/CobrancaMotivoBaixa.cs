namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Data;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("cobranca_motivoBaixa")]
    public class CobrancaMotivoBaixa : EntityBase, IPersisteableEntity 
    {
        Object _id;
        String _codigo;
        String _descricao;

        public CobrancaMotivoBaixa() { }
        public CobrancaMotivoBaixa(Object id) { _id = id; }

        #region Properties 

        [DBFieldInfo("motivobaixa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("motivobaixa_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo= value; }
        }

        [DBFieldInfo("motivobaixa_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }


        #endregion

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

        public static IList<CobrancaMotivoBaixa> CarregarTodos()
        {
            String qry = "* from cobranca_motivoBaixa order by motivobaixa_descricao";

            return LocatorHelper.Instance.ExecuteQuery<CobrancaMotivoBaixa>(qry, typeof(CobrancaMotivoBaixa));
        }
    }

    [DBTable("cobranca_baixa")]
    public class CobrancaBaixa : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _cobrancaId;
        Object _motivoId;
        Int32 _tipo;
        Object _usuarioId;
        String _obs;
        Boolean _baixaFinanceira;
        Boolean _baixaProvisoria;
        DateTime _data;

        public CobrancaBaixa() { }
        public CobrancaBaixa(Object id) { _id = id; }

        #region Properties 

        [DBFieldInfo("cobrancabaixa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("cobrancabaixa_cobrancaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        [DBFieldInfo("cobrancabaixa_motivoId", FieldType.Single)]
        public Object MotivoID
        {
            get { return _motivoId; }
            set { _motivoId= value; }
        }

        [DBFieldInfo("cobrancabaixa_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("cobrancabaixa_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("cobrancabaixa_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("cobrancabaixa_baixaFinanceira", FieldType.Single)]
        public Boolean BaixaFinanceira
        {
            get { return _baixaFinanceira; }
            set { _baixaFinanceira= value; }
        }

        [DBFieldInfo("cobrancabaixa_baixaProvisoria", FieldType.Single)]
        public Boolean BaixaProvisoria
        {
            get { return _baixaProvisoria; }
            set { _baixaProvisoria= value; }
        }

        [DBFieldInfo("cobrancabaixa_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }


        #endregion

        public string strTipo
        {
            get
            {
                if (_tipo == 0)
                    return "Baixa";
                else if (_tipo == 1)
                    return "Estorno";
                else if (_tipo == 2)
                    return "Alteração";
                else
                    return "";
            }
        }

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

        public static IList<CobrancaBaixa> CarregarTodos(Object cobrancaId)
        {
            return CarregarTodos(cobrancaId, null);
        }

        public static IList<CobrancaBaixa> CarregarTodos(Object cobrancaId, PersistenceManager pm)
        {
            String qry = String.Concat("* from cobranca_baixa where cobrancabaixa_cobrancaId=", cobrancaId, " order by cobrancabaixa_data desc");

            return LocatorHelper.Instance.ExecuteQuery<CobrancaBaixa>(qry, typeof(CobrancaBaixa), pm);
        }

        public static CobrancaBaixa CarregarUltima(Object cobrancaId)
        {
            return CarregarUltima(cobrancaId, null);
        }

        public static CobrancaBaixa CarregarUltima(Object cobrancaId, PersistenceManager pm)
        {
            String qry = "top 1 * from cobranca_baixa where cobrancabaixa_cobrancaId=" + cobrancaId + " order by cobrancabaixa_data desc";

            IList<CobrancaBaixa> list = LocatorHelper.Instance.ExecuteQuery<CobrancaBaixa>(qry, typeof(CobrancaBaixa), pm);

            if (list == null || list.Count == 0)
                return null;
            else
                return list[0];
        }

        public static void Remover(Object cobrancaId, Object motivoId, PersistenceManager pm)
        {
            String cmd = String.Concat("delete from cobranca_baixa where cobrancabaixa_cobrancaId=",
                cobrancaId, " and cobrancabaixa_motivoId=", motivoId);

            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }
    }
}
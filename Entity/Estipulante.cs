namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("estipulante")]
    public class Estipulante : EntityBase, IPersisteableEntity
    {
        public enum eTipoTaxa : int
        {
            PorVida = 0,
            PorProposta
        }

        Object _id;
        String _descricao;
        String _textoBoleto;
        Boolean _ativo;

        #region propriedades 

        [DBFieldInfo("estipulante_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("estipulante_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("estipulante_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("estipulante_textoBoleto", FieldType.Single)]
        public String TextoBoleto
        {
            get { return _textoBoleto; }
            set { _textoBoleto= value; }
        }

        //[DBFieldInfo("estipulante_dataVencimento", FieldType.Single)]
        //public Int32 DiaVencimento
        //{
        //    get;
        //    set;
        //}

        #endregion

        public Estipulante() { _ativo = true; }
        public Estipulante(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            String comm = "DELETE FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(comm, null);
            base.Remover(this);

        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<Estipulante> Carregar(Boolean apenasAtivos)
        {
            String query = "* FROM estipulante";
            if (apenasAtivos) { query += " WHERE estipulante_ativo=1"; }
            query += " ORDER BY estipulante_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Estipulante>(query, typeof(Estipulante));
        }

        public static Boolean Duplicado(Object estipulanteId, String descricao)
        {
            String qry = "SELECT DISTINCT(estipulante_id) FROM estipulante WHERE estipulante_descricao=@descricao";

            String[] pNames = new String[] { "@descricao" };
            String[] pValues = new String[] { descricao };

            if (estipulanteId != null)
            {
                qry += " AND estipulante_id <> " + estipulanteId;
            }

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, pNames, pValues);

            if (returned == null || returned == DBNull.Value)
                return false;
            else
                return true;
        }

        public static Object CarregaID(String descricao, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@descricao" };
            String[] paramVl = new String[] { descricao };
            return LocatorHelper.Instance.ExecuteScalar("SELECT estipulante_id FROM estipulante WHERE estipulante_descricao=@descricao", paramNm, paramVl, pm);
        }
    }

    [DBTable("estipulante_taxa")]
    public class EstipulanteTaxa : EntityBase, IPersisteableEntity
    {
        public enum eTipoTaxa : int
        {
            PorVida = 0,
            PorProposta
        }

        Object _id;
        Object _estipulanteId;
        Decimal _valor;
        Int32 _tipoTaxa;
        DateTime _vigencia;

        #region propriedades

        [DBFieldInfo("estipulantetaxa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("estipulantetaxa_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId= value; }
        }

        [DBFieldInfo("estipulantetaxa_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [DBFieldInfo("estipulantetaxa_tipo", FieldType.Single)]
        public Int32 TipoTaxa
        {
            get { return _tipoTaxa; }
            set { _tipoTaxa= value; }
        }

        public String strTipoTaxa
        {
            get
            {
                if (_tipoTaxa == (Int32)eTipoTaxa.PorProposta)
                    return "POR PROPOSTA";
                else
                    return "POR VIDA";
            }
        }

        [DBFieldInfo("estipulantetaxa_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia= value; }
        }

        #endregion

        public EstipulanteTaxa() { _valor = 0; TipoTaxa = (Int32)eTipoTaxa.PorProposta; }
        public EstipulanteTaxa(Object id) : this() { _id = id; }

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

        public static IList<EstipulanteTaxa> CarregarTodas(Object estipulanteId)
        {
            String query = "* FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + estipulanteId + " ORDER BY estipulantetaxa_vigencia DESC";

            return LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa));
        }

        public static EstipulanteTaxa CarregarVigente(Object estipulanteId)
        {
            return CarregarVigente(estipulanteId, null);
        }
        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, PersistenceManager pm)
        {
            String query = "TOP 1 * FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + estipulanteId + " ORDER BY estipulantetaxa_vigencia DESC";
            IList<EstipulanteTaxa> lista = LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, DateTime dataRef)
        {
            return CarregarVigente(estipulanteId, dataRef, null);
        }
        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, DateTime dataRef, PersistenceManager pm)
        {
            String query = String.Concat(
                "TOP 1 * FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=", 
                estipulanteId, 
                " and estipulantetaxa_vigencia <= '", dataRef.ToString("yyyy-MM-dd 23:59:59.995"), "'", 
                " ORDER BY estipulantetaxa_vigencia DESC");

            IList<EstipulanteTaxa> lista = LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }
    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("taxa")]
    public class Taxa : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _tabelaValorId;
        Decimal _over;
        Decimal _fixo;
        Boolean _embutido;
        Decimal _valorEmbutido;
        Decimal _percentual;
        DateTime _data;

        #endregion

        #region propriedades 

        [DBFieldInfo("taxa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("taxa_tabelaValorId", FieldType.Single)]
        public Object TabelaValorID
        {
            get { return _tabelaValorId; }
            set { _tabelaValorId= value; }
        }

        [DBFieldInfo("taxa_over", FieldType.Single)]
        public Decimal Over
        {
            get { return _over; }
            set { _over= value; }
        }

        [DBFieldInfo("taxa_fixo", FieldType.Single)]
        public Decimal Fixo
        {
            get { return _fixo; }
            set { _fixo= value; }
        }

        [DBFieldInfo("taxa_embutido", FieldType.Single)]
        public Boolean Embutido
        {
            get { return _embutido; }
            set { _embutido= value; }
        }

        [DBFieldInfo("taxa_valorEmbutido", FieldType.Single)]
        public Decimal ValorEmbutido
        {
            get { return _valorEmbutido; }
            set { _valorEmbutido= value; }
        }

        [DBFieldInfo("taxa_percentualReajuste", FieldType.Single)]
        public Decimal PercentualReajuste
        {
            get { return _percentual; }
            set { _percentual= value; }
        }

        [DBFieldInfo("taxa_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        public Taxa() { _percentual = 0; }
        public Taxa(Object id) : this() { _id = id; }

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

        public static Taxa CarregarPorTabela(Object tabelaValorId)
        {
            return CarregarPorTabela(tabelaValorId, null);
        }
        public static Taxa CarregarPorTabela(Object tabelaValorId, PersistenceManager pm)
        {
            String query = "* FROM taxa WHERE taxa_tabelaValorId=" + tabelaValorId + " ORDER BY taxa_data DESC, taxa_id DESC";
            IList<Taxa> lista = LocatorHelper.Instance.ExecuteQuery<Taxa>(query, typeof(Taxa), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }
}

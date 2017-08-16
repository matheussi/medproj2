namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("tabela_reajuste")]
    public class TabelaReajuste : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoAdmId;
        String _descricao;
        DateTime _data;
        Object _corrente;

        #region propriedades 

        [DBFieldInfo("tabelareajuste_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("tabelareajuste_contratoid", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        [DBFieldInfo("tabelareajuste_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("tabelareajuste_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        /// <summary>
        /// ID da tabela de reajuste atualmente ativa no contrato administrativo
        /// </summary>
        [Joinned("contratoadm_tabelaReajusteAtualId")]
        public Object Corrente
        {
            get { return _corrente; }
            set { _corrente= value; }
        }

        #endregion

        public TabelaReajusteItem TabelaReajusteItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public TabelaReajuste() { }

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

        public static IList<TabelaReajuste> CarregarTodas(Object contratoId)
        {
            String query = "tabela_reajuste.*, contratoadm_tabelaReajusteAtualId FROM tabela_reajuste LEFT JOIN contratoADM ON tabelareajuste_id=contratoadm_tabelaReajusteAtualId WHERE tabelaReajuste_contratoId=" + contratoId + " ORDER BY contratoadm_tabelaReajusteAtualId DESC, tabelareajuste_data DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaReajuste>(query, typeof(TabelaReajuste));
        }
    }

    [Serializable()]
    [DBTable("tabela_reajuste_item")]
    public class TabelaReajusteItem : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _tabelaId;
        int _idadeInicio;
        Decimal _percentualReajuste;

        #region propriedades

        [DBFieldInfo("tabelareajusteitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("tabelareajusteitem_tabelaid", FieldType.Single)]
        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId= value; }
        }

        [DBFieldInfo("tabelareajusteitem_idadeInicio", FieldType.Single)]
        public int IdadeInicio
        {
            get { return _idadeInicio; }
            set { _idadeInicio= value; }
        }

        //[DBFieldInfo("tabelareajusteitem_tipoAcomodacao", FieldType.Single)]
        //public int TipoAcomodacao
        //{
        //    get { return _tipoAcomodacao; }
        //    set { _tipoAcomodacao= value; }
        //}

        [DBFieldInfo("tabelareajusteitem_reajuste", FieldType.Single)]
        public Decimal PercentualReajuste
        {
            get { return _percentualReajuste; }
            set { _percentualReajuste= value; }
        }

        #endregion

        public TabelaReajusteItem() { }

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

        public static IList<TabelaReajusteItem> CarregarPorTabela(Object TabelaID)
        {
            String query = "* FROM tabela_reajuste_item WHERE tabelareajusteitem_tabelaid=" + TabelaID + " ORDER BY tabelareajusteitem_idadeInicio";
            return LocatorHelper.Instance.ExecuteQuery<TabelaReajusteItem>(query, typeof(TabelaReajusteItem));
        }
    }
}
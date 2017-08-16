namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa os tipos disponíveis de produto de almoxarifado.
    /// </summary>
    [DBTable("almox_tipo_produto")]
    public class AlmoxTipoProduto : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _descricao;
        Boolean _numerado;

        #region propriedades 

        [DBFieldInfo("almox_tipoproduto_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("almox_tipoproduto_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("almox_tipoproduto_numerado", FieldType.Single)]
        public Boolean Numerado
        {
            get { return _numerado; }
            set { _numerado= value; }
        }

        #endregion

        public AlmoxTipoProduto() { _numerado = false; }
        public AlmoxTipoProduto(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            try
            {
                base.Remover(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AlmoxTipoProduto> CarregarTodos()
        {
            String query = "* FROM almox_tipo_produto ORDER BY almox_tipoproduto_descricao";
            return LocatorHelper.Instance.ExecuteQuery<AlmoxTipoProduto>(query, typeof(AlmoxTipoProduto));
        }
    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("grupo_venda")]
    public class GrupoDeVenda : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _descricao;
        Boolean _ativo;
        DateTime _data;

        #region propriedades 

        [DBFieldInfo("grupovenda_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("grupovenda_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("grupovenda_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("grupovenda_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion


        public GrupoDeVenda() { _data = DateTime.Now; _ativo = true; }
        public GrupoDeVenda(Object id) : this() { _id = id; }

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

        public static IList<GrupoDeVenda> Carregar(Boolean apenasAtivos)
        {
            String qry = "* FROM grupo_venda ";
            if (apenasAtivos) { qry += "WHERE grupovenda_ativo=1 "; }
            qry += "ORDER BY grupovenda_descricao";

            return LocatorHelper.Instance.ExecuteQuery<GrupoDeVenda>(qry, typeof(GrupoDeVenda));
        }
    }
}

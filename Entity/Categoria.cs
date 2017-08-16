namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("categoria")]
    public class Categoria : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _perfilId;
        String _descricao;
        Boolean _ativo;

        String _perfilDescricao;

        #region propriedades 

        [DBFieldInfo("categoria_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("categoria_perfilId", FieldType.Single)]
        public Object PerfilID
        {
            get { return _perfilId; }
            set { _perfilId= value; }
        }

        [DBFieldInfo("categoria_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("categoria_ativa", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [Joinned("perfil_descricao")]
        public String PerfilDescricao
        {
            get { return _perfilDescricao; }
            set { _perfilDescricao= value; }
        }

        #endregion

        public Categoria() { _ativo = true; }

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

        public static IList<Categoria> Carregar(Boolean apenasAtivas)
        {
            return Carregar(apenasAtivas, false);
        }

        public static IList<Categoria> Carregar_OrdenadoPorPerfil(Boolean apenasAtivas)
        {
            return Carregar(apenasAtivas, true);
        }

        static IList<Categoria> Carregar(Boolean apenasAtivas, Boolean ordenadoPorPerfil)
        {
            String query = "categoria.*, perfil_descricao FROM categoria LEFT JOIN perfil ON categoria_perfilId=perfil_id";
            if (apenasAtivas) { query += " WHERE categoria_ativa=1"; }

            if(!ordenadoPorPerfil)
                query += " ORDER BY categoria_descricao";
            else
                query += " ORDER BY perfil_descricao, categoria_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Categoria>(query, typeof(Categoria));
        }
    }
}
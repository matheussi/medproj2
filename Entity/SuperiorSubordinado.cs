namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.BusinessLayer;

    [DBTableAttribute("superior_subordinado")]
    public class SuperiorSubordinado : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _subordinadoId;
        Object _superiorId;
        DateTime _data;

        String _subordinadoNome;
        String _subordinadoApelido;
        String _subordinadoPerfilId;
        String _superiorNome;
        String _superiorApelido;
        String _superiorFilialNome;
        String _superiorPerfilId;
        String _superiorPerfilDescricao;
        String _superiorBanco;
        String _superiorConta;
        String _superiorAgencia;

        #endregion

        #region properties 

        [DBFieldInfo("ss_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("ss_subordinadoId", FieldType.Single)]
        public Object SubordinadoID
        {
            get { return _subordinadoId; }
            set { _subordinadoId= value; }
        }

        [DBFieldInfo("ss_superiorId", FieldType.Single)]
        public Object SuperiorID
        {
            get { return _superiorId; }
            set { _superiorId= value; }
        }

        [DBFieldInfo("ss_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("subordinadoNome")]
        public String SubordinadoNome
        {
            get { return _subordinadoNome; }
            set { _subordinadoNome= value; }
        }

        [Joinned("subordinadoApelido")]
        public String SubordinadoApelido
        {
            get { return _subordinadoApelido; }
            set { _subordinadoApelido= value; }
        }

        [Joinned("subordinadoPerfilID")]
        public String SubordinadoPerfilID
        {
            get { return _subordinadoPerfilId; }
            set { _subordinadoPerfilId= value; }
        }

        [Joinned("superiorNome")]
        public String SuperiorNome
        {
            get { return _superiorNome; }
            set { _superiorNome= value; }
        }

        [Joinned("superiorApelido")]
        public String SuperiorApelido
        {
            get { return _superiorApelido; }
            set { _superiorApelido= value; }
        }

        [Joinned("superiorFilialNome")]
        public String SuperiorFilialNome
        {
            get { return _superiorFilialNome; }
            set { _superiorFilialNome= value; }
        }

        [Joinned("superiorPerfilID")]
        public String SuperiorPerfilID
        {
            get { return _superiorPerfilId; }
            set { _superiorPerfilId= value; }
        }

        [Joinned("superiorPerfilDescricao")]
        public String SuperiorPerfilDescricao
        {
            get { return _superiorPerfilDescricao; }
            set { _superiorPerfilDescricao= value; }
        }

        [Joinned("superiorBanco")]
        public String SuperiorBanco
        {
            get { return _superiorBanco; }
            set { _superiorBanco= value; }
        }

        [Joinned("superiorConta")]
        public String SuperiorConta
        {
            get { return _superiorConta; }
            set { _superiorConta= value; }
        }

        [Joinned("superiorAgencia")]
        public String SuperiorAgencia
        {
            get { return _superiorAgencia; }
            set { _superiorAgencia= value; }
        }

        #endregion

        public SuperiorSubordinado() { }
        public SuperiorSubordinado(Object id) { this._id = id; }

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

        public static IList<SuperiorSubordinado> CarregarSuperiores(Object usuarioId)
        {
            return CarregarSuperiores(usuarioId, null);
        }
        public static IList<SuperiorSubordinado> CarregarSuperiores(Object usuarioId, PersistenceManager pm)
        {
            return CarregarSuperiores(usuarioId, null, pm);
        }

        public static IList<SuperiorSubordinado> CarregarSuperiores(Object usuarioId, DateTime? dataRef, PersistenceManager pm)
        {
            String dataParam = "", top = " ";
            if (dataRef != null)
            {
                dataParam = String.Concat(" AND ss_data <= '", dataRef.Value.Year, "/", dataRef.Value.Month, "/", dataRef.Value.Day, "' ");
                top = " TOP 1 ";
            }

            String query = String.Concat("SELECT", top, "superior_subordinado.*, b.usuario_nome as subordinadoNome, b.usuario_apelido as subordinadoApelido, a.usuario_nome as superiorNome, a.usuario_apelido as superiorApelido, a.usuario_banco as superiorBanco, a.usuario_agencia as superiorAgencia, a.usuario_conta as superiorConta ,filial_nome as superiorFilialNome, sup.perfil_id as superiorPerfilID, sup.perfil_descricao as superiorPerfilDescricao, sub.perfil_id as subordinadoPerfilID ",
                "FROM superior_subordinado ",
                "INNER JOIN usuario a ON ss_superiorId = a.usuario_id ",
                "LEFT  JOIN filial ON a.usuario_filialId = filial_id ",
                "INNER JOIN perfil sup ON a.usuario_perfilId = sup.perfil_id ",
                "INNER JOIN usuario b ON ss_subordinadoId = b.usuario_id ",
                "INNER JOIN perfil sub ON b.usuario_perfilId = sub.perfil_id ",
                "WHERE ss_subordinadoId=", usuarioId, dataParam,
                "ORDER BY ss_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<SuperiorSubordinado>(query, typeof(SuperiorSubordinado), pm);
        }
    }
}

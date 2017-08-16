namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("comissao_usuario")]
    public class ComissionamentoUsuario : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _usuarioId;
        Object _tabelaComissionamentoid;
        Object _grupoVendaId;
        Object _perfilId;
        DateTime _data;
        Boolean _ativo;

        String _usuarioNome;
        //String _operadoraNome;
        String _tabelaComissionamentoNome;
        DateTime _tabelaComissionamentoData;
        String _tabelaComissionamentoCategoriaNome;
        String _grupoVendaDescricao;
        String _perfilDescricao;

        #endregion

        #region propriedades 

        [DBFieldInfo("comissaousuario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaousuario_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("comissaousuario_perfilId", FieldType.Single)]
        public Object PerfilID
        {
            get { return _perfilId; }
            set { _perfilId= value; }
        }

        [Joinned("comissaomodelo_id")]
        [DBFieldInfo("comissaousuario_tabelaComissionamentoId", FieldType.Single)]
        public Object TabelaComissionamentoID
        {
            get { return _tabelaComissionamentoid; }
            set { _tabelaComissionamentoid= value; }
        }

        [DBFieldInfo("comissaousuario_grupoVendaId", FieldType.Single)]
        public Object GrupoVendaID
        {
            get { return _grupoVendaId; }
            set { _grupoVendaId= value; }
        }

        [DBFieldInfo("comissaousuario_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("comissaousuario_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [Joinned("usuario_nome")]
        public String UsuarioNome
        {
            get { return _usuarioNome; }
            set { _usuarioNome= value; }
        }

        //[Joinned("operadora_nome")]
        //public String OperadoraNome
        //{
        //    get { return _operadoraNome; }
        //    set { _operadoraNome= value; }
        //}

        [Joinned("comissaomodelo_descricao")]
        public String TabelaComissionamentoNome
        {
            get { return _tabelaComissionamentoNome; }
            set { _tabelaComissionamentoNome= value; }
        }

        [Joinned("comissaomodelo_data")]
        public DateTime TabelaComissionamentoData
        {
            get { return _tabelaComissionamentoData; }
            set { _tabelaComissionamentoData= value; }
        }

        [Joinned("categoria_descricao")]
        public String TabelaComissionamentoCategoriaNome
        {
            get { return _tabelaComissionamentoCategoriaNome; }
            set { _tabelaComissionamentoCategoriaNome= value; }
        }

        [Joinned("grupovenda_descricao")]
        public String GrupoVendaDescricao
        {
            get { return _grupoVendaDescricao; }
            set { _grupoVendaDescricao= value; }
        }

        [Joinned("perfil_descricao")]
        public String PerfilDescricao
        {
            get { return _perfilDescricao; }
            set { _perfilDescricao= value; }
        }

        /// <summary>
        /// Índice 0: ID deste objeto ou "_" se null, 
        /// Índice 1: ID da tabela modelo de comissionamento.
        /// </summary>
        public String IDIndexado
        {
            get
            {
                String id = "";

                if (this._id == null)
                    id = "_|" + Convert.ToString(this._tabelaComissionamentoid);
                else
                    id = String.Concat(this._id, "|", this._tabelaComissionamentoid);

                return id;
            }
        }

        public String Resumo
        {
            get
            {
                String _texto = "";

                _texto = String.Concat(this._tabelaComissionamentoNome, " (",
                    this._tabelaComissionamentoCategoriaNome, ")");

                return _texto;
            }
        }

        #endregion

        public ComissionamentoUsuario() { _ativo = true; _data = DateTime.Now; }

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

        public static void Salvar(IList<ComissionamentoUsuario> lista)
        {
            if (lista == null) { return; }

            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                foreach (ComissionamentoUsuario obj in lista)
                {
                    pm.Save(obj);
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        public static ComissionamentoUsuario CarregarVigente(Object usuarioId, DateTime? data)
        {
            return CarregarVigente(usuarioId, data, null);
        }
        public static ComissionamentoUsuario CarregarVigente(Object usuarioId, DateTime? data, PersistenceManager pm)
        {
            String dataParam = "";
            if (data != null)
            {
                dataParam = String.Concat(" AND comissaousuario_data <= '", data.Value.Year, "-", data.Value.Month, "-", data.Value.Day, "' ");
            }

            String query = String.Concat("TOP 1 comissao_usuario.*, grupovenda_descricao, usuario_nome, comissaomodelo_id, comissaomodelo_descricao, comissaomodelo_data, categoria_descricao, perfil_descricao ",
                    " FROM comissao_usuario ",
                    "   INNER JOIN usuario ON comissaousuario_usuarioId=usuario_id ",
                    "   INNER JOIN comissao_modelo ON comissaousuario_tabelaComissionamentoId=comissaomodelo_id ",
                    "   INNER JOIN categoria ON comissaomodelo_categoriaId=categoria_id ",
                    "   LEFT JOIN grupo_venda ON comissaousuario_grupovendaId=grupovenda_id ",
                    "   LEFT JOIN perfil ON perfil_id = comissaousuario_perfilId ",
                    " WHERE comissaousuario_ativo=1 AND comissaousuario_usuarioId=", usuarioId, dataParam, " ORDER BY comissaousuario_data DESC, comissaomodelo_descricao");

            IList<ComissionamentoUsuario> lista = LocatorHelper.Instance.ExecuteQuery
                <ComissionamentoUsuario>(query, typeof(ComissionamentoUsuario), pm);

            if (lista == null) { return null; }
            else { return lista[0]; }
        }

        public static IList<ComissionamentoUsuario> Carregar(Object contratoId, Object usuarioId, Boolean apenasTabelasAssinaladasAoUsuario)
        {
            String query = "";

            //if (apenasTabelasAssinaladasAoUsuario)
            //{
            query = String.Concat("comissao_usuario.*, contratoadm_descricao, usuario_nome, comissaomodelo_id, comissaomodelo_descricao, comissaomodelo_data, categoria_descricao, perfil_descricao ",
                    " FROM comissao_usuario ",
                    " INNER JOIN usuario ON comissaousuario_usuarioId=usuario_id ",
                    " INNER JOIN comissao_modelo ON comissaousuario_tabelaComissionamentoId=comissaomodelo_id ",
                    " INNER JOIN contratoAdm ON comissaomodelo_contratoAdmId=contratoadm_id ",
                    " INNER JOIN categoria ON comissaomodelo_categoriaId=categoria_id ",
                    " LEFT JOIN perfil ON perfil_id = comissaousuario_perfilId ",
                    " WHERE comissaousuario_usuarioId=", usuarioId, " AND comissaomodelo_contratoAdmId=", contratoId);
            //}
            //else
            //{
            //    query = String.Concat("comissao_usuario.*, contratoadm_descricao, usuario_nome, comissaomodelo_id, comissaomodelo_descricao, comissaomodelo_data, categoria_descricao ",
            //        " FROM comissao_modelo ",
            //        " INNER JOIN operadora ON comissaomodelo_operadoraId=operadora_id ",
            //        " INNER JOIN categoria ON comissaomodelo_categoriaId=categoria_id ",
            //        " LEFT  JOIN comissao_usuario ON comissaousuario_tabelaComissionamentoId=comissaomodelo_id ",
            //        " LEFT  JOIN usuario ON comissaousuario_usuarioId=usuario_id AND usuario_id=", usuarioId,
            //        " WHERE contratoadm_id=", contratoId);
            //}

            return LocatorHelper.Instance.ExecuteQuery
                <ComissionamentoUsuario>(query, typeof(ComissionamentoUsuario));
        }

        public static IList<ComissionamentoUsuario> Carregar(Object usuarioId)
        {
            String query = String.Concat("comissao_usuario.*, grupovenda_descricao, contratoadm_descricao, usuario_nome, comissaomodelo_id, comissaomodelo_descricao, comissaomodelo_data, categoria_descricao, perfil_descricao ",
                    " FROM comissao_usuario ",
                    " INNER JOIN usuario ON comissaousuario_usuarioId=usuario_id ",
                    " INNER JOIN comissao_modelo ON comissaousuario_tabelaComissionamentoId=comissaomodelo_id ",
                    " LEFT JOIN contratoAdm ON comissaomodelo_contratoAdmId=contratoadm_id ",
                    " INNER JOIN categoria ON comissaomodelo_categoriaId=categoria_id ",
                    " LEFT JOIN grupo_venda ON comissaousuario_grupovendaId=grupovenda_id ",
                    " LEFT JOIN perfil ON perfil_id = comissaousuario_perfilId ",
                    " WHERE comissaousuario_usuarioId=", usuarioId, " ORDER BY comissaousuario_data DESC, comissaomodelo_descricao");

            return LocatorHelper.Instance.ExecuteQuery
                <ComissionamentoUsuario>(query, typeof(ComissionamentoUsuario));
        }
    }
}

/*
select comissao_usuario.*, operadora_nome, comissaomodelo_id, comissaomodelo_descricao, categoria_descricao  
	FROM comissao_modelo  
		INNER JOIN operadora ON comissaomodelo_operadoraId=operadora_id  
		INNER JOIN categoria ON comissaomodelo_categoriaId=categoria_id 
		LEFT  JOIN comissao_usuario ON comissaousuario_tabelaComissionamentoId=comissaomodelo_id  
		LEFT  JOIN usuario ON comissaousuario_usuarioId=usuario_id AND comissaousuario_usuarioId=8 
WHERE operadora_id=1
*/
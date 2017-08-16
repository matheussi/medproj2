namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Data;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.BusinessLayer;

    [DBTableAttribute("usuario")]
    public class Usuario : EntityBase, IPersisteableEntity
    {
        #region enum UsuarioDonoTipo 
        /// <summary>
        /// Determina o tipo de usário.
        /// </summary>
        public enum UsuarioDonoTipo : int
        {
            /// <summary>
            /// Usuário indefinido
            /// </summary>
            Indefinido,
            /// <summary>
            /// Usuário corretor
            /// </summary>
            Corretor,
            /// <summary>
            /// Usuário supervisor
            /// </summary>
            Supervisor
        }
        #endregion

        #region fields 

        Object _id;
        String _nome;
        String _codigo;
        String _apelido;
        String _email;
        String _marcaotica;
        String _senha;
        Object _superiorId;
        Object _perfilId;
        Object _categoriaId;
        Object _filialId;
        Boolean _ativo;
        Boolean _liberaContratos;
        Boolean _alteraValorContratos;
        Boolean _systemUser;
        Boolean _alteraProdutor;

        DateTime _dataNascimento;
        Int32 _sexo;
        Int32 _estadoCivil;
        Int32 _tipo;
        String _documento1;
        String _documento2;
        String _ddd1;
        String _fone1;
        String _ramal1;
        String _ddd2;
        String _fone2;
        String _ramal2;
        String _ddd3;
        String _fone3;
        String _ramal3;
        String _entrevistadoPor;
        DateTime _entrevistadoEm;
        String _banco;
        String _agencia;
        String _contaTipo;
        String _conta;
        String _favorecido;
        String _obs;

        Object _empresaCobrancaId;
        Boolean _extraPermission;
        Boolean _extraPermission2;

        String _filialDescricao;
        String _perfilDescricao;

        /// <summary>
        /// Determina se a senha foi encriptada.
        /// </summary>
        Boolean _senhaEncriptada;

        #endregion

        #region propriedades

        [DBFieldInfo("usuario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("usuario_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("usuario_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo= value; }
        }

        [DBFieldInfo("usuario_apelido", FieldType.Single)]
        public String Apelido
        {
            get { return _apelido; }
            set { _apelido= value; }
        }

        [DBFieldInfo("usuario_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        [DBFieldInfo("usuario_marcaotica", FieldType.Single)]
        public String MarcaOtica
        {
            get { return _marcaotica; }
            set { _marcaotica= value; }
        }

        [DBFieldInfo("usuario_senha", FieldType.Single)]
        public String Senha
        {
            get { return _senha; }
            set { _senha = value; _senhaEncriptada = false; }
        }

        [DBFieldInfo("usuario_superiorId", FieldType.Single)]
        public Object SuperiorID
        {
            get { return _superiorId; }
            set { _superiorId= value; }
        }

        [DBFieldInfo("usuario_perfilId", FieldType.Single)]
        public Object PerfilID
        {
            get { return _perfilId; }
            set { _perfilId= value; }
        }

        [DBFieldInfo("usuario_categoriaId", FieldType.Single)]
        public Object CategoriaID
        {
            get { return _categoriaId; }
            set { _categoriaId= value; }
        }

        [DBFieldInfo("usuario_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("usuario_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("usuario_liberaContratos", FieldType.Single)]
        public Boolean LiberaContratos
        {
            get { return _liberaContratos; }
            set { _liberaContratos= value; }
        }

        [DBFieldInfo("usuario_alteraValorContratos", FieldType.Single)]
        public Boolean AlteraValorContratos
        {
            get { return _alteraValorContratos; }
            set { _alteraValorContratos = value; }
        }

        [DBFieldInfo("usuario_system", FieldType.Single)]
        public Boolean SystemUser
        {
            get { return _systemUser; }
            set { _systemUser= value; }
        }

        [DBFieldInfo("usuario_dataNascimento", FieldType.Single)]
        public DateTime DataNascimento
        {
            get { return _dataNascimento; }
            set { _dataNascimento = value; }
        }

        [DBFieldInfo("usuario_sexo", FieldType.Single)]
        public Int32 Sexo
        {
            get { return _sexo; }
            set { _sexo = value; }
        }

        [DBFieldInfo("usuario_estadoCivil", FieldType.Single)]
        public Int32 EstadoCivil
        {
            get { return _estadoCivil; }
            set { _estadoCivil = value; }
        }

        [DBFieldInfo("usuario_tipoPessoa", FieldType.Single)]
        public Int32 TipoPessoa
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("usuario_documento1", FieldType.Single)]
        public String Documento1
        {
            get { return _documento1; }
            set { _documento1 = value; }
        }

        [DBFieldInfo("usuario_documento2", FieldType.Single)]
        public String Documento2
        {
            get { return _documento2; }
            set { _documento2 = value; }
        }

        [DBFieldInfo("usuario_ddd1", FieldType.Single)]
        public String DDD1
        {
            get { return _ddd1; }
            set { _ddd1 = value; }
        }

        [DBFieldInfo("usuario_fone1", FieldType.Single)]
        public String Fone1
        {
            get { return _fone1; }
            set { _fone1 = value; }
        }

        [DBFieldInfo("usuario_ramal1", FieldType.Single)]
        public String Ramal1
        {
            get { return _ramal1; }
            set { _ramal1= value; }
        }

        [DBFieldInfo("usuario_ddd2", FieldType.Single)]
        public String DDD2
        {
            get { return _ddd2; }
            set { _ddd2 = value; }
        }

        [DBFieldInfo("usuario_fone2", FieldType.Single)]
        public String Fone2
        {
            get { return _fone2; }
            set { _fone2 = value; }
        }

        [DBFieldInfo("usuario_ramal2", FieldType.Single)]
        public String Ramal2
        {
            get { return _ramal2; }
            set { _ramal2 = value; }
        }

        [DBFieldInfo("usuario_ddd3", FieldType.Single)]
        public String DDD3
        {
            get { return _ddd3; }
            set { _ddd3 = value; }
        }

        [DBFieldInfo("usuario_fone3", FieldType.Single)]
        public String Celular
        {
            get { return _fone3; }
            set { _fone3 = value; }
        }

        [DBFieldInfo("usuario_ramal3", FieldType.Single)]
        public String CelularOperadora
        {
            get { return _ramal3; }
            set { _ramal3 = value; }
        }

        [DBFieldInfo("usuario_entrevistadoPor", FieldType.Single)]
        public String EntrevistadoPor
        {
            get { return _entrevistadoPor; }
            set { _entrevistadoPor = value; }
        }

        [DBFieldInfo("usuario_entrevistadoEm", FieldType.Single)]
        public DateTime EntrevistadoEm
        {
            get { return _entrevistadoEm; }
            set { _entrevistadoEm = value; }
        }

        [DBFieldInfo("usuario_banco", FieldType.Single)]
        public String Banco
        {
            get { return _banco; }
            set { _banco = value; }
        }

        [DBFieldInfo("usuario_agencia", FieldType.Single)]
        public String Agencia
        {
            get { return _agencia; }
            set { _agencia = value; }
        }

        [DBFieldInfo("usuario_contaTipo", FieldType.Single)]
        public String ContaTipo
        {
            get { return _contaTipo; }
            set { _contaTipo= value; }
        }

        [DBFieldInfo("usuario_conta", FieldType.Single)]
        public String Conta
        {
            get { return _conta; }
            set { _conta= value; }
        }

        [DBFieldInfo("usuario_favorecido", FieldType.Single)]
        public String Favorecido
        {
            get { return _favorecido; }
            set { _favorecido= value; }
        }

        [DBFieldInfo("usuario_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("usuario_alteraProdutor", FieldType.Single)]
        public Boolean AlteraProdutor
        {
            get { return _alteraProdutor; }
            set { _alteraProdutor= value; }
        }

        [DBFieldInfo("usuario_empresaCobrancaId", FieldType.Single)]
        public Object EmpresaCobrancaID
        {
            get { return _empresaCobrancaId; }
            set { _empresaCobrancaId= value; }
        }

        /// <summary>
        /// Usado para extender o perfil.
        /// </summary>
        [DBFieldInfo("usuario_extraPermission", FieldType.Single)]
        public Boolean ExtraPermission
        {
            get { return _extraPermission; }
            set { _extraPermission= value; }
        }

        /// <summary>
        /// Usado para extender o perfil no cadastro de operadora.
        /// </summary>
        [DBFieldInfo("usuario_extraPermission2", FieldType.Single)]
        public Boolean ExtraPermission2
        {
            get { return _extraPermission2; }
            set { _extraPermission2= value; }
        }

        [Joinned("perfil_descricao")]
        public String PerfilDescricao
        {
            get { return _perfilDescricao; }
            set { _perfilDescricao= value; }
        }

        [Joinned("filial_nome")]
        public String FilialDescricao
        {
            get { return _filialDescricao; }
            set { _filialDescricao= value; }
        }

        public String Nome_e_Perfil
        {
            get { return String.Concat(_nome, " (", _perfilDescricao, ")"); }
        }

        #endregion

        /// <summary>
        /// Encripta a senha do usuário.
        /// </summary>
        void EncriptaSenha()
        {
            if (_senhaEncriptada) { return; }
            this._senha = HashHelper.Hash(this._senha, ObtemSALTParaHash());
            _senhaEncriptada = true;
        }

        public String Encripta(String clearPwd)
        {
            return HashHelper.Hash(clearPwd, ObtemSALTParaHash());
        }

        /// <summary>
        /// Obtém o 'Salt' para encrementar o hash da senha.
        /// </summary>
        /// <returns>'Salt' para encrementar o hash da senha.</returns>
        static String ObtemSALTParaHash()
        {
            return ConfigurationManager.AppSettings["HashSalt"];
        }

        /// <summary>
        /// Método construtor
        /// </summary>
        public Usuario() { _senhaEncriptada = false; _ativo = true; _systemUser = true; }
        public Usuario(Object id) : this() { _id = id; }

        #region métodos EntityBase 
        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            this.EncriptaSenha();
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        public static void Remover(Object id)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM comissao_usuario WHERE comissaousuario_usuarioId=" + id, pm);
                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM usuario WHERE usuario_id=" + id, pm);
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

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
            _senhaEncriptada = true;
        }
        #endregion

        //public static IList<Usuario> CarregarPorCpf(String cpf, Boolean objCompleto)
        //{
        //    String query = "";

        //    if (objCompleto)
        //        query = "usuario.*, perfil_descricao FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id";
        //    else
        //        query = "usuario_id, usuario_nome FROM usuario";

        //    query += " WHERE usuario_cpf=@usuario_cpf";

        //    return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(query, 
        //        new String[1] { "@usuario_cpf" }, new String[1] { cpf }, typeof(Usuario));
        //}



        public static Usuario CarregarParcial(Object usuarioId)
        {
            String qry = "usuario_id, usuario_nome, usuario_documento1 FROM usuario WHERE usuario_id=" + usuarioId;
            IList<Usuario> lista = LocatorHelper.Instance.ExecuteQuery<Usuario>(qry, typeof(Usuario));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        /// <summary>
        /// Método para Carregar o ID do Perfil do Usuário.
        /// </summary>
        /// <param name="usuarioId">ID do Usuário.</param>
        /// <returns>Retorna o ID do Perfil do Usuário.</returns>
        public static Object CarregarPerfilID(Object usuarioId)
        {
            if (usuarioId == null)
                return null;

            String strSQL = String.Concat("SELECT usuario_perfilId FROM usuario WHERE usuario_id = ", usuarioId.ToString());

            Object retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, null, null);

            if (retVal == null || (retVal is DBNull))
                return null;
            else
                return retVal;
        }

        public static IList<Usuario> CarregarTodos(Object perfilId)
        {
            String query = "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id LEFT JOIN filial ON filial_id=usuario_filialId ";

            if (perfilId != null)
            {
                query += " WHERE usuario_perfilId=" + Convert.ToInt32(perfilId);
            }

            query += " ORDER BY usuario_nome";

            return LocatorHelper.Instance.ExecuteQuery<Usuario>(query, typeof(Usuario));
        }

        public static DataTable CarregarTodos_Parcial(Object perfilId)
        {
            String query = "usuario_id as ID,usuario_nome as Nome, perfil_descricao as PerfilDescricao, filial_nome as FilialNome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id LEFT JOIN filial ON filial_id=usuario_filialId ";

            if (perfilId != null)
            {
                query += " WHERE usuario_perfilId=" + Convert.ToInt32(perfilId);
            }

            query += " ORDER BY usuario_nome";

            return LocatorHelper.Instance.ExecuteQuery(query, "result").Tables[0];
        }

        public static IList<Usuario> CarregarCorretores()
        {
            return CarregarTodos(3);
        }

        public static IList<Usuario> CarregaSubordinados(Object usuarioId)
        {
            String query = "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id LEFT JOIN filial ON usuario_filialId=filial_id WHERE usuario_superiorId=" + usuarioId + " ORDER BY usuario_nome, perfil_descricao, filial_nome";

            return LocatorHelper.Instance.ExecuteQuery<Usuario>(query, typeof(Usuario)); 
        }

        /// <summary>
        /// Método para Carregar os Subordinados de um Determinado perfil.
        /// </summary>
        /// <param name="usuarioId">ID do Usuário.</param>
        /// <param name="perfilId">ID do Perfil.</param>
        /// <returns>Uma lista de Usuários preenchidos.</returns>
        public static IList<Usuario> CarregaSubordinados(Object usuarioId, Object perfilId)
        {
            if (usuarioId != null && perfilId != null)
            {
                String query = String.Concat("usuario.*, perfil_descricao, filial_nome ",
                                             "FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id ",
                                             "LEFT JOIN filial ON usuario_filialId = filial_id ",
                                             "WHERE (usuario_superiorId = @usuarioSuperiorID AND usuario_perfilid = @usuarioPerfilID)",
                                             "ORDER BY usuario_nome, perfil_descricao, filial_nome");

                String[] strParam = new String[2] { "@usuarioSuperiorID", "@usuarioPerfilID" };
                String[] strValue = new String[2] { usuarioId.ToString(), perfilId.ToString() };

                try
                {
                    return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(query, strParam, strValue, typeof(Usuario));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Método para Carregar os Subordinados de um Determinado perfil.
        /// </summary>
        /// <param name="usuarioId">ID do Usuário.</param>
        /// <param name="perfilId">ID do Perfil.</param>
        /// <returns>Uma lista de Usuários preenchidos.</returns>
        public static void CarregaSubordinadosRecursivo(Object usuarioId, Object perfilId, ref List<Usuario> subordinadosCallback)
        {
            if (usuarioId != null)
            {
                IList<Usuario> subordinados = Usuario.CarregaSubordinados(usuarioId);

                if (subordinados != null && subordinados.Count > 0)
                {
                    foreach (Usuario objUsuario in subordinados)
                    {
                        Usuario.CarregaSubordinadosRecursivo(objUsuario.ID, perfilId, ref subordinadosCallback);

                        if (objUsuario.PerfilID != null && objUsuario.PerfilID.ToString() == perfilId.ToString())
                            subordinadosCallback.Add(objUsuario);
                    }
                }
            }
        }

        public static IList<Usuario> CarregaSuperiores(Object usuarioId)
        {
            IList<Usuario> lista = null;
            String sql = "";

            PersistenceManager pm = new PersistenceManager();
            //pm.TransactionContext();

            sql = "SELECT usuario_superiorId FROM usuario WHERE usuario_id=" + usuarioId;
            Object superiorId = LocatorHelper.Instance.ExecuteScalar(sql, null, null);

            if (superiorId == null || superiorId == DBNull.Value)
                return null;

            lista = new List<Usuario>();
            IList<Usuario> superior = null;

            do
            {
                sql = "usuario.*, perfil_descricao FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE usuario_id=" + superiorId;

                superior = LocatorHelper.Instance.ExecuteQuery<Usuario>(sql, typeof(Usuario));
                if (superior != null) { lista.Add(superior[0]); superiorId = superior[0].SuperiorID; }
                else { superiorId = null; }

            } while (superiorId != null);

            //pm.Commit();

            return lista;
        }

        public static IList<Usuario> CarregarPorFilial(Object filialId, Boolean apenasComissionaveis, Boolean apenasParticipanteDeContrato)
        {
            String query = "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id ";

            if (filialId == null)
            {
                query = String.Concat(query, "LEFT JOIN filial ON usuario_filialId=filial_id ",
                    "WHERE filial_id IS NULL");
            }
            else
            {
                query = String.Concat(query, "INNER JOIN filial ON usuario_filialId=filial_id ",
                    "WHERE filial_id =", filialId);
            }

            if (apenasComissionaveis)
            {
                query = String.Concat(query, " AND perfil_comissionavel=1");
            }

            if (apenasParticipanteDeContrato)
            {
                query = String.Concat(query, " AND perfil_participanteContrato=1");
            }

            query = String.Concat(query, " ORDER BY usuario_nome");

            return LocatorHelper.Instance.ExecuteQuery<Usuario>(query, typeof(Usuario));
        }

        public static IList<Usuario> CarregarPorNomeEmail(String nome, String email, Object perfilId)
        {
            String top = "";

            if (String.IsNullOrEmpty(nome) && perfilId == null) { top = " top 350 "; }
            String query = top + "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id LEFT JOIN filial ON usuario_filialId=filial_id WHERE usuario_nome LIKE @usuario_nome ";

            if (perfilId != null)
                query += "AND perfil_id=" + perfilId;

            if (!String.IsNullOrEmpty(email))
                query += " AND usuario_email like @usuario_email ";

            String[] paramName = new String[] { "@usuario_nome", "@usuario_email" };
            String[] paramValue = new String[] { "%" + nome + "%", "%" + email + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                <Usuario>(query, paramName, paramValue, typeof(Usuario));
        }

        public static IList<Usuario> CarregarPorParametro(String nome, Object perfilId, String filialId)
        {
            String top = "";

            if (String.IsNullOrEmpty(nome) && perfilId == null) { top = " top 350 "; }
            String query = top + "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id LEFT JOIN filial ON usuario_filialId=filial_id WHERE usuario_nome LIKE @NOME ";

            if(perfilId != null)
                query += "AND perfil_id=" + perfilId;

            String[] paramName  = new String[] { "@NOME" };
            String[] paramValue = new String[] { "%" + nome + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                <Usuario>(query, paramName, paramValue, typeof(Usuario));
        }

        public static IList<Usuario> CarregarPorParametro(Object filialId, Object perfilId, Perfil.eTipo tipo)
        {
            String query = "usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id ";

            if (filialId == null || Convert.ToInt32(filialId) <= 0)
                query = String.Concat(query, "LEFT JOIN filial ON usuario_filialId=filial_id ");
            else
                query = String.Concat(query, "INNER JOIN filial ON usuario_filialId=filial_id ");

            Boolean condicaoFilial = false; Boolean condicaoPerfil = false;
            if (filialId != null && Convert.ToInt32(filialId) > 0)
            {
                query = String.Concat(query, " WHERE filial_id = ", filialId);
                condicaoFilial = true;
            }
            else if (Convert.ToInt32(filialId) == -1)
            {
                query += "WHERE filial_id IS NULL";
                condicaoFilial = true;
            }
            else
                condicaoFilial = false;

            if (perfilId != null && Convert.ToInt32(perfilId) > 0)
            {
                condicaoPerfil = true;
                if(condicaoFilial)
                    query = String.Concat(query, " AND perfil_id = ", perfilId);
                else
                    query = String.Concat(query, " WHERE perfil_id = ", perfilId);
            }

            if (tipo != Perfil.eTipo.Indefinido)
            {
                if (condicaoPerfil || condicaoFilial)
                    query = String.Concat(query, " AND perfil_tipo = ", Convert.ToInt32(tipo));
                else
                    query = String.Concat(query, " WHERE perfil_tipo = ", Convert.ToInt32(tipo));

            }

            query = String.Concat(query, " ORDER BY usuario_nome, perfil_descricao");

            return LocatorHelper.Instance.ExecuteQuery<Usuario>(query, typeof(Usuario));
        }

        public static IList<Usuario> CarregarPorParametro(Object filialId, Object perfilId, Perfil.eTipo[] tipos)
        {
            return CarregarPorParametro(filialId, perfilId, tipos, null, null);
        }

        public static IList<Usuario> CarregarPorParametro(Object filialId, Object perfilId, Perfil.eTipo[] tipos, String documento, String apelido)
        {
            String query = "TOP 100 usuario.*, perfil_descricao, filial_nome FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id ";

            if (filialId == null || Convert.ToInt32(filialId) <= 0)
                query = String.Concat(query, "LEFT JOIN filial ON usuario_filialId=filial_id ");
            else
                query = String.Concat(query, "INNER JOIN filial ON usuario_filialId=filial_id ");

            Boolean condicaoFilial = false; Boolean condicaoPerfil = false;
            if (filialId != null && Convert.ToInt32(filialId) > 0)
            {
                query = String.Concat(query, " WHERE filial_id = ", filialId);
                condicaoFilial = true;
            }
            else if (Convert.ToInt32(filialId) == -1)
            {
                query += "WHERE filial_id IS NULL";
                condicaoFilial = true;
            }
            else
                condicaoFilial = false;

            if (perfilId != null && Convert.ToInt32(perfilId) > 0)
            {
                condicaoPerfil = true;
                if (condicaoFilial)
                    query = String.Concat(query, " AND perfil_id = ", perfilId);
                else
                    query = String.Concat(query, " WHERE perfil_id = ", perfilId);
            }

            if (condicaoPerfil || condicaoFilial)
                query = String.Concat(query, " AND perfil_tipo IN (");
            else
                query = String.Concat(query, " WHERE perfil_tipo IN (");

            for (int i = 0; i < tipos.Length; i++)
            {
                if (i > 0) { query = String.Concat(query, ",", Convert.ToInt32(tipos[i])); }
                else { query = String.Concat(query, Convert.ToInt32(tipos[i])); }
            }

            query = String.Concat(query, ")");

            if (!String.IsNullOrEmpty(documento))
            {
                query = String.Concat(query, " AND usuario_documento1 LIKE @DOC");

                String[] paramNm = null;
                String[] paramVl = null;

                if (String.IsNullOrEmpty(apelido))
                {
                    paramNm = new String[] { "@DOC" };
                    paramVl = new String[] { "%" + documento + "%" };

                    query = String.Concat(query, " ORDER BY usuario_nome, perfil_descricao");
                }
                else
                {
                    query = String.Concat(query, " AND usuario_apelido LIKE @Apelido");

                    paramNm = new String[] { "@DOC", "@Apelido" };
                    paramVl = new String[] { "%" + documento + "%", "%" + apelido + "%" };
                }

                return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(query, paramNm, paramVl, typeof(Usuario));
            }
            else
            {
                if (String.IsNullOrEmpty(apelido))
                {
                    query = String.Concat(query, " ORDER BY usuario_nome, perfil_descricao");
                    return LocatorHelper.Instance.ExecuteQuery<Usuario>(query, typeof(Usuario));
                }
                else
                {
                    query = String.Concat(query, " AND usuario_apelido LIKE @Apelido");
                    query = String.Concat(query, " ORDER BY usuario_nome, perfil_descricao");

                    String[] paramNm = new String[] { "@Apelido" };
                    String[] paramVl = new String[] { "%" + apelido + "%" };

                    return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(query, paramNm, paramVl, typeof(Usuario));
                }
            }
        }

        public static IList<Usuario> CarregarProdutorPorDoc(String doc)
        {
            String qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1 FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_tipo=0 AND usuario_documento1 LIKE @DOC ORDER BY usuario_apelido";

            String[] param = new String[] { "@DOC" };
            String[] value = new String[] { doc + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario));
        }

        public static IList<Usuario> CarregarCorretorPorDoc(String doc)
        {
            return CarregarCorretorPorDoc(doc, null);
        }

        public static IList<Usuario> CarregarCorretorPorDoc(String doc, PersistenceManager pm)
        {
            String qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1 FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_documento1 LIKE @DOC ORDER BY usuario_apelido";

            String[] param = new String[] { "@DOC" };
            String[] value = new String[] { doc + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario), pm);
        }
        public static IList<Usuario> CarregarCorretorPorDocOuNome(String dado, PersistenceManager pm)
        {
            String qry = "";

            int result = 0;
            if (int.TryParse(dado, out result))
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1 FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_documento1 LIKE @PARAM ORDER BY usuario_apelido";
            else
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1 FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_nome LIKE @PARAM ORDER BY usuario_apelido";

            String[] param = new String[] { "@PARAM" };
            String[] value = new String[] { dado + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario), pm);
        }

        public static IList<Usuario> CarregarCorretorPorDocOuNome(String dado, Object filialId, PersistenceManager pm)
        {
            return CarregarCorretorPorDocOuNome(dado, filialId, false, pm);
            //String qry = "";

            //int result = 0;
            //if (int.TryParse(dado, out result))
            //    qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_documento1 LIKE @PARAM";
            //else
            //    qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_nome LIKE @PARAM";

            //if (filialId != null)
            //{
            //    qry += " AND usuario_filialId=" + filialId;
            //}

            //qry += " ORDER BY usuario_apelido";

            //String[] param = new String[] { "@PARAM" };
            //String[] value = new String[] { dado + "%" };

            //return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario), pm);
        }

        public static IList<Usuario> CarregarCorretorPorDocOuNome(String dado, Object filialId, Boolean somenteAtivo, PersistenceManager pm)
        {
            String qry = "";

            int result = 0;
            if (int.TryParse(dado, out result))
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_documento1 LIKE @PARAM";
            else
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_nome LIKE @PARAM";

            if (filialId != null)
            {
                qry += " AND usuario_filialId=" + filialId;
            }

            if (somenteAtivo)
            {
                qry += " AND usuario_ativo=1 ";
            }

            qry += " ORDER BY usuario_apelido";

            String[] param = new String[] { "@PARAM" };
            String[] value = new String[] { dado + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario), pm);
        }

        public static IList<Usuario> CarregarCorretorPorDocOuNomeOuCodigo(String dado, Object filialId, Boolean somenteAtivo, PersistenceManager pm)
        {
            String qry = "";

            int result = 0;
            if (int.TryParse(dado, out result))
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa, usuario_codigo FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND (usuario_documento1 LIKE @PARAM or usuario_codigo like @PARAM or usuario_marcaOtica like @PARAM)";
            else
                qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1, usuario_tipoPessoa, usuario_codigo FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_participanteContrato=1 AND usuario_nome LIKE @PARAM";

            if (filialId != null)
            {
                qry += " AND usuario_filialId=" + filialId;
            }

            if (somenteAtivo)
            {
                qry += " AND usuario_ativo=1 ";
            }

            qry += " ORDER BY usuario_apelido";

            String[] param = new String[] { "@PARAM" };
            String[] value = new String[] { dado + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario), pm);
        }

        public static IList<Usuario> CarregarOperadorMKTPorDoc(String doc)
        {
            String qry = "TOP 10 usuario_id, usuario_nome, usuario_documento1 FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE perfil_id=" + Convert.ToInt32(Perfil.OperadorCallCenterIDKey) + " AND (usuario_documento1 LIKE @DOC or usuario_nome like @DOC) ORDER BY usuario_apelido";

            String[] param = new String[] { "@DOC" };
            String[] value = new String[] { doc + "%" };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Usuario>(qry, param, value, typeof(Usuario));
        }

        public static Usuario Autentica(String email, String senha)
        {
            String[] Params = new String[] { "@email", "@senha" };
            String[] Values = new String[] { email, HashHelper.Hash(senha, ObtemSALTParaHash()) };
            String query = "usuario.*, perfil_descricao FROM usuario INNER JOIN perfil ON usuario_perfilId=perfil_id WHERE usuario_ativo=1 AND usuario_email=@email AND usuario_senha=@senha";

            IList<Usuario> retorno = LocatorHelper.Instance.
                ExecuteParametrizedQuery<Usuario>(query, Params, Values, typeof(Usuario));

            if (retorno == null || retorno.Count == 0)
                return null;
            else
                return retorno[0];
        }

        /// <summary>
        /// Método para Alterar o Superior de um Usuário. O Histórico NÃO É SALVO no método.
        /// </summary>
        /// <param name="subordinadoId">ID do Usuário.</param>
        /// <param name="superiorId">ID do Novo Superior.</param>
        public static void AlteraSuperior(Object subordinadoId, Object superiorId)
        {
            Usuario.AlteraSuperior(new Object[] { subordinadoId }, superiorId);
        }

        /// <summary>
        /// Método para Alterar o Superior de um ou mais Usuário. O Histórico NÃO É SALVO no método.
        /// </summary>
        /// <param name="subordinadoId">Array de ID's de Usuário.</param>
        /// <param name="superiorId">ID do Novo Superior.</param>
        public static void AlteraSuperior(Object[] subordinadoId, Object superiorId)
        {
            Usuario.AlteraSuperior(subordinadoId, superiorId, DateTime.MinValue);
        }

        /// <summary>
        /// Método para Alterar o Superior de um ou mais Usuário. O Histórico É SALVO dentro do método.
        /// </summary>
        /// <param name="subordinadoId">Array de ID's de Usuário.</param>
        /// <param name="superiorId">ID do Novo Superior.</param>
        /// <param name="vigencia">Data de Vigência.</param>
        public static void AlteraSuperior(Object[] subordinadoId, Object superiorId, DateTime vigencia)
        {
            String strSQL              = String.Empty;
            String strSubordinadoRange = String.Empty;
            
            if (subordinadoId != null && subordinadoId.Length > 0)
            {
                List<SuperiorSubordinado> lstSSHistory = new List<SuperiorSubordinado>(subordinadoId.Length);
                SuperiorSubordinado ssHistory          = null;

                strSubordinadoRange = " IN (";

                for (Int32 i = 0; i < subordinadoId.Length; i++)
                {
                    if (i > 0)
                        strSubordinadoRange = String.Concat(strSubordinadoRange, ",");

                    strSubordinadoRange = String.Concat(strSubordinadoRange, subordinadoId[i].ToString());

                    if (vigencia.CompareTo(DateTime.MinValue) > 0)
                    {
                        ssHistory               = new SuperiorSubordinado();
                        ssHistory.Data          = vigencia;
                        ssHistory.SubordinadoID = subordinadoId[i];
                        ssHistory.SuperiorID    = superiorId;

                        lstSSHistory.Add(ssHistory);
                    }
                }

                strSubordinadoRange = String.Concat(strSubordinadoRange, ")");

                if (superiorId != null)
                    strSQL = String.Concat("UPDATE usuario SET usuario_superiorId = ", superiorId, " WHERE usuario_id", strSubordinadoRange);
                else
                    strSQL = String.Concat("UPDATE usuario SET usuario_superiorId = NULL WHERE usuario_id", strSubordinadoRange);

                try
                {
                    NonQueryHelper.Instance.ExecuteNonQuery(strSQL, null);

                    for (Int32 i = 0; i < lstSSHistory.Count; i++)
                        lstSSHistory[i].Salvar();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    lstSSHistory = null;
                }
            }
        }

        public static Boolean ChecaDocumento(Object usuarioId, String documento1, out String msg, Boolean cpf)
        {
            msg = "";

            if (!Beneficiario.ValidaCpf(documento1) && cpf)
            {
                msg = "CPF informado inválido.";
                return false;
            }

            if (!ChecaCnpj(documento1) && !cpf)
            {
                msg = "CNPJ informado inválido.";
                return false;
            }

            //if (!Beneficiario.ValidaCpf(documento1) && !ChecaCnpj(documento1))
            //{
            //    if(cpf)
            //        msg = "CPF informado inválido.";
            //    else
            //        msg = "CNPJ informado inválido.";
            //    return false;
            //}

            String query = "SELECT usuario_id FROM usuario WHERE usuario_documento1=@DOC";
            if (usuarioId != null)
                query += " AND usuario_id <> " + usuarioId;

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@DOC" }, new String[] { documento1 }).Tables[0];

            if (dt != null && dt.Rows.Count > 0 && cpf)
            {
                //if(cpf)
                msg = "O CPF informado ja está cadastrado.";
                //else
                //    msg = "O CNPJ informado ja está cadastrado.";

                return dt == null || dt.Rows.Count == 0;
            }
            else
                return true;
        }

        public static Boolean ChecaLogin(Object usuarioId, String login)
        {
            String query = "SELECT usuario_id FROM usuario WHERE usuario_email=@login";
            if (usuarioId != null)
                query += " AND usuario_id <> " + usuarioId;

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@login" }, new String[] { login }).Tables[0];

            return dt == null || dt.Rows.Count == 0;
        }

        public static Boolean ChecaCnpj(String cnpj)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"].ToUpper() == "Y")
            {
                return true;
            }

            Int32[] digitos, soma, resultado;
            Int32 nrDig;
            String ftmt;
            Boolean[] cnpjOk;
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("-", "");
            if (cnpj == "00000000000000")
            {
                return false;
            }

            ftmt = "6543298765432";
            digitos = new Int32[14];
            soma = new Int32[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new Int32[2];
            resultado[0] = 0;
            resultado[1] = 0;
            cnpjOk = new Boolean[2];
            cnpjOk[0] = false;
            cnpjOk[1] = false;

            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(cnpj.Substring(nrDig, 1));

                    if (nrDig <= 11)
                    {
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));
                    }

                    if (nrDig <= 12)
                    {
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                    }
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }

                return (cnpjOk[0] && cnpjOk[1]);

            }
            catch
            {
                return false;
            }
        }

        public static Boolean TemVinculoComOperadora(Object operadoraId, Object produtorId)
        {
            String query = String.Concat("SELECT DISTINCT(contratoadm_operadoraid)",
                " FROM comissao_usuario",
                " INNER JOIN comissao_modelo ON comissaousuario_tabelacomissionamentoId=comissaomodelo_id ",
                " INNER JOIN comissaoGrupo ON comissaomodelo_id=comissaogrupo_tabelaComissaoId ",
                " INNER JOIN contratoAdmGrupo ON contratoadmgrupo_grupoId=comissaogrupo_id ",
                " INNER JOIN contratoadm ON contratoadmgrupo_contratoAdmId=contratoadm_id ",
                " WHERE comissaousuario_usuarioid=", produtorId);

            System.Data.DataTable dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset").Tables[0];

            if (dt == null || dt.Rows.Count == 0) { return false; }

            return true;
        }

        /// <summary>
        /// Representa o usuário autenticado no sistema.
        /// </summary>
        public class Autenticado
        {
            private Autenticado(){}

            public static String ID
            {
                get
                {
                    if (System.Web.HttpContext.Current.Session["_uid"] == null) { return String.Empty; }
                    return Convert.ToString(System.Web.HttpContext.Current.Session["_uid"]);
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_uid"] = value;
                }
            }

            public static String Nome
            {
                get
                {
                    return System.Web.HttpContext.Current.User.Identity.Name;
                }
            }

            public static String Email
            {
                get
                {
                    return System.Web.HttpContext.Current.Session["_email"] as String;
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_email"] = value;
                }
            }

            public static String PerfilID
            {
                get
                {
                    if (System.Web.HttpContext.Current.Session["_perfilId"] == null) { return String.Empty; }
                    return Convert.ToString(System.Web.HttpContext.Current.Session["_perfilId"]);
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_perfilId"] = value;
                }
            }

            public static Object PerfilDescricao
            {
                get
                {
                    return System.Web.HttpContext.Current.Session["_perfilDes"];
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_perfilDes"] = value;
                }
            }

            public static String EmpresaCobrancaID
            {
                get
                {
                    return System.Web.HttpContext.Current.Session["_empcob"] as String;
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_empcob"] = value;
                }
            }

            public static Boolean ExtraPermission
            {
                get
                {
                    if (System.Web.HttpContext.Current.Session["_extraper"] == null) return false;
                    return Convert.ToBoolean(System.Web.HttpContext.Current.Session["_extraper"]);
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_extraper"] = value;
                }
            }

            public static Boolean ExtraPermission2
            {
                get
                {
                    if (System.Web.HttpContext.Current.Session["_extraper2"] == null) return false;
                    return Convert.ToBoolean(System.Web.HttpContext.Current.Session["_extraper2"]);
                }

                set
                {
                    System.Web.HttpContext.Current.Session["_extraper2"] = value;
                }
            }

            public static void Encerrar()
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Session.Clear();
                    System.Web.HttpContext.Current.Session.Abandon();
                }
                System.Web.Security.FormsAuthentication.SignOut();
            }
        }

        public Perfil Perfil
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Perfil Perfil1
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Comissionamento Comissionamento
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public GrupoDeVenda GrupoDeVenda
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public static String GerarArquivoAniversariantes(Int32 dia, Int32 mes)
        {
            String strWhere = "";

            if (dia > 0)
            {
                strWhere = String.Concat(" AND DAY(usuario_dataNascimento)= ", dia);
            }

            String qry = String.Concat(
                "SELECT usuario_nome, usuario_dataNascimento, endereco_logradouro, endereco_numero, endereco_complemento, endereco_bairro, endereco_cidade, endereco_uf, endereco_cep ",
                "FROM usuario ",
	            "INNER JOIN perfil ON usuario_perfilId = perfil_id ",
	            "INNER JOIN endereco ON endereco_donoId = usuario_id AND endereco_donotipo = 4 ",
                "WHERE ", 
                "usuario_tipoPessoa = 1 AND ", 
                "perfil_id = 3 AND ", 
                "MONTH(usuario_dataNascimento) = ", mes,
                strWhere);

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                if (row["endereco_logradouro"] == null || row["endereco_logradouro"] == DBNull.Value || Convert.ToString(row["endereco_logradouro"]).Trim() == "")
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(Environment.NewLine);
                }

                sb.Append(row["usuario_nome"]);
                sb.Append(";");
                sb.Append(row["endereco_logradouro"]);
                sb.Append(",");
                sb.Append(row["endereco_numero"]);
                sb.Append(" ");
                sb.Append(row["endereco_complemento"]);
                sb.Append(";");
                sb.Append(row["endereco_bairro"]);
                sb.Append(";");
                sb.Append(row["endereco_cidade"]);
                sb.Append(";");
                sb.Append(row["endereco_uf"]);
                sb.Append(";");
                sb.Append(row["endereco_cep"]);
                sb.Append(";");
                if (row["usuario_nome"] != null)
                {
                    sb.Append(Convert.ToString(row["usuario_nome"]).Split(' ')[0]);
                    sb.Append(";");
                }
                if (row["usuario_dataNascimento"] != null)
                {
                    DateTime data = Convert.ToDateTime(row["usuario_dataNascimento"]);
                    sb.Append(String.Concat(data.Day.ToString(), " de ", GetStringMes(data.Month)));
                    sb.Append(";");
                }
            }

            return sb.ToString();
        }

        private static String GetStringMes(Int32 Mes)
        {
            String strMes = "";

            switch (Mes.ToString())
            {
                case "1":
                    strMes = "janeiro";
                    break;
                case "2":
                    strMes = "fevereiro";
                    break;
                case "3":
                    strMes = "março";
                    break;
                case "4":
                    strMes = "abril";
                    break;
                case "5":
                    strMes = "maio";
                    break;
                case "6":
                    strMes = "junho";
                    break;
                case "7":
                    strMes = "julho";
                    break;
                case "8":
                    strMes = "agosto";
                    break;
                case "9":
                    strMes = "setembro";
                    break;
                case "10":
                    strMes = "outubro";
                    break;
                case "11":
                    strMes = "novembro";
                    break;
                case "12":
                    strMes = "dezembro";
                    break;
            }

            return strMes;
        }

    }

    [DBTable("usuario_filial")]
    public class UsuarioFilial : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _usuarioId;
        Object _filialId;
        DateTime _data;

        String _usuarioNome;
        String _filialNome;

        #region properties 

        [DBFieldInfo("usuariofilial_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("usuariofilial_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("usuariofilial_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("usuariofilial_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("usuario_nome")]
        public String UsuarioNome
        {
            get { return _usuarioNome; }
            set { _usuarioNome= value; }
        }

        [Joinned("filial_nome")]
        public String FilialNome
        {
            get { return _filialNome; }
            set { _filialNome= value; }
        }

        #endregion

        public UsuarioFilial() { _data = DateTime.Now; }
        public UsuarioFilial(Object id) : this() { _id = id; }

        #region métodos EntityBase 
        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        #endregion

        public static UsuarioFilial CarregarVigente(Object usuarioId, DateTime dataRef, PersistenceManager pm)
        {
            String query = String.Concat(
                "TOP 1 usuario_filial.*,filial_nome FROM usuario_filial INNER JOIN filial ON usuariofilial_filialId=filial_id ",
                "   WHERE ",
                "       usuariofilial_usuarioId=" + Convert.ToInt32(usuarioId), " AND ",
                "       usuariofilial_data <= '", dataRef.Year, "-", dataRef.Month, "-", dataRef.Day, "' ",
                "   ORDER BY usuariofilial_data DESC");

            IList<UsuarioFilial> obj = LocatorHelper.Instance.ExecuteQuery<UsuarioFilial>(query, typeof(UsuarioFilial), pm);

            if (obj == null)
                return null;
            else
                return obj[0];
        }

        public static IList<UsuarioFilial> CarregarTodos(Object usuarioId)
        {
            return CarregarTodos(usuarioId, null);
        }

        public static IList<UsuarioFilial> CarregarTodos(Object usuarioId, PersistenceManager pm)
        {
            String query = "usuario_filial.*, usuario_nome, filial_nome FROM usuario_filial INNER JOIN usuario ON usuariofilial_usuarioId = usuario_id LEFT JOIN filial ON filial_id=usuariofilial_filialId ";
            query += " WHERE usuariofilial_usuarioId=" + Convert.ToInt32(usuarioId);
            query += " ORDER BY usuariofilial_data DESC";

            return LocatorHelper.Instance.ExecuteQuery<UsuarioFilial>(query, typeof(UsuarioFilial), pm);
        }

        //public static UsuarioFilial CarregarPorFilial(Object filialId, PersistenceManager pm)
        //{
        //    String query = "select top 1 usuario_filial.*, usuario_nome, filial_nome FROM usuario_filial INNER JOIN usuario ON usuariofilial_usuarioId = usuario_id LEFT JOIN filial ON filial_id=usuariofilial_filialId ";
        //    query += " WHERE usuariofilial_filialId=" + Convert.ToInt32(filialId);
        //    query += " ORDER BY usuariofilial_data DESC";

        //    IList<UsuarioFilial> list = LocatorHelper.Instance.ExecuteQuery<UsuarioFilial>(query, typeof(UsuarioFilial), pm);

        //    if (list == null) return null;

        //    return list[0];
        //}

        public static void SetaAtual(Object usuarioId)
        {
            String command = "";

            command = "SELECT TOP 1 usuariofilial_filialid FROM usuario_filial WHERE usuariofilial_usuarioId=@ID ORDER BY usuariofilial_data DESC";

            Object fid = LocatorHelper.Instance.ExecuteScalar(command, new String[] { "@ID" }, new String[] { Convert.ToString(usuarioId) });

            if (fid != null && fid != DBNull.Value)
            {
                command = "UPDATE usuario SET usuario_filialId=" + fid + " WHERE usuario_id=" + usuarioId;
            }
            else
            {
                command = "UPDATE usuario SET usuario_filialId=NULL WHERE usuario_id=" + usuarioId;
            }

            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }
    }

    [DBTable("usuario_grupoVenda")]
    public class UsuarioGrupoVenda : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _usuarioId;
        Object _grupoVendaId;
        DateTime _data;

        String _usuarioNome;
        String _grupoNome;

        #endregion

        #region properties 

        [DBFieldInfo("usuariogvenda_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("usuariogvenda_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        [DBFieldInfo("usuariogvenda_grupovendaId", FieldType.Single)]
        public Object GrupoVendaID
        {
            get { return _grupoVendaId; }
            set { _grupoVendaId= value; }
        }

        [DBFieldInfo("usuariogvenda_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("usuario_nome")]
        public String UsuarioNome
        {
            get { return _usuarioNome; }
            set { _usuarioNome= value; }
        }

        [Joinned("grupovenda_descricao")]
        public String GrupoVendaDescricao
        {
            get { return _grupoNome; }
            set { _grupoNome= value; }
        }

        #endregion

        public UsuarioGrupoVenda() { _data = DateTime.Now; }
        public UsuarioGrupoVenda(Object id) : this() { _id = id; }

        #region métodos EntityBase 
        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        #endregion

        public static IList<UsuarioGrupoVenda> CarregarTodos(Object usuarioId)
        {
            String query = "usuario_grupoVenda.*, usuario_nome, grupovenda_descricao FROM usuario_grupoVenda INNER JOIN usuario ON usuariogvenda_usuarioId = usuario_id LEFT JOIN grupo_venda ON grupovenda_id=usuariogvenda_grupovendaId ";
            query += " WHERE usuariogvenda_usuarioId=" + Convert.ToInt32(usuarioId);
            query += " ORDER BY usuariogvenda_data DESC";

            return LocatorHelper.Instance.ExecuteQuery<UsuarioGrupoVenda>(query, typeof(UsuarioGrupoVenda));
        }
    }

    [DBTable("usuario_contato")]
    public class UsuarioContato : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _usuarioId;
        String _nome;
        String _departamento;
        String _ddd;
        String _fone;
        String _ramal;
        String _email;

        #endregion

        #region propriedades 

        [DBFieldInfo("usuariocontato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("usuariocontato_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("usuariocontato_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("usuariocontato_departamento", FieldType.Single)]
        public String Departamento
        {
            get { return _departamento; }
            set { _departamento= value; }
        }

        [DBFieldInfo("usuariocontato_ddd", FieldType.Single)]
        public String DDD
        {
            get { return _ddd; }
            set { _ddd= value; }
        }

        [DBFieldInfo("usuariocontato_fone", FieldType.Single)]
        public String Fone
        {
            get { return _fone; }
            set { _fone= value; }
        }

        [DBFieldInfo("usuariocontato_ramal", FieldType.Single)]
        public String Ramal
        {
            get { return _ramal; }
            set { _ramal= value; }
        }

        [DBFieldInfo("usuariocontato_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        #endregion

        public UsuarioContato() { }
        public UsuarioContato(Object id) { _id = id; }

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

        public static IList<UsuarioContato> Carregar(Object usuarioId)
        {
            String query = "* FROM usuario_contato WHERE usuariocontato_usuarioId=" + usuarioId + " ORDER BY usuariocontato_nome";
            return LocatorHelper.Instance.ExecuteQuery<UsuarioContato>(query, typeof(UsuarioContato));
        }
    }

    [DBTable("usuario_controleAcesso")]
    public class ControleAcesso : EntityBase, IPersisteableEntity
    {
        public enum eDiaSemana : int
        {
            Indefinido,
            Domingo,
            Segunda,
            Terca,
            Quarta,
            Quinta,
            Sexta,
            Sabado
        }

        #region fields 

        Object _id;
        Object _usuarioId;
        Int32 _diaSemana;
        String _horaInicio;
        String _horaFim;
        String _ips;

        #endregion

        #region properties 

        [DBFieldInfo("controle_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("controle_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("controle_diaSemana", FieldType.Single)]
        public Int32 DiaDaSemana
        {
            get { return _diaSemana; }
            set { _diaSemana= value; }
        }
        public String strDiaDaSemana
        {
            get
            {
                switch ((eDiaSemana)_diaSemana)
                {
                    case eDiaSemana.Domingo:
                    {
                        return "Domingo";
                    }
                    case eDiaSemana.Segunda:
                    {
                        return "Segunda-feira";
                    }
                    case eDiaSemana.Terca:
                    {
                        return "Terça-feira";
                    }
                    case eDiaSemana.Quarta:
                    {
                        return "Quarta-feira";
                    }
                    case eDiaSemana.Quinta:
                    {
                        return "Quinta-feira";
                    }
                    case eDiaSemana.Sexta:
                    {
                        return "Sexta-feira";
                    }
                    case eDiaSemana.Sabado:
                    {
                        return "Sabado";
                    }
                    default :
                    {
                        return "";
                    }
                }
            }
        }

        [DBFieldInfo("controle_horaInicio", FieldType.Single)]
        public String HoraInicio
        {
            get { return _horaInicio; }
            set { _horaInicio= value; }
        }

        [DBFieldInfo("controle_horaFim", FieldType.Single)]
        public String HoraFim
        {
            get { return _horaFim; }
            set { _horaFim= value; }
        }

        /// <summary>
        /// IPs separados por ';'.
        /// </summary>
        [DBFieldInfo("controle_ips", FieldType.Single)]
        public String IPs
        {
            get { return _ips; }
            set { _ips= value; }
        }

        #endregion

        public ControleAcesso() { }
        public ControleAcesso(Object id) { _id = id; }

        #region EntityBase methods 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<ControleAcesso> CarregarPorUsuario(Object usuarioId)
        {
            String qry = "* FROM usuario_controleAcesso WHERE controle_usuarioId=" + usuarioId;
            qry += " ORDER BY controle_diaSemana";

            return LocatorHelper.Instance.ExecuteQuery<ControleAcesso>(qry, typeof(ControleAcesso));
        }
        public static ControleAcesso CarregarPorUsuario(Object usuarioId, eDiaSemana dia)
        {
            String qry = "* FROM usuario_controleAcesso WHERE controle_usuarioId=" + usuarioId;
            qry += " AND controle_diaSemana=" + Convert.ToInt32(dia).ToString();
            qry += " ORDER BY controle_diaSemana";

            IList<ControleAcesso> list = LocatorHelper.Instance.ExecuteQuery<ControleAcesso>(qry,typeof(ControleAcesso));

            if (list == null)
                return null;
            else
                return list[0];
        }

        //bool checar acesso para hoje hoje(usuario)
    }
}
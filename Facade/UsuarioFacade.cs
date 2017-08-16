namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class UsuarioFacade : FacadeBase
    {
        #region Singleton 

        static UsuarioFacade _instance;
        public static UsuarioFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new UsuarioFacade(); }
                return _instance;
            }
        }
        #endregion

        private UsuarioFacade() { }

        public Usuario Salvar(Usuario usuario)
        {
            using (ISession sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    sessao.SaveOrUpdate(usuario);
                    tran.Commit();
                }
            }

            return usuario;
        }

        public Usuario LogOn(string login, string senha)
        {
            Usuario ret = null;

            using (ISession sessao = ObterSessao())
            {
                ret = sessao.Query<Usuario>()
                    .Fetch(u => u.Unidade)
                    .ThenFetch(un => un.Owner)
                    .Where(u => u.Login == login && u.Senha == senha && u.Ativo).SingleOrDefault();
            }

            return ret;
        }

        public Usuario Carregar(string login)
        {
            Usuario ret = null;

            using (ISession sessao = ObterSessao())
            {
                ret = sessao.Query<Usuario>()
                    .Fetch(u => u.Unidade)
                    .ThenFetch(un => un.Owner)
                    .Where(u => u.Login == login && u.Ativo).SingleOrDefault();
            }

            return ret;
        }

        public Usuario Carregar(long id)
        {
            Usuario ret = null;

            using (ISession sessao = ObterSessao())
            {
                ret = sessao.Query<Usuario>()
                    .Fetch(u => u.Unidade)
                    .ThenFetch(un => un.Owner)
                    .Where(u => u.ID == id)
                    .Single();
            }

            return ret;
        }

        public List<Usuario> Carregar(TipoUsuario tipo, string nome)
        {
            List<Usuario> ret = null;

            using (ISession sessao = ObterSessao())
            {
                if (tipo == TipoUsuario.ContratoDePrestador)
                {
                    ret = sessao.Query<Usuario>()
                        .Fetch(u => u.Unidade)
                        .ThenFetch(un => un.Owner)
                        .Where(u => u.Tipo == tipo && (u.Nome.Contains(nome) || u.Unidade.Nome.Contains(nome) || u.Unidade.Owner.Nome.Contains(nome)))
                        .OrderBy(u => u.Unidade.Owner.Nome).OrderBy(u => u.Nome)
                        .Take(200)
                        .ToList();
                }
                else
                {
                    ret = sessao.Query<Usuario>()
                        .Fetch(u => u.Unidade)
                        .ThenFetch(un => un.Owner)
                        .Where(u => u.Tipo == tipo && u.Nome.Contains(nome))
                        .Take(200)
                        .ToList();
                }
            }

            return ret;
        }

        /// <summary>
        /// True caso o login possa ser usado, False caso o login NÃO possa ser usado
        /// </summary>
        public bool VerificarLogin(long? id, string login)
        {
            Usuario ret = null;

            using (ISession sessao = ObterSessao())
            {
                if (id != null)
                {
                    ret = sessao.Query<Usuario>()
                        .Where(u => u.ID != id.Value && u.Login == login)
                        .FirstOrDefault();
                }
                else
                {
                    ret = sessao.Query<Usuario>()
                        .Where(u => u.Login == login)
                        .FirstOrDefault();
                }
            }

            return ret == null;
        }

        public Usuario CarregarPorUnidade(long unidadeId)
        {
            Usuario ret = null;

            using (ISession sessao = ObterSessao())
            {
                ret = sessao.Query<Usuario>()
                    .Fetch(u => u.Unidade)
                    .Where(us => us.Unidade.ID == unidadeId)
                    .OrderBy(u => u.Nome)
                    .FirstOrDefault();
            }

            return ret;
        }
    }
}

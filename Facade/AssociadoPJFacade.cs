namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class AssociadoPJFacade : FacadeBase
    {
        #region singleton 

        static AssociadoPJFacade _instance;
        public static AssociadoPJFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new AssociadoPJFacade(); }
                return _instance;
            }
        }
        #endregion

        private AssociadoPJFacade() { }

        public long Salvar(AssociadoPJ obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(obj);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public AssociadoPJ Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                AssociadoPJ obj = sessao.Query<AssociadoPJ>().Where(e => e.ID == id).Single();

                return obj;
            }
        }
        public List<AssociadoPJ> Carregar(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<AssociadoPJ>().OrderBy(e => e.Nome).ToList();
                }
                else
                {
                    return sessao.Query<AssociadoPJ>()
                        .Where(e => e.Nome.Contains(nome))
                        .OrderBy(e => e.Nome).ToList();
                }
            }
        }

        public void Excluir(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        AssociadoPJ obj = sessao.Query<AssociadoPJ>().Where(e => e.ID == id).Single();

                        sessao.Delete(obj);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
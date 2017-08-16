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

    public class FilialFacade : FacadeBase
    {
        FilialFacade() { }

        #region Singleton 

        static FilialFacade _instancia;
        public static FilialFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new FilialFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(Filial filial)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(filial);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return filial.ID;
        }

        public Filial Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Filial>().Where(s => s.ID == id).Single();
            }
        }

        public List<Filial> CarregarTodos(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<Filial>()
                        .OrderBy(s => s.Nome).ToList();
                }
                else
                {
                    return sessao.Query<Filial>()
                        .Where(s => s.Nome.Contains(nome))
                        .OrderBy(s => s.Nome).ToList();
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
                        Filial obj = sessao.Query<Filial>().Where(s => s.ID == id).Single();

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

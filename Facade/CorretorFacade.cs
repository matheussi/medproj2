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

    public class CorretorFacade : FacadeBase
    {
        CorretorFacade() { }

        #region Singleton 

        static CorretorFacade _instancia;
        public static CorretorFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new CorretorFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(Corretor segmento)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(segmento);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return segmento.ID;
        }

        public Corretor Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Corretor>()
                    //.Fetch(c => c.TabelaComissao)
                    .Where(s => s.ID == id).Single();
            }
        }

        public List<Corretor> CarregarTodos(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<Corretor>()
                        //.Fetch(c => c.TabelaComissao)
                        .OrderBy(s => s.Nome).ToList();
                }
                else
                {
                    return sessao.Query<Corretor>()
                        //.Fetch(c => c.TabelaComissao)
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
                        Corretor obj = sessao.Query<Corretor>().Where(s => s.ID == id).Single();

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

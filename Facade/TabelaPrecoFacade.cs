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

    public class TabelaPrecoFacade : FacadeBase
    {
        TabelaPrecoFacade() { }

        #region singleton 

        static TabelaPrecoFacade _instancia;
        public static TabelaPrecoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new TabelaPrecoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(TabelaPreco tabela)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    
                    try
                    {
                        sessao.SaveOrUpdate(tabela);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return tabela.ID;
        }

        public TabelaPreco Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<TabelaPreco>()
                    .Fetch(t=> t.Vigencias)
                    .Where(t => t.ID == id).Single();
            }
        }

        public List<TabelaPreco> Carregar(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<TabelaPreco>()
                        .OrderBy(t => t.Nome)
                        .ToList();
                }
                else
                {
                    return sessao.Query<TabelaPreco>()
                        .Where(t => t.Nome.Contains(nome))
                        .OrderBy(t => t.Nome)
                        .ToList();
                }
            }
        }

        public List<TabelaPreco> CarregarComVigencia(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<TabelaPreco>()
                        .Fetch(t => t.Vigencias)
                        .OrderBy(t => t.Nome)
                        .ToList();
                }
                else
                {
                    return sessao.Query<TabelaPreco>()
                        .Fetch(t => t.Vigencias)
                        .Where(t => t.Nome.Contains(nome))
                        .OrderBy(t => t.Nome)
                        .ToList();
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
                        TabelaPreco obj = sessao.Query<TabelaPreco>().Where(t => t.ID == id).Single();

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

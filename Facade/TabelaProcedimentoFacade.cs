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

    using LinqKit;

    public class TabelaProcedimentoFacade : FacadeBase
    {
        TabelaProcedimentoFacade() { }

        #region Singleton 

        static TabelaProcedimentoFacade _instancia;
        public static TabelaProcedimentoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new TabelaProcedimentoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(TabelaProcedimento tabela)
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

        public TabelaProcedimento Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                TabelaProcedimento tabela = sessao.Query<TabelaProcedimento>()
                    .Fetch(t => t.Segmento)
                    .FetchMany(t => t.Procedimentos)
                    .FetchMany(t => t.TabelasDeVigencias)
                    .Where(t => t.ID == id).Single();

                return tabela;
            }
        }

        public List<TabelaProcedimento> Carregar(int? codigo, string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (codigo.HasValue || (nome != null && nome.Trim() != ""))
                {
                    var predicadoOr = PredicateBuilder.False<TabelaProcedimento>();

                    if (codigo.HasValue)
                        predicadoOr = predicadoOr.Or(t => t.Codigo == codigo.Value);

                    if (nome != null && nome.Trim() != "")
                        predicadoOr = predicadoOr.Or(t => t.Nome.Contains(nome));

                    return sessao.Query<TabelaProcedimento>()
                        .AsExpandable()
                        .Where(predicadoOr)
                        .OrderBy(e => e.Nome)
                        .ToList();
                }
                else
                {
                    return sessao.Query<TabelaProcedimento>()
                        .OrderBy(e => e.Nome)
                        .Take(200)
                        .ToList();
                }
            }
        }

        public List<TabelaProcedimento> CarregarPorSegmento(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<TabelaProcedimento>()
                    .Fetch(t => t.Segmento)
                    .FetchMany(t => t.Procedimentos)
                    .FetchMany(t => t.TabelasDeVigencias)
                    .Where(t => t.Segmento.ID == id)
                    .OrderBy(t => t.Nome)
                    .ToList();
            }
        }

        public List<TabelaProcedimento> Carregar()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<TabelaProcedimento>()
                    .Fetch(t => t.Segmento)
                    .OrderBy(t => t.Nome)
                    .ToList();
            }
        }
    }
}

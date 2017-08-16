namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using LinqKit;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class TabelaCoberturaFacade : FacadeBase
    {
        TabelaCoberturaFacade() { }

        #region Singleton 

        static TabelaCoberturaFacade _instancia;
        public static TabelaCoberturaFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new TabelaCoberturaFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(TabelaCobertura tabela)
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

        public TabelaCobertura CarregarPorContratoADM(long? id, long contratoAdmId)
        {
            TabelaCobertura tabela = null;

            using (var sessao = ObterSessao())
            {
                if (id == null)
                {
                    tabela = sessao.Query<TabelaCobertura>()
                        .Fetch(t => t.ContratoAdm)
                        .Fetch(t => t.AssociadoPj)
                        .FetchMany(t => t.Itens)
                        .FetchMany(t => t.Vigencias)
                        .Where(t => t.ContratoAdm.ID == contratoAdmId).SingleOrDefault();
                }
                else
                {
                    tabela = sessao.Query<TabelaCobertura>()
                        .Fetch(t => t.ContratoAdm)
                        .Fetch(t => t.AssociadoPj)
                        .FetchMany(t => t.Itens)
                        .FetchMany(t => t.Vigencias)
                        .Where(t => t.ContratoAdm.ID == contratoAdmId && t.ID != id.Value)
                        .FirstOrDefault();
                }

                return tabela;
            }
        }

        public TabelaCobertura Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                TabelaCobertura tabela = sessao.Query<TabelaCobertura>()
                    .Fetch(t => t.ContratoAdm)
                    .Fetch(t => t.AssociadoPj)
                    .FetchMany(t => t.Itens)
                    .FetchMany(t => t.Vigencias)
                    .Where(t => t.ID == id).Single();

                return tabela;
            }
        }

        public List<TabelaCobertura> Carregar(Int64? idContratoAdm, string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (idContratoAdm.HasValue || (nome != null && nome.Trim() != ""))
                {
                    var predicadoOr = PredicateBuilder.False<TabelaCobertura>();

                    if (idContratoAdm.HasValue)
                        predicadoOr = predicadoOr.Or(t => t.ContratoAdm.ID == idContratoAdm.Value);

                    if (nome != null && nome.Trim() != "")
                        predicadoOr = predicadoOr.Or(t => t.Nome.Contains(nome));

                    return sessao.Query<TabelaCobertura>()
                        .AsExpandable()
                        .Where(predicadoOr)
                        .OrderBy(e => e.Nome)
                        .ToList();
                }
                else
                {
                    return sessao.Query<TabelaCobertura>()
                        .OrderBy(e => e.Nome)
                        .Take(200)
                        .ToList();
                }
            }
        }

        //public List<TabelaProcedimento> CarregarPorSegmento(long id)
        //{
        //    using (var sessao = ObterSessao())
        //    {
        //        return sessao.Query<TabelaProcedimento>()
        //            .Fetch(t => t.Segmento)
        //            .FetchMany(t => t.Procedimentos)
        //            .FetchMany(t => t.TabelasDeVigencias)
        //            .Where(t => t.Segmento.ID == id)
        //            .OrderBy(t => t.Nome)
        //            .ToList();
        //    }
        //}

        public List<TabelaCobertura> Carregar()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<TabelaCobertura>()
                    .Fetch(t => t.ContratoAdm)
                    .OrderBy(t => t.Nome)
                    .ToList();
            }
        }

        public long SalvarItem(ItemCobertura item)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(item);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return item.ID;
        }

        public ItemCobertura CarregarItem(long id)
        {
            using (var sessao = ObterSessao())
            {
                ItemCobertura item = sessao.Query<ItemCobertura>()
                    .Fetch(t => t.Tabela)
                    .Where(t => t.ID == id).Single();

                return item;
            }
        }

        public List<ItemCobertura> CarregarItens(long tabelaId, string descricao)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(descricao))
                {
                    return sessao.Query<ItemCobertura>()
                        .Where(p => p.Tabela.ID == tabelaId)
                        .OrderBy(p => p.Descricao)
                        .ToList();
                }
                else
                {
                    return sessao.Query<ItemCobertura>()
                        .Where(p => p.Tabela.ID == tabelaId && p.Descricao.Contains(descricao))
                        .OrderBy(p => p.Descricao)
                        .ToList();
                }
            }
        }

        public void ExcluirItem(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        ItemCobertura obj = sessao.Get<ItemCobertura>(id);
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

        /*******************************************************************************************/

        public long SalvarVigencia(VigenciaCobertura vigencia)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(vigencia);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return vigencia.ID;
        }

        public VigenciaCobertura CarregarVigencia(long id)
        {
            using (var sessao = ObterSessao())
            {
                VigenciaCobertura item = sessao.Query<VigenciaCobertura>()
                    .Fetch(t => t.Tabela)
                    .Where(t => t.ID == id).Single();

                return item;
            }
        }

        public List<VigenciaCobertura> CarregarVigencias(long tabelaId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<VigenciaCobertura>()
                    .Where(p => p.Tabela.ID == tabelaId)
                    .OrderByDescending(p => p.Inicio)
                    .ToList();
            }
        }

        public void ExcluirVigencia(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        VigenciaCobertura obj = sessao.Get<VigenciaCobertura>(id);
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
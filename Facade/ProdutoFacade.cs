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

    public class ProdutoFacade : FacadeBase
    {
        ProdutoFacade() { }

        #region Singleton 

        static ProdutoFacade _instancia;
        public static ProdutoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new ProdutoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(Produto produto)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        produto.Operadora = sessao.Get<Operadora>(produto.Operadora.ID);
                        produto.AssociadoPj = sessao.Get<AssociadoPJ>(produto.AssociadoPj.ID);
                        produto.ContratoAdm = sessao.Get<ContratoADM>(produto.ContratoAdm.ID);

                        sessao.SaveOrUpdate(produto);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return produto.ID;
        }

        public Produto Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                Produto prod = sessao.Query<Produto>()
                    .Fetch(p => p.ContratoAdm)
                    .Fetch(p => p.AssociadoPj)
                    .Fetch(p => p.Operadora)
                    .Where(p => p.ID == id).Single();

                return prod;
            }
        }

        public List<Produto> Carregar()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Produto>()
                    .Fetch(p => p.ContratoAdm)
                    .Fetch(p => p.AssociadoPj)
                    .Fetch(p => p.Operadora)
                    .OrderBy(t => t.Nome)
                    .ToList();
            }
        }

        public List<Produto> Carregar(long? operadoraId, long? associadoPjId, long? contratoAdmId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Produto>()
                    .Fetch(p => p.ContratoAdm)
                    .Fetch(p => p.AssociadoPj)
                    .Fetch(p => p.Operadora)
                    .Where(p => p.Operadora.ID == operadoraId.Value && p.AssociadoPj.ID == associadoPjId.Value && p.ContratoAdm.ID == contratoAdmId.Value)
                    .OrderBy(t => t.Nome)
                    .ToList();
            }
        }

        public List<Produto> Carregar(Int64 idContratoAdm, string nome)
        {
            using (var sessao = ObterSessao())
            {
                var predicadoOr = PredicateBuilder.False<Produto>();

                predicadoOr = predicadoOr.Or(t => t.ContratoAdm.ID == idContratoAdm);

                if (nome != null && nome.Trim() != "")
                    predicadoOr = predicadoOr.Or(t => t.Nome.Contains(nome));

                return sessao.Query<Produto>()
                    .AsExpandable()
                    .Where(predicadoOr)
                    .OrderBy(e => e.Nome)
                    .ToList();
            }
        }

        /*******************************************************************************************/

        public long SalvarItem(ProdutoItem item)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(item);

                        //todo: setar o idusuario e data alteracao do produto

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

        public ProdutoItem CarregarItem(long id)
        {
            using (var sessao = ObterSessao())
            {
                ProdutoItem item = sessao.Query<ProdutoItem>()
                    .Where(t => t.ID == id).Single();

                return item;
            }
        }

        public List<ProdutoItem> CarregarItens(long produtoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ProdutoItem>()
                    .Where(i => i.ProdutoId == produtoId)
                    .OrderBy(i => i.Nome)
                    .OrderByDescending(i => i.Vigencia)
                    .ToList();
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
                        ProdutoItem obj = sessao.Get<ProdutoItem>(id);
                        sessao.Delete(obj);

                        //todo: setar o idusuario e data alteracao do produto

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

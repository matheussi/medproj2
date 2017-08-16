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

    public class PrestadorUnidadeFacade : FacadeBase
    {
        PrestadorUnidadeFacade() { }

        #region singleton 

        static PrestadorUnidadeFacade _instancia;
        public static PrestadorUnidadeFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new PrestadorUnidadeFacade(); }
                return _instancia;
            }
        }
        #endregion

        public PrestadorUnidade Salvar(PrestadorUnidade unidade)
        {
            using (ISession sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    unidade.SetaCoordenadas();
                    sessao.SaveOrUpdate(unidade);

                    if (unidade.Procedimentos != null && unidade.Procedimentos.Count > 0)
                    {
                        unidade.Procedimentos.ForEach(delegate(UnidadeProcedimento up)
                        {
                            //up.Unidade = sessao.Get<PrestadorUnidade>(unidade.ID);
                            sessao.SaveOrUpdate(up);
                        });
                    }

                    tran.Commit();
                }
            }

            return unidade;
        }

        public PrestadorUnidade Salvar(PrestadorUnidade unidade, List<UnidadeProcedimento> procedimentos)
        {
            using (ISession sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    unidade.SetaCoordenadas();
                    sessao.SaveOrUpdate("PrestadorUnidade", unidade);

                    if (procedimentos != null && procedimentos.Count > 0)
                    {
                        procedimentos.ForEach(delegate(UnidadeProcedimento up)
                        {
                            up.Unidade = sessao.Get<PrestadorUnidade>(unidade.ID);
                            sessao.SaveOrUpdate(up);
                        });
                    }

                    tran.Commit();

                    unidade.Procedimentos = new List<UnidadeProcedimento>();
                    procedimentos.ForEach(up => unidade.Procedimentos.Add(up));
                }
            }

            return unidade;
        }


        public void TempSetaCoord()
        {
            using (ISession sessao = ObterSessao())
            {
                var lista = sessao.Query<PrestadorUnidade>().ToList();

                using (ITransaction tran = sessao.BeginTransaction())
                {
                    foreach (var unidade in lista)
                    {
                        unidade.SetaCoordenadas();
                        sessao.Update(unidade);
                    }

                    tran.Commit();
                }
            }
        }

        public void Excluir(long id)
        {
            using (ISession sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        Banco b = sessao.Query<Banco>().Where(bc => bc.Unidade.ID == id).FirstOrDefault();
                        if (b != null) { sessao.Delete(b); }

                        PrestadorUnidade unidade = sessao.Get<PrestadorUnidade>(id);
                        sessao.Delete(unidade);

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

        public List<PrestadorUnidade> Carrega()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<PrestadorUnidade>()
                    .Fetch(pu => pu.Owner)
                    .OrderBy(pu => pu.Owner.Nome)
                    .ToList();
            }
        }

        public PrestadorUnidade CarregaPorId(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<PrestadorUnidade>()
                    .Fetch(pu => pu.Owner)
                    .Fetch(pu => pu.Regiao)
                    .Fetch(pu => pu.Tabela)
                    .Fetch(pu => pu.TabelaPreco).ThenFetchMany(t => t.Vigencias)
                    //.FetchMany(pu => pu.Procedimentos) /////problema de plano cartesiano! 
                    .Where(pu => pu.ID == id)
                    .Single();
            }
        }

        public List<PrestadorUnidade> CarregaPorPrestadorId(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<PrestadorUnidade>()
                    .Fetch(pu => pu.Owner)
                    .Fetch(pu => pu.Regiao)
                    .Fetch(pu => pu.Tabela)
                    .Fetch(pu => pu.TabelaPreco)
                    .FetchMany(pu => pu.Procedimentos)
                    .Where(pu => pu.Owner.ID == id)
                    .ToList();
            }
        }

        public object CarregarEspecialidadesPorPrestador(long prestadorId)
        {
            using (var contexto = ObterSessao())
            {
                var lista = 
                    (from ue in contexto.Query<UnidadeEspecialidade>()
                     join pu in contexto.Query<PrestadorUnidade>() on ue.Unidade.ID equals pu.ID
                     join p in contexto.Query<Prestador>() on pu.Owner.ID equals p.ID
                     join e in contexto.Query<Especialidade>() on ue.Especialidade.ID equals e.ID
                     where p.ID == prestadorId
                     orderby pu.Nome, e.Nome
                     select new
                     {
                        ID = ue.ID,
                        EspecialidadeNome = e.Nome,
                        ContratoNome = pu.Nome,
                        ContratoEndereco = pu.Endereco,
                        ContratoCidade = pu.Cidade
                     }
                ).ToList();

                return lista;
            }
        }
    }
}
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

    public class AgendaAtribuicaoProcedimentoFacade : FacadeBase
    {
        AgendaAtribuicaoProcedimentoFacade() { }

        #region singleton 

        static AgendaAtribuicaoProcedimentoFacade _instancia;
        public static AgendaAtribuicaoProcedimentoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaAtribuicaoProcedimentoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaAtribuicaoProcedimento Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaAtribuicaoProcedimento>()
                    .Fetch(i => i.Tabela)
                    .FetchMany(i => i.Contratos)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public AgendaAtribuicaoProcedimento Salvar(AgendaAtribuicaoProcedimento agenda)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(agenda);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }

                sessao.Close();
                sessao.Dispose();
            }

            return agenda;
        }

        public List<AgendaAtribuicaoProcedimento> Carregar(DateTime de, DateTime ate, AgendaStatus status)
        {
            List<AgendaAtribuicaoProcedimento> lista = null;

            using (var sessao = ObterSessao())
            {
                if (status == AgendaStatus.Concluido)
                {
                    lista = sessao.Query<AgendaAtribuicaoProcedimento>()
                        .Fetch(i => i.Tabela)
                        //.FetchMany(i => i.Contratos)
                        .Where(i => i.DataConclusao.HasValue && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        //.Take(250)
                        .ToList();
                }
                else if (status == AgendaStatus.Pendente)
                {
                    lista = sessao.Query<AgendaAtribuicaoProcedimento>()
                        .Fetch(i => i.Tabela)
                        //.FetchMany(i => i.Contratos)
                        //.Take(250)
                        .Where(i => i.DataConclusao.HasValue == false && i.DataCriacao >= de && i.DataCriacao <= ate && i.Ativa == true)
                        .OrderByDescending(i => i.DataCriacao)
                        .ToList();
                }
                else
                {
                    lista = sessao.Query<AgendaAtribuicaoProcedimento>()
                        .Fetch(i => i.Tabela)
                        //.FetchMany(i => i.Contratos)
                        .Where(i => i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
            }

            return lista;
        }

        public List<AgendaAtribProcedRESULTADO> CarregarLog(Int64 agendaId)
        {
            List<AgendaAtribProcedRESULTADO> lista = null;

            using (var sessao = ObterSessao())
            {
                lista = sessao.Query<AgendaAtribProcedRESULTADO>()
                        .Fetch(i => i.Agenda)
                        .Fetch(i => i.Procedimento)
                        .Fetch(i => i.ContratoDePrestador)
                        .Where(i => i.Agenda.ID == agendaId)
                        .OrderBy(i => i.ID)
                        .ToList();
            }

            return lista;
        }
    }
}

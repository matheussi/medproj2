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

    public class AgendaImportacaoFacade : FacadeBase
    {
        AgendaImportacaoFacade() { }

        #region singleton 

        static AgendaImportacaoFacade _instancia;
        public static AgendaImportacaoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaImportacaoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaImportacao Salvar(AgendaImportacao agenda)
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
            }

            return agenda;
        }

        public AgendaImportacao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaImportacao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Fetch(i => i.Plano)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public AgendaImportacao CarregarPendenteParaProcessamento(DateTime data)
        {
            AgendaImportacao agenda = null;

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<AgendaImportacao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Fetch(i => i.Plano)
                    .Where(i => i.DataConclusao.HasValue == false && i.Ativa == true && i.DataProcessamento <= data)
                    .OrderBy(i => i.DataCriacao)
                    .FirstOrDefault();

                sessao.Close();
            }

            return agenda;
        }

        public List<AgendaImportacao> Carregar(DateTime de, DateTime ate, AgendaStatus status)
        {
            List<AgendaImportacao> lista = null;

            using (var sessao = ObterSessao())
            {
                if (status == AgendaStatus.Concluido)
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Where(i => i.DataConclusao.HasValue && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
                else if (status == AgendaStatus.Pendente)
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Take(250)
                        .Where(i => i.DataConclusao.HasValue == false && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .ToList();
                }
                else
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Where(i => i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
            }

            return lista;
        }

        public List<AgendaImportacaoItemLog> CarregarLog(long agendaId)
        {
            List<AgendaImportacaoItemLog> log = null;

            using (var sessao = ObterSessao())
            {
                log = sessao.Query<AgendaImportacaoItemLog>()
                    .Fetch(l => l.Agenda)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Contrato)
                    .Where(l => l.Agenda.ID == agendaId)
                    .ToList();
            }

            return log;
        }
    }
}

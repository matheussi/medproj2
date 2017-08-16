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

    public class AgendaExportacaoKitFacade : FacadeBase
    {
        private AgendaExportacaoKitFacade() { }

        #region singleton 

        static AgendaExportacaoKitFacade _instancia;
        public static AgendaExportacaoKitFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaExportacaoKitFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaExportacaoKit Salvar(AgendaExportacaoKit agenda)
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

        public AgendaExportacaoKit Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaExportacaoKit>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public List<AgendaExportacaoKit> Carregar(DateTime de, DateTime ate, AgendaStatus status)
        {
            List<AgendaExportacaoKit> lista = null;

            using (var sessao = ObterSessao())
            {
                if (status == AgendaStatus.Concluido)
                {
                    lista = sessao.Query<AgendaExportacaoKit>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Where(i => i.DataConclusao.HasValue && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
                else if (status == AgendaStatus.Pendente)
                {
                    lista = sessao.Query<AgendaExportacaoKit>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Take(250)
                        .Where(i => i.DataConclusao.HasValue == false && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .ToList();
                }
                else
                {
                    lista = sessao.Query<AgendaExportacaoKit>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Where(i => i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
            }

            return lista;
        }

        public AgendaExportacaoKit CarregarPendenteParaProcessamento(DateTime data)
        {
            AgendaExportacaoKit agenda = null;

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<AgendaExportacaoKit>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Where(i => i.DataConclusao.HasValue == false && i.Ativa == true && i.DataProcessamento <= data)
                    .OrderBy(i => i.DataCriacao)
                    .FirstOrDefault();

                sessao.Close();
            }

            return agenda;
        }

        public List<AgendaExportacaoKitItemLog> CarregarLog(long agendaId)
        {
            List<AgendaExportacaoKitItemLog> log = null;

            using (var sessao = ObterSessao())
            {
                log = sessao.Query<AgendaExportacaoKitItemLog>()
                    .Fetch(l => l.Agenda)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Contrato)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Beneficiario)
                    .Where(l => l.Agenda.ID == agendaId)
                    .ToList();
            }

            return log;
        }
    }
}

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

    public class AgendaExportacaoCartaoFacade : FacadeBase
    {
        AgendaExportacaoCartaoFacade() { }

        #region singleton 

        static AgendaExportacaoCartaoFacade _instancia;
        public static AgendaExportacaoCartaoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaExportacaoCartaoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaExportacaoCartao Salvar(AgendaExportacaoCartao agenda)
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

        public AgendaExportacaoCartao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaExportacaoCartao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public List<AgendaExportacaoCartao> Carregar(DateTime de, DateTime ate, AgendaStatus status)
        {
            List<AgendaExportacaoCartao> lista = null;

            using (var sessao = ObterSessao())
            {
                if (status == AgendaStatus.Concluido)
                {
                    lista = sessao.Query<AgendaExportacaoCartao>()
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
                    lista = sessao.Query<AgendaExportacaoCartao>()
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
                    lista = sessao.Query<AgendaExportacaoCartao>()
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

        public AgendaExportacaoCartao CarregarPendenteParaProcessamento(DateTime data)
        {
            AgendaExportacaoCartao agenda = null;

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<AgendaExportacaoCartao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Where(i => i.DataConclusao.HasValue == false && i.Ativa == true && i.DataProcessamento <= data)
                    .OrderBy(i => i.DataCriacao)
                    .FirstOrDefault();

                sessao.Close();
                sessao.Dispose();
            }

            return agenda;
        }

        public List<AgendaExportacaoCartaoItem> CarregarLog(long agendaId)
        {
            List<AgendaExportacaoCartaoItem> log = null;

            using (var sessao = ObterSessao())
            {
                log = sessao.Query<AgendaExportacaoCartaoItem>()
                    .Fetch(l => l.Agenda)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Contrato).ThenFetch(c => c.EnderecoReferencia)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Beneficiario)
                    .Where(l => l.Agenda.ID == agendaId)
                    .ToList();

                //TODO: arrumar isso!
                if (log != null)
                {
                    foreach (AgendaExportacaoCartaoItem i in log)
                    {
                        if (i.Titular != null && i.Titular.Contrato != null)
                        {
                            //i.Titular.Contrato.EnderecoReferencia = sessao.Get<Endereco>(i.Titular.Contrato.EnderecoReferenciaID);
                            i.Titular.Contrato.EnderecoReferencia =
                                sessao.Query<Endereco>().FirstOrDefault(e => e.DonoId == i.Titular.Beneficiario.ID && e.DonoTipo == 0);
                        }
                    }
                }
            }

            return log;
        }
    }
}

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

    public class ConfigAdicionalFacade : FacadeBase
    {
        ConfigAdicionalFacade() { }

        #region Singleton 

        static ConfigAdicionalFacade _instancia;
        public static ConfigAdicionalFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new ConfigAdicionalFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(ConfigAdicional obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public ConfigAdicional Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                ConfigAdicional obj = sessao.Query<ConfigAdicional>()
                    .FetchMany(o => o.Contratos)
                    .Where(t => t.ID == id).Single();

                return obj;
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
                        ConfigAdicional obj = sessao.Get<ConfigAdicional>(id);
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

        public List<ConfigAdicional> Carregar(string nome = null)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigAdicional>()
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        /// <summary>
        /// Carrega por associado pj (estipulante)
        /// </summary>
        public List<ConfigAdicional> CarregarPorAssoc(long associadoPjId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigAdicional>()
                    .Where(c => c.AssociadoPj.ID == associadoPjId)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        /// <summary>
        /// Carrega por associado pj (estipulante) - SOMENTE marcado como TODOS OS CONTRATOS
        /// </summary>
        public List<ConfigAdicional> CarregarPorAssoc(long associadoPjId, long contratoAdmId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigAdicional>()
                    .Where(c => c.AssociadoPj.ID == associadoPjId && c.ContratoAdm.ID == contratoAdmId && c.TodosContratos)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        public List<ConfigAdicional> CarregarPor(long contratoAdmId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigAdicional>()
                    .Where(c => c.ContratoAdm.ID == contratoAdmId)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        public List<ConfigAdicional> CarregarPor(long contratoAdmId, long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigAdicional>()
                    .Where(c => (c.ContratoAdm.ID == contratoAdmId && !c.TodosContratos) && (c.Contratos.Any(co => co.ID == contratoId)))
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }
    }
}

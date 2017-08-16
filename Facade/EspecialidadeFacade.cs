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

    public class EspecialidadeFacade : FacadeBase
    {
        private EspecialidadeFacade() { }

        #region Singleton

        static EspecialidadeFacade _instance;
        public static EspecialidadeFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new EspecialidadeFacade(); }
                return _instance;
            }
        }
        #endregion

        public long Salvar(Especialidade especialidade)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(especialidade);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return especialidade.ID;
        }

        public Especialidade Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                Especialidade espec = sessao.Query<Especialidade>().Where(e => e.ID == id).SingleOrDefault();

                return espec;
            }
        }

        public Especialidade Carregar(string nome, ISession sessao)
        {
            if (sessao == null)
            {
                using (sessao = ObterSessao())
                {
                    return sessao.Query<Especialidade>().Where(e => e.Nome.ToUpper() == nome.ToUpper()).FirstOrDefault();
                }
            }
            else
            {
                return sessao.Query<Especialidade>().Where(e => e.Nome.ToUpper() == nome.ToUpper()).FirstOrDefault();
            }
        }

        /// <summary>
        /// Carrega todas as especialidades de um segmento. Padrão: segmentoId = 2.
        /// </summary>
        /// <param name="segmentoId">Se omitido, carregará as especialidades do seguimento id 2: saúde</param>
        /// <returns></returns>
        public List<Especialidade> Carregar(string nome, long segmentoId = 2)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<Especialidade>().Where(e => e.SegmentoId == segmentoId).OrderBy(e => e.Nome).ToList();
                }
                else
                {
                    return sessao.Query<Especialidade>()
                        .Where(e => e.Nome.Contains(nome) && e.SegmentoId == segmentoId)
                        .OrderBy(e => e.Nome).ToList();
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
                        Especialidade obj = sessao.Query<Especialidade>().Where(e => e.ID == id).Single();

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

        /*--------------------------------------------------------------*/

        public List<UnidadeEspecialidade> CarregarProcedimentosDaUnidade(long unidadeId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<UnidadeEspecialidade>()
                    .Fetch(e => e.Especialidade)
                    .Fetch(e => e.Unidade)
                    .Where(e => e.Unidade.ID == unidadeId)
                    .ToList();
            }
        }

        /*--------------------------------------------------------------*/

        public UnidadeEspecialidade CarregarEspecialidadeDaUnidade(long unidadeId, string especialidade, ISession sessao)
        {
            if (sessao == null)
            {
                using (sessao = ObterSessao())
                {
                    return sessao.Query<UnidadeEspecialidade>()
                        .Fetch(e => e.Especialidade)
                        .Fetch(e => e.Unidade)
                        .Where(e => e.Unidade.ID == unidadeId && e.Especialidade.Nome.ToUpper() == especialidade.ToUpper())
                        .FirstOrDefault();
                }
            }
            else
            {
                return sessao.Query<UnidadeEspecialidade>()
                        .Fetch(e => e.Especialidade)
                        .Fetch(e => e.Unidade)
                        .Where(e => e.Unidade.ID == unidadeId && e.Especialidade.Nome.ToUpper() == especialidade.ToUpper())
                        .FirstOrDefault();
            }
        }

        public long SalvarUnidadeEspecialidade(UnidadeEspecialidade especialidade)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(especialidade);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return especialidade.ID;
        }

        public UnidadeEspecialidade CarregarUnidadeEspecialidade(long id)
        {
            using (var sessao = ObterSessao())
            {
                UnidadeEspecialidade espec = sessao.Query<UnidadeEspecialidade>()
                    .Fetch(e => e.Unidade)
                    .Fetch(e => e.Especialidade)
                    .Where(e => e.ID == id)
                    .Single();

                return espec;
            }
        }

        public void ExcluirUnidadeEspecialidade(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        UnidadeEspecialidade obj = sessao.Query<UnidadeEspecialidade>().Where(e => e.ID == id).Single();

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

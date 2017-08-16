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

    public class PlanoFacade : FacadeBase
    {
        #region singleton 

        static PlanoFacade _instance;
        public static PlanoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new PlanoFacade(); }
                return _instance;
            }
        }

        #endregion

        public long Salvar(Plano obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(obj);
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

        public Plano Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                Plano obj = sessao.Query<Plano>()
                    .Fetch(p => p.ContratoAdm)
                    .Where(e => e.ID == id).Single();

                return obj;
            }
        }

        public Plano CarregarPorContratoAdmId(long id)
        {
            using (var sessao = ObterSessao())
            {
                Plano obj = sessao.Query<Plano>()
                    .Fetch(p => p.ContratoAdm)
                    .Where(p => p.ContratoAdm.ID == id).FirstOrDefault();

                return obj;
            }
        }
    }
}

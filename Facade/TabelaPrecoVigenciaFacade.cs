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

    public class TabelaPrecoVigenciaFacade : FacadeBase
    {
        TabelaPrecoVigenciaFacade() { }

        #region singleton 

        static TabelaPrecoVigenciaFacade _instancia;
        public static TabelaPrecoVigenciaFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new TabelaPrecoVigenciaFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(TabelaPrecoVigencia vigencia)
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

        public TabelaPrecoVigencia Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<TabelaPrecoVigencia>().Where(t => t.ID == id).Single();
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
                        TabelaPrecoVigencia obj = sessao.Query<TabelaPrecoVigencia>().Where(v => v.ID == id).Single();

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

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

    public class SegmentoFacade : FacadeBase
    {
        SegmentoFacade() { }

        #region Singleton 

        static SegmentoFacade _instancia;
        public static SegmentoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new SegmentoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(Segmento segmento)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(segmento);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return segmento.ID;
        }

        public Segmento Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Segmento>().Where(s => s.ID == id).Single();
            }
        }

        public List<Segmento> CarregarTodos(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<Segmento>()
                        .OrderBy(s => s.Nome).ToList();
                }
                else
                {
                    return sessao.Query<Segmento>()
                        .Where(s => s.Nome.Contains(nome))
                        .OrderBy(s => s.Nome).ToList();
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
                        Segmento obj = sessao.Query<Segmento>().Where(s => s.ID == id).Single();

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

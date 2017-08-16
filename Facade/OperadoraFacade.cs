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

    public class OperadoraFacade : FacadeBase
    {
        #region singleton 

        static OperadoraFacade _instance;
        public static OperadoraFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new OperadoraFacade(); }
                return _instance;
            }
        }

        #endregion

        public Operadora Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                Operadora obj = sessao.Query<Operadora>().Where(e => e.ID == id).Single();

                return obj;
            }
        }

        public List<Operadora> Carregar(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<Operadora>()
                        .OrderBy(e => e.Nome).ToList();
                }
                else
                {
                    return sessao.Query<Operadora>()
                        .Where(e => e.Nome.Contains(nome))
                        .OrderBy(e => e.Nome).ToList();
                }
            }
        }
    }
}

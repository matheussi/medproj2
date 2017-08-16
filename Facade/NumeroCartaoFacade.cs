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

    public class NumeroCartaoFacade : FacadeBase
    {
        NumeroCartaoFacade() { }

        #region singleton  

        static NumeroCartaoFacade _instancia;
        public static NumeroCartaoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new NumeroCartaoFacade (); }
                return _instancia;
            }
        }
        #endregion

        public NumeroCartao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<NumeroCartao>()
                    .Where(i => i.ID == id)
                    .Single();
            }
        }
    }
}

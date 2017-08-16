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

    public class BeneficiarioFacade : FacadeBase
    {
        BeneficiarioFacade() { }

        #region singleton 

        static BeneficiarioFacade _instancia;
        public static BeneficiarioFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new BeneficiarioFacade(); }
                return _instancia;
            }
        }
        #endregion

        public List<Beneficiario> CarregarPJ()
        {
            using (var contexto = ObterSessao())
            {
                var lista = contexto.Query<Beneficiario>()
                    .Where(b => b.Tipo == TipoPessoa.Juridica)
                    .OrderBy(b => b.Nome)
                    .ToList();

                return lista;
            }
        }
    }
}

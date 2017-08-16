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

    public class BancoFacade : FacadeBase
    {
        BancoFacade() { }

        #region singleton  

        static BancoFacade _instancia;
        public static BancoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new BancoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public Banco Salvar(Banco banco)
        {
            using (var contexto = ObterSessao())
            {
                using (ITransaction tran = contexto.BeginTransaction())
                {
                    contexto.SaveOrUpdate(banco);
                    tran.Commit();

                    return banco;
                }
            }
        }

        public Banco CarregarPorUnidadeId(long unidadeId)
        {
            using (var contexto = ObterSessao())
            {
                Banco banco = contexto.Query<Banco>()
                    .Where(b => b.Unidade.ID == unidadeId)
                    .SingleOrDefault();

                return banco;
            }
        }
    }
}
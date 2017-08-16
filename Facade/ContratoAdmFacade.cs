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

    public class ContratoAdmFacade : FacadeBase
    {
        #region singleton 

        static ContratoAdmFacade _instance;
        public static ContratoAdmFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ContratoAdmFacade(); }
                return _instance;
            }
        }

        #endregion

        private ContratoAdmFacade() { }

        public long Salvar(ContratoADM obj)
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

        public ContratoADM Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                ContratoADM obj = sessao.Query<ContratoADM>()
                    .Fetch(c => c.Operadora)
                    .Fetch(c => c.AssociadoPJ)
                    .Where(e => e.ID == id).Single();

                return obj;
            }
        }

        public List<ContratoADM> Carregar(string descricao)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(descricao))
                {
                    return sessao.Query<ContratoADM>()
                        .Fetch(c => c.Operadora)
                        .Fetch(c => c.AssociadoPJ)
                        //.Where(c => c.ID == 1 && c.Descricao == "denis")
                        .OrderBy(e => e.Descricao).ToList();
                }
                else
                {
                    return sessao.Query<ContratoADM>()
                        .Fetch(c => c.Operadora)
                        .Fetch(c => c.AssociadoPJ)
                        .Where(e => e.Descricao.Contains(descricao))
                        .OrderBy(e => e.Descricao).ToList();
                }
            }
        }

        public List<ContratoADM> CarregarTodos(long associadoPjID)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ContratoADM>()
                    .Fetch(c => c.Operadora)
                    .Fetch(c => c.AssociadoPJ)
                    .Where(e => e.AssociadoPJ.ID == associadoPjID)
                    .OrderBy(e => e.Descricao).ToList();
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
                        ContratoADM obj = sessao.Query<ContratoADM>().Where(e => e.ID == id).Single();

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

            //MyExpression teste = new MyExpression();
            //var t = teste.Init().AddField("nome").AddField("endereco").AddField("sexo").AddCondition("id", "1").Evaluate;
        }
    }

    //public class MyExpression
    //{
    //    string result = "";

    //    public MyExpression Init()
    //    {
    //        result = "select";
    //        return this;
    //    }

    //    public MyExpression AddField(string field)
    //    {
    //        if (result.Length > 6) result += ",";
    //        result += " " + field + " ";
    //        return this;
    //    }

    //    public MyExpression AddCondition(string field, string value)
    //    {
    //        result += " where " + field + " = " + value;
    //        return this;
    //    }

    //    public string Evaluate
    //    {
    //        get { return result; }
    //    }
    //}
}

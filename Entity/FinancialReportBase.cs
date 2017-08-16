namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("reportbaseAgenda")]
    public class FinancialReportBase : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _filialIdArr;
        String _operadoraIdArr;
        String _estipulanteIdArr;
        DateTime _vencimentoDe;
        DateTime _vencimentoAte;
        DateTime _processarEm;
        Boolean _processado;

        public FinancialReportBase() { }

        #region properties 

        [DBFieldInfo("rbp_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("rbp_filialIdArr", FieldType.Single)]
        public String FilialIdArr
        {
            get { return _filialIdArr; }
            set { _filialIdArr= value; }
        }

        [DBFieldInfo("rbp_operadoraIdArr", FieldType.Single)]
        public String OperadoraIdArr
        {
            get { return _operadoraIdArr; }
            set { _operadoraIdArr= value; }
        }

        [DBFieldInfo("rbp_estipulanteIdArr", FieldType.Single)]
        public String EstipulanteIdArr
        {
            get { return _estipulanteIdArr; }
            set { _estipulanteIdArr= value; }
        }

        [DBFieldInfo("rbp_vencimentoDe", FieldType.Single)]
        public DateTime VencimentoDe
        {
            get { return _vencimentoDe; }
            set { _vencimentoDe= value; }
        }

        [DBFieldInfo("rbp_vencimentoAte", FieldType.Single)]
        public DateTime VencimentoAte
        {
            get { return _vencimentoAte; }
            set { _vencimentoAte= value; }
        }

        [DBFieldInfo("rpb_processarEm", FieldType.Single)]
        public DateTime ProcessarEm
        {
            get { return _processarEm; }
            set { _processarEm= value; }
        }

        [DBFieldInfo("rpb_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }
        #endregion

        #region EntityBase methods 

        public void Salvar(Boolean limparAgenda)
        {
            if (limparAgenda)
            {
                NonQueryHelper.Instance.ExecuteNonQuery("TRUNCATE TABLE reportbaseAgenda", null);
            }

            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static FinancialReportBase CarregarPendente(PersistenceManager pm)
        {
            String qry = " TOP 1 * FROM reportbaseAgenda WHERE rpb_processado=0 AND GETDATE() >= rpb_processarEm ";

            IList<FinancialReportBase> list = LocatorHelper.Instance.ExecuteQuery<FinancialReportBase>(qry, typeof(FinancialReportBase), pm);

            if (list == null) { return null; }
            else { return list[0]; }
        }
    }
}

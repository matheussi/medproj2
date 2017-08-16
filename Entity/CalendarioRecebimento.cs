namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("calendario_recebimento")]
    public class CalendarioRecebimento : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _calendarioAdmissaoId;
        Int32 _faturaDia;
        Int32 _faturaTipo;
        Int32 _comissaoDia;
        Int32 _comissaoTipo;
        Int32 _percentualComissao;
        DateTime _data;

        #endregion

        #region properties 

        [DBFieldInfo("calendariorecebimento_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("calendariorecebimento_calendarioVenctoId", FieldType.Single)]
        public Object CalendarioVencimentoID
        {
            get { return _calendarioAdmissaoId; }
            set { _calendarioAdmissaoId= value; }
        }

        [DBFieldInfo("calendariorecebimento_faturaDia", FieldType.Single)]
        public Int32 FaturaDia
        {
            get { return _faturaDia; }
            set { _faturaDia= value; }
        }

        [DBFieldInfo("calendariorecebimento_admissaoDeTipo", FieldType.Single)]
        public Int32 FaturaTipo
        {
            get { return _faturaTipo; }
            set { _faturaTipo= value; }
        }

        [DBFieldInfo("calendariorecebimento_comissaoDia", FieldType.Single)]
        public Int32 ComissaoDia
        {
            get { return _comissaoDia; }
            set { _comissaoDia= value; }
        }

        [DBFieldInfo("calendariorecebimento_comissaoTipo", FieldType.Single)]
        public Int32 ComissaoTipo
        {
            get { return _comissaoTipo; }
            set { _comissaoTipo= value; }
        }

        [DBFieldInfo("calendariorecebimento_percentualComissao", FieldType.Single)]
        public Int32 ComissaoPercentual
        {
            get { return _percentualComissao; }
            set { _percentualComissao= value; }
        }

        [DBFieldInfo("calendariorecebimento_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        public String StrFaturaTipo
        {
            get { return CalendarioHelper.TraduzTipo(_faturaTipo, false); }
        }

        public String StrComissaoTipo
        {
            get { return CalendarioHelper.TraduzTipo(_comissaoTipo, false); }
        }

        #endregion

        public CalendarioRecebimento() { _data = DateTime.Now; }
        public CalendarioRecebimento(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
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

        //public static IList<CalendarioRecebimento> CarregarPorCalendarioDeAdmissao(Object calendarioAdmissaoId)
        //{
        //    String sql = "* FROM calendario_recebimento WHERE calendariorecebimento_calendarioAdmissId=" + calendarioAdmissaoId;

        //    return LocatorHelper.Instance.ExecuteQuery<CalendarioRecebimento>(sql, typeof(CalendarioRecebimento));
        //}

        public static IList<CalendarioRecebimento> CarregarPorCalendarioDeVencimento(Object calendarioVenctoId)
        {
            String sql = "* FROM calendario_recebimento WHERE calendariorecebimento_calendarioVenctoId=" + calendarioVenctoId;

            return LocatorHelper.Instance.ExecuteQuery<CalendarioRecebimento>(sql, typeof(CalendarioRecebimento));
        }
    }
}

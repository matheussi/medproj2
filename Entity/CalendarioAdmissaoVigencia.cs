namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    internal class CalendarioHelper
    {
        public static String TraduzTipo(Int32 tipo, Boolean prefixo)
        {
            switch (tipo)
            {
                case -1:
                {
                    return "mês anterior";
                }
                case 0:
                {
                    return "mês atual";
                }
                case 1:
                {
                    return "mês próximo";
                }
                case 2:
                {
                    return "mês seguinte ao mês próximo";
                }
                default:
                {
                    return "?";
                }
            }
        }
    }

    [DBTable("calendario")]
    public class CalendarioAdmissaoVigencia : EntityBase, IPersisteableEntity
    {
        public enum eDataLimiteTipo : int
        {
            TodoDia=1,
            TextoPersonalizado
        }

        #region fields 

        Object _id;
        Object _contratoId;
        Int32  _admissaoDe_Dia;
        Int32  _admissaoDe_Tipo;
        Int32  _admissaoAte_Dia;
        Int32  _admissaoAte_Tipo;
        Int32  _vigenciaDia;
        Int32  _vigenciaTipo;
///////////////////////////////////////////////////////////////////////////////////////////////
        //Int32  _vencimentoDia;
        //Int32  _vencimentoTipo;
        //Int32  _dataSemJuros_Dia;
        //Int32  _dataLimite_Tipo;
        //Object _dataLimite_Valor;

        DateTime _data;

        //const Int32 _mesAtual             = 8;
        //const Int32 _mesAnterior          = 7;
        //const Int32 _mesProximo           = 9;
        //const Int32 _mesSeguinteAoProximo = 10;

        #endregion

        #region properties 

        [DBFieldInfo("calendario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("calendario_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("calendario_admissaoDeDia", FieldType.Single)]
        public Int32 AdmissaoDe_Dia
        {
            get { return _admissaoDe_Dia; }
            set { _admissaoDe_Dia= value; }
        }

        [DBFieldInfo("calendario_admissaoDeTipo", FieldType.Single)]
        public Int32 AdmissaoDe_Tipo
        {
            get { return _admissaoDe_Tipo; }
            set { _admissaoDe_Tipo= value; }
        }

        [DBFieldInfo("calendario_admissaoAteDia", FieldType.Single)]
        public Int32 AdmissaoAte_Dia
        {
            get { return _admissaoAte_Dia; }
            set { _admissaoAte_Dia= value; }
        }

        [DBFieldInfo("calendario_admissaoAteTipo", FieldType.Single)]
        public Int32 AdmissaoAte_Tipo
        {
            get { return _admissaoAte_Tipo; }
            set { _admissaoAte_Tipo= value; }
        }

        [DBFieldInfo("calendario_vigenciaDia", FieldType.Single)]
        public Int32 VigenciaDia
        {
            get { return _vigenciaDia; }
            set { _vigenciaDia= value; }
        }

        [DBFieldInfo("calendario_vigenciaTipo", FieldType.Single)]
        public Int32 VigenciaTipo
        {
            get { return _vigenciaTipo; }
            set { _vigenciaTipo= value; }
        }

        //[DBFieldInfo("calendario_vencimentoDia", FieldType.Single)]
        //public Int32 VencimentoDia
        //{
        //    get { return _vencimentoDia; }
        //    set { _vencimentoDia= value; }
        //}

        //[DBFieldInfo("calendario_vencimentoTipo", FieldType.Single)]
        //public Int32 VencimentoTipo
        //{
        //    get { return _vencimentoTipo; }
        //    set { _vencimentoTipo= value; }
        //}

        //[DBFieldInfo("calendario_dataSemJurosDia", FieldType.Single)]
        //public Int32 DataSemJuros_Dia
        //{
        //    get { return _dataSemJuros_Dia; }
        //    set { _dataSemJuros_Dia= value; }
        //}

        //[DBFieldInfo("calendario_dataLimiteTipo", FieldType.Single)]
        //public Int32 DataLimite_Tipo
        //{
        //    get { return _dataLimite_Tipo; }
        //    set { _dataLimite_Tipo= value; }
        //}

        //[DBFieldInfo("calendario_dataLimiteValor", FieldType.Single)]
        //public Object DataLimite_Valor
        //{
        //    get { return _dataLimite_Valor; }
        //    set { _dataLimite_Valor= value; }
        //}

        //public String strDataLimite
        //{
        //    get
        //    {
        //        if (_dataLimite_Valor == null) { return "&nbsp;"; }
        //        if (_dataLimite_Tipo == (Int32)eDataLimiteTipo.TextoPersonalizado)
        //            return Convert.ToString(_dataLimite_Valor);
        //        else
        //            return "Todo dia " + Convert.ToString(_dataLimite_Valor);
        //    }
        //}

        [DBFieldInfo("calendario_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        public String strData
        {
            get { return _data.ToString("dd/MM/yyyy"); }
        }

        public String StrAdmissaoDe_Tipo
        {
            get { return CalendarioHelper.TraduzTipo(_admissaoDe_Tipo, false); }
        }

        public String StrAdmissaoAte_Tipo
        {
            get { return CalendarioHelper.TraduzTipo(_admissaoAte_Tipo, false); }
        }

        public String StrVigenciaTipo
        {
            get { return CalendarioHelper.TraduzTipo(_vigenciaTipo, false); }
        }

        //public String StrVencimentoTipo
        //{
        //    get { return CalendarioHelper.TraduzTipo(_vencimentoTipo, false); }
        //}

        #endregion

        public CalendarioRecebimento CalendarioRecebimento
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public CalendarioAdmissaoVigencia() { this._data = DateTime.Now; }
        public CalendarioAdmissaoVigencia(Object id) : this() { this._id = id; }

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

        public static IList<CalendarioAdmissaoVigencia> CarregarPorContrato(Object contratoId)
        {
            return CarregarPorContrato(contratoId, null);
        }
        public static IList<CalendarioAdmissaoVigencia> CarregarPorContrato(Object contratoId, PersistenceManager pm)
        {
            String sql = "* FROM calendario WHERE calendario_contratoId=" + contratoId;

            return LocatorHelper.Instance.ExecuteQuery
                <CalendarioAdmissaoVigencia>(sql, typeof(CalendarioAdmissaoVigencia), pm);
        }

        public static IList<CalendarioAdmissaoVigencia> CarregarPorContrato(Object contratoId, DateTime admissao, PersistenceManager pm)
        {
            String sql = "* FROM calendario WHERE calendario_contratoId=" + contratoId + " AND calendario_data <= '" + admissao.ToString("yyyy-MM-dd 23:59:59") + "' ORDER BY calendario_data DESC";

            return LocatorHelper.Instance.ExecuteQuery
                <CalendarioAdmissaoVigencia>(sql, typeof(CalendarioAdmissaoVigencia), pm);
        }

        static Int32 ultimoDiaDoMes(Int32 mes, Int32 ano)
        {
            DateTime data = new DateTime(ano, mes, 1);
            return data.AddMonths(1).AddDays(-1).Day;
        }

        public static void CalculaDatasDeVigenciaEVencimento(Object contratoAdmID, DateTime dataAdmissao, out DateTime vigencia, out DateTime vencimento, out Int32 diaDataSemJuros, out Object valorDataLimite, out CalendarioVencimento rcv)
        {
            CalculaDatasDeVigenciaEVencimento(contratoAdmID, dataAdmissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, null);
        }
        public static void CalculaDatasDeVigenciaEVencimento(Object contratoAdmID, DateTime dataAdmissao, out DateTime vigencia, out DateTime vencimento, out Int32 diaDataSemJuros, out Object valorDataLimite, out CalendarioVencimento rcv, PersistenceManager pm)
        {
            IList<CalendarioAdmissaoVigencia> lista = CarregarPorContrato(contratoAdmID, dataAdmissao, pm);

            vigencia   = DateTime.MinValue;
            vencimento = DateTime.MinValue;
            diaDataSemJuros = -1;
            valorDataLimite = null;
            rcv = null;

            if (lista == null) { return; }

            DateTime dtAdmissaoDe_Cadastrada  = DateTime.MinValue;
            DateTime dtAdmissaoAte_Cadastrada = DateTime.MinValue;

            Int32 fatorSomaMes = 0;
            foreach (CalendarioAdmissaoVigencia cal in lista)
            {
                if (cal.AdmissaoDe_Tipo == 0 && cal.AdmissaoAte_Tipo == 0) //mes atual
                {
                    if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && dataAdmissao.Day <= cal.AdmissaoAte_Dia)
                    {
                        dtAdmissaoDe_Cadastrada = new DateTime(
                            dataAdmissao.Year, dataAdmissao.Month, cal.AdmissaoDe_Dia);

                        try
                        {
                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.Year, dataAdmissao.Month, cal.AdmissaoAte_Dia);
                        }
                        catch
                        {
                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.Year, dataAdmissao.Month, ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                        }
                    }
                    else
                        continue;
                }
                else
                {
                    #region DE

                    try 
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia);
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia);
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.Year,
                                    dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                    }
                    catch
                    {
                        #region tratamento de erro. 

                        #region ano atual

                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                        ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year));
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month,
                                        ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                    ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year)); //nao tem dia 31, entao tenta 30. Corrigir isso!
                            }
                        }
                        #endregion

                        #region anos anteriores

                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                        ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year));
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month,
                                        ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.Year,
                                    dataAdmissao.Month,
                                    ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));// cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                        #endregion

                        #endregion
                    }
                    #endregion
                }

                if (cal.AdmissaoAte_Tipo == 0) //mes atual
                {
                }
                else
                {
                    #region ATE

                    fatorSomaMes = cal.AdmissaoAte_Tipo;
                    if (fatorSomaMes == 0) { fatorSomaMes++; }
                    try 
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 1;
                            else
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Year,
                                dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Month, cal.AdmissaoAte_Dia);
                        }
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, cal.AdmissaoAte_Dia); //dtAdmissaoDe_Cadastrada.AddMonths(cal.AdmissaoAte_Tipo + 1).Month, cal.AdmissaoAte_Dia);
                        }
                    }
                    catch
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            //if (cal.AdmissaoDe_Tipo == -1)
                            //    fatorSomaMes = 1;
                            //else
                            //    fatorSomaMes = 0;
                            fatorSomaMes = cal.AdmissaoDe_Tipo;

                            //dtAdmissaoAte_Cadastrada = new DateTime(
                            //    dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Year,
                            //    dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Month,
                            //    ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Year)); //cal.AdmissaoAte_Dia - 1

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.AddMonths(fatorSomaMes).Year,
                                dataAdmissao.AddMonths(fatorSomaMes).Month,
                                ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Year)); //cal.AdmissaoAte_Dia - 1
                        }
                        #region
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, 1);

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, ultimoDiaDoMes(dtAdmissaoAte_Cadastrada.Month, dtAdmissaoAte_Cadastrada.Year));
                        }
                        #endregion
                    }
                    #endregion

                }

                IList<CalendarioVencimento> listaVencto = CalendarioVencimento.CarregarTodos(cal.ID);
                if (listaVencto == null || listaVencto.Count == 0) { continue; }


                if (listaVencto.Count == 1)
                {
                    #region arrumar...

                    try 
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1)
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cal.VigenciaTipo).Month, dataAdmissao.AddMonths(cal.VigenciaTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }

                    CalendarioVencimento cv = listaVencto[0];
                    try 
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cv.VencimentoTipo).Month, dataAdmissao.AddMonths(cv.VencimentoTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }

                    diaDataSemJuros = cv.DataSemJuros_Dia;
                    valorDataLimite = cv.DataLimite_Valor;

                    return;

                    #endregion
                }
                else if (dtAdmissaoDe_Cadastrada <= dataAdmissao &&
                    dtAdmissaoAte_Cadastrada >= dataAdmissao)
                {
                    try 
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cal.VigenciaTipo).Month, dataAdmissao.AddMonths(cal.VigenciaTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }

                    CalendarioVencimento cv = listaVencto[0];
                    try 
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cv.VencimentoTipo).Month, dataAdmissao.AddMonths(cv.VencimentoTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }

                    diaDataSemJuros = cv.DataSemJuros_Dia;
                    valorDataLimite = cv.DataLimite_Valor;

                    return;
                }
                else// if(dataAdmissao.Day >= dtAdmissaoDe_Cadastrada.Day && dataAdmissao.Day <= dtAdmissaoAte_Cadastrada.Day)
                {
                    //Int32 fatorSoma = 1; //deve ser a diferenca entre o mes admissao e o mes cadastrado ate
                    //DateTime paramDE  = new DateTime(dataAdmissao.Year, dataAdmissao.Month, dtAdmissaoDe_Cadastrada.Day);
                    //DateTime paramATE = new DateTime(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo+1).Year, paramDE.AddMonths(cal.AdmissaoAte_Tipo+fatorSoma).Month, dtAdmissaoAte_Cadastrada.Day);

                    //if (paramDE <= dataAdmissao && paramATE >= dataAdmissao)
                    //{
                    //    try 
                    //    {
                    //        vigencia = new DateTime(
                    //            paramATE.AddMonths(cal.VigenciaTipo).Year,
                    //            paramATE.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);
                    //    }
                    //    catch
                    //    {
                    //        vigencia = new DateTime(
                    //            paramATE.AddMonths(cal.VigenciaTipo).Year,
                    //            paramATE.AddMonths(cal.VigenciaTipo).Month,
                    //            ultimoDiaDoMes(paramATE.AddMonths(cal.VigenciaTipo).Month, paramATE.AddMonths(cal.VigenciaTipo).Year));
                    //    }

                    //    CalendarioVencimento cv = listaVencto[0];
                    //    try 
                    //    {
                    //        vencimento = new DateTime(
                    //            paramATE.AddMonths(cv.VencimentoTipo).Year,
                    //            paramATE.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);
                    //    }
                    //    catch
                    //    {
                    //        vencimento = new DateTime(
                    //            paramATE.AddMonths(cv.VencimentoTipo).Year,
                    //            paramATE.AddMonths(cv.VencimentoTipo).Month,
                    //            ultimoDiaDoMes(paramATE.AddMonths(cv.VencimentoTipo).Month, paramATE.AddMonths(cv.VencimentoTipo).Year));
                    //    }

                    //    diaDataSemJuros = cv.DataSemJuros_Dia;
                    //    valorDataLimite = cv.DataLimite_Valor;

                          continue;//return;
                    //}
                }
            }
        }

        public static void CalculaDatasDeVigenciaEVencimento(Object contratoAdmID, DateTime dataAdmissao, out DateTime vigencia, out DateTime vencimento, out Int32 diaDataSemJuros, out Object valorDataLimite, out CalendarioVencimento rcv, out Int32 limiteAposVencimento, PersistenceManager pm)
        {
            IList<CalendarioAdmissaoVigencia> lista = CarregarPorContrato(contratoAdmID, dataAdmissao, pm);

            vigencia = DateTime.MinValue;
            vencimento = DateTime.MinValue;
            diaDataSemJuros = -1;
            valorDataLimite = null;
            rcv = null;
            limiteAposVencimento = 0;

            if (lista == null) { return; }

            DateTime dtAdmissaoDe_Cadastrada = DateTime.MinValue;
            DateTime dtAdmissaoAte_Cadastrada = DateTime.MinValue;

            Int32 fatorSomaMes = 0;
            foreach (CalendarioAdmissaoVigencia cal in lista)
            {
                if (cal.AdmissaoDe_Tipo == 0 && cal.AdmissaoAte_Tipo == 0) //mes atual
                {
                    if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && dataAdmissao.Day <= cal.AdmissaoAte_Dia)
                    {
                        dtAdmissaoDe_Cadastrada = new DateTime(
                            dataAdmissao.Year, dataAdmissao.Month, cal.AdmissaoDe_Dia);

                        try
                        {
                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.Year, dataAdmissao.Month, cal.AdmissaoAte_Dia);
                        }
                        catch
                        {
                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.Year, dataAdmissao.Month, ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                        }
                    }
                    else
                        continue;
                }
                else
                {
                    #region DE

                    try 
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia);
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, cal.AdmissaoDe_Dia);
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.Year,
                                    dataAdmissao.Month, cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                    }
                    catch
                    {
                        #region tratamento de erro. 

                        #region ano atual

                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                        ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year));
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month,
                                        ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                    dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                    ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year)); //nao tem dia 31, entao tenta 30. Corrigir isso!
                            }
                        }
                        #endregion

                        #region anos anteriores

                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                            {
                                if (dataAdmissao.Day < cal.AdmissaoDe_Dia)
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year,
                                        dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month,
                                        ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoDe_Tipo).Year));
                                }
                                else
                                {
                                    dtAdmissaoDe_Cadastrada = new DateTime(
                                        dataAdmissao.Year,
                                        dataAdmissao.Month,
                                        ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));
                                }
                            }
                            else
                            {
                                dtAdmissaoDe_Cadastrada = new DateTime(
                                    dataAdmissao.Year,
                                    dataAdmissao.Month,
                                    ultimoDiaDoMes(dataAdmissao.Month, dataAdmissao.Year));// cal.AdmissaoDe_Dia); //dia 31
                            }
                        }
                        #endregion

                        #endregion
                    }
                    #endregion
                }

                if (cal.AdmissaoAte_Tipo == 0) //mes atual
                {
                }
                else
                {
                    #region ATE

                    fatorSomaMes = cal.AdmissaoAte_Tipo;
                    if (fatorSomaMes == 0) { fatorSomaMes++; }
                    try 
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 1;
                            else
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Year,
                                dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Month, cal.AdmissaoAte_Dia);
                        }
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, cal.AdmissaoAte_Dia); //dtAdmissaoDe_Cadastrada.AddMonths(cal.AdmissaoAte_Tipo + 1).Month, cal.AdmissaoAte_Dia);
                        }
                    }
                    catch
                    {
                        if (dataAdmissao.Year == DateTime.Now.Year)
                        {
                            //if (cal.AdmissaoDe_Tipo == -1)
                            //    fatorSomaMes = 1;
                            //else
                            //    fatorSomaMes = 0;
                            fatorSomaMes = cal.AdmissaoDe_Tipo;

                            //dtAdmissaoAte_Cadastrada = new DateTime(
                            //    dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Year,
                            //    dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo + fatorSomaMes).Month,
                            //    ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Year)); //cal.AdmissaoAte_Dia - 1

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dataAdmissao.AddMonths(fatorSomaMes).Year,
                                dataAdmissao.AddMonths(fatorSomaMes).Month,
                                ultimoDiaDoMes(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Month, dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo).Year)); //cal.AdmissaoAte_Dia - 1
                        }
                        #region
                        else
                        {
                            if (cal.AdmissaoDe_Tipo == -1)
                                fatorSomaMes = 0;

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, 1);

                            dtAdmissaoAte_Cadastrada = new DateTime(
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Year,
                                dtAdmissaoDe_Cadastrada.AddMonths(fatorSomaMes).Month, ultimoDiaDoMes(dtAdmissaoAte_Cadastrada.Month, dtAdmissaoAte_Cadastrada.Year));
                        }
                        #endregion
                    }
                    #endregion

                }

                IList<CalendarioVencimento> listaVencto = CalendarioVencimento.CarregarTodos(cal.ID);
                if (listaVencto == null || listaVencto.Count == 0) { continue; }


                if (listaVencto.Count == 1)
                {
                    #region arrumar...

                    try 
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cal.VigenciaTipo).Month, dataAdmissao.AddMonths(cal.VigenciaTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }

                    CalendarioVencimento cv = listaVencto[0];
                    try 
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cv.VencimentoTipo).Month, dataAdmissao.AddMonths(cv.VencimentoTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }

                    diaDataSemJuros = cv.DataSemJuros_Dia;
                    valorDataLimite = cv.DataLimite_Valor;
                    limiteAposVencimento = cv.LimiteAposVencimento;

                    return;

                    #endregion
                }
                else if (dtAdmissaoDe_Cadastrada <= dataAdmissao &&
                    dtAdmissaoAte_Cadastrada >= dataAdmissao)
                {
                    try 
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vigencia = new DateTime(
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Year,
                            dataAdmissao.AddMonths(cal.VigenciaTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cal.VigenciaTipo).Month, dataAdmissao.AddMonths(cal.VigenciaTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vigencia = vigencia.AddMonths(1); //
                            }
                        }
                    }

                    CalendarioVencimento cv = listaVencto[0];
                    try 
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }
                    catch
                    {
                        vencimento = new DateTime(
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Year,
                            dataAdmissao.AddMonths(cv.VencimentoTipo).Month,
                            ultimoDiaDoMes(dataAdmissao.AddMonths(cv.VencimentoTipo).Month, dataAdmissao.AddMonths(cv.VencimentoTipo).Year));

                        if (cal.AdmissaoDe_Tipo == -1)
                        {
                            if (dataAdmissao.Day >= cal.AdmissaoDe_Dia && lista.Count == 1) 
                            {
                                vencimento = vencimento.AddMonths(1); //
                            }
                        }
                    }

                    diaDataSemJuros = cv.DataSemJuros_Dia;
                    valorDataLimite = cv.DataLimite_Valor;
                    limiteAposVencimento = cv.LimiteAposVencimento;

                    return;
                }
                else// if(dataAdmissao.Day >= dtAdmissaoDe_Cadastrada.Day && dataAdmissao.Day <= dtAdmissaoAte_Cadastrada.Day)
                {
                    //Int32 fatorSoma = 1; //deve ser a diferenca entre o mes admissao e o mes cadastrado ate
                    //DateTime paramDE  = new DateTime(dataAdmissao.Year, dataAdmissao.Month, dtAdmissaoDe_Cadastrada.Day);
                    //DateTime paramATE = new DateTime(dataAdmissao.AddMonths(cal.AdmissaoAte_Tipo+1).Year, paramDE.AddMonths(cal.AdmissaoAte_Tipo+fatorSoma).Month, dtAdmissaoAte_Cadastrada.Day);

                    //if (paramDE <= dataAdmissao && paramATE >= dataAdmissao)
                    //{
                    //    try 
                    //    {
                    //        vigencia = new DateTime(
                    //            paramATE.AddMonths(cal.VigenciaTipo).Year,
                    //            paramATE.AddMonths(cal.VigenciaTipo).Month, cal.VigenciaDia);
                    //    }
                    //    catch
                    //    {
                    //        vigencia = new DateTime(
                    //            paramATE.AddMonths(cal.VigenciaTipo).Year,
                    //            paramATE.AddMonths(cal.VigenciaTipo).Month,
                    //            ultimoDiaDoMes(paramATE.AddMonths(cal.VigenciaTipo).Month, paramATE.AddMonths(cal.VigenciaTipo).Year));
                    //    }

                    //    CalendarioVencimento cv = listaVencto[0];
                    //    try 
                    //    {
                    //        vencimento = new DateTime(
                    //            paramATE.AddMonths(cv.VencimentoTipo).Year,
                    //            paramATE.AddMonths(cv.VencimentoTipo).Month, cv.VencimentoDia);
                    //    }
                    //    catch
                    //    {
                    //        vencimento = new DateTime(
                    //            paramATE.AddMonths(cv.VencimentoTipo).Year,
                    //            paramATE.AddMonths(cv.VencimentoTipo).Month,
                    //            ultimoDiaDoMes(paramATE.AddMonths(cv.VencimentoTipo).Month, paramATE.AddMonths(cv.VencimentoTipo).Year));
                    //    }

                    //    diaDataSemJuros = cv.DataSemJuros_Dia;
                    //    valorDataLimite = cv.DataLimite_Valor;

                    continue;//return;
                    //}
                }
            }
        }
    }

    [DBTable("calendarioVencimento")]
    public class CalendarioVencimento : EntityBase, IPersisteableEntity
    {
        public enum eDataLimiteTipo : int
        {
            TodoDia = 1,
            TextoPersonalizado
        }

        #region campos 

        Object   _id;
        Object   _calendarioAdmissaoId;
        Int32    _vencimentoDia;
        Int32    _vencimentoTipo;
        Int32    _dataSemJuros_Dia;
        Int32    _dataLimite_Tipo;
        Object   _dataLimite_Valor;
        DateTime _data;
        Int32    _limiteAposVencimento;
        #endregion

        #region propriedades 

        [DBFieldInfo("calendariovencto_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("calendariovencto_calendarioAdmissaoId", FieldType.Single)]
        public Object CalendarioAdmissaoID
        {
            get { return _calendarioAdmissaoId; }
            set { _calendarioAdmissaoId= value; }
        }

        [DBFieldInfo("calendario_vencimentoDia", FieldType.Single)]
        public Int32 VencimentoDia
        {
            get { return _vencimentoDia; }
            set { _vencimentoDia= value; }
        }

        [DBFieldInfo("calendariovencto_vencimentoTipo", FieldType.Single)]
        public Int32 VencimentoTipo
        {
            get { return _vencimentoTipo; }
            set { _vencimentoTipo= value; }
        }

        public String StrVencimentoTipo
        {
            get { return CalendarioHelper.TraduzTipo(_vencimentoTipo, false); }
        }

        [DBFieldInfo("calendariovencto_dataSemJurosDia", FieldType.Single)]
        public Int32 DataSemJuros_Dia
        {
            get { return _dataSemJuros_Dia; }
            set { _dataSemJuros_Dia= value; }
        }

        [DBFieldInfo("calendariovencto_dataLimiteTipo", FieldType.Single)]
        public Int32 DataLimite_Tipo
        {
            get { return _dataLimite_Tipo; }
            set { _dataLimite_Tipo= value; }
        }

        [DBFieldInfo("calendariovencto_dataLimiteValor", FieldType.Single)]
        public Object DataLimite_Valor
        {
            get { return _dataLimite_Valor; }
            set { _dataLimite_Valor= value; }
        }

        public String strDataLimite
        {
            get
            {
                if (_dataLimite_Valor == null) { return "&nbsp;"; }
                if (_dataLimite_Tipo == (Int32)eDataLimiteTipo.TextoPersonalizado)
                    return Convert.ToString(_dataLimite_Valor);
                else
                    return "Todo dia " + Convert.ToString(_dataLimite_Valor);
            }
        }

        [DBFieldInfo("calendariovencto_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        public String strData 
        {
            get
            {
                if (_data == DateTime.MinValue) { return "&nbsp;"; }
                else { return _data.ToString("dd/MM/yyyy"); }
            }
        }

        [DBFieldInfo("calendariovencto_limiteAposVencimento", FieldType.Single)]
        public Int32 LimiteAposVencimento
        {
            get { return _limiteAposVencimento; }
            set { _limiteAposVencimento= value; }
        }

        public String strLimiteAposVencimento
        {
            get
            {
                if (_limiteAposVencimento <= 0) return "nenhum";

                return _limiteAposVencimento.ToString() + " após o vencimento";
            }
        }

        #endregion

        public CalendarioVencimento() { _data = DateTime.Now; }
        public CalendarioVencimento(Object id) : this() { _id = id; }

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

        public static IList<CalendarioVencimento> CarregarTodos(Object calendarioAdmissaoId)
        {
            String qry = "* FROM calendarioVencimento WHERE calendariovencto_calendarioAdmissaoId=" + calendarioAdmissaoId + " ORDER BY calendariovencto_data DESC";

            return LocatorHelper.Instance.ExecuteQuery<CalendarioVencimento>(qry, typeof(CalendarioVencimento));
        }

        public static IList<CalendarioVencimento> CarregarTodos(Object calendarioAdmissaoId, PersistenceManager pm)
        {
            String qry = "* FROM calendarioVencimento WHERE calendariovencto_calendarioAdmissaoId=" + calendarioAdmissaoId + " ORDER BY calendariovencto_data DESC";

            return LocatorHelper.Instance.ExecuteQuery<CalendarioVencimento>(qry, typeof(CalendarioVencimento), pm);
        }
    }
}
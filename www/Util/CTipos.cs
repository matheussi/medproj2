using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace MedProj.www.Util
{
    public class CTipos
    {
        public static T CTipo<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        

        public static decimal ToDecimal(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == string.Empty)
                return decimal.Zero;

            try
            {
                return Convert.ToDecimal(param);
            }
            catch
            {
                return decimal.Zero;
            }
        }

        public static String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }

        public static Int32 CToInt(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(param);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static Nullable<Int32> CToIntNullable(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return null;
            else
                return Convert.ToInt32(param);
        }

        public static Int64 CToLong(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToInt64(param);
        }
        public static Nullable<Int64> CToLongNullable(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return null;
            else
                return Convert.ToInt64(param);
        }

        public static DateTime? ToDateTime(string date, int hora, int minuto, int segundo, int milessegundo)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            string[] arr = date.Split('/');

            if (arr.Length != 3) return null;

            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]),
                    Int32.Parse(arr[1]), Int32.Parse(arr[0]), hora, minuto, segundo, milessegundo);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public static DateTime? CStringToDateTimeG(String strdata)
        {
            String[] arr = strdata.Split('/');
            if (arr.Length != 3) { return null; }

            return CStringToDateTime(strdata);
        }

        public static DateTime CStringToDateTime(String strdata)
        {
            String[] arr = strdata.Split('/');

            if (arr.Length != 3) { return DateTime.MinValue; }

            DateTime resultado = DateTime.MinValue;

            bool ret = DateTime.TryParseExact(strdata, "dd/MM/yyyy",
                new System.Globalization.CultureInfo("pt-Br"),
                System.Globalization.DateTimeStyles.None, out resultado);

            if (ret)
                return resultado;
            else
                return DateTime.MinValue;
        }
        public static DateTime CStringToDateTime(string date, int hora, int minuto, int segundo, int milessegundo)
        {
            if (string.IsNullOrWhiteSpace(date)) return DateTime.MinValue;

            string[] arr = date.Split('/');

            if (arr.Length != 3) return DateTime.MinValue;

            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]),
                    Int32.Parse(arr[1]), Int32.Parse(arr[0]), hora, minuto, segundo, milessegundo);
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime CObjectToDateTime(object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value) return DateTime.MinValue;
            else
            {
                try
                {
                    return Convert.ToDateTime(param, cinfo);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
    }
}
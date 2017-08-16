namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    public abstract class EntityBase
    {
        protected EntityBase() { }

        protected static readonly String DateFormat     = "yyyy-MM-dd";
        protected static readonly String DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        protected void Salvar(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Save(entity);
            pm = null;
        }

        public static DateTime ToDateTime(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
            {
                return DateTime.MinValue;
            }
            else
            {
                return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
            }
        }

        public static String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }

        public static Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value)
                return Decimal.Zero;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        protected void Remover(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Remove(entity);
            pm = null;
        }

        protected void Carregar(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Load(entity);
            pm = null;
        }

        protected String FormataTelefone(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return fone; }

            fone = fone.Replace("(", "").Replace(")", "").Replace(" ", ""); ;

            try
            {
                if (fone.Length == 10)
                {
                    return String.Format("{0:(##) ####-####}", Convert.ToDouble(fone));
                }
                else if (fone.Length == 8)
                {
                    return String.Format("{0: ####-####}", Convert.ToDouble(fone));
                }
                else
                    return fone;
            }
            catch
            {
                return String.Empty;
            }
        }

        protected String ToLower(String param)
        {
            if (param == null)
                return null;
            else
                return param.ToLower();
        }

        public static String GeraNumeroDeContrato(Int32 numero, Int32 qtdZerosEsquerda, String letra)
        {
            String _numero = Convert.ToString(numero);

            if (qtdZerosEsquerda > 0)
            {
                String mascara = new String('0', qtdZerosEsquerda);
                _numero = String.Format("{0:" + mascara + "}", numero);
            }

            if (!String.IsNullOrEmpty(letra))
                _numero = letra + _numero;

            return _numero;
        }

        public static String PrimeiraPosicaoELetra(String param)
        {
            if (String.IsNullOrEmpty(param)) { return ""; }

            String pos1 = param.Substring(0, 1);

            if (pos1 != "0" && pos1 != "1" && pos1 != "2" && pos1 != "3" && pos1 != "4" &&
                pos1 != "5" && pos1 != "6" && pos1 != "7" && pos1 != "8" && pos1 != "9")
            {
                return param.Substring(0, 1);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Método para Retirar a acentuação de um texto.
        /// </summary>
        /// <param name="Texto">Texto a ser modificado.</param>
        /// <returns>Texto sem acentuação.</returns>
        public static String RetiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }

        #region AppendPreparedField

        /// <summary>
        /// Inclui o Valor de um Campo de acordo com o seu tamanho. 
        /// </summary>
        /// <param name="SB">StringBuilder com as informações.</param>
        /// <param name="Value">Valor a ser Incluído.</param>
        /// <param name="ValueLength">Tamanho máximo do Valor a ser Incluído.</param>
        /// <returns>True se conseguiu incluir e False se não conseguir incluir.</returns>
        internal static Boolean AppendPreparedField(ref StringBuilder SB, Object Value, Int32 ValueLength)
        {
            if (SB != null && Value != null)
            {
                Value = EntityBase.RetiraAcentos(Value.ToString());

                if (Value.ToString().Length > ValueLength)
                    SB.Append(Value.ToString().Substring(0, ValueLength));
                else
                    SB.Append(Value.ToString().PadRight(ValueLength, ' '));

                return true;
            }

            return false;
        }

        #endregion

        internal static String Join(IList<String> list, String separator)
        {
            if (list == null) { return null; }

            StringBuilder sb = new StringBuilder();
            foreach (String item in list)
            {
                if (sb.Length > 0) { sb.Append(separator); }
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}

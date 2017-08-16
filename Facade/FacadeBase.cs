namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using NHibernate.Dialect;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using FluentNHibernate.Conventions.Helpers;

    using MedProj.Entidades.Map.NHibernate;
    using MedProj.Entidades;

    /// <summary>
    /// Classe base para os facades.
    /// </summary>
    public class FacadeBase
    {
        //--------------- NHibernate --------------//

        //protected void Excluir<T>(T objeto)
        //{
        //    using (var sessao = ObterSessao())
        //    {
        //        using (ITransaction tran = sessao.BeginTransaction())
        //        {
        //            try
        //            {
        //                T obj = sessao.Query<T>().Where(t => t.ID == id).Single();

        //                sessao.Delete(obj);
        //                tran.Commit();
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

        private static NHibernate.Cfg.Configuration _config = null;
        private static Hashtable _allFactories = new Hashtable();

        protected static ISession ObterSessao()
        {
            string chave = "1";
            ISessionFactory _sessionFactory = null;

            if (!_allFactories.ContainsKey(chave))
            {
                _sessionFactory = CriarSessaoNHibernate();
                _allFactories.Add(chave, _sessionFactory);
            }
            else
            {
                _sessionFactory = (ISessionFactory)_allFactories[chave];
            }

            ISession sessao = _sessionFactory.OpenSession();
            sessao.FlushMode = FlushMode.Commit;

            return sessao;
        }

        static ISessionFactory CriarSessaoNHibernate()
        {
            string connString = ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString;

            var f = Fluently.Configure().Database((MsSqlConfiguration.MsSql2008.Dialect<MsSql2008Dialect>().ConnectionString(connString).ShowSql()))
                    .ExposeConfiguration(p => p.Properties["command_timeout"] = "1000") //timeout
                    .ExposeConfiguration(p => p.Properties["hibernate.cache.use_query_cache"] = "false") //sem cache 
                    .ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>().Conventions.Setup(x => x.Add(AutoImport.Never())));

            _config = f.BuildConfiguration();
            return f.BuildSessionFactory();
        }

        protected DateTime CToDateTime(String param6pos, Int32 hora, Int32 minunto, Int32 segundo, Boolean ddMMyy)
        {
            Int32 dia;
            Int32 mes;
            Int32 ano;

            if (ddMMyy)
            {
                dia = Convert.ToInt32(param6pos.Substring(0, 2));
                mes = Convert.ToInt32(param6pos.Substring(2, 2));
                ano = Convert.ToInt32(param6pos.Substring(4, 2));
            }
            else
            {
                ano = Convert.ToInt32(param6pos.Substring(0, 2));
                mes = Convert.ToInt32(param6pos.Substring(2, 2));
                dia = Convert.ToInt32(param6pos.Substring(4, 2));
            }

            if (ano >= 0 && ano <= 95)
                ano = Convert.ToInt32("20" + ano.ToString());
            else
                ano = Convert.ToInt32("19" + ano.ToString());

            DateTime data = new DateTime(ano, mes, dia, hora, minunto, segundo);
            return data;
        }

        protected DateTime CToDateTime10(String param10pos, Int32 hora, Int32 minunto, Int32 segundo, Boolean ddMMyy)
        {
            Int32 dia;
            Int32 mes;
            Int32 ano;

            if (ddMMyy)
            {
                dia = Convert.ToInt32(param10pos.Substring(0, 2));
                mes = Convert.ToInt32(param10pos.Substring(2, 2));
                ano = Convert.ToInt32(param10pos.Substring(4, 4));
            }
            else
            {
                ano = Convert.ToInt32(param10pos.Substring(0, 2));
                mes = Convert.ToInt32(param10pos.Substring(2, 2));
                dia = Convert.ToInt32(param10pos.Substring(4, 4));
            }

            //if (ano >= 0 && ano <= 95)
            //    ano = Convert.ToInt32("20" + ano.ToString());
            //else
            //    ano = Convert.ToInt32("19" + ano.ToString());

            DateTime data = new DateTime(ano, mes, dia, hora, minunto, segundo);
            return data;
        }

        protected string CToString(object param)
        {
            if (param == null || param == DBNull.Value)
                return "";
            else
                return Convert.ToString(param);
        }

        protected bool CToBool(object param)
        {
            if (param == null || param == DBNull.Value)
                return false;

            if (Convert.ToString("param") == "0") return false;
            if (Convert.ToString("param") == "1") return true;

            try
            {
                return Convert.ToBoolean(param);
            }
            catch
            {
                return false;
            }
        }

        protected String RetiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }
    }

    internal class SqlStatementInterceptor : EmptyInterceptor
    {
        public static bool LogSQLOn;
        public static string LogSQL;

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            if (SqlStatementInterceptor.LogSQLOn)
                SqlStatementInterceptor.LogSQL += "\n" + sql;

            return sql;
        }
    }

    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }
}

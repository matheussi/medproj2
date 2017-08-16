namespace MedProj.www.orientador
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Data.OleDb;
    using System.Web.Security;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;

    using NHibernate;
    using NHibernate.Dialect;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using LC.Web.PadraoSeguros.Facade;
    using MedProj.Entidades.Map.NHibernate;
    using FluentNHibernate.Conventions.Helpers;

    public partial class _default : System.Web.UI.Page
    {
        private static NHibernate.Cfg.Configuration _config = null;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregaEstados();
                this.carregaCidades();
                this.carregaBairros();
                this.carregaEspecialidades();
            }
        }

        void carregaEspecialidades()
        {
            IList<MedProj.Entidades.Especialidade> lista = EspecialidadeFacade.Instance.Carregar(string.Empty);

            cboEspecialidade.Items.Clear();

            if (lista != null)
            {
                foreach (var espec in lista)
                {
                    cboEspecialidade.Items.Add(new ListItem(espec.Nome, espec.ID.ToString()));
                }
            }

            //using (var sessaoF = CriarSessaoNHibernate())
            //{
            //    using (var sessao = sessaoF.OpenSession())
            //    {
            //        using (IDbCommand cmd = sessao.Connection.CreateCommand())
            //        {
            //            cmd.CommandText = "select distinct(Especialidade) as especialidade from procedimento where Especialidade <> '' and Especialidade is not null order by Especialidade";

            //            cboEspecialidade.Items.Clear();

            //            using (IDataReader dr = cmd.ExecuteReader())
            //            {
            //                while (dr.Read())
            //                {
            //                    cboEspecialidade.Items.Add(new ListItem(dr.GetString(0).ToUpper(), dr.GetString(0)));
            //                }

            //                dr.Close();
            //            }
            //        }
            //    }
            //}
        }

        void carregaEstados()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(UF) as uf from prestador_unidade where UF <> '' and UF is not null order by UF";

                        cboUf.Items.Clear();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboUf.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }

        void carregaCidades()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(Cidade) as cid from prestador_unidade where UF='" + cboUf.SelectedValue + "' and Cidade <> '' and Cidade is not null order by Cidade";

                        cboCidade.Items.Clear();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboCidade.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }

        void carregaBairros()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(Bairro) from prestador_unidade where UF='" + cboUf.SelectedValue + "' and Cidade='" + cboCidade.SelectedValue + "' and Bairro <> '' and Bairro is not null order by Bairro";

                        cboBairro.Items.Clear();
                        cboBairro.Items.Add("todos");

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboBairro.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }

        protected void cboUf_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaCidades();
            this.carregaBairros();
        }

        protected void cboCidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaBairros();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Prestador");
            dt.Columns.Add("Telefone");
            dt.Columns.Add("Endereco");
            dt.Columns.Add("Bairro");
            dt.Columns.Add("Cidade");
            dt.Columns.Add("UF");

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = string.Concat(
                            "select pu.* from prestador_unidade pu ",
                            "   inner join unidade_especialidade ue on pu.ID = ue.Unidade_ID ",
                            "   inner join prestador pre on pre.ID = pu.Owner_ID ",                  //adicionado
                            "where ",
                            "   ue.Especialidade_ID=", cboEspecialidade.SelectedValue,
                            "   AND (pre.Deletado = 0 or pre.Deletado is null) ",                   //adicionado
                            "   AND pu.UF = '", cboUf.SelectedValue, "'",
                            "   AND pu.Cidade = '", cboCidade.SelectedValue, "'");

                        if (cboBairro.SelectedIndex > 0)
                        {
                            cmd.CommandText += string.Concat(" and pu.Bairro='", cboBairro.SelectedValue, "'");
                        }

                        cmd.CommandText += " order by pu.Nome ";

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DataRow row = dt.NewRow();

                                row["Prestador"] = dr["Nome"];
                                row["Telefone"] = dr["Telefone"];

                                if (dr["Numero"] != null && dr["Numero"] != DBNull.Value && Convert.ToString(dr["Numero"]).Trim() != "")
                                    row["Endereco"] = string.Concat(dr["Endereco"], ", ", dr["Numero"]);
                                else
                                    row["Endereco"] = dr["Endereco"];

                                row["Bairro"] = dr["Bairro"];
                                row["Cidade"] = dr["Cidade"];
                                row["UF"] = dr["UF"];

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            #region comentado 
            //using (var sessaoF = CriarSessaoNHibernate())
            //{
            //    using (var sessao = sessaoF.OpenSession())
            //    {
            //        using (IDbCommand cmd = sessao.Connection.CreateCommand())
            //        {
            //            cmd.CommandText = string.Concat(
            //                "select pu.* from prestador_unidade pu ",
            //                "   inner join unidade_procedimento up on pu.ID = up.Unidade_ID ",
            //                "   inner join procedimento p on up.Procedimento_ID = p.ID ",
            //                "where ",
            //                "   p.Especialidade='", cboEspecialidade.SelectedValue, "'",
            //                "   AND pu.UF = '", cboUf.SelectedValue, "'",
            //                "   AND pu.Cidade = '", cboCidade.SelectedValue, "'");

            //            if (cboBairro.SelectedIndex > 0)
            //            {
            //                cmd.CommandText += string.Concat(" and pu.Bairro='", cboBairro.SelectedValue, "'");
            //            }

            //            cmd.CommandText += " order by pu.Nome ";

            //            using (IDataReader dr = cmd.ExecuteReader())
            //            {
            //                while (dr.Read())
            //                {
            //                    DataRow row = dt.NewRow();

            //                    row["Prestador"] = dr["Nome"];
            //                    row["Telefone"] = dr["Telefone"];

            //                    if(dr["Numero"] != null && dr["Numero"] != DBNull.Value && Convert.ToString(dr["Numero"]).Trim() != "")
            //                        row["Endereco"] = string.Concat(dr["Endereco"], ", ", dr["Numero"]);
            //                    else
            //                        row["Endereco"] = dr["Endereco"];

            //                    row["Bairro"] = dr["Bairro"];
            //                    row["Cidade"] = dr["Cidade"];
            //                    row["UF"] = dr["UF"];

            //                    dt.Rows.Add(row);
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            if (dt.Rows.Count > 0)
            {
                pnlResult.Visible   = true;
                pnlNoResult.Visible = false;
            }
            else
            {
                pnlResult.Visible   = false;
                pnlNoResult.Visible = true;
            }

            grid.DataSource = dt;
            grid.DataBind();
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
    }

    internal class SqlStatementInterceptor : EmptyInterceptor
    {
        public static bool LogSQLOn = false;
        public static string LogSQL;

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            if (SqlStatementInterceptor.LogSQLOn)
                SqlStatementInterceptor.LogSQL += "\n" + sql;

            return sql;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LC.Framework.Phantom;
using LC.Web.PadraoSeguros.Facade;
using LC.Web.PadraoSeguros.Entity;
using System.Collections;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace MedProj.www
{
    public partial class invoice_notfication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.processa();
            }

        }

        void processa()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "insert into iugu_retorno (url) values (@valor)";
                cmd.Parameters.AddWithValue("@valor", Request.Url.ToString());
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
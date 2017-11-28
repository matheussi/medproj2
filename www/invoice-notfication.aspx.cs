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
            //if (!IsPostBack)
            //{
                this.processa();
            //}

        }

        void processa()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString))
            {
                /*
                 event=invoice.created&
                 data[id]=D504EEBD8D77470EA8172DF9E83B21E1&
                 data[status]=pending&
                 data[account_id]=3195951A25A24FEFB85F103BB11591F7&
                 data[subscription_id]=14740BE6F8644E53A6224DFC467CB970&
                 param=pvys67uYerM
                 * 
                 * 
                 event=invoice.status_changed&
                 data[id]=D504EEBD8D77470EA8172DF9E83B21E1&
                 data[status]=paid&
                 data[account_id]=3195951A25A24FEFB85F103BB11591F7&
                 data[subscription_id]=14740BE6F8644E53A6224DFC467CB970&
                 param=pvys67uYerM
                 */

                string collec = "";
                foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                {
                    if (collec.Length > 0) collec += "&";
                    collec += key + "=" + HttpContext.Current.Request.Form[key];
                }

                //collec += "id=" + new Util.Crypto.SecureQueryString().Decrypt(HttpContext.Current.Request.Form["param"]);

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "insert into iugu_retorno (url) values (@valor)";
                cmd.Parameters.AddWithValue("@valor", Request.Url.ToString() + "?" + collec);
                //cmd.Parameters.AddWithValue("@valor", Request.Url.ToString() + "&id=" + id + "&status=" + status);
                cmd.ExecuteNonQuery();

                conn.Close();
            }


            string evnt = Request.Form["event"];
            if (evnt != "invoice.status_changed") return;

            string status = Request.Form["data[status]"];
            if (status != "paid") return;

            string crpto = Request.Form["param"];
            if (string.IsNullOrEmpty(crpto)) return;

            string id = Request.Form["data[id]"];

            Cobranca cobranca = Cobranca.CarregarPorIuguId(id, null); // new Cobranca(boletoId);
            //if (cobranca.Iugu_Id.ToUpper() == id.ToUpper())
            //{
                cobranca.Pago = true;

                cobranca.DataPgto = DateTime.Now;
                cobranca.ValorPgto = cobranca.Valor;
                cobranca.Salvar();

                using (iugu_srv.iugu_interop proxy = new iugu_srv.iugu_interop())
                {
                    try
                    {
                        string msg = "", token = ConfigurationManager.AppSettings["ws_token"];

                        var ret = proxy.ObterCobranca_DataPagto(id, token, out msg);

                        if (ret != null)
                        {
                            cobranca.DataPgto =  Convert.ToDateTime(ret, new System.Globalization.CultureInfo("pt-Br"));
                            cobranca.Salvar();
                        }
                    }
                    catch
                    {
                    }
                }
            //}
        }
    }
}
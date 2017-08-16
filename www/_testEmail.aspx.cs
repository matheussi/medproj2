using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MedProj.www
{
    public partial class _testEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void cmdEnviar_Click(object sender, EventArgs e)
        {
            SmtpClient client = new SmtpClient(txtSmtp.Text);

            if (txtLogin.Text != "")
            {
                client.Credentials = new System.Net.NetworkCredential(txtLogin.Text, txtSenha.Text);
            }

            MailMessage msg = new MailMessage();

            MailAddress from = new MailAddress(txtFrom.Text, "Clube Azul");
            msg.From = from;

            msg.To.Add(txtTo.Text);
            msg.Subject = "teste";
            msg.Body = "corpo";

            if (txtPorta.Text != "")
            {
                client.Port = Convert.ToInt32(txtPorta.Text);
            }

            msg.IsBodyHtml = true;

            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<hr>" + ex.StackTrace + "<hr>" + ex.Source + "<hr>");

                if (ex.InnerException != null)
                {
                    Response.Write(ex.InnerException.Message + "<hr>" + ex.InnerException.StackTrace + "<hr>" + ex.InnerException.Source);
                }
            }
            finally
            {
                msg.Dispose();
                client.Dispose();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MailSrv35
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public bool Enviar(string assunto, string corpo, string para, bool html, string erro, string token)
        {
            erro = "";

            if (token != "123456789/") return false;

            SmtpClient client = new SmtpClient("");
            MailAddress from = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["mailFrom"], "Clube Azul");
            MailMessage msg = new MailMessage();

            msg.From = from;
            msg.To.Add(para);
            msg.Subject = assunto;
            msg.Body = corpo;

            msg.IsBodyHtml = html;

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }
    }
}

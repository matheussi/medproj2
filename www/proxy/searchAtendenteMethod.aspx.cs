using LC.Framework.Phantom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MedProj.www.proxy
{
    public partial class searchAtendenteMethod : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessRequest(Request["name_startsWith"]);
            }
        }

        public void ProcessRequest(String param)
        {
            if (string.IsNullOrEmpty(param)) return;

            String qry = "select distinct(atendimento_cadastrado) as Atendente from _atendimento where atendimento_cadastrado like '%" + param + "%'";
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("{ \"Atendentes\" : [");

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) { sb.Append(" , "); }
                    sb.Append(" { \"Atendente\" : \""); sb.Append(dt.Rows[i][0]); sb.Append("\" } ");
                }
            }

            sb.Append(" ] } ");
            Response.Clear();
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "application/json";
            Response.Write(sb.ToString());
            Response.End();
        }
    }
}
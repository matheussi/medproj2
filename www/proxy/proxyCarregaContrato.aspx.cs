namespace MedProj.www.proxy
{
    using System;
    using System.Web;
    using System.Text;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using Ent = MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using Enty = LC.Web.PadraoSeguros.Entity;
    using System.Data.SqlClient;

    public partial class proxyCarregaContrato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.processaRequisicao(Request["name_startsWith"]);
            }
        }

        void processaRequisicao(string param)
        {
            
            //var lista = Enty.Contrato.CarregarPorParametros(param);
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            IList<Enty.Contrato> lista = new List<Enty.Contrato>();
            //lista.Add(new Enty.Contrato { ID = 1, Numero = "adfad", BeneficiarioTitularNome = "Titular" });
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = String.Concat("select top 8 contrato_id,contrato_numero, beneficiario_nome ",
                "FROM contrato ",
                " inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_tipoPessoa=1 and beneficiario_nome LIKE '%", param, "%'  or contrato_numero like '%", param, "%'",
                " ORDER BY beneficiario_nome ");

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add
                        (
                            new Enty.Contrato 
                            { 
                                ID = Convert.ToString(dr["contrato_id"]),
                                Numero = Convert.ToString(dr["contrato_numero"]),
                                BeneficiarioTitularNome = Convert.ToString(dr["beneficiario_nome"])
                            }
                        );
                    }
                }
            }

            sb.Append("{ \"Contratos\" : [");

            if (lista != null)
            {
                int i = 0;

                foreach (Enty.Contrato c in lista)
                {
                    //solicitacao
                    //if (valor == decimal.Zero) continue;

                    if (i > 0) sb.Append(" , ");

                    sb.Append(" { \"ID\" : \""); sb.Append(c.ID); sb.Append("\", ");
                    //sb.Append("\"Numero\" : \""); sb.Append(c.Numero); sb.Append("\", ");
                    //sb.Append("\"Titular\" : \""); sb.Append(EntityBase.RetiraAcentos(proc.Procedimento.Especialidade)); sb.Append("\", ");
                    sb.Append("\"Titular\" : \""); sb.Append(EntityBase.RetiraAcentos(c.BeneficiarioTitularNome)); sb.Append("\" } ");

                    i++;
                }
            }

            sb.Append(" ] } ");

            Response.Clear();
            Response.ContentEncoding = System.Text.Encoding.UTF8; //.GetEncoding("ISO-8859-1");
            Response.ContentType = "application/json";
            Response.Write(sb.ToString());
            Response.End();
        }
    }
}
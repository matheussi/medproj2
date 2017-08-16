using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.www.Util;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.especialidades
{
    public partial class especialidade : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
                {
                    this.carregarEspecialidade();
                }
            }
        }

        void carregarEspecialidade()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            Especialidade obj = EspecialidadeFacade.Instance.Carregar(id);

            txtEspecialidade.Text = obj.Nome;
            txtDescricao.Text = obj.Descricao;

            //using (Contexto contexto = new Contexto())
            //{
            //    long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            //    Especialidade obj = contexto.Especialidades.Where(e => e.ID == id).Single();

            //    txtEspecialidade.Text = obj.Nome;
            //    txtDescricao.Text = obj.Descricao;
            //}
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("especialidades.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtEspecialidade.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome da especialidade.");
                txtEspecialidade.Focus();
                return;
            }

            Especialidade obj = new Especialidade();

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            if (id != 0)
            {
                obj.ID = id;
            }

            obj.Descricao = txtDescricao.Text;
            obj.Nome = txtEspecialidade.Text;

            EspecialidadeFacade.Instance.Salvar(obj);

            //using (Contexto contexto = new Contexto())
            //{
            //    if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
            //    {
            //        long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            //        obj = contexto.Especialidades.Where(es => es.ID == id).Single();
            //        obj.Descricao = txtDescricao.Text;
            //        obj.Nome = txtEspecialidade.Text;
            //    }
            //    else
            //    {
            //        obj = new Especialidade();
            //        obj.Descricao = txtDescricao.Text;
            //        obj.Nome = txtEspecialidade.Text;

            //        contexto.Especialidades.Add(obj);
            //    }

            //    contexto.SaveChanges();
            //}

            Response.Redirect("especialidades.aspx");
        }
    }
}
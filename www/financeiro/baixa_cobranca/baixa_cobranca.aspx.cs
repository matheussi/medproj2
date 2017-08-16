using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.Entidades;
using MedProj.Entidades.Enuns;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.financeiro
{
    public partial class baixa_cobranca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDataDe.Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                txtDataAte.Text = DateTime.Now.ToString("dd/MM/yyyy");

                this.carregarArquivos();
            }
        }

        void carregarArquivos()
        {
            bool processados = cboFiltro.SelectedValue == "1" ? true : false;

            DateTime de  = Util.CTipos.CStringToDateTime(txtDataDe.Text, 0, 0, 0, 10);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAte.Text, 23, 59, 59, 998);

            if(de == DateTime.MinValue || ate == DateTime.MinValue) return;

            grid.DataSource = ArquivoBaixaFacade.Instance.Carregar(de, ate, processados);
            grid.DataBind();
        }

        protected void opt_CheckedChanged(object sender, EventArgs e)
        {
            if (optEnviar.Checked)
            {
                pnlEnviar.Visible = true;
                pnlVisualizar.Visible = false;
            }
            else
            {
                pnlEnviar.Visible = false;
                pnlVisualizar.Visible = true;
            }
        }

        protected void cmdVisualizar_Click(object sender, EventArgs e)
        {
            this.carregarArquivos();
        }

        protected void cmdEnviar_Click(object sender, EventArgs e)
        {
            if (cboTipoArquivo.SelectedIndex <= 0)
            {
                Util.Geral.Alerta(this, "Selecione o tipo de arquivo.");
                cboTipoArquivo.Focus();
                return;
            }

            if (fuEnviar.PostedFile != null && !string.IsNullOrEmpty(fuEnviar.PostedFile.FileName))
            {
                string repPath = Server.MapPath("~/files/arqretorno") + "/";

                string arqNome = Path.GetFileName(fuEnviar.PostedFile.FileName);
                string arqDestino = string.Concat(repPath, arqNome);

                if (File.Exists(arqDestino)) File.Delete(arqDestino);

                fuEnviar.PostedFile.SaveAs(arqDestino);

                String conteudo = "";
                using (StreamReader stream = new StreamReader(arqDestino))
                {
                    conteudo = stream.ReadToEnd();
                    stream.Close();
                }

                ArquivoBaixa ab = new ArquivoBaixa();
                ab.Corpo = conteudo;
                ab.Nome = arqNome;
                ab.Tipo = (TipoArquivoBaixa)Enum.Parse(typeof(TipoArquivoBaixa), cboTipoArquivo.SelectedValue);

                try
                {
                    ArquivoBaixaFacade.Instance.Salvar(ab);
                    this.carregarArquivos();
                    Util.Geral.Alerta(this, "Dados salvos com sucesso.");
                }
                catch
                {
                    Util.Geral.Alerta(this, "Erro inesperado.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ArquivoBaixa arq = e.Row.DataItem as ArquivoBaixa;
                ImageButton imgBtn = e.Row.Cells[3].FindControl("imgBtn") as ImageButton;

                if (arq.Processamento.HasValue)
                {
                    imgBtn.ImageUrl = "~/images/tick.png";
                    imgBtn.ToolTip = "visualizar cobranças baixadas";
                    imgBtn.CommandName = "Ver";
                }
                else
                {
                    imgBtn.ImageUrl = "~/images/seta_baixa.png";
                    imgBtn.ToolTip = "baixar cobranças";
                    imgBtn.OnClientClick = "return confirm('Deseja iniciar a baixa de pagamentos?');";
                    imgBtn.CommandName = "Baixar";
                }


            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

            if (e.CommandName.Equals("Baixar"))
            {
                bool ok = ArquivoBaixaFacade.Instance.ProcessarBaixa(id);

                if (ok)
                {
                    this.carregarArquivos();
                    Util.Geral.Alerta(this, "Processamento realizado com sucesso.");
                }
                else
                {
                    Util.Geral.Alerta(this, "Erro ao processar arquivo.");
                }
            }
            else if (e.CommandName.Equals("Ver"))
            {
                List<ArquivoBaixaItem> lista = ArquivoBaixaFacade.Instance.CarregarItens(id);
                gridBaixaItens.DataSource = lista;
                gridBaixaItens.DataBind();

                if (lista == null || lista.Count == 0) litBaixasTotais.Text = "";
                else
                {
                    int baixado = 0; decimal valor = 0;
                    foreach (var item in lista)
                    {
                        if (item.Status == TipoItemBaixa.DepositoBaixado || item.Status == TipoItemBaixa.TituloLiquidado)
                        {
                            baixado++;
                            valor += item.Cobranca.ValorPago;
                        }
                    }

                    litBaixasTotais.Text = string.Concat("Títulos: ", lista.Count.ToString(), 
                        " - Liquidados: ", baixado.ToString(), " (R$ ", valor.ToString("N2"), ")");
                }

                Util.Geral.JSScript(this, "showModal()");
            }
        }

        protected void gridBaixaItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ArquivoBaixaItem item = e.Row.DataItem as ArquivoBaixaItem;
                Literal lit = e.Row.Cells[2].FindControl("litStatus") as Literal;

                switch (item.Status)
                {
                    #region 
                    case TipoItemBaixa.TituloCanceladoLiquidado:
                    {
                        lit.Text = "Título de contrato cancelado liquidado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Orange;
                        break;
                    }
                    case TipoItemBaixa.TituloLiquidado:
                    {
                        lit.Text = "Título liquidado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Green;
                        break;
                    }
                    case TipoItemBaixa.TituloNaoEncontrado:
                    {
                        lit.Text = "Título não encontrado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                        break;
                    }
                    case TipoItemBaixa.DepositoBaixado:
                    {
                        lit.Text = "Depósito localizado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Green;
                        break;
                    }
                    case TipoItemBaixa.DepositoBaixadoContratoCancelado:
                    {
                        lit.Text = "Depósito de contrato cancelado localizado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Orange;
                        break;
                    }
                    case TipoItemBaixa.DepositoNaoIdentificado:
                    {
                        lit.Text = "Depósito não localizado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                        break;
                    }
                    case TipoItemBaixa.PagamentoDuplicado:
                    {
                        lit.Text = "Pagamento duplicado";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Orange;
                        break;
                    }
                    case TipoItemBaixa.OcorrenciaSemLiquidacao:
                    {
                        lit.Text = "Confirmação de título";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Blue;
                        break;
                    }
                    case TipoItemBaixa.AlteracaoRejeitada:
                    {
                        lit.Text = "Alteração de título rejeitada";
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                        break;
                    }
                    #endregion
                }

                if (item.Cobranca != null)
                {
                    lit = e.Row.Cells[4].FindControl("litValorPago") as Literal;
                    lit.Text = item.Cobranca.ValorPago.ToString("N2");

                    lit = e.Row.Cells[5].FindControl("litDataPago") as Literal;

                    if(item.Cobranca.DataPagamento.HasValue)
                        lit.Text = item.Cobranca.DataPagamento.Value.ToString("dd/MM/yyyy");

                    //lit = e.Row.Cells[5].FindControl("litTitular") as Literal;
                    //lit.Text = item.
                }
                else if (item.Contrato != null)
                {
                    lit = e.Row.Cells[4].FindControl("litValorPago") as Literal;
                    lit.Text = item.ValorPago.ToString("N2");

                    lit = e.Row.Cells[5].FindControl("litDataPago") as Literal;
                    lit.Text = item.DataRemessa.Value.ToString("dd/MM/yyyy");
                }
            }
        }
    }
}
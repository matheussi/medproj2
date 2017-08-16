<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cnab.aspx.cs" Inherits="MedProj.www.financeiro.cnab.cnab" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Movimentações CNAB
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">


        <ContentTemplate>

            <div class="panel panel-default">
                <%--<div class="panel-heading text-center" style="position:relative;">--%>
                <div class="panel-heading">
                    <div class="row" style="margin-left:10px">
                        <asp:RadioButton ID="optNovos" Text="Novos registros de cobrança" runat="server" GroupName="b" Checked="true" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                        &nbsp;
                        <asp:RadioButton ID="optEdicao" Text="Alterações de cobranças" runat="server" GroupName="b" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                    </div>

                    <div class="row" style="margin-left:10px">
                        <asp:RadioButton ID="optPendencias" Text="Cobranças ainda não registradas" runat="server" GroupName="a" Checked="true" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                        &nbsp;
                        <asp:RadioButton ID="optArquivosCNAB" Text="Arquivos padrão CNAB ja gerados" runat="server" GroupName="a" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                    </div>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <asp:Panel ID="pnlPendencias" runat="server">
                            <div class="form-group">
                                <label class="col-xs-1 control-label" style="padding-top:7px">Filtro</label>
                                <div class="col-xs-3" style="padding-top:7px">
                                    <asp:DropDownList ID="cboTipoFiltroPendencias" SkinID="comboPadrao1" runat="server" Width="100%">
                                        <asp:ListItem Text="Cobranças geradas HOJE" Value="0" Selected="True" />
                                        <asp:ListItem Text="Cobranças geradas ONTEM" Value="1" />
                                        <asp:ListItem Text="Período informado ao lado" Value="2" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2"  style="padding-left:0px;padding-top:7px">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                        de
                                        </span>
                                        <asp:TextBox ID="txtDataDePendentes" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                    </div>
                                </div>
                                
                                <div class="col-md-2" style="padding-left:0px;padding-top:7px">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                        até
                                        </span>
                                        <asp:TextBox ID="txtDataAtePendentes" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                    </div>
                                </div>

                                <div class="col-md-3" style="padding-left:0px;padding-top:4px">
                                    <asp:Button ID="cmdPesquisarPendentes" Text="pesquisar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdPesquisarPendentes_click" />
                                </div>

                            </div>


                            <div class="form-group">
                                <div class="col-md-12" style="text-align:center">
                                    <asp:Literal ID="litPendenciasMsg" runat="server" EnableViewState="false" />
                                    <asp:GridView HorizontalAlign="Center" ID="grid" Width="99%" SkinID="gridPadrao" runat="server" OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand" DataKeyNames="ID">
                                        <Columns>
                                            <asp:TemplateField ItemStyle-Width="1%">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSel" Text="" runat="server" Checked="true" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Nome">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "Contrato.ContratoBeneficiario.Beneficiario.Nome") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="DataCriacao" HeaderText="Criação" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ValorCobrado" HeaderText="Valor" DataFormatString="{0:N2}">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <%--<asp:TemplateField HeaderText="Processado">
                                                <ItemTemplate>
                                                    <%# Eval("Processamento") == null ? "" : Convert.ToDateTime(Eval("Processamento")).ToString("dd/MM/yyyy HH:mm") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>

                                            <%--<asp:TemplateField ItemStyle-Width="1%">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgBtn" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="Baixar" />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:Button ID="cmdGerarCnab" Visible="false" Text="Gerar arquivo CNAB" OnClick="cmdGerarCnab_Click" SkinID="botaoPadraoWarning" OnClientClick="return confirm('Deseja realmente gerar o arquivo de remessa com as cobranças selecionadas?');" Width="350px" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                        <!---------------------------------------------------------------------------------------------------->
                        <asp:Panel ID="pnlArquivosCNAB" runat="server" Visible="true">
                            <div class="col-md-12">
                                <div class="row">
                                    <label class="col-md-12 text-left">Informe o período:</label>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="col-md-2"  style="padding-left:0px;">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                de
                                                </span>
                                                <asp:TextBox ID="txtDataDe" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                            </div>
                                        </div>
                                        <div class="col-md-2" style="padding-left:0px;">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                até
                                                </span>
                                                <asp:TextBox ID="txtDataAte" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                            </div>
                                        </div>
                                        <div class="col-md-6" style="padding-left:0px;">
                                            <asp:Button ID="cmdVisualizar" Text="pesquisar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdVisualizar_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12" style="text-align:center">
                                        <asp:Literal ID="litCNABsMsg" runat="server" EnableViewState="false" />
                                        <asp:GridView HorizontalAlign="Center" ID="gridCnabs" Width="99%" SkinID="gridPadrao" runat="server" OnRowCommand="gridCnabs_RowCommand" OnRowDataBound="gridCnabs_RowDataBound" DataKeyNames="ID">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemStyle Width="1%" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton CommandName="Baixar" ID="btnEditaCadastro" AlternateText="Editar" runat="server" Text="CNAB" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="QTDBoletos" HeaderText="Boletos" >
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>

                    </div><!--row fim-->
                </div>
            </div>

        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
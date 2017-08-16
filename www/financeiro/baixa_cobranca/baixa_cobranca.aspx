<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="baixa_cobranca.aspx.cs" Inherits="MedProj.www.financeiro.baixa_cobranca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Baixa de Cobranças
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviar" />
        </Triggers>

        <ContentTemplate>


            <div class="panel panel-default">
                <%--<div class="panel-heading text-center" style="position:relative;">--%>
                <div class="panel-heading">
                    <div class="row" style="margin-left:10px">
                        <asp:RadioButton ID="optEnviar" Text="Enviar arquivo de baixa bancária" runat="server" GroupName="a" Checked="true" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                        &nbsp;
                        <asp:RadioButton ID="optVisualizar" Text="Visualizar baixas bancárias realizadas" runat="server" GroupName="a" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                    </div>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <asp:Panel ID="pnlEnviar" runat="server">
                            <div class="form-group">
                                <label class="col-xs-2 control-label">Tipo de arquivo</label>
                                <div class="col-xs-4" style="padding-top:7px">
                                    <asp:DropDownList ID="cboTipoArquivo" SkinID="comboPadrao1" runat="server" Width="100%">
                                        <asp:ListItem Text="selecione" Value="-1" Selected="True" />
                                        <asp:ListItem Text="Depósito Identificado" Value="1" />
                                        <asp:ListItem Text="Retorno pagamentos por boleto" Value="0" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">Informe o arquivo</label>
                                <div class="col-xs-4" style="padding-top:7px">
                                    <asp:FileUpload ID="fuEnviar" Width="100%" runat="server" />
                                </div>
                                <div class="col-xs-6 text-left" style="padding-top:7px">
                                    <asp:Button ID="cmdEnviar" Text="Enviar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdEnviar_Click" OnClientClick="return confirm('Submeter arquivo de baixa bancária?');" />
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnlVisualizar" runat="server" Visible="false">
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
                                        <div class="col-md-2"  style="padding-left:0px;">
                                            <div class="input-group">
                                                <asp:DropDownList ID="cboFiltro" SkinID="comboPadrao1" Width="100%" runat="server">
                                                    <asp:ListItem Text="Em aberto" Value="0" Selected="True" />
                                                    <asp:ListItem Text="Processados" Value="1" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-md-6" style="padding-left:0px;">
                                            <asp:Button ID="cmdVisualizar" Text="pesquisar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdVisualizar_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:GridView ID="grid" Width="60%" SkinID="gridPadrao" runat="server" OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand" DataKeyNames="ID">
                                            <Columns>
                                                <asp:BoundField DataField="Nome" HeaderText="Nome">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Criacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Processado">
                                                    <ItemTemplate>
                                                        <%# Eval("Processamento") == null ? "" : Convert.ToDateTime(Eval("Processamento")).ToString("dd/MM/yyyy HH:mm") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="1%">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgBtn" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="Baixar" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>

                        <!--Modal Cobranças baixadas-->
                        <div class="modal fade" id="modalItens" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header text-left">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                        <h2 class="modal-title">Cobranças baixadas</h2>
                                    </div>
                                    <div class="modal-body">
                                        <div class="form-group">
                                            <div class="col-xs-12">
                                                <asp:GridView ID="gridBaixaItens" Width="100%" SkinID="gridPadrao" runat="server" OnRowDataBound="gridBaixaItens_RowDataBound" DataKeyNames="ID">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Arquivo" >
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litArquivo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Arquivo.Nome")%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Data" HeaderText="Data Baixa" HeaderStyle-Wrap="false" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Status" >
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litStatus" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Identificacao" HeaderText="Identificação" HeaderStyle-Wrap="false">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Valor Pago" HeaderStyle-Wrap="false" >
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litValorPago" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Data Pagto" HeaderStyle-Wrap="false" >
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litDataPago" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Titular" HeaderStyle-Wrap="false" >
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litTitular" Text='<%#DataBinder.Eval(Container.DataItem, "Titular")%>' runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <asp:Literal ID="litBaixasTotais" runat="server" EnableViewState="false" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div><!--row fim-->
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        function showModal()
        {
            $('#modalItens').modal('show');
        }
    </script>
</asp:Content>

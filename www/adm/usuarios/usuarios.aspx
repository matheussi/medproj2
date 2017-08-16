<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="usuarios.aspx.cs" Inherits="MedProj.www.adm.usuarios.usuarios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Usuários
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">

    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right">
                    <h3 class="panel-title"><asp:Button ID="lnkNovo" Width="110" SkinID="botaoPadrao1" Text="Novo Usuário" runat="server" EnableViewState="false" OnClick="lnkNovo_Click" /></h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Localizar</label>
                        <div class="col-xs-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome</span>
                                <asp:TextBox SkinID="txtPadrao" runat="server" MaxLength="30" ID="txtNome" Width="98%" EnableViewState="false" />
                            </div>
                        </div>

                        <div class="col-xs-4" >
                            <div class="input-group">
                                <span class="input-group-addon">Tipo</span>
                                <asp:DropDownList ID="cboTipoUsuario" SkinID="comboPadrao1" runat="server" Width="98%" EnableViewState="true">
                                    <asp:ListItem Text="Administrador" Value="0" Selected="True" />
                                    <asp:ListItem Text="Operador" Value="1" />
                                    <asp:ListItem Text="Prestador" Value="2" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <asp:Panel ID="pnlSelCliente" Visible="false" runat="server">
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Cliente</span>
                                    <asp:DropDownList ID="cboCliente" SkinID="comboPadrao1" runat="server" Width="100%" />
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="col-xs-2 text-right">
                            <asp:Button ID="cmdProcurar" Width="110" SkinID="botaoPadrao1" Text="Procurar" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                        </div>
                    </div>

                    <div class="form-group text-right">
                        <%--<div class="col-xs-12 text-right">
                            <asp:Button ID="cmdProcurar" Width="110" SkinID="botaoPadrao1" Text="Procurar" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                        </div>--%>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-12 text-right">
                            <asp:GridView ID="grid" runat="server" SkinID="gridPadrao" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="Tipo" HeaderText="Perfil" />
                                    <asp:TemplateField HeaderText="Nome">
                                        <ItemTemplate>
                                            <asp:Literal ID="litNome" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Nome")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prestador">
                                        <ItemTemplate>
                                            <asp:Literal ID="litPrestador" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Unidade.Owner.Nome")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acesso">
                                        <ItemTemplate>
                                            <asp:Literal ID="litAcesso" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Link" Text="Editar" CommandName="Editar">
                                        <ItemStyle Width="1%" />
                                        <ControlStyle CssClass="btn btn-info" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>
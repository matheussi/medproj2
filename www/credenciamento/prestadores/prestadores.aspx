<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="prestadores.aspx.cs" Inherits="MedProj.www.credenciamento.prestadores.prestadores" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Credenciados
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo prestador" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <%--<label class="col-md-4 text-left">Região:</label>--%>
                            <%--<label class="col-md-6 text-left">Especialidade:</label>--%>
                            <label class="col-md-6 text-left">Segmento:</label>
                            <label class="col-md-6 text-left">Nome ou razão social:</label>
                        </div>
                        <div class="row">
                            <%--<div class="col-md-4">
                                <div class="col-md-12"  style="padding-left:0px;">
                                      <asp:DropDownList ID="cboRegiao" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>
                            </div>--%>
                            <%--<div class="col-md-6">
                                <asp:DropDownList ID="cboEspecialidade" SkinID="comboPadrao1" Width="100%" runat="server" />
                            </div>--%>
                            <div class="col-md-6">
                                <asp:DropDownList ID="cboSegmento" SkinID="comboPadrao1" Width="100%" runat="server" />
                            </div>
                            <div class="col-md-6"><asp:TextBox ID="txtNomeEmpresaCliente" SkinID="txtPadrao" Width="100%" runat="server" /></div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="text-right col-md-12"><asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" /></div>
                    </div>
                        <div class="space"></div>

                        <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Nome" HeaderText="Prestador" />

                                <asp:TemplateField HeaderText="Especialidade" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litSegmento" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EspecialidadeNome")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="EspecialidadeEnde" HeaderText="Endereço" />

                                <%--<asp:TemplateField HeaderText="Nome" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litNome" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cliente.Nome")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Empresa" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litEmpresa" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cliente.Empresa")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="E-mail" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litEmailGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cliente.Email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Telefone" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litTelefone" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cliente.Telefone")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Responsável">
                                    <ItemTemplate>
                                        <asp:Literal ID="litUsuario" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Usuario.Nome")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>

                                <asp:ButtonField ButtonType="Link" Text="Excluir" CommandName="Excluir" Visible="false">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Print">
                                    <ItemStyle Width="1%" />
                                    <ControlStyle CssClass="glyphicon glyphicon-print" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                    <ItemStyle Width="1%" />
                                    <ControlStyle Width="1%" />
                                    <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar">
                                    <ItemStyle Width="1%" />
                                    <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <asp:Literal ID="litMensagem" EnableViewState="false" runat="server" />
                   
                    </div>
            </div>

           

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

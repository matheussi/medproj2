<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="coberturas.aspx.cs" Inherits="MedProj.www.adm.cobertura.coberturas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Tabelas de Coberturas
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Nova tabela" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <%--<div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Código:</label>
                            <label class="col-md-8 text-left">Procedimento:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-4"  style="padding-left:0px;">
                                      <asp:TextBox ID="txtCodigo" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event);" />
                                </div>
                                <div class="col-md-6">
                                    <asp:TextBox ID="txtProcedimento" Width="100%" SkinID="txtPadrao" runat="server" />
                                </div>

                                <div class="col-md-2">
                                    <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>--%>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <%--<hr>--%>
                    <div class="text-right col-md-12"></div>
                        <div class="space"></div>

                        <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" 
                            AutoGenerateColumns="False" AllowPaging="true" PageSize="100" 
                            OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID"
                            OnRowCommand="grid_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Nome" HeaderText="Tabela" />

                                <asp:ButtonField ButtonType="Link" Text="Excluir" CommandName="Excluir" Visible="false">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar">
                                    <ItemStyle Width="1%" />
                                    <ControlStyle Width="1%" />
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

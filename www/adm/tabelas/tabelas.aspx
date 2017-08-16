<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="tabelas.aspx.cs" Inherits="MedProj.www.adm.tabelas.tabelas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Tabelas de preço e vigências
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Nova tabela" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-12 text-left">Tabela:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <asp:TextBox ID="txtTabela" SkinID="txtPadrao" Width="99%" runat="server" />
                            </div>
                            <div class="col-md-4 text-left"><asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" /></div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <hr>
                    <div class="space"></div>

                    <asp:GridView ID="grid" runat="server" SkinID="gridPadrao" Width="100%" AutoGenerateColumns="False" AllowPaging="true" PageSize="100" OnPageIndexChanging="grid_PageIndexChanging" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" DataKeyNames="ID">
                        <Columns>
                            <asp:TemplateField HeaderText="Tabela" >
                                <ItemTemplate>
                                    <asp:Literal ID="litTabela" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Nome")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                <ItemStyle Width="1%" />
                                <ControlStyle Width="1%" />
                                <ControlStyle CssClass="glyphicon glyphicon-remove" />
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

<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="produtos.aspx.cs" Inherits="MedProj.www.adm.produto.produtos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Produtos
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo produto" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Operadora:</label>
                            <label class="col-md-4 text-left">Associado PJ:</label>
                            <label class="col-md-4 text-left">Contrato Adm:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-4"  style="padding-left:0px;">
                                    <asp:DropDownList ID="cboOperadora" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="cboAssociadoPj" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboAssociadoPj_SelectedIndexChanged" />
                                </div>

                                <div class="col-md-3">
                                    <asp:DropDownList ID="cboContratoAdm" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>

                                <div class="col-md-1">
                                    <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadraoINFO_Small" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                </div>
                            </div>
                        </div>
                    </div><%----%>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="text-right col-md-12"></div>
                        <div class="space"></div>

                        <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" 
                            AutoGenerateColumns="False" AllowPaging="true" PageSize="100" 
                            OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID"
                            OnRowCommand="grid_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Nome" HeaderText="Produto" />

                                <asp:TemplateField HeaderText="Operadora" HeaderStyle-Wrap="false" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litOp" Text='<%#DataBinder.Eval(Container.DataItem, "Operadora.Nome")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Associado PJ" HeaderStyle-Wrap="false" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litOp" Text='<%#DataBinder.Eval(Container.DataItem, "AssociadoPj.Nome")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Contrato Adm" HeaderStyle-Wrap="false" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litOp" Text='<%#DataBinder.Eval(Container.DataItem, "ContratoAdm.Descricao")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

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

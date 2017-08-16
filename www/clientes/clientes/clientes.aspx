<%@ Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="clientes.aspx.cs" Inherits="MedProj.www.clientes.clientes.clientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Contratos
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo contrato" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Cartão:</label>
                            <label class="col-md-3 text-left">Nome:</label>
                            <label class="col-md-5 text-left">Associado PJ:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-4"  style="padding-left:0px;">
                                      <asp:TextBox ID="txtCartao" Width="100%" SkinID="txtPadrao" runat="server" />
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtNome" Width="100%" SkinID="txtPadrao" runat="server" />
                                </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="cboAssociadoPJ" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>
                                <div class="col-md-1">
                                    <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <hr>
                    <div class="space"></div>

                    <asp:GridView ID="gridContratos" Width="100%" SkinID="gridPadrao"
                        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                        DataKeyNames="ID,Rascunho,Cancelado,Inativo" onrowcommand="gridContratos_RowCommand" 
                        onrowdatabound="gridContratos_RowDataBound" PageSize="25"
                        OnPageIndexChanging="gridContratos_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Numero" HeaderText="Número">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Titular">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="EmpresaCobranca" HeaderText="Empresa" Visible="false">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" ImageUrl="~/images/rascunho.png" ToolTip="rascunho" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="1%" />
                            </asp:TemplateField>
                            <asp:ButtonField Text="<img src='../../images/active.png' title='excluir' alt='excluir' border='0' />" CommandName="inativar" >
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:ButtonField Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                        </Columns>
                    </asp:GridView>
                    <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

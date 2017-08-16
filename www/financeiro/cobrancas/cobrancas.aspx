<%@ Page Title="" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cobrancas.aspx.cs" Inherits="MedProj.www.financeiro.cobrancas.cobrancas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Cobranças
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
                    <div class="space"></div>

                    <asp:Panel ID="pnlContratos" Visible="false" runat="server">
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
                                <asp:ButtonField Visible="false" Text="<img src='../../images/active.png' title='excluir' alt='excluir' border='0' />" CommandName="inativar" >
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField Visible="false" Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField Visible="false" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkExibir" Text="cobranças" CommandName="cobrancas" CommandArgument='<%#Eval("ID")%>' runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
                    </asp:Panel>

                    <asp:Panel ID="pnlCobrancas" Visible="false" runat="server">
                        <asp:GridView ID="gridCobranca" runat="server" SkinID="gridPadrao" DataKeyNames="ID,HeaderParcID,HeaderItemID"
                            AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridCobranca_RowDataBound"
                            OnRowCommand="gridCobranca_RowCommand" OnPageIndexChanging="gridCobranca_PageIndexChanging"
                            AllowPaging="True" PageSize="15">
                            <Columns>
                                <asp:BoundField DataField="Parcela" HeaderText="Parc." Visible="false">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Valor">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkValor" CommandArgument='<%# Container.DataItemIndex %>' runat="server" Font-Size="10px" Text='<%# Bind("Valor", "{0:C}") %>' CommandName="detalhe" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="ValorPgto" HeaderText="Valor Pago" DataFormatString="{0:C}">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Vencimento">
                                    <ItemTemplate>
                                        <asp:Label ID="txtVenctoCobGrid" Width="90px" runat="server" Font-Size="10px" Text='<%# Bind("DataVencimento", "{0:dd/MM/yyyy}") %>'/>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="strEnviado" HeaderText="Enviado">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="strPago" HeaderText="Pago">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="strDataPago" HeaderText="Data Pgto">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField Text="<img src='../../images/mail.gif' border='0' alt='enviar e-mail' title='enviar e-mail' />"
                                    CommandName="email">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:ButtonField>
                                <asp:ButtonField Visible="False" ButtonType="Image" ImageUrl="~/images/refresh.png"
                                    Text="recalcular" CommandName="recalcular">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField Text="<img src='../../images/print.png' border='0' alt='imprimir' title='imprimir' />" 
                                    CommandName="print">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:ButtonField>
                            </Columns>
                            <PagerSettings PageButtonCount="30" />
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

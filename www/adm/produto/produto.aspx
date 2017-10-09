<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="produto.aspx.cs" Inherits="MedProj.www.adm.produto.produto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Produto
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="col-xs-10 text-right">
                        <asp:Button ID="cmdVoltar2" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar2" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                    <div class="clearfix"></div>

                    <hr />
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Nome</label>
                        <div class="col-xs-8"><asp:TextBox ID="txtNome" runat="server" Width="100%" MaxLength="250" SkinID="txtPadrao" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Operadora</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboOperadora" runat="server" Width="100%" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboEstipulante" runat="server" Width="100%" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato ADM</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboContratoADM" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
                    </div>

                    <asp:Panel ID="pnlItens" runat="server" Visible="false" CssClass="alert alert-warning">
                    
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Produto</label>
                            <div class="col-xs-8"><asp:TextBox ID="txtProduto" runat="server" Width="100%" MaxLength="250" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Vigência</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtVigencia" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Valor</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtValor" runat="server" onkeypress="filtro_SoNumeros(event);" Width="100%" SkinID="txtPadrao" MaxLength="15" /></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Valor Net</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtValorNet" runat="server" onkeypress="filtro_SoNumeros(event);" Width="100%" SkinID="txtPadrao" MaxLength="15" /></div>
                            <div class="col-md-1"><div class="col-xs=2"><asp:LinkButton id="cmdAdd" runat="server" OnClick="cmdAdd_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div></div>
                        </div>

                        <hr />

                        <div class="form-group">
                            <div class="col-xs-10">
                                <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" AllowPaging="true" PageSize="100" OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Nome" HeaderText="Item" />
                                        <asp:BoundField DataField="Vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}" />
                                        <asp:BoundField DataField="ValorNet" HeaderText="ValorNet" DataFormatString="{0:N2}" />
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
                            </div>
                        </div>

                    </asp:Panel>

                    <div class="col-xs-10 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

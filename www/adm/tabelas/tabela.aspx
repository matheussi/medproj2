<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="tabela.aspx.cs" Inherits="MedProj.www.adm.tabelas.tabela" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Vigência
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Tabela de preço</label>
                        <div class="col-xs-8">
                            <asp:TextBox ID="txtNome" runat="server" Width="100%" SkinID="txtPadrao" MaxLength="150" />
                        </div>
                    </div>

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">Data</label>
                        <div class="col-xs-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    início
                                </span>
                                <asp:TextBox ID="txtDataDe" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                            </div>
                        </div>
                        <div class="input-group">
                            <span class="input-group-addon">
                                fim&nbsp;&nbsp;
                            </span>
                            <asp:TextBox ID="txtDataAte" Width="110" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                        </div>
                    </div>--%>

                    

                    <asp:Panel ID="pnlVigencias" runat="server" Visible="false">

                        <div class="alert alert-warning col-xs-10">

                            <div class="form-group">
                                <h4 class="col-xs-2 control-label">Vigências</h4>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-2 control-label">Valor R$</label>
                                <div class="col-xs-2"><asp:TextBox ID="txtCH" runat="server" MaxLength="10" Width="100%" SkinID="txtPadrao" /></div>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-2 control-label">Data</label>
                                <div class="col-xs-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            início
                                        </span>
                                        <asp:TextBox ID="txtDataDe" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                    </div>
                                </div>
                                <div class="col-xs-1 text-left" style="padding-top:5px">
                                    <asp:LinkButton ID="lnkSalvarVigencia" runat="server" CssClass="glyphicon glyphicon-floppy-disk" ToolTip="salvar vigência" OnClick="lnkSalvarVigencia_Click"></asp:LinkButton>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-xs-12 text-left">
                                    <asp:GridView ID="gridVigencias" DataKeyNames="ID" SkinID="gridPadrao" Width="100%" runat="server" OnRowCommand="gridVigencias_RowCommand" OnRowDataBound="gridVigencias_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="DataInicio" HeaderText="Início" DataFormatString="{0:dd/MM/yyyy}" />
                                            <asp:BoundField DataField="ValorReal" HeaderText="Valor" DataFormatString="{0:N2}" />

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
                                        <RowStyle BackColor="WhiteSmoke" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <div class="form-group">
                        <div class="col-xs-10 text-left">
                        <hr />
                        </div>
                    </div>

                    <div class="col-xs-10 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

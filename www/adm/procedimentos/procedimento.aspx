<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="procedimento.aspx.cs" Inherits="MedProj.www.adm.procedimentos.procedimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Tabela de Procedimentos
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
                        <label class="col-xs-2 control-label">Tabela</label>
                        <div class="col-xs-8"><asp:TextBox ID="txtTabela" runat="server" Width="100%" MaxLength="250" SkinID="txtPadrao" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Segmento</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboSegmento" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Código</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtCodigo" runat="server" onkeypress="filtro_SoNumeros(event);" Width="100%" SkinID="txtPadrao" /></div>
                    </div>

                    <hr />

                    <asp:Panel ID="pnlProcedimentos" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Procedimento</label>
                            <div class="col-xs-8"><asp:TextBox ID="txtProcedimento" runat="server" Width="100%" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Especialidade</label>
                            <div class="col-xs-8">
                                <asp:RadioButton ID="optEspecCombo" GroupName="espec" runat="server" Checked="true" Visible="false" />
                                <asp:DropDownList ID="cboEspecialidade" runat="server" Width="250" SkinID="comboPadrao1" Visible="false" />
                                <asp:RadioButton ID="optEspecTxt" GroupName="espec" runat="server" Checked="false" Visible="false" />
                                <asp:TextBox ID="txtEspecTxt" runat="server" Width="100%" MaxLength="200" SkinID="txtPadrao" />
                                </row>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Categoria</label>
                            <div class="col-xs-8"><asp:TextBox ID="txtCategoria" runat="server" Width="100%" MaxLength="200" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Código</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtProcedimentoCodigo" runat="server" Width="100%" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">CH</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtProcedimentoCH" runat="server" Width="100%" SkinID="txtPadrao" /></div>
                            <div class="col-xs=2"><asp:LinkButton id="cmdAdd" runat="server" OnClick="cmdAdd_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div>
                        </div>

                        <div class="space"></div>
                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-5">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        Localizar&nbsp;&nbsp;
                                    </span>
                                    <asp:TextBox ID="txtLocalizar" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="150"  />
                                </div>
                            </div>
                            <div class="col-xs-1 text-left">
                                <asp:LinkButton ID="cmdLocalizar" runat="server" OnClick="cmdLocalizar_Click" ToolTip="localizar..."><span class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-10">
                                <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" AllowPaging="true" PageSize="100" OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Codigo" HeaderText="Código" />
                                        <asp:BoundField DataField="Especialidade" HeaderText="Especialidade" />
                                        <asp:BoundField DataField="Nome" HeaderText="Procedimento" />
                                        <asp:BoundField DataField="CH" HeaderText="CH" DataFormatString="{0:N2}" />
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

                        <hr />
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

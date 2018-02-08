<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cobertura.aspx.cs" Inherits="MedProj.www.adm.cobertura.cobertura" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Tabela de Coberturas
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
                        <div class="col-xs-8"><asp:TextBox ID="txtTabela" runat="server" Width="100%" MaxLength="250" SkinID="txtPadrao" /></div>
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

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">Valor por vida</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtValorPorVida" runat="server" onkeypress="filtro_SoNumeros(event);" Width="100%" SkinID="txtPadrao" MaxLength="15" /></div>
                    </div>--%>

                    <hr />

                    <asp:Panel ID="pnlProcedimentos" runat="server" Visible="false">

                        <div class="form-group">
                            <label class="col-xs-10 text-center"><strong>Vigências</strong></label>
                        </div>

                        <div class="form-group">
                            <label class="col-md-2 control-label">Início</label>
                            <div class="col-md-2"><asp:TextBox ID="txtInicio" onkeypress="filtro_SoNumeros(event);mascara_DATA(this, event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="10" /></div>
                            <label class="col-md-2 control-label">Valor</label>
                            <div class="col-md-2"><asp:TextBox ID="txtValorVig" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                        </div>
                        <div class="form-group">
                            <label class="col-md-2 control-label">Valor Net</label>
                            <div class="col-md-2"><asp:TextBox ID="txtValorNetVig" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                            <div class="col-md=2"><asp:LinkButton id="cmdAddVig" runat="server" OnClick="cmdAddVig_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="gridVig" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" AllowPaging="true" PageSize="100" OnPageIndexChanging="gridVig_PageIndexChanging" DataKeyNames="ID" OnRowCommand="gridVig_RowCommand" OnRowDataBound="gridVig_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Inicio" HeaderText="Início" DataFormatString="{0:dd/MM/yyyy}" />
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

                        <hr />
                        <div class="form-group ">
                            <label class="col-xs-10 text-center"><strong>Itens da cobertura</strong></label>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Descrição</label>
                            <div class="col-xs-8"><asp:TextBox ID="txtDescricao" runat="server" Width="100%" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Status</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="cboStatusItem" runat="server" Width="100%" SkinID="comboPadrao1">
                                    <asp:ListItem Text="Contratado" Value="Contratado" Selected="True" />
                                    <asp:ListItem Text="Inativo" Value="NaoContratado" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Valor</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtValorItem" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                            <div class="col-xs=2"><asp:LinkButton id="cmdAdd" runat="server" OnClick="cmdAdd_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div>
                        </div>

                        <div class="space"></div>
                        <%--<div class="form-group">
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
                        </div>--%>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" AllowPaging="true" PageSize="100" OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}" />
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

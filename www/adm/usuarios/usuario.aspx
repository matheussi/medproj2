<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="usuario.aspx.cs" Inherits="MedProj.www.adm.usuarios.usuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Usuário - Detalhe
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">

    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">&nbsp;</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Nome</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtNome" SkinID="txtPadrao" runat="server" Width="250" MaxLength="250" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Login</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtLogin" SkinID="txtPadrao" runat="server" Width="250" MaxLength="75" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Senha</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtSenha" SkinID="txtPadrao" runat="server" Width="200" MaxLength="20" TextMode="Password" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Status</label>
                        <div class="col-xs-10" style="padding-top:7px"><asp:CheckBox ID="chkAtivo" Text="Ativo" Checked="true" runat="server" /></div>
                    </div>

                    <div class="form-group" runat="server" id="divDataCadastro" enableviewstate="false" visible="false">
                        <label class="col-xs-2 control-label">Data de Cadastro</label>
                        <div class="col-xs-10"><asp:Literal ID="litDataCadastro" runat="server" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Tipo</label>
                        <div class="col-xs-10">
                            <asp:DropDownList SkinID="comboPadrao1" ID="cboTipoUsuario" runat="server" Width="290" AutoPostBack="true" OnSelectedIndexChanged="cboTipoUsuario_SelectedIndexChanged">
                                <asp:ListItem Text="Administrador" Value="0" Selected="True" />
                                <asp:ListItem Text="Operador"  Value="1" />
                                <asp:ListItem Text="Prestador" Value="2" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <asp:Panel ID="pnlPrestador" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Prestador</label>
                            <div class="col-xs-10"><asp:DropDownList ID="cboPrestador" SkinID="comboPadrao1" runat="server" Width="290" AutoPostBack="true" OnSelectedIndexChanged="cboPrestador_SelectedIndexChanged" /></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Unidade</label>
                            <div class="col-xs-10"><asp:DropDownList ID="cboUnidade" SkinID="comboPadrao1" runat="server" Width="290" /></div>
                        </div>

                    </asp:Panel>

                    <hr />
                    <div class="col-xs-12 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" SkinID="botaoPadrao1" EnableViewState="false" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" SkinID="botaoPadrao1" EnableViewState="false" />
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
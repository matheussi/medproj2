<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="meusdados.aspx.cs" Inherits="MedProj.www.meusdados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Meus dados
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

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">E-mail</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtEmail" SkinID="txtPadrao" runat="server" Width="250" MaxLength="75" ReadOnly="true" /></div>
                    </div>--%>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Login</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtLogin" SkinID="txtPadrao" runat="server" Width="250" MaxLength="75" ReadOnly="true" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Senha</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtSenha" SkinID="txtPadrao" runat="server" Width="200" MaxLength="20" TextMode="Password" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Confirme a Senha</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtSenha2" SkinID="txtPadrao" runat="server" Width="200" MaxLength="20" TextMode="Password" /></div>
                    </div>

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">Data de Cadastro</label>
                        <div class="col-xs-10" style="padding-top:7px"><asp:Literal ID="litDataCadastro" runat="server" /></div>
                    </div>--%>

                    <asp:Panel ID="pnlAgente" runat="server" Visible="false" EnableViewState="false">

                    </asp:Panel>

                    <hr />
                    <div class="col-xs-2 text-left"></div>
                    <div class="col-xs-8 text-left">
                    </div>
                    <div class="col-xs-2 text-right">
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" SkinID="botaoPadrao1" EnableViewState="false" />
                    </div>

                </div>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
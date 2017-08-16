<%@ Page Theme="padrao" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="texto.aspx.cs" Inherits="MedProj.www.financeiro.config_emails.texto" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Texto de aviso
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
                        <label class="col-xs-2 control-label">Descrição</label>
                        <div class="col-xs-8">
                            <asp:TextBox ID="txtDescricao" runat="server" Width="100%" MaxLength="149" SkinID="txtPadrao" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Texto</label>
                        <div class="col-xs-8">
                            <CKEditor:CKEditorControl Height="450px" Toolbar="Basic" ID="txtEmail" BasePath="~/ckeditor" runat="server" ToolbarBasic="Source|-|NewPage|Preview|Bold|Italic|Underline|-|NumberedList|BulletedList|-|Link|Unlink|-|Format|Font|FontSize|TextColor|BGColor"></CKEditor:CKEditorControl>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Metatags</label>
                        <label class="col-xs-8">
                            <strong>[#NOME]: </strong>Nome do beneficiário <br />
                            <strong>[#VNCT]: </strong>Data de vencimento da cobrança<br />
                            <strong>[#VLOR]: </strong>Valor da cobrança<br />
                            <strong>[#CMPT]: </strong>Competência da cobrança<br />
                            <strong>[#QTDV]: </strong>Quantidade de vidas do contrato<br />
                            <strong>[#LINK]: </strong>Link clicável para o boleto ([#LINK/] para fechar o link)<br />
                            <strong>[#ELINK]:</strong>Exibe o link por extenso
                        </label>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-8 col-xs-offset-2">
                            <asp:CheckBox ID="chkAtivo" Checked="true" Text="Ativo" runat="server" />
                        </div>
                    </div>
                    
                    <hr />
                    <div class="col-xs-12 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

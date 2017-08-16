<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="config_emails.aspx.cs" Inherits="MedProj.www.financeiro.config_emails.config_emails" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Configuração de emails de cobrança e avisos
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
                        <label class="col-xs-2 control-label">Tipo</label>
                        <div class="col-xs-4"><asp:DropDownList ID="cboTipo" runat="server" Width="100%" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Texto</label>
                        <div class="col-xs-4"><asp:DropDownList ID="cboTexto" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="cboEstipulante" runat="server" Width="100%" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_SelectedIndexChanged" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato ADM</label>
                        <div class="col-xs-4"><asp:DropDownList ID="cboContratoADM" runat="server" Width="100%" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboContratoADM_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato</label>
                        <div class="col-xs-8">
                            <asp:CheckBox ID="chkContratosTodos" runat="server" Text="(Todos)" style="font-size:9pt" AutoPostBack="true"  OnCheckedChanged="chkContratosTodos_CheckedChanged"/>
                            <asp:ListBox ID="lstContratos" runat="server" Rows="10" SelectionMode="Multiple" Width="100%" SkinID="comboPadrao1" />
                        </div>
                    </div>

                    <!--Para aviso de vencimento passado-->
                    <asp:Panel ID="pnlFrequencia" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Dias decorridos do vencimento</label>
                            <div class="col-xs-2">
                                <asp:TextBox ID="txtFrequencia" runat="server" Width="50%" MaxLength="3" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event);"/>
                            </div>
                        </div>
                    </asp:Panel>

                    <!--Para aviso de vencimento próximo-->
                    <asp:Panel ID="pnlDiasAntecedencia" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Dias de antecedência</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtDiasAntecedencia" runat="server" Width="50%" MaxLength="3" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event);"/></div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlTexto" runat="server" Visible="false" EnableViewState="false">

                    <div class="form-group">
                        <label class="col-xs-2 control-label">E-mail</label>
                        <div class="col-xs-8">
                            <CKEditor:CKEditorControl Toolbar="Basic" ID="txtEmail" BasePath="~/ckeditor" runat="server" ToolbarBasic="Source|-|NewPage|Preview|Bold|Italic|Underline|-|NumberedList|BulletedList|-|Link|Unlink|-|Format|Font|FontSize|TextColor|BGColor"></CKEditor:CKEditorControl>
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
                        <%--<div class="col-xs-8">
                        </div>--%>
                    </div>

                    </asp:Panel>

                    <%--
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Executar em</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtExecutarEm" runat="server" Width="100%" MaxLength="10" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);"/></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboEstipulante" /></div>
                        <label class="col-xs-2 control-label">Operadora</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboOperadora"  /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboContrato" /></div>
                        <label class="col-xs-2 control-label">Plano</label>
                        <div class="col-xs-3">
                            <asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboPlano" AutoPostBack="false" />
                        </div>
                    </div>
--%>

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

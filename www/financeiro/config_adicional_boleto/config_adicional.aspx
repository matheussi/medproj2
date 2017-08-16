<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="config_adicional.aspx.cs" Inherits="MedProj.www.financeiro.config_adicional_boleto.config_adicional" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Configuração de valores adicionais em boleto
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

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Valor</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtValor" runat="server" Width="100%" SkinID="txtPadrao" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Texto no boleto</label>
                        <div class="col-xs-5"><asp:TextBox ID="txtTextoNoBoleto" runat="server" Width="100%" SkinID="txtPadrao" MaxLength="150" /></div>
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

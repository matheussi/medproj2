<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="contratoadm.aspx.cs" Inherits="MedProj.www.adm.contratoadm.contratoadm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Contrato Administrativo
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
                        <div class="col-xs-10"><asp:TextBox ID="txtDescricao" SkinID="txtPadrao" runat="server" Width="250" MaxLength="250" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Plano</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtPlano" SkinID="txtPadrao" runat="server" Width="150" MaxLength="250" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Operadora</label>
                        <div class="col-xs-10">
                            <asp:DropDownList SkinID="comboPadrao1" ID="cboOperadora" runat="server" Width="290" AutoPostBack="false" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-10">
                            <asp:DropDownList SkinID="comboPadrao1" ID="cboAssociadoPJ" runat="server" Width="290" AutoPostBack="false" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Status</label>
                        <div class="col-xs-10" style="padding-top:7px"><asp:CheckBox ID="chkAtivo" Text="Ativo" Checked="true" runat="server" /></div>
                    </div>

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
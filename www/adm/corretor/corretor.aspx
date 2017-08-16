<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="corretor.aspx.cs" Inherits="MedProj.www.adm.corretor.corretor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Corretor
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
                        <label class="col-md-2 col-sm-12 control-label">Corretor</label>
                        <div class="col-md-10 col-sm-12"><asp:TextBox ID="txtNome" runat="server" Width="70%" MaxLength="250" SkinID="txtPadrao" /></div>
                    </div>

                    <%--<div class="form-group">
                        <label class="col-md-2 col-sm-12 control-label">Tabela</label>
                        <div class="col-md-10 col-sm-12"><asp:DropDownList ID="cboTabela" runat="server" Width="70%" SkinID="comboPadrao1" /></div>
                    </div>--%>

                    <%--<div class="form-group">
                        <label class="col-md-2 col-sm-12 control-label">Status</label>
                        <div class="col-md-10 col-sm-12" style="margin-top:5px"><asp:CheckBox ID="chkStatus" Text="Ativo" Checked="true" runat="server" /></div>
                    </div>--%>

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

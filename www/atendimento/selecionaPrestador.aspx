<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="selecionaPrestador.aspx.cs" Inherits="MedProj.www.atendimento.selecionaPrestador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Selecione o prestador
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">
        </div>
        <div class="panel-body">
            <asp:UpdatePanel ID="up" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Prestador</label>
                        <div class="col-xs-8" style="padding-top:2px"><asp:DropDownList ID="cboPrestador" runat="server" SkinID="comboPadrao1" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboPrestador_SelectedIndexChanged"/></div>
                        <div class="col-xs-2" style="padding-top:1px"></div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Unidade</label>
                        <div class="col-xs-8" style="padding-top:2px"><asp:DropDownList ID="cboUnidade" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                        <div class="col-xs-2" style="padding-top:1px"><asp:Button id="cmdSel" Text="selecionar" SkinID="botaoPadraoINFO_Small" runat="server" OnClick="cmdSel_Click" /></div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
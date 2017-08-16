<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="exportacaoCartao.aspx.cs" Inherits="MedProj.www.adm.importacao.exportacaoCartao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Agenda de exportação - Cartão
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">

    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkArquivoLog" />
            <asp:PostBackTrigger ControlID="lnkArquivoDados" />
        </Triggers>
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">&nbsp;</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Descrição</label>
                        <div class="col-xs-8"><asp:TextBox ID="txtDescricao" runat="server" Width="100%" MaxLength="149" SkinID="txtPadrao" /></div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Executar em</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtExecutarEm" runat="server" Width="100%" MaxLength="10" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);"/></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Filial</label>
                        <div class="col-xs-3"><asp:DropDownList ID="cboFilial" runat="server" Width="100%" MaxLength="10" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboEstipulante" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_SelectedIndexChanged" /></div>
                        <label class="col-xs-2 control-label">Operadora</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="True" Width="100%" runat="server" ID="cboOperadora" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="false" Width="100%" runat="server" ID="cboContrato" /></div>
                        <%--<label class="col-xs-2 control-label">Plano</label>
                        <div class="col-xs-3">
                            <asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboPlano" AutoPostBack="false" />
                        </div>--%>
                    </div>

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">Acomodação</label>
                        <div class="col-xs-2"><asp:DropDownList SkinID="comboPadrao1" Width="395px" runat="server" ID="cboAcomodacao" /></div>
                    </div>--%>

                    <asp:Panel ID="pnlDownload" Visible="false" runat="server">
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:LinkButton ID="lnkArquivoDados" Text="Clique para abrir o arquivo de dados" runat="server" OnClick="lnkArquivoDados_Click" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:LinkButton ID="lnkArquivoLog" Text="Clique para abrir o arquivo de log" runat="server" OnClick="lnkArquivoLog_Click" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:CheckBox ID="chkInativo" Text="Inativa" runat="server" ForeColor="Red" />
                            </div>
                        </div>
                    </asp:Panel>

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

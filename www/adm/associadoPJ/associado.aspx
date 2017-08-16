<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="associado.aspx.cs" Inherits="MedProj.www.adm.associadoPJ.associado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Associado PJ
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
                        <div class="col-xs-10"><asp:TextBox ID="txtDescricao" runat="server" Width="70%" SkinID="txtPadrao" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Beneficiário</label>
                        <div class="col-xs-10"><asp:DropDownList ID="cboBeneficiario" runat="server" Width="70%" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label pull-left">Data de validade</label>
                        <div class="col-xs-2" style="margin-top:5px"><asp:RadioButton ID="cboDataFixa" Text="Data Fixa" runat="server" GroupName="a" Checked="true" AutoPostBack="true" OnCheckedChanged="cbo_CheckedChanged" /></div>
                        <div class="col-xs-3" style="margin-top:5px"><asp:RadioButton ID="cboQtdMeses" Text="Meses a partir da vigência" runat="server" GroupName="a" AutoPostBack="true" OnCheckedChanged="cbo_CheckedChanged"  /></div>

                        <asp:Panel ID="pnlDataFixa" runat="server">
                            <div class="col-xs-2 text-right" style="margin-top:5px">Data</div>
                            <div class="col-xs-2" style="margin-top:1px"><asp:TextBox ID="txtDataFixa" MaxLength="10" Width="75px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" SkinID="txtPadrao" runat="server"/></div>
                        </asp:Panel>

                        <asp:Panel ID="pnlMeses" runat="server" Visible="false">
                            <div class="col-xs-2 text-right" style="margin-top:5px">Meses</div>
                            <div class="col-xs-2" style="margin-top:1px"><asp:TextBox MaxLength="3" Width="50px" ID="txtMesesAPartirDaVigencia" onkeypress="filtro_SoNumeros(event);" SkinID="txtPadrao" runat="server"/></div>
                        </asp:Panel>
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
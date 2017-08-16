﻿<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="adimplenciaStatus.aspx.cs" Inherits="MedProj.www.financeiro.reports.adimplenciaStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/common.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Adimplência e inadimplência
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkToExcel" />
            <asp:PostBackTrigger ControlID="lnkToExcelT" />
        </Triggers>
        <ContentTemplate>
            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo contrato" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" Visible="false" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-3 text-left">Tipo:</label>
                            <label class="col-md-3 text-left" style="padding-left:25px;">Associado PJ:</label>
                            <label class="col-md-2 text-left">De:</label>
                            <label class="col-md-2 text-left">Até:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-3" style="padding-left:0px;">
                                      <asp:DropDownList ID="cboTipo" Width="100%" SkinID="comboPadrao1" runat="server" OnSelectedIndexChanged="cboTipo_SelectedIndexChanged" AutoPostBack="true" >
                                          <asp:ListItem Text="Adimplentes" Value="Adimplentes" Selected="True" />
                                          <asp:ListItem Text="Inadimplentes" Value="Inadimplentes" Selected="False" />
                                          <asp:ListItem Text="Cobranças não geradas" Value="NaoGeradas" Selected="False" />
                                      </asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="cboAssociadoPJ" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>
                                <div class="col-md-2">
                                    <asp:TextBox ID="txtDe" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </div>
                                <div class="col-md-2">
                                    <asp:TextBox ID="txtAte" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </div>
                                <div class="col-md-1">
                                    <asp:Button ID="cmdProcurar" Text="Exibir" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="space"></div>

                    <asp:LinkButton ID="lnkToExcelT" Text="exportar para o excel " Visible="false" runat="server" OnClick="cmdToExcel_Click" /> <asp:ImageButton ID="lnkToExcel" ImageAlign="AbsMiddle" Visible="false" ToolTip="exportar para o excel" ImageUrl="~/Images/excel.png" runat="server" OnClick="cmdToExcel_Click" />
                    <br /><br />
                    <asp:GridView ID="gridContratos" Width="100%" SkinID="gridPadrao"
                        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                        DataKeyNames="ContratoID" onrowcommand="gridContratos_RowCommand" 
                        onrowdatabound="gridContratos_RowDataBound" PageSize="25"
                        OnPageIndexChanging="gridContratos_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="ContratoNumero" HeaderText="Número">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BeneficiarioDocumento" HeaderText="CNPJ">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BeneficiarioNome" HeaderText="Titular">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AssociadoPJ" HeaderText="AssociadoPJ">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ContratoADM" HeaderText="ContratoADM">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                            <asp:BoundField DataField="CobrancaVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                            <asp:BoundField DataField="CobrancaVidas" HeaderText="Vidas">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                            <asp:BoundField DataField="CobrancaValorPago" HeaderText="Valor" DataFormatString="{0:N2}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                            <asp:BoundField DataField="CobrancaDataPago" HeaderText="Pagamento" DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                            <asp:BoundField DataField="CobrancaValorPendente" Visible="false" HeaderText="Valor" DataFormatString="{0:N2}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>

                        </Columns>
                    </asp:GridView>
                    <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
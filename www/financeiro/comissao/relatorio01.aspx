<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="relatorio01.aspx.cs" Inherits="MedProj.www.financeiro.comissao.relatorio01" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Relatório de comissionamento
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdExportar" />
            <asp:PostBackTrigger ControlID="cmdExportarExcel" />
            <%--<asp:PostBackTrigger ControlID="lnkToExcelT" />--%>
        </Triggers>
        <ContentTemplate>
            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"></div>
                    <div class="col-md-12">

                        <div class="row">
                            <label class="col-md-12 text-left">Tabela:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12" >
                                <asp:DropDownList ID="cboTabelaComissao" Width="100%" SkinID="comboPadrao1" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row" style="padding-top:9px;">
                            <label class="col-md-3 text-left">Corretor:</label>
                            <label class="col-md-3 text-left" style="padding-left:25px;">Associado PJ:</label>
                            <label class="col-md-2 text-left">De:</label>
                            <label class="col-md-2 text-left">Até:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-3" style="padding-left:0px;">
                                      <asp:DropDownList ID="cboCorretor" Width="100%" SkinID="comboPadrao1" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="cboAssociadoPJ" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>
                                <div class="col-md-2">
                                    <asp:TextBox ID="txtDe" Width="85px" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </div>
                                <div class="col-md-1">
                                    <asp:TextBox ID="txtAte" Width="85px" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </div>
                                <div class="col-md-3">
                                    <asp:Button ID="cmdProcurar" Text="Exibir" SkinID="botaoPadraoINFO_Small" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                    <asp:Button ID="cmdExportar" Text="PDF" SkinID="botaoPadraoWarning_Small" EnableViewState="false" runat="server" OnClick="cmdExportar_Click" />
                                    <asp:Button ID="cmdExportarExcel" Text="CSV" SkinID="botaoPadraoWarning_Small" EnableViewState="false" runat="server" OnClick="cmdExportarCSV_Click" />
                                </div>
                            </div>
                        </div>

                        
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="space"></div>

                    <asp:Literal ID="litTotal" runat="server" />
                    <!--<br /><br />-->
                    <asp:GridView ID="gridCobrancas" Width="100%" SkinID="gridPadrao"
                        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                        DataKeyNames="CobrancaId,ContratoId" onrowcommand="gridCobrancas_RowCommand" 
                        onrowdatabound="gridCobrancas_RowDataBound" PageSize="25"
                        OnPageIndexChanging="gridCobrancas_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Corretor" HeaderText="Corretor">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Estipulante" HeaderText="Associado">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Beneficiario" HeaderText="Beneficiario">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BeneficiarioCartao" HeaderText="Cartão">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CobrancaParcela" HeaderText="Parcela">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CobrancaDataPago" HeaderText="Pagto" DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CobrancaValor" HeaderText="Valor" DataFormatString="{0:N}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CobrancaQtdVidas" HeaderText="Vidas">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="PercentualComissao" HeaderText="Percentual" DataFormatString="{0:N}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Calculado" HeaderText="Comissão" DataFormatString="{0:N}">
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

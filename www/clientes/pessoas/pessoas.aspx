<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="pessoas.aspx.cs" Inherits="MedProj.www.clientes.pessoas.pessoas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../css/style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Pessoas
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-65px;"><asp:Button runat="server" ID="cmdNovo" SkinID="botaoPadrao1" Text="Nova" Width="80" onclick="cmdNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Nome:</label>
                            <label class="col-md-3 text-left">CPF/CNPJ:</label>
                            <label class="col-md-3 text-left">RG/IE:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-4"  style="padding-left:0px;">
                                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtNome" Width="250" />
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCPF" Width="150" MaxLength="14" onkeypress="filtro_SoNumeros(event);" /><%--<ajaxToolkit:MaskedEditExtender runat="server" id="meeCPF" Mask="999,999,999-99" TargetControlID="txtCPF" />--%>
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtRG" Width="150" />
                                </div>

                                <div class="col-md-2">
                                    <asp:Button runat="server" ID="cmdConsultar" Text="Buscar" SkinID="botaoPadraoINFO" onclick="cmdConsultar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <hr>
                    <div class="space"></div>

                    <div class="col-md-12">
                        <asp:GridView DataKeyNames="ID,EnriquecimentoID" Width="100%" ID="gridBeneficiarios" 
                            runat="server" AutoGenerateColumns="False" SkinID="gridPadrao" 
                            onrowcommand="gridBeneficiarios_RowCommand" 
                            onrowdatabound="gridBeneficiarios_RowDataBound">
                            <Columns>
                                <asp:BoundField ItemStyle-Wrap="false" DataField="Nome" HeaderText="Nome">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField ItemStyle-Wrap="false" DataField="FTelefone" HeaderText="Fone">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField ItemStyle-Wrap="false"  DataField="FCelular" HeaderText="Celular">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField ItemStyle-Wrap="false"  DataField="Email" HeaderText="E-mail">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField Visible="false" DataField="TipoParticipacaoContrato" HeaderText="Titular">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="contratos" Text="<img src='../../images/detail2.png' title='contratos' alt='contratos' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="editar" Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <asp:Panel ID="pnlContratos" EnableViewState="false" Visible="false" runat="server">
                            <table width="100%" cellpadding="3" cellspacing="0" style="border-top: solid 1px lightgray;border-left: solid 1px lightgray;border-right: solid 1px lightgray">
                                <tr>
                                    <td align="left" class="tdNormal1"><span style="color:black" class="subtitulo" runat="server" id="lblSuperior" enableviewstate="false">Contratos do beneficiário</span></td>
                                    <%--<td align="right" class="tdNormal1"><asp:ImageButton ID="cmdFecharContato" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar"  /></td>--%>
                                </tr>
                            </table>
                            <asp:GridView DataKeyNames="ID" Width="100%" ID="gridContratos" EnableViewState="false"
                                runat="server" AutoGenerateColumns="False" SkinID="gridPadrao"
                                OnRowCommand="gridContratos_RowCommand" OnRowDataBound="gridContratos_RowDataBound">
                                <Columns>
                                    <asp:BoundField ItemStyle-Wrap="false" DataField="Numero" HeaderText="Número">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField ItemStyle-Wrap="false" DataField="OperadoraDescricao" HeaderText="Operadora">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="EstipulantDescricao" HeaderText="Associado">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField ItemStyle-Wrap="false"  DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField ItemStyle-Wrap="false" DataField="TipoParticipacaoContrato" HeaderText="Titular">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="editar" Text="<img src='../../images/edit.png' alt='editar' border='0' />">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                            <br />
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <table runat="server" enableviewstate="false" visible="false">
                <tr>
                    <td><span class="subtitulo">Tipo de busca</span></td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton Text="Qualquer parte do campo" runat="server" ID="optQualquer" GroupName="a" Checked="true" />
                        &nbsp;
                        <asp:RadioButton Text="Início do campo " runat="server" ID="optInicio" GroupName="a"  />
                        &nbsp;
                        <asp:RadioButton Text="Campo inteiro" runat="server" ID="optInteiro" GroupName="a"  />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

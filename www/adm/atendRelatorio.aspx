<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="atendRelatorio.aspx.cs" Inherits="MedProj.www.adm.atendRelatorio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">Relatório de atendimento</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" width="355">
                <tr>
                    <td class="tdPrincipal1">Categoria</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboCategoria" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Subcategoria</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboSubCategoria" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr runat="server" visible="false">
                    <td class="tdPrincipal1" valign="top">Operadora</td>
                    <td class="tdNormal1"><asp:ListBox ID="cboOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Aberto por</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtAbertoPor" Width="98%" runat="server" SkinID="textboxSkin" /> </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Período</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" ID="txtDe" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                        <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                         a 
                        <asp:TextBox runat="server" ID="txtAte" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                        <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Status</td>
                    <td class="tdNormal1">
                        <asp:DropDownList ID="cboStatuc" SkinID="dropdownSkin" runat="server">
                            <asp:ListItem Text="Todos" Value="0" Selected="True" />
                            <asp:ListItem Text="Concluídos" Value="1" />
                            <asp:ListItem Text="Em aberto" Value="2" />
                        </asp:DropDownList>
                        &nbsp;
                        <asp:Button ID="cmdLocalizar" Text="Localizar" SkinID="botaoAzulBorda" runat="server" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:ImageButton Visible="false" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />
            <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" PageSize="100" runat="server" AllowPaging="false" AutoGenerateColumns="False"  
                DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnPageIndexChanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoNumero" HeaderText="Contrato">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoVigencia" HeaderText="Vigência" Visible="false" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="DataCancelamento" Visible="false" HeaderText="Cancelamento" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="PlanoDescricao" Visible="false" HeaderText="Plano">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularNome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularCPF" HeaderText="CPF">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>

                    <asp:BoundField DataField="Saude" HeaderStyle-Wrap="false" Visible="false" HeaderText="Matr. Saúde">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Dental" HeaderStyle-Wrap="false" Visible="false" HeaderText="Matr. Dental">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>

                    <asp:BoundField DataField="AtendimentoTipo" HeaderText="Atendimento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="IniciadoPor" HeaderText="Aberto por">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataInicio" HeaderText="Aberto em" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ResolvidoPor" HeaderText="Concluído por">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="strDataFim" HeaderText="Concluído em" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataPrevisao" HeaderText="Previsão" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">

        $(document).ready(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(configAutocomplete);

            configAutocomplete(null, null);
        });

        function configAutocomplete(sender, args) {
            $("#<%= txtAbertoPor.ClientID %>").autocomplete
            ({
                source: function (request, response) {

                    $.ajax({
                        url: "../proxy/searchAtendenteMethod.aspx",
                        dataType: "json",
                        data: {
                            featureClass: "P",
                            style: "full",
                            maxRows: 12,
                            name_startsWith: request.term
                        },
                        success: function (data) {
                            response($.map(data.Atendentes, function (item) {
                                return {

                                    label: item.Atendente,
                                    value: item.Atendente,
                                    data: item
                                }
                            }));
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    showItem(ui.item ? ui.item : undefined);
                },
                search: function (event, ui) {
                    showItem(ui.item ? ui.item : undefined);
                }
            });
        }

        function showItem(item) {
            if (item != null && item != undefined)
            {
            }
            else
            {
            }
        }
    </script>
</asp:Content>

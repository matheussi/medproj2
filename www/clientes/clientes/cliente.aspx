<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cliente.aspx.cs" Inherits="MedProj.www.clientes.clientes.cliente" %>
<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
    <link href="../../css/style.css" rel="stylesheet" />
    <style type="text/css">
        .ajax__calendar_container { z-index : 9999999999999999999000 ; }
    </style>

</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="title" runat="server">
    Contrato
</asp:Content>
<asp:Content ID="cContent" ContentPlaceHolderID="content" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">
        </div>
        <div class="panel-body">
            <div class="form-group">
                <div class="col-xs-12">
                    <ajaxToolkit:TabContainer style="z-index:-1000" BorderStyle="None" BorderWidth="0" Width="90%" ID="tab" runat="server" ActiveTabIndex="0">
                        <ajaxToolkit:TabPanel runat="server" ID="p1">
                            <HeaderTemplate>
                                <span class="subtitulo">Dados comuns</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <br />
                                        <input type="hidden" runat="server" id="txtIdTitularOculto" />
                                        <asp:Panel ID="pnlSelNumeroContral" runat="server" EnableViewState="false" Visible="false">
                                            <table width="600px" cellpadding="4" cellspacing="0" style="border: solid 1px #507CD1">
                                                <tr>
                                                    <td class="tdNormal1">
                                                        Selecione abaixo para qual operadora está cadastrando esta proposta.
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                        </asp:Panel>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Núm. do Cartão</label>
                                            <div class="col-xs-3"><asp:TextBox Width="100%" runat="server" SkinID="txtPadrao" MaxLength="18" ID="txtNumeroContrato" /></div>
                                            <label class="col-xs-2 control-label">Confirme</label>
                                            <div class="col-xs-3"><asp:TextBox Width="100%" runat="server" SkinID="txtPadrao" MaxLength="18" ID="txtNumeroContratoConfirme" /></div>
                                            <div class="col-xs-2 text-left">
                                                <asp:LinkButton ID="lnkOkContrato" runat="server" Text="confirmar" OnClick="lnkOkContrato_Click" Visible="false" EnableViewState="false" />
                                                <asp:Image id="imgOk" ImageUrl="~/Images/tick.png" runat="server" Visible="false" EnableViewState="false" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Filial</label>
                                            <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" runat="server" ID="cboFilial" Width="100%" /></div>
                                            <label class="col-xs-2 control-label">Produtor</label>
                                            <div class="col-xs-4">
                                                <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCorretor" Width="100%" />
                                                <input type="hidden" name="txtCorretorID" id="txtCorretorID" runat="server" />
                                                <table id="Table1" width="100%" cellpadding="1" cellspacing="0" runat="server" enableviewstate="false" visible="false">
                                                    <tr>
                                                        <td width="90px">
                                                            Ident. Corretor
                                                        </td>
                                                        <td width="174px">
                                                            <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCorretorTerceiroIdentificacao"
                                                                Width="170px" MaxLength="240" />
                                                        </td>
                                                        <td width="40px" align="center">
                                                            CPF
                                                        </td>
                                                        <td>
                                                            <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCorretorTerceiroCPF" Width="79px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="90px">
                                                            Ident. Superior
                                                        </td>
                                                        <td width="174px">
                                                            <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtSuperiorTerceiroIdentificacao" Width="170px" MaxLength="240" />
                                                        </td>
                                                        <td width="40px" align="center">
                                                            CPF
                                                        </td>
                                                        <td>
                                                            <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtSuperiorTerceiroCPF" Width="79px" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Núm. Matrícula</label>
                                            <div class="col-xs-3"><asp:TextBox Width="100%" runat="server" SkinID="txtPadrao" MaxLength="12" ID="txtNumeroMatricula" /></div>
                                            <label class="col-xs-2 control-label">Tipo proposta</label>
                                            <div class="col-xs-3"><asp:DropDownList runat="server" SkinID="comboPadrao1" ID="cboTipoProposta" AutoPostBack="true" OnSelectedIndexChanged="cboTipoProposta_SelectedIndexChanged" Width="100%" /></div>
                                        </div>

                                        <%--<div class="form-group">
                                            <label class="col-xs-2 control-label">Corretor</label>
                                            <div class="col-xs-8">
                                                <asp:DropDownList runat="server" SkinID="comboPadrao1" ID="cboCorretor" Width="100%" />
                                            </div>
                                        </div>--%>
                        
                                        <table runat="server" visible="false" enableviewstate="false" cellpadding="2" width="100%" border="0" style="border: solid 0px gray">
                                            <tr runat="server" visible="false" enableviewstate="false">
                                                <td class="tdPrincipal1">
                                                    Plataforma
                                                </td>
                                                <td class="tdNormal1" colspan="3">
                                                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtOperador" Width="390px" />
                                                    <input type="hidden" name="txtOperadorID" id="txtOperadorID" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    
                                                </td>
                                                <td class="tdNormal1" colspan="3">
                                                    
                                                </td>
                                            </tr>
                                        </table>

                                        <hr />

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Associado PJ</label>
                                            <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboEstipulante" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_SelectedIndexChanged" /></div>
                                            <label class="col-xs-2 control-label">Operadora</label>
                                            <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="True" Width="100%" runat="server" ID="cboOperadora" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" /></div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Contrato</label>
                                            <div class="col-xs-4"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="True" Width="100%" runat="server" ID="cboContrato" OnSelectedIndexChanged="cboContrato_SelectedIndexChanged" /></div>
                                            <label class="col-xs-1 control-label">Plano</label>
                                            <div class="col-xs-4">
                                                <asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboPlano" AutoPostBack="true" OnSelectedIndexChanged="cboPlano_SelectedIndexChanged" />
                                                <asp:Button runat="server" SkinID="botaoAzulBorda" Text="migrar" ID="cmdAlterarPlano" OnClick="cmdAlterarPlano_Click" Visible="false" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Admissão</label>
                                            <div class="col-xs-2">
                                                <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtAdmissao" Width="90px" AutoPostBack="true" OnTextChanged="txtAdmissao_TextChanged" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                            </div>
                                            <div class="col-xs-1">
                                                <asp:Image Visible="false" SkinID="imgCanlendario" ID="imgAdmissao" runat="server" EnableViewState="false" />
                                                <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtAdmissao" PopupButtonID="imgAdmissao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </div>
                                            <label class="col-xs-2 control-label">Vigência</label>
                                            <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtVigencia" BackColor="lightgray" ReadOnly="false" Width="90px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" /></div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Contrato PJ</label>
                                            <div class="col-xs-9">
                                                <asp:TextBox SkinID="txtPadrao" Width="100%" runat="server" ID="txtContratoPJ"  />
                                                <input type="hidden" name="txtContratoPJId" id="txtContratoPJId" runat="server" />
                                            </div>
                                        </div>
                                        <br />
                                        <table cellpadding="2" width="100%" border="0" style="border: solid 0px gray">
                                            <tr runat="server" enableviewstate="false" visible="false">
                                                <td class="tdPrincipal1" width="102px">
                                                    Acomodação
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:DropDownList SkinID="dropdownSkin" Width="395px" runat="server" ID="cboAcomodacao"
                                                        AutoPostBack="true" OnSelectedIndexChanged="cboAcomodacao_SelectedIndexChanged" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel runat="server" ID="pnlInfoAnterior" Visible="false">
                                            <table cellpadding="2" width="100%" border="0" style="border: solid 1px gray">
                                                <tr>
                                                    <td class="tdPrincipal1" width="102px">
                                                        Empresa anterior
                                                    </td>
                                                    <td class="tdNormal1">
                                                        <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtEmpresaAnterior" Width="395px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdPrincipal1" width="102px">
                                                        Matrícula
                                                    </td>
                                                    <td class="tdNormal1">
                                                        <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtEmpresaAnteriorMatricula"
                                                            MaxLength="100" Width="120px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdPrincipal1" width="102px">
                                                        Tempo
                                                    </td>
                                                    <td class="tdNormal1">
                                                        <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtEmpresaAnteriorMeses" MaxLength="4"
                                                            Width="30px" />&nbsp;meses
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlHistoricoPlano" Visible="false">
                                            <table cellpadding="0" cellspacing="0" width="600" style="border: solid 1px #507CD1">
                                                <tr>
                                                    <td class="tdNormal1">
                                                        <asp:CheckBox ID="chkHistoricoPlano" title='Histórico de alterações de plano' Text="Histórico de alterações de plano"
                                                            runat="server" SkinID="checkboxSkin" AutoPostBack="true" OnCheckedChanged="chkHistoricoPlano_CheckedChanged"
                                                            EnableViewState="true" />
                                                    </td>
                                                </tr>
                                                <tr id="trHistoricoPlano" runat="server" visible="false" enableviewstate="true">
                                                    <td class="tdNormal1">
                                                        <asp:GridView ID="gridHistoricoPlano" OnRowDataBound="gridHistoricoPlano_RowDataBound"
                                                            OnRowCommand="gridHistoricoPlano_RowCommand" Width="100%" SkinID="gridViewSkin"
                                                            EnableViewState="true" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                                                            <Columns>
                                                                <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Data" HeaderText="Admissão" DataFormatString="{0:dd/MM/yyyy}">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />"
                                                                    CommandName="excluir">
                                                                    <ItemStyle Width="1%" />
                                                                </asp:ButtonField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <table cellpadding="2" width="100%" border="0" style="border: solid 0px gray" runat="server" enableviewstate="false" visible="false">
                                            <tr>
                                                <td class="tdPrincipal1" width="80px">
                                                    Vencimento
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtVencimento" BackColor="lightgray"
                                                        ReadOnly="false" Width="86px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                                </td>
                                            </tr>
                                        </table>
                                        <br /><br /><br /><br /><br /><br />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="p2" runat="server">
                            <HeaderTemplate>
                                <span class="subtitulo">Beneficiário Titular</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <br />
                                <asp:UpdatePanel runat="server" ID="upBeneficiario" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" Visible="false" ID="pnlSelTitular" EnableViewState="false">
                                            <asp:GridView ID="gridSelTitular" Width="65%" SkinID="gridViewSkin" EnableViewState="false"
                                                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID"
                                                OnRowCommand="gridSelTitular_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />"
                                                        CommandName="usar">
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">
                                                <asp:RadioButton ID="optCPF" Text="CPF" GroupName="tipo" Checked="true" runat="server" AutoPostBack="true" OnCheckedChanged="optTipo_CheckedChanged" />
                                                &nbsp;
                                                <asp:RadioButton ID="optCNPJ" Text="CNPJ" GroupName="tipo" Checked="false" runat="server" AutoPostBack="true" OnCheckedChanged="optTipo_CheckedChanged"/>
                                            </label>
                                            <div class="col-xs-3">
                                                <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtCPF" onkeypress="filtro_SoNumeros(event);" Width="100%" />
                                            </div>
                                            <div class="col-xs-1">
                                                <%--<ajaxToolkit:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCPF" />--%>
                                                <asp:ImageButton runat="server" ImageUrl="~/Images/search.png" ToolTip="localizar..." ID="cmdCarregaBeneficiarioPorCPF" EnableViewState="true" OnClick="cmdCarregaBeneficiarioPorCPF_Click" />
                                            </div>

                                            <asp:Panel ID="pnlRGTitular" Visible="true" runat="server">
                                                <label class="col-xs-1 control-label">RG</label>
                                                <div class="col-xs-2"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtRG" Width="100%" /></div>
                                                <div class="col-xs-1">
                                                    <div class="row" style="padding-top:7px">
                                                        <asp:ImageButton runat="server" ImageUrl="~/Images/search.png" ToolTip="localizar..." ID="cmdCarregaBeneficiarioPorRG" EnableViewState="true" OnClick="cmdCarregaBeneficiarioPorRG_Click" />
                                                        <asp:ImageButton Visible="false" ImageUrl="~/images/change.gif" ToolTip="alterar beneficiário" EnableViewState="true" runat="server" ID="cmdAlterarBeneficiarioTitular" />
                                                        <asp:ImageButton EnableViewState="true" runat="server" ToolTip="novo beneficiário" ID="cmdNovoTitular" ImageUrl="~/images/new.png" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-1">
                                                
                                                </div>
                                                <div class="col-xs-1">
                                                
                                                </div>
                                            </asp:Panel>
                                        </div>

                                        <hr />

                                        <asp:Panel ID="pnlTitular_DataNasc_EstCivil_MatrSau" Visible="true" runat="server">
                                            <div class="form-group">
                                                <label class="col-xs-2 control-label">Data Nasc.</label>
                                                <div class="col-xs-2">
                                                    <asp:TextBox runat="server" SkinID="txtPadrao"  ID="txtDataNascimento" Width="90px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                                </div>
                                                <div class="col-xs-1 text-left">
                                                    <asp:Image Visible="false" SkinID="imgCanlendario" ID="imgDataNascimento" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimento" TargetControlID="txtDataNascimento" PopupButtonID="imgDataNascimento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="col-xs-2 control-label">Estado Civil</label>
                                                <div class="col-xs-3"><asp:DropDownList Width="100%" runat="server" SkinID="comboPadrao1" ID="cboEstadoCivil" /></div>
                                                <label class="col-xs-2 control-label">Casamento</label>
                                                <div class="col-xs-2"><asp:TextBox ID="txtTitDataCasamento" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/></div>
                                                <div class="col-xs-1 text-left">
                                                    <asp:Image Visible="false" SkinID="imgCanlendario" ID="imgTitDataCasamento" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceTitDataCasamento" TargetControlID="txtTitDataCasamento" PopupButtonID="imgTitDataCasamento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="col-xs-2 control-label">Matr.-Saúde</label>
                                                <div class="col-xs-3"><asp:TextBox ID="txtNumMatriculaSaude" Width="100%" SkinID="txtPadrao" MaxLength="16" runat="server" /></div>
                                                <label class="col-xs-2 control-label">Matr.-Dental</label>
                                                <div class="col-xs-2"><asp:TextBox ID="txtNumMatriculaDental" Width="100%" SkinID="txtPadrao" MaxLength="16" runat="server" /></div>
                                            </div>
                                        </asp:Panel>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Nome</label>
                                            <div class="col-xs-7"><asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" SkinID="txtPadrao" ID="txtNome" Width="100%" /></div>
                                        </div>

                                        <asp:Panel ID="pnlTitular_Sexo_Peso_Altura" Visible="true" runat="server">
                                            <div class="form-group">
                                                <label class="col-xs-2 control-label">Sexo</label>
                                                <div class="col-xs-3">
                                                    <asp:DropDownList BackColor="LightGray" Enabled="false" Width="100%" runat="server" SkinID="comboPadrao1" ID="cboSexo">
                                                        <asp:ListItem Text="MASCULINO" Value="1" Selected="True" />
                                                        <asp:ListItem Text="FEMININO" Value="2" />
                                                    </asp:DropDownList>
                                                </div>
                                                <label class="col-xs-1 control-label">Peso</label>
                                                <div class="col-xs-1"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtTitPeso" Width="100%" /></div>
                                                <label class="col-xs-1 control-label">Altura</label>
                                                <div class="col-xs-1"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtTitAltura" Width="100%" /></div>
                                            </div>
                                        </asp:Panel>

                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">
                                                <asp:Literal ID="lblNomeMae_Contato" Text="Nome da mãe" runat="server" />
                                            </label>
                                            <div class="col-xs-7"><asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" SkinID="txtPadrao" ID="txtNomeMae" Width="100%" /></div>
                                        </div>

                                        <asp:Panel ID="pnlCompraCarencia" Visible="false" EnableViewState="false" runat="server">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">Compra de carência</div>
                                                <div class="panel-body">
                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">Operadora</label>
                                                        <div class="col-xs-4">
                                                            <asp:TextBox runat="server" ID="cboCarenciaOperadora" SkinID="txtPadrao" Width="100%" />
                                                            <input type="hidden" name="txtCarenciaOperadoraID" id="txtCarenciaOperadoraID" runat="server" />
                                                        </div>
                                                        <label class="col-xs-1 control-label">Matrícula</label>
                                                        <div class="col-xs-2"><asp:TextBox runat="server" ID="txtCarenciaMatricula" Width="100%" SkinID="txtPadrao" /></div>
                                                    </div>

                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">Tempo de contr.</label>
                                                        <div class="col-xs-2">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">de</span>
                                                                <asp:TextBox ID="txtTitTempoContratoDe" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-2">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">até</span>
                                                                <asp:TextBox ID="txtTitTempoContratoAte" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                                <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" ID="txtCarenciaTempoContrato" MaxLength="4" SkinID="textboxSkin" Width="20" Visible="false" />
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-2 text-left" style="padding-top:4px;margin-left:-19px"><i><font size="1">(data do último pagto.)</font></i></div>
                                                    </div>

                                                    <div class="form-group">
                                                        <label style="margin-top:-16px" class="col-xs-2 control-label">Código</label>
                                                        <div style="margin-top:-16px" class="col-xs-2"><asp:TextBox runat="server" ID="txtCarenciaCodigo" Width="100%" SkinID="txtPadrao" /></div>
                                                        <label style="margin-top:-16px" class="col-xs-2 control-label">Portabilidade</label>
                                                        <div style="margin-top:-16px" class="col-xs-3"><asp:TextBox runat="server" ID="txtPortabilidade" Width="100%" SkinID="txtPadrao" /></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>

                                        <asp:Panel ID="pnlTitular_ResponsavelLegal" Visible="true" runat="server">
                                            <div class="panel panel-default" style="margin-top:-1px">
                                                <div class="panel-heading">Responsável Legal (<i>preencher somente quando o titular for menor de idade</i>)</div>
                                                <div class="panel-body">
                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">Nome</label>
                                                        <div class="col-xs-7"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtNomeResponsavel" Width="100%" /></div>
                                                    </div>

                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">CPF</label>
                                                        <div class="col-xs-2">
                                                            <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtCPFResponsavel" Width="100%" />
                                                            <ajaxToolkit:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPFResponsavel" Mask="999,999,999-99" TargetControlID="txtCPFResponsavel" />
                                                        </div>
                                                        <label class="col-xs-2 control-label">RG</label>
                                                        <div class="col-xs-3"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtRGResponsavel" Width="100%" /></div>
                                                    
                                                    </div>

                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">Nascimento</label>
                                                        <div class="col-xs-2">
                                                            <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDataNascimentoResponsavel" Width="100%" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                        </div>
                                                        <div class="col-xs-1 text-left">
                                                            <asp:Image Visible="false" SkinID="imgCanlendario" ID="imgDataNascimentoResponsavel" runat="server" EnableViewState="false" />
                                                            <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimentoResponsavel" TargetControlID="txtDataNascimentoResponsavel" PopupButtonID="imgDataNascimentoResponsavel" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <label class="col-xs-2 control-label">Sexo</label>
                                                        <div class="col-xs-2">
                                                            <asp:DropDownList Width="100%" runat="server" SkinID="comboPadrao1" ID="cboSexoResponsavel">
                                                                <asp:ListItem Text="MASCULINO" Value="1" Selected="True" />
                                                                <asp:ListItem Text="FEMININO" Value="2" />
                                                            </asp:DropDownList>
                                                        </div>
                                                        <label class="col-xs-2 control-label">Parentesco</label>
                                                        <div class="col-xs-3"><asp:DropDownList Width="100%" runat="server" SkinID="comboPadrao1" ID="cboParentescoResponsavel" /></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="p3" runat="server">
                            <HeaderTemplate>
                                <span class="subtitulo">Dados cadastrais</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel runat="server" ID="upDadosCadastrais" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <br />
                                        <span runat="server" id="spanEnderecosDisponiveis_Titular" class="subtitulo"><b>Endereços do titular</b></span><br />
                                        <asp:GridView ID="gridEnderecosDisponiveis_Titular" runat="server" SkinID="gridPadrao"
                                            DataKeyNames="ID" AutoGenerateColumns="False" Width="90%" OnRowDataBound="gridEnderecosDisponiveis_Titular_RowDataBound"
                                            OnRowCommand="gridEnderecosDisponiveis_Titular_RowCommand">
                                            <Columns>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="Logradouro" HeaderText="Endereço">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="97%" />
                                                </asp:BoundField>
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/Images/search.png" CommandName="usar">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField ButtonType="Button" CommandName="referencia" Text="referência" HeaderText="Tipo">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="1%" />
                                                    <ControlStyle Width="80" BorderWidth="0" BackColor="#507CD1" ForeColor="White" />
                                                </asp:ButtonField>
                                                <asp:ButtonField ButtonType="Button" CommandName="cobranca" Text="cobrança">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="1%" />
                                                    <ControlStyle Width="80" BorderWidth="0" BackColor="#507CD1" ForeColor="White" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>

                                        <span class="subtitulo"><b>Endereços cadastrados</b></span><br />
                                        <asp:GridView ID="gridEnderecosSelecionados" runat="server" SkinID="gridPadrao"
                                            DataKeyNames="ID" AutoGenerateColumns="False" Width="90%" OnRowDataBound="gridEnderecosSelecionados_RowDataBound"
                                            OnRowCommand="gridEnderecosSelecionados_RowCommand">
                                            <Columns>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="Logradouro" HeaderText="Endereço">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Logradouro" HeaderText="Tipo">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="1%" />
                                                </asp:BoundField>
                                                <asp:ButtonField Text="<img src='../../images/delete.png' title='excluir' alt='excluir' border='0' />"
                                                    CommandName="excluir">
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>

                                        <div class="panel panel-default">
                                            <div class="panel-heading">Endereço (visualização)</div>
                                            <div class="panel-body">
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">CEP</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCEP" Width="100%" MaxLength="9" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">Logradouro</label>
                                                    <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtLogradouro" Width="100%" MaxLength="300" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Número</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtNumero" Width="100%" MaxLength="9" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">Complemento</label>
                                                    <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtComplemento" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Bairro</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtBairro" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">Cidade</label>
                                                    <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCidade" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">UF</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtUF" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">Tipo</label>
                                                    <div class="col-xs-4">
                                                        <asp:DropDownList BackColor="LightGray" Enabled="false" Width="100%" runat="server" ID="cboTipoEndereco" SkinID="comboPadrao1">
                                                            <asp:ListItem Text="RESIDENCIAL" Value="0" Selected="True" />
                                                            <asp:ListItem Text="COMERCIAL" Value="1" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-xs-2"><asp:Button ID="cmdEnderecoAcoes" Visible="false" runat="server" SkinID="botaoPadraoINFO_Small" Text="Gravar" OnClick="cmdEnderecoAcoes_Click" /></div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" style="margin-top:-30px">
                                            <div class="panel-heading">Contato</div>
                                            <div class="panel-body">

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">DDD</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDD1" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Fone</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtFone1" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Ramal</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtRamal1" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">DDD</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDD2" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Fone</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtFone2" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Ramal</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtRamal2" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">DDD</label>
                                                    <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDD3" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                    <label class="col-xs-1 control-label">Celular</label>
                                                    <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtFone3" Width="100%" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label">E-mail</label>
                                                    <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" Width="100%" runat="server" ID="txtEmail" BackColor="lightgray" ReadOnly="true" /></div>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="p4" runat="server" Visible="false">
                            <HeaderTemplate>
                                <span class="subtitulo">Dependentes</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <br />
                                <span class="subtitulo">Beneficiário Dependente</span><br />
                                <asp:UpdatePanel runat="server" ID="upDependente" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" Visible="false" ID="pnlSelDependente" EnableViewState="false">
                                            <asp:GridView PageSize="150" ID="gridSelDependente" Width="65%" SkinID="gridViewSkin"
                                                EnableViewState="false" runat="server" AllowPaging="false" AutoGenerateColumns="False"
                                                DataKeyNames="ID" OnRowCommand="gridSelDependente_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />"
                                                        CommandName="usar">
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                            <br />
                                        </asp:Panel>
                                        <table cellpadding="2" width="69%">
                                            <tr>
                                                <td width="120" class="tdPrincipal1">
                                                    CPF
                                                </td>
                                                <td class="tdNormal1" width="143">
                                                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtCPFDependente" Width="112px" />
                                                    <ajaxToolkit:MaskedEditExtender runat="server" EnableViewState="False" ID="meeCPFDependente"
                                                        Mask="999,999,999-99" TargetControlID="txtCPFDependente" />
                                                    &nbsp;
                                                    <asp:ImageButton runat="server" ImageUrl="~/Images/search.png" ToolTip="localizar..."
                                                        ID="cmdCarregaBeneficiarioDependentePorCPF" EnableViewState="False" OnClick="cmdCarregaBeneficiarioDependentePorCPF_Click" />
                                                </td>
                                                <td width="119" class="tdPrincipal1">
                                                    Nome
                                                </td>
                                                <td class="tdNormal1">
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtRGDependente" Width="117px" />
                                                                <asp:ImageButton runat="server" ImageUrl="~/Images/search.png" ToolTip="localizar..."
                                                                    ID="cmdCarregaBeneficiarioDependentePorRG" EnableViewState="False" OnClick="cmdCarregaBeneficiarioDependentePorRG_Click" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:ImageButton Visible="false" ImageUrl="~/images/new.png" ToolTip="alterar beneficiário"
                                                                    EnableViewState="False" runat="server" ID="cmdAlterarBeneficiarioDependente"
                                                                    OnClick="cmdAlterarBeneficiarioDependente_Click" />
                                                                <asp:ImageButton ImageUrl="~/images/new.png" ToolTip="novo beneficiário" EnableViewState="true"
                                                                    runat="server" ID="cmdNovoBeneficiario" />&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <table cellpadding="2" width="69%">
                                            <tr>
                                                <td class="tdPrincipal1" width="120px">
                                                    Data Nasc.
                                                </td>
                                                <td class="tdNormal1" width='143px'>
                                                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDataNascimentoDependente" Width="112px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    <asp:Image SkinID="imgCanlendario" ID="imgDataNascimentoDependente" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimentoDependente" TargetControlID="txtDataNascimentoDependente" PopupButtonID="imgDataNascimentoDependente" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                </td>
                                                <td class="tdPrincipal1" width='119px'>
                                                    Parentesco
                                                </td>
                                                <td class="tdNormal1" colspan="4">
                                                    <asp:DropDownList Width="118px" runat="server" SkinID="txtPadrao" ID="cboParentescoDependente" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Estado Civil
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:DropDownList Width="118px" runat="server" SkinID="txtPadrao" ID="cboEstadoCivilDependente" />
                                                </td>
                                                <td class="tdPrincipal1">
                                                    Data Casamento
                                                </td>
                                                <td class="tdNormal1" colspan="4">
                                                    <asp:TextBox ID="txtDepDataCasamento" Width="80" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    <asp:Image SkinID="imgCanlendario" ID="imgDepDataCasamento" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepDataCasamento" TargetControlID="txtDepDataCasamento" PopupButtonID="imgDepDataCasamento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Matrícula - Saúde
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox ID="txtNumMatriculaSaudeDep" Width="118px" SkinID="txtPadrao" MaxLength="16"
                                                        runat="server" />
                                                </td>
                                                <td class="tdPrincipal1">
                                                    Matrícula - Dental
                                                </td>
                                                <td class="tdNormal1" colspan="4">
                                                    <asp:TextBox ID="txtNumMatriculaDentalDep" Width="80" SkinID="txtPadrao" MaxLength="16"
                                                        runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Nome
                                                </td>
                                                <td colspan="6" class="tdNormal1">
                                                    <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" SkinID="txtPadrao"
                                                        ID="txtNomeDependente" Width="97%" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table cellpadding="2" width="69%">
                                            <tr>
                                                <td width="120" class="tdPrincipal1">
                                                    Sexo
                                                </td>
                                                <td class="tdNormal1">
                                                    <table width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <asp:DropDownList BackColor="lightgray" Width="107px" runat="server" SkinID="txtPadrao"
                                                                    ID="cboSexoDependente" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <table cellpadding="2" width="69%">
                                            <tr>
                                                <td width="120" class="tdPrincipal1">
                                                    Peso
                                                </td>
                                                <td class="tdNormal1" width="56">
                                                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDepPeso" Width="50" />
                                                </td>
                                                <td width="69" class="tdPrincipal1">
                                                    Altura
                                                </td>
                                                <td width="60" class="tdNormal1">
                                                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDepAltura" Width="50" />
                                                </td>
                                                <td width="69" class="tdPrincipal1">
                                                    Admissão
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox EnableViewState="false" runat="server" SkinID="txtPadrao" ID="txtDepAdmissao" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    <asp:Image SkinID="imgCanlendario" ID="imgDepAdmissao" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepAdmissao" TargetControlID="txtDepAdmissao" PopupButtonID="imgDepAdmissao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <span class="subtitulo">Compra de carência</span><br />
                                        <table cellpadding="2" width="69%">
                                            <tr>
                                                <td width="120" class="tdPrincipal1">
                                                    Operadora
                                                </td>
                                                <td colspan="2" class="tdNormal1">
                                                    <asp:TextBox runat="server" ID="cboCarenciaDependenteOperadora" SkinID="txtPadrao" Width="300" />
                                                    <input type="hidden" name="txtCarenciaDependenteOperadoraID" id="txtCarenciaDependenteOperadoraID" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Matrícula
                                                </td>
                                                <td colspan="2" class="tdNormal1">
                                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteMatricula" SkinID="txtPadrao" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Tempo de contrato
                                                </td>
                                                <td colspan="2" class="tdNormal1_NonBold">
                                                    de <asp:TextBox ID="txtDepTempoContratoDe" Width="61" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    <asp:Image SkinID="imgCanlendario" ID="imgDepTempoContratoDe" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepTempoContratoDe" TargetControlID="txtDepTempoContratoDe" PopupButtonID="imgDepTempoContratoDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                    &nbsp;&nbsp;a&nbsp;&nbsp;<asp:TextBox ID="txtDepTempoContratoAte" Width="61" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    <asp:Image SkinID="imgCanlendario" ID="imgDepTempoContratoAte" runat="server" EnableViewState="false" />
                                                    <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepTempoContratoAte" TargetControlID="txtDepTempoContratoAte" PopupButtonID="imgDepTempoContratoAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                    <i><span class="subtitulo"><font size="1">(data do último pagto.)</font></span></i>
                                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteTempoContrato" MaxLength="4" SkinID="txtPadrao" Width="20" Visible="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Código
                                                </td>
                                                <td colspan="2" class="tdNormal1">
                                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteCodigo" SkinID="txtPadrao" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdPrincipal1">
                                                    Portabilidade
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox runat="server" ID="txtDependentePortabilidade" SkinID="txtPadrao" />
                                                </td>
                                                <td class="tdNormal1" align="right" width="1%">
                                                    <asp:ImageButton ImageUrl="~/images/add.gif" EnableViewState="true" runat="server" ID="cmdAddDependente" ToolTip="adicionar ao contrato" OnClick="cmdAddDependente_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <span runat="server" id="spanDependentesCadastrados" class="subtitulo">Dependentes cadastrados</span>
                                        <br />
                                        <asp:GridView Width="69%" ID="gridDependentes" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="ID,BeneficiarioID" SkinID="gridViewSkin" OnRowCommand="gridDependentes_RowCommand"
                                            OnRowDataBound="gridDependentes_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="NumeroSequencial" HeaderText="Núm." ItemStyle-Width="1%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="BeneficiarioNome" HeaderText="Nome">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="ParentescoDescricao" HeaderText="Parentesco">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="EstadoCivilDescricao" HeaderText="EstadoCivil">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Tipo" HeaderText="Tipo">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Data" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Admissão">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='remover do contrato' alt='remover do contrato' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" ForeColor="#CC0000" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editar" ButtonType="Link" Text="<img src='images/edit.png' title='editar cadastro' alt='editar cadastro' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editarDadosDaProposta" ButtonType="Link" Text="<img src='images/detail2.png' title='editar' alt='editar' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="p5" runat="server" Visible="false">
                            <HeaderTemplate>
                                <span class="subtitulo">Ficha de Saúde</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <br />
                                <asp:UpdatePanel runat="server" ID="upFichaSaude" UpdateMode="Always">
                                    <ContentTemplate>
                                        <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" width="70%">
                                            <tr>
                                                <td class="tdPrincipal1" width="90">
                                                    Beneficiário
                                                </td>
                                                <td class="tdNormal1">
                                                    <asp:DropDownList Width="300px" ID="cboBeneficiarioFicha" runat="server" SkinID="dropdownSkin"
                                                        AutoPostBack="True" OnSelectedIndexChanged="cboBeneficiarioFicha_SelectedIndexChanged" />
                                                    &nbsp;
                                                    <asp:ImageButton Visible="false" ID="cmdCarregarComboFichaSaudeBeneficiarios" ImageUrl="~/images/refresh.png"
                                                        ToolTip="atualizar lista" runat="server" OnClick="cmdCarregarComboFichaSaudeBeneficiarios_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <span class="subtitulo">Ficha de saúde</span>
                                        <asp:DataList CellPadding="0" CellSpacing="0" ID="dlFicha" DataKeyField="ID" runat="server"
                                            OnItemCommand="dlFicha_ItemCommand" OnItemDataBound="dlFicha_ItemDataBound">
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <br />
                                                <table cellpadding="3" cellspacing="0" width="600">
                                                    <tr>
                                                        <td colspan="2" bgcolor='#EFF3FB' style="border-left: solid 1px #507CD1; border-top: solid 1px #507CD1; border-bottom: solid 1px #507CD1" align="left">
                                                            <asp:Label ID="lblQuesta" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoTexto") %>' runat="server" />
                                                            <asp:Literal ID="litItemDeclaracaoID" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoID") %>' runat="server" Visible="false" />
                                                        </td>
                                                        <td bgcolor='#EFF3FB' style="border-top: solid 1px #507CD1; border-bottom: solid 1px #507CD1; border-right: solid 1px #507CD1" align="center" width="1%">
                                                            <asp:CheckBox OnCheckedChanged="chkFSim_CheckedChanged" AutoPostBack="true" SkinID="checkboxSkin" ID="chkFSim" runat="server" Checked='<%# Bind("Sim") %>' />
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="tr1Ficha" visible="false">
                                                        <td style="border-left: solid 1px #507CD1">
                                                            Data
                                                        </td>
                                                        <td colspan="2" style="border-right: solid 1px #507CD1">
                                                            Descrição
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="tr2Ficha" visible="false">
                                                        <td style="border-left: solid 1px #507CD1; border-bottom: solid 1px #507CD1">
                                                            <asp:TextBox SkinID="txtPadrao" Width="66px" runat="server" ID="txtFichaSaudeData" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                        </td>
                                                        <td width="90%" colspan="2" style="border-bottom: solid 1px #507CD1; border-right: solid 1px #507CD1">
                                                            <asp:TextBox ID="txtFichaSaudeDescricao" Width="99%" SkinID="txtPadrao" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' />
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="tr4Ficha" visible="false" >
                                                        <td style="border-left: solid 1px #507CD1" width="5%">
                                                            CID
                                                        </td>
                                                        <td style="" width="10%">
                                                            Observações
                                                        </td>
                                                        <td style="border-right: solid 1px #507CD1">
                                                            Aprovado?
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="tr5Ficha" visible="false">
                                                        <td style="border-left: solid 1px #507CD1;border-bottom: solid 1px #507CD1" valign="top">
                                                            Inicial 
                                                            <asp:TextBox Width="30" MaxLength="4" SkinID="txtPadrao" ID="txtCIDInicial" runat="server" Text='<%# Bind("CIDInicial") %>' />
                                                            &nbsp;
                                                            Final 
                                                            <asp:TextBox Width="30" MaxLength="4" SkinID="txtPadrao" ID="txtCIDFinal" runat="server" Text='<%# Bind("CIDFinal") %>' />
                                                        </td>
                                                        <td style="border-bottom: solid 1px #507CD1" valign="top">
                                                            <asp:TextBox Width="99%" MaxLength="500" TextMode="MultiLine" Height="50" SkinID="txtPadrao" ID="txtOBSMedico" runat="server" Text='<%# Bind("OBSMedico") %>' />
                                                        </td>
                                                        <td style="border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1" valign="top">
                                                            <asp:CheckBox ID="chkAprovado" runat="server" Checked='<%# Bind("AprovadoPeloMedico") %>' />
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="tr3Ficha" visible="false">
                                                        <td colspan="3" style="border-left: solid 1px #507CD1; border-bottom: solid 1px #507CD1;border-right: solid 1px #507CD1" align="center">
                                                            <asp:Button ID="cmdSalvarFicha" SkinID="botaoPequeno" Text="salvar" runat="server" CommandName="salvar" CommandArgument="<%# Container.ItemIndex %>" />
                                                            <asp:Literal runat="server" EnableViewState="false" ID="litFichaAviso" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:DataList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
        
                        <ajaxToolkit:TabPanel ID="p6" runat="server">
                            <HeaderTemplate>
                                <span class="subtitulo">Status</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="upFinalizacao" UpdateMode="Conditional" runat="server" >
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label class="col-md-4" style="padding-top:7px">Data cadastro</label>
                                                    <div class="col-md-8">
                                                        <asp:TextBox runat="server" BackColor="lightgray" ReadOnly="true" SkinID="txtPadrao" ID="txtDataContrato" Width="90" />
                                                    </div>
                                                </div>
                                                <asp:Panel ID="pnlTempFinalizacao" runat="server" EnableViewState="false" Visible="false">
                                                    <div class="form-group">
                                                        <label class="col-md-4">Desconto R$</label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox runat="server" ReadOnly="false" SkinID="txtPadrao" ID="txtDesconto" Width="90" AutoPostBack="true" OnTextChanged="txtDesconto_TextChanged" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-md-4">Valor R$</label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox runat="server" ReadOnly="false" SkinID="txtPadrao" Text="0,00" ID="txtValorTotal" Width="90" />
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-md-12">
                                                            <asp:CheckBox ID="chkCobrarTaxa" Text="Cobrar taxa associativa" SkinID="checkboxSkin2" runat="server" AutoPostBack="true" OnCheckedChanged="chkCobrarTaxa_CheckedChanged" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>

                                                <div class="form-group">
                                                    <div class="col-md-12">
                                                        <asp:Literal ID="litSumario" runat="server" Visible="false" />
                                                    </div>
                                                </div>
                                                <p class="alert alert-warning">Observações</p>
                                                <div class="form-group">
                                                    <div class="col-md-12">
                                                        <asp:TextBox ID="txtObsEdit" runat="server" SkinID="txtPadrao" Width="100%" TextMode="MultiLine" />
                                                        <asp:TextBox ID="txtObs"     runat="server" SkinID="txtPadrao" Width="100%" TextMode="MultiLine" />
                                                    </div>
                                                </div>

                                            </div>
                                            <div class="col-md-6">
                                                <div class="alert alert-success">Status da proposta</div>

                                                <div class="form-group">
                                                    <div class="col-md-3">
                                                        <asp:RadioButton ID="optNormal" onclick='return false;' Checked="true" Text="Normal" GroupName="status" runat="server" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <asp:RadioButton ID="optInativo" onclick='return false;' Text="Inativo" GroupName="status" runat="server" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <asp:RadioButton ID="optCancelado" onclick='return false;' Text="Cancelado" GroupName="status" runat="server" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <asp:LinkButton CssClass="btn btn-info btn-sm" ID="lnkAlterarStatus" Text="alterar" runat="server" OnClick="lnkAlterarStatus_Click" />
                                                    </div>
                                                </div>

                                                <div class="alert alert-warning">Histórico de mudança de status da proposta</div>

                                                <div class="form-group">
                                                    <div class="col-md-12">
                                                        <asp:GridView ID="gridHistoricoStatus" runat="server" SkinID="gridPadrao"
                                                            DataKeyNames="ID,StatusID" AutoGenerateColumns="False" Width="100%">
                                                            <Columns>
                                                                <asp:BoundField ItemStyle-Wrap="false" DataField="StatusDescricao" HeaderText="Motivo" Visible="true">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField ItemStyle-Wrap="false" DataField="StatusTipoTRADUZIDO" HeaderText="Tipo">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField ItemStyle-Wrap="false" DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle Width="1%" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <table width="100%" border='0' runat="server" enableviewstate="false" visible="false">
                                            <tr>
                                                <td valign="top" width="50%">
                                                    <table cellpadding="2" width="100%">
                                                        <tr>
                                                            <td class="tdPrincipal1" width="90px">
                                                
                                                            </td>
                                                            <td class="tdNormal1">
                                                
                                                            </td>
                                                        </tr>
                                                        <tr id="trDesconto" runat="server">
                                                            <td class="tdPrincipal1" width="90px">
                                                
                                                            </td>
                                                            <td class="tdNormal1">
                                                
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdPrincipal1" width="90px">
                                                            </td>
                                                            <td class="tdNormal1">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <br />
                                                    <table cellpadding="2" width="100%">
                                                        <tr>
                                                            <td class="tdPrincipal1">
                                                
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table align="center" cellpadding="2" cellspacing="0" width="300" runat="server" id="tblMsgRegras" visible="false">
                                                        <tr>
                                                            <td colspan="2">
                                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td align="center">
                                                                            <b><font color='red'>A seguintes regras foram quebradas</font></b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="tdNormal1">
                                                                            <asp:Literal runat="server" ID="litMsgErroRegra" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" bgcolor="whitesmoke" style="border: solid 1px black" align="center">
                                                                <b><font color='blue'>Liberação</font></b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="50" style="border-left: solid 1px black">
                                                                <b>Usuário</b>
                                                            </td>
                                                            <td width="250" style="border-right: solid 1px black">
                                                                <asp:TextBox Width="95%" TextMode="Password" MaxLength="75" runat="server" SkinID="txtPadrao"
                                                                    ID="txtLogin" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="border-left: solid 1px black">
                                                                <b>Senha</b>
                                                            </td>
                                                            <td style="border-right: solid 1px black">
                                                                <asp:TextBox Width="95%" TextMode="Password" ReadOnly="true" MaxLength="40" runat="server" SkinID="txtPadrao" ID="txtSenha" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="border-left: solid 1px black; border-bottom: solid 1px black;
                                                                border-right: solid 1px black" align="center">
                                                                <asp:Button OnClick="cmdLiberar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false"
                                                                    ID="cmdLiberar" Text="Liberar" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <br />
                                    
                                                    <br />
                                                    <table cellpadding="2" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td class="tdPrincipal1" valign="top">
                                                                <b>Observações</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" width="50%" bgcolor="red">
                                                    <table cellpadding="2" width="100%" style="top:1px; padding-top:1px">
                                                        <tr>
                                                            <td class="tdPrincipal1" align="center" colspan="2" height="20">
                                                
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="90px" class="tdPrincipal1">
                                                                Status
                                                            </td>
                                                            <td class="tdNormal1">
                                                
                                                
                                                
                                                                &nbsp;
                                                
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table cellpadding="0" width="100%" border='0'>
                                                        <tr>
                                                            <td class="tdPrincipal1" align="center" height="20">
                                                
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>

                                            <ajaxToolkit:ModalPopupExtender ID="mpeAlteraStatus" runat="server" PopupControlID="pnlAlteraStatus" CancelControlID="cmdFecharHistoricoStatus" TargetControlID="target2">
                                            </ajaxToolkit:ModalPopupExtender>
                                            <asp:Panel ID="pnlAlteraStatus" EnableViewState="true" runat="server">
                                                <asp:LinkButton runat="server" EnableViewState="false" ID="target2" />
                                                <table width="500" cellpadding="0" cellspacing="4" style="border:solid 4px gray;background-color:white">
                                                    <tr>
                                                        <td class="tdPrincipal1" align="center" colspan="2" height="20">
                                                            Status da proposta
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdNormal1">Selecione</td>
                                                        <td class="tdNormal1">
                                                            <asp:RadioButton ID="optNormalEdit"    Text="Reativação"   GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" Checked="true" />
                                                            <asp:RadioButton ID="optInativoEdit"   Text="Inativação"   GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" />
                                                            <asp:RadioButton ID="optCanceladoEdit" Text="Cancelamento" GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdNormal1">Motivo</td>
                                                        <td class="tdNormal1"><asp:DropDownList ID="cboStatusMotivo" runat="server" Width="300" SkinID="dropdownSkin" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdNormal1">Data</td>
                                                        <td class="tdNormal1"><asp:TextBox runat="server" EnableViewState="false" CssClass="textbox" ID="txtDataInativacao" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" /></td>
                                                    </tr>
                                                    <tr height='10'><td height='10' style="border-top:solid 1px Gray" colspan="2"></td></tr>
                                                    <tr>
                                                        <td align="center" colspan="2">
                                                            <asp:Button Text="Fechar" SkinID="botaoPequeno" ID="cmdFecharHistoricoStatus" runat="server" EnableViewState="false" />
                                                            <asp:Button Text="Salvar" SkinID="botaoPequeno" ID="cmdSalvarHistoricoStatus" runat="server" EnableViewState="false" OnClick="cmdSalvarHistoricoStatus_Click" OnClientClick="return confirm('Deseja realmente alterar o status da proposta?');" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel ID="p5a" runat="server">
                            <HeaderTemplate>
                                <span class="subtitulo">Utilização</span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel runat="server" ID="upAdicionais" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlTempAdicional" EnableViewState="false" Visible="false" runat="server">
                                            <br />
                                            <table cellpadding="2" width="70%">
                                                <tr>
                                                    <td class="tdPrincipal1" width="90">
                                                        Beneficiário
                                                    </td>
                                                    <td class="tdNormal1">
                                                        <asp:DropDownList Width="300px" ID="cboBeneficiarioAdicional" runat="server" SkinID="dropdownSkin"
                                                            AutoPostBack="True" OnSelectedIndexChanged="cboBeneficiarioAdicional_SelectedIndexChanged" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <span class="subtitulo">Adicionais disponíveis</span><br />
                                            <asp:GridView ID="gridAdicional" runat="server" SkinID="gridViewSkin" DataKeyNames="AdicionalID,ID"
                                                AutoGenerateColumns="False" Width="650px" OnRowDataBound="gridAdicional_RowDataBound"
                                                OnRowCommand="gridAdicional_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="AdicionalDescricao" HeaderText="Produto">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle Wrap="False" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox OnCheckedChanged="checkboxGridAdicional_Changed" AutoPostBack="true"
                                                                SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>

                                        <asp:Panel ID="pnlUtilizacaoPF" runat="server" Visible="true">
                                            <div class="alert alert-info">
                                                 <asp:Literal ID="litSaldo" Text="<strong>Saldo:</strong> R$ 520,00" runat="server" />
                                            </div>
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <label class="col-md-12 text-left">Informe o período:</label>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="col-md-2"  style="padding-left:0px;">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">
                                                                de&nbsp;
                                                                </span>
                                                                <asp:TextBox ID="txtDataDeUtilizacao" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2" style="padding-left:0px;">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">
                                                                até
                                                                </span>
                                                                <asp:TextBox ID="txtDataAteUtilizacao" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2" style="padding-left:0px;">
                                                            <asp:Button ID="cmdVisualizarUtilizacao" Text="pesquisar" SkinID="botaoPadraoINFO_Small" runat="server" style="padding-top:-25px" OnClick="cmdVisualizarUtilizacao_Click"/>
                                                        </div>
                                                        <div class="col-md-6" style="padding-left:0px;">
                                                            <asp:Button ID="cmdOperacoesManuais" Text="oper. manuais" SkinID="botaoPadraoINFO_Small" runat="server" style="padding-top:-25px" OnClick="cmdOperacoesManuais_Click"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-xs-12">
                                                    <asp:GridView ID="gridUtilizacao" runat="server" SkinID="gridPadrao" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID">
                                                        <Columns>
                                                            <%--<asp:BoundField DataField="Prestador" HeaderText="Prestador" />
                                                            <asp:BoundField DataField="Especialidade" HeaderText="Especialidade" />
                                                            <asp:BoundField DataField="Procedimento" HeaderText="Procedimento" />
                                                            <asp:BoundField DataField="Valor" HeaderText="Valor" />--%>
                                                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                                            <asp:BoundField DataField="Data" HeaderText="Data" ItemStyle-Width="1%" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Wrap="false" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                            <div class="alert alert-danger" style="height:60px">
                                                <div class="row">
                                                    <label class="col-xs-1" style="margin-top:2px">Senha</label>
                                                    <div class="col-xs-2">
                                                        <asp:TextBox ID="txtSenhaContrato" MaxLength="20" TextMode="Password" ReadOnly="true" runat="server" SkinID="txtPadrao" Width="100%" />
                                                    </div>
                                                    <div class="col-xs-2 text-left">
                                                        <input type="button" id="cmdAlterarSenha" value="alterar senha" class="btn btn-info btn-sm" onclick="if (document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').readOnly) { document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').readOnly = false; document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').focus(); document.getElementById('cmdAlterarSenha').value = 'proteger senha'; document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').type = 'text'; } else { document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').readOnly = true; document.getElementById('cmdAlterarSenha').value = 'alterar senha'; document.getElementById('ctl00_content_tab_p5a_txtSenhaContrato').type = 'password'; }" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlUtilizacaoPJ" runat="server" Visible="false">
                                            <div class="alert alert-info">
                                                 <div class="form-group">
                                                     <label class="col-xs-2 control-label">Vidas cobertas</label>
                                                     <div class="col-md-10" style="margin-top:3px">
                                                         <asp:TextBox ID="txtVidasCobertas" Width="80px" MaxLength="4" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event);" />
                                                     </div>
                                                 </div>
                                            </div>
                                        </asp:Panel>
                                        <br />
                                        <div class="alert alert-danger">
                                        <div class="form-group">
                                            <div class="col-md-10">
                                                <b>Emissão de cobranças</b>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:DropDownList ID="cboFormaEmissaoCobranca" runat="server" SkinID="comboPadrao1" Width="100%">
                                                    <asp:ListItem Text="Emissão padrão" Value="0" />
                                                    <asp:ListItem Text="Emissão via Iugu" Value="1" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel ID="p7Atendimento" runat="server" Visible="true">
                            <HeaderTemplate>
                                <asp:Literal ID="litAtendimentoHeader" runat="server" Text="<span class='subtitulo'>Atendimento</span>" />
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel runat="server" ID="upAtendimento" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="alert alert-info">Histórico do atendimento</div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="alert alert-info">Detalhe do atendimento</div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:GridView ID="gridAtendimento" runat="server" SkinID="gridPadrao" DataKeyNames="ID" style="margin-top:1px"
                                                    AutoGenerateColumns="False" Width="100%" PageSize="10" OnRowDataBound="gridAtendimento_RowDataBound"
                                                    OnRowCommand="gridAtendimento_RowCommand" OnPageIndexChanging="gridAtendimento_PageIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="ID" HeaderText="Protocolo" Visible="true">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="TituloOuCategoria" HeaderText="Atendimento">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="DataInicio" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle Width="1%" />
                                                        </asp:BoundField>
                                                        <%--<asp:ButtonField ButtonType="Image" ImageUrl="~/Images/search.png" Text="ver" CommandName="detalhe">--%>
                                                        <asp:ButtonField ButtonType="Link" Text="<img border='0' alt='ver' title='ver' src='../../images/search.png' />" CommandName="detalhe">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:ButtonField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label class="col-md-2">Protocolo</label>
                                                    <div class="col-md-10"><asp:Literal ID="lblAtendimentoProtocolo" runat="server" Text="-------" /></div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2">Texto</label>
                                                    <div class="col-md-10">
                                                        <asp:TextBox Width="100%" Height="78" ID="txtTexto"  TextMode="MultiLine" runat="server" SkinID="txtPadrao" />
                                                        <asp:TextBox Width="100%" Height="78" ID="txtTexto2" TextMode="MultiLine" runat="server" SkinID="txtPadrao" Visible="false" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2">Data</label>
                                                    <div class="col-md-3">
                                                        <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDataInicio" Width="100%" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    </div>
                                                    <div class="col-md-2">
                                                        <asp:Image SkinID="imgCanlendario" ID="imgDataInicio" runat="server" EnableViewState="false" />
                                                        <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataInicio" TargetControlID="txtDataInicio" PopupButtonID="imgDataInicio" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                    </div>
                                                    <div class="col-md-5 text-left">
                                                        <asp:Literal ID="litCriadoPor" runat="server" EnableViewState="false" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2">Tipo</label>
                                                    <div class="col-md-10">
                                                        <asp:DropDownList ID="cboTipoAtendimento" SkinID="comboPadrao1" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboTipoAtendimento_SelectedIndexChanged" runat="server" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2">Subtipo</label>
                                                    <div class="col-md-10">
                                                        <asp:DropDownList ID="cboSubTipoAtendimento" SkinID="comboPadrao1" Width="100%" runat="server" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2">Previsão</label>
                                                    <div class="col-md-3">
                                                        <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDataPrevisao" Width="100%" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    </div>
                                                    <div class="col-md-7">
                                                        <asp:Image SkinID="imgCanlendario" ID="imgDataPrevisao" runat="server" EnableViewState="false" />
                                                        <ajaxToolkit:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataPrevisao" TargetControlID="txtDataPrevisao" PopupButtonID="imgDataPrevisao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                    </div>
                                                    <div class="col-md-5"></div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-5">
                                                        <asp:CheckBox ID="chkAtendimentoConcluido" Text="Concluído" runat="server" SkinID="checkboxSkin" />
                                                    </div>
                                                    <div class="col-md-7 text-left">
                                                        <asp:Literal ID="litResolvidoPor" runat="server" EnableViewState="true" />
                                                    </div>
                                                </div>
                                                <hr />
                                                <div class="form-group">
                                                    <div class="col-md-6 text-center">
                                                        <asp:Button ID="cmdFecharAtendimento" Font-Size="10px" Text="Novo" SkinID="botaoPadraoINFO_Small" OnClick="cmdFecharAtendimento_Click" runat="server" />
                                                    </div>
                                                    <div class="col-md-6 text-center">
                                                        <asp:Button ID="cmdSalvarAtendimento" Font-Size="10px" Text="Gravar" SkinID="botaoPadraoINFO_Small" OnClick="cmdSalvarAtendimento_Click" runat="server" />
                                                    </div>
                                                </div>
                                                <hr />
                                            </div>
                                        </div>
                                        <hr />
                                        <asp:Panel ID="pnlAtendimento" runat="server" Visible="true">
                                            <table width="100%" runat="server" enableviewstate="false" visible="false">
                                                <tr>
                                                    <td id="tdAtendimento" runat="server">
                                                        <table cellpadding="0" cellspacing="1" width="100%">
                                                            <tr>
                                                                <td valign="top" width="50%">

                                                                    <asp:Button Visible="false" ID="cmdNovoAtendimento" Font-Size="10px" Text="Novo" SkinID="botaoAzulBorda" runat="server" />
                                                                </td>
                                                                <td valign="top">
                                                                    <table style="border: solid 0px #507CD1" width="100%">
                                                                        <tr id="trAssunto" runat="server" visible="false" enableviewstate="false">
                                                                            <td>
                                                                                Assunto
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox Width="200" ID="txtTitulo" runat="server" SkinID="txtPadrao" MaxLength="250" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top">
                                                                
                                                                            </td>
                                                                            <td>
                                                                
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                                &nbsp;
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                
                                                                
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="center">
                                                                
                                                                                &nbsp;&nbsp;&nbsp;
                                                                
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="p8Cobranca" runat="server">
                            <HeaderTemplate>
                                <asp:Literal ID="litCobrancaHeader" runat="server" Text="<span class='subtitulo'>Cobranças</span>" />
                            </HeaderTemplate>
                            <ContentTemplate>
                            <asp:UpdatePanel runat="server" ID="upCobrancas">
                                <ContentTemplate>
                                    <br />
                                    <div class="form-group">
                                        <label class="col-xs-2 control-label">E-mail para envio</label>
                                        <div class="col-xs-3"><asp:TextBox ID="txtEmailAtendimento" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="70" /></div>
                                        <label class="col-xs-2 control-label">Cópia para</label>
                                        <div class="col-xs-3"><asp:TextBox ID="txtEmailAtendimentoCC" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="70" /></div>
                                    </div>
                                    <hr />
                                    <div class="form-group">
                                        <label class="col-xs-2 control-label text-left">Nova cobrança</label>
                                        <div class="col-xs-2">
                                            <div class="input-group">
                                                <span class="input-group-addon">Vencto.</span>
                                                <asp:TextBox ID="txtVencimentoCob" BackColor="WhiteSmoke" Width="90px" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-xs-2">
                                            <div class="input-group">
                                                <span class="input-group-addon">Competência</span>
                                                <asp:TextBox ID="txtCompetencia" BackColor="WhiteSmoke" Width="60px" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event);" MaxLength="6" />
                                            </div>
                                        </div>
                                        <div class="col-xs-2">
                                            <asp:Panel ID="pnlTipoCobrancaNormal" runat="server">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Valor</span>
                                                    <asp:TextBox ID="txtValorCob" BackColor="WhiteSmoke" Width="100%" MaxLength="10" SkinID="txtPadrao" Text="0,00" runat="server" />
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlTipoCobrancaPJ" runat="server" Visible="false">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Vidas</span>
                                                    <asp:TextBox ID="txtQtdVidasCob" BackColor="WhiteSmoke" Width="100%" MaxLength="4" SkinID="txtPadrao" Text="1" runat="server" onkeypress="return filtro_SoNumeros(event);" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        <div class="col-xs-1">
                                            <asp:Button EnableViewState="False" ID="cmdGerarCobranca" Text="Gerar cobrança" SkinID="botaoPadraoINFO" OnClick="cmdGerarCobranca_Click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar uma nova cobrança?');" runat="server" Font-Size="11px" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-xs-10">
                                            <asp:CheckBox ID="chkTodasCobrancas" ForeColor="Gray" Font-Size="9pt" Text="Exibir também as cobranças canceladas" runat="server" AutoPostBack="true" OnCheckedChanged="chkTodasCobrancas_CheckedChanged" />
                                            <asp:GridView ID="gridCobranca" runat="server" SkinID="gridPadrao" DataKeyNames="ID,HeaderParcID,HeaderItemID,Cancelada"
                                                AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridCobranca_RowDataBound"
                                                OnRowCommand="gridCobranca_RowCommand" OnPageIndexChanging="gridCobranca_PageIndexChanging"
                                                AllowPaging="True" PageSize="15">
                                                <Columns>
                                                    <asp:BoundField DataField="Parcela" HeaderText="Parc." Visible="false">
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Valor">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkValor" CommandArgument='<%# Container.DataItemIndex %>' runat="server"
                                                                Font-Size="10px" Text='<%# Bind("Valor", "{0:C}") %>' CommandName="detalhe" />
                                                            <asp:Panel ID="pnlComposite" runat="server" Visible="false" >
                                                                <asp:Literal ID="litComposite" runat="server" Text="aguardando..." />
                                                            </asp:Panel>
                                                            <ajaxToolkit:BalloonPopupExtender Enabled="false" ID="balloon" UseShadow="false" DisplayOnClick="false"
                                                                DisplayOnMouseOver="true" BalloonPopupControlID="pnlComposite" runat="server"
                                                                TargetControlID="lnkValor" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="ValorPgto" HeaderText="Valor Pago" DataFormatString="{0:C}">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Vencimento">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtVenctoCobGrid" Width="90px" runat="server" Font-Size="10px" Text='<%# Bind("DataVencimento", "{0:dd/MM/yyyy}") %>'
                                                                SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="strEnviado" HeaderText="Enviado">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="strPago" HeaderText="Pago">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="strDataPago" HeaderText="Data Pgto">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="Competencia" HeaderText="Compet.">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>

                                                    <asp:ButtonField Text="<img src='../../images/mail.gif' border='0' alt='enviar e-mail' title='enviar e-mail' />"
                                                        CommandName="email">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Visible="true" ButtonType="Image" ImageUrl="~/images/edit.png"
                                                        Text="alterar" CommandName="detalhe">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Text="<img src='../../images/print.png' border='0' alt='imprimir' title='imprimir' />" 
                                                        CommandName="print">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:ButtonField>
                                                </Columns>
                                                <PagerSettings PageButtonCount="30" />
                                            </asp:GridView>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <div class="col-md-10">
                                            <b>Acréscimos e descontos agendados</b>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-10">
                                            <label class="col-md-2 control-label text-left">Tipo</label>
                                            <div class="col-md-2">
                                                <asp:DropDownList ID="cboTipoDescontoAcrescimo" runat="server" SkinID="comboPadrao1" Width="100%">
                                                    <asp:ListItem Text="Nenhum" Value="0" Selected="True" />
                                                    <asp:ListItem Text="Acréscimo" Value="1" />
                                                    <asp:ListItem Text="Desconto" Value="2"  />
                                                </asp:DropDownList>
                                            </div>
                                            <label class="col-md-2 control-label text-left">Valor</label>
                                            <div class="col-md-2">
                                                <asp:TextBox ID="txtValorDecontoAcrescimo" Width="100%" MaxLength="15" SkinID="txtPadrao" Text="0,00" runat="server" onkeypress="filtro_SoNumeros(event);" />
                                            </div>
                                            <label class="col-md-2 control-label text-left">Até</label>
                                            <div class="col-md-2">
                                                <asp:TextBox ID="txtDataDescontoAcrescimo" Width="100%" MaxLength="10" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event);mascara_DATA(this, event);" />
                                            </div>
                                        </div>
                                    </div>
                                    

                                    <asp:Panel ID="pnlConfComissao" runat="server" EnableViewState="false" Visible="false">
                                        <div class="form-group" style="text-align:left">
                                            <label class="col-xs-10 control-label left">Regras para início de comissionamento</label>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-xs-2 control-label">Estágio</label>
                                            <div class="col-xs-2">
                                                <asp:DropDownList runat="server" ID="cboComissaoEstagio" Width="100%" SkinID="comboPadrao1">
                                                    <asp:ListItem Text="Vitalício" Value="0" Selected="True" />
                                                    <asp:ListItem Text="Parcela 1" Value="1" />
                                                    <asp:ListItem Text="Parcela 2" Value="2" />
                                                    <asp:ListItem Text="Parcela 3" Value="3" />
                                                    <asp:ListItem Text="Parcela 4" Value="4" />
                                                    <asp:ListItem Text="Parcela 5" Value="5" />
                                                </asp:DropDownList>
                                            </div>
                                            <label class="col-xs-2 control-label">Em</label>
                                            <div class="col-xs-2">
                                                <asp:TextBox ID="txtcomissaoInicioEm" runat="server" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" Width="90"/>
                                            </div>
                                            <div class="col-xs-2">
                                                <asp:Button ID="cmdSalvarComissaoConf" Text="gravar" runat="server" SkinID="botaoPadraoINFO_Small" OnClick="cmdSalvarComissaoConf_Click" />
                                            </div>
                                            <div class="col-xs-2">
                                                <asp:Button ID="cmdExcluirComissaoConf" Text="excluir" runat="server" SkinID="botaoPadraoDANGER_Small" OnClick="cmdExcluirComissaoConf_Click" />
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <table runat="server" enableviewstate="false" visible="false" style="border: solid 1px #507CD1" width="800" cellpadding="3" cellspacing="0">
                                        <tr>
                                            <td class="tdNormal1" valign="top" colspan="2">
                                                <asp:Label ID="lblCodigoCliente" runat="server" Visible="false" />
                                            </td>
                                            <td class="tdPrincipal1"  align="center" rowspan="2" valign="top">
                                            
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="border-top: solid 1px #507CD1" colspan="2" valign="top">
                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                    <asp:Panel ID="pnlHideCob" runat="server" Visible="false" EnableViewState="false"></asp:Panel>
                                                    <tr>
                                                        <td colspan="2">
                                                            <b>Gerar nova parcela</b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" height="4">
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" visible="false" enableviewstate="false">
                                                        <td width="60">
                                                            Parcela
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtParcelaCob" ReadOnly="True" BackColor="WhiteSmoke" Width="25px"
                                                                MaxLength="4" SkinID="txtPadrao" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" visible="false" enableviewstate="false">
                                                        <td colspan="2" height="4"></td>
                                                    </tr>
                                                    <tr>
                                                        <td width="50">
                                                            Vencimento
                                                        </td>
                                                        <td>
                                                        
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" height="4">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Valor
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                            <asp:ImageButton Visible="False" EnableViewState="False" ID="btnCalcularValorCob"
                                                                ImageUrl="~/images/refresh.png" ImageAlign="Bottom" ToolTip="calcular valor"
                                                                runat="server" OnClick="btnCalcularValorCob_Click" />&nbsp;&nbsp;
                                                        
                                                        </td>
                                                    </tr>
                                                    <asp:Panel runat="server" ID="pnlTemp" Visible="false">
                                                    <tr height="10">
                                                        <td height="10">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="2">
                                                            <br />
                                                            <asp:LinkButton ID="cmdRelacaoCobrancas" runat="server" OnClick="cmdRelacaoCobrancas_Click" Text="Relação de parcelas" />
                                                            <br />
                                                            <asp:LinkButton ID="cmdTabelaValor" runat="server" OnClick="cmdTabelaValor_Click" Text="Consultar tabela de valores" />
                                                            <hr width='250' align="center" />
                                                            <b><asp:Label EnableViewState="false" ID="cmdDemonsPagto" runat="server" Text="Demonstrativo de pagamentos" /> <br /></b>
                                                            Ps Padrão:&nbsp;
                                                            <asp:ImageButton Visible="true" ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="imgDemonstPagtoPrint" runat="server" OnClick="imgDemonstPagtoPrint_Click"/>&nbsp;
                                                            <asp:ImageButton Visible="true" ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="true"  ID="imgDemonstPagtoMail" runat="server" OnClick="imgDemonstPagtoMail_Click" OnClientClick="return confirm('Deseja realmente enviar o DEMONSTRATIVO DE PAGTOS para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                                            <br />
                                                            Qualicorp:&nbsp;
                                                            <asp:ImageButton Visible="true" ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="imgDemonstPagtoQualiPrint" runat="server" OnClick="imgDemonstPagtoQualiPrint_Click"/>&nbsp;
                                                            <asp:ImageButton Visible="true" ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="true"  ID="imgDemonstPagtoQualiMail" runat="server" OnClick="imgDemonstPagtoQualiMail_Click" OnClientClick="return confirm('Deseja realmente enviar o DEMONSTRATIVO DE PAGTOS para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                                            <br />
                                                            <b><asp:Label EnableViewState="false" ID="lblCartaDePermanecia" Text="Carta de permanência:" runat="server" /></b> &nbsp;
                                                            <asp:ImageButton ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="cmdCartaDePermanenciaPrint" runat="server" OnClick="cmdCartaDePermanenciaPrint_Click"/>&nbsp;
                                                            <asp:ImageButton ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="cmdCartaDePermanecia"       runat="server" OnClick="cmdCartaDePermanecia_Click" OnClientClick="return confirm('Deseja realmente enviar a CARTA DE PERMANÊNCIA para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                                            <br />
                                                            <b><asp:Label EnableViewState="false" ID="Label1" Text="Termo de quitação Anual:" runat="server" /></b> &nbsp;
                                                            <asp:ImageButton ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="cmdTermoAnualPrint" runat="server" OnClick="cmdTermoAnualPrint_Click"/>&nbsp;
                                                            <asp:ImageButton ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="cmdTermoAnual"       runat="server" OnClick="cmdTermoAnual_Click" OnClientClick="return confirm('Deseja realmente enviar o TERMO DE QUITAÇÃO para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                                        </td>
                                                    </tr>
                                                    </asp:Panel>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </div>
            </div>
        </div>
    </div>

    <table cellpadding="2" width="70%">
        <tr>
            <td align="left">
                <asp:Button Width="70" OnClick="cmdVoltar_Click" SkinID="botaoPadrao1" runat="server"
                    EnableViewState="false" ID="cmdVoltar" Text="Voltar" />
            </td>
            <td align="right">
                <asp:UpdatePanel ID="upSalvar" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:Button EnableViewState="true" Width="70" SkinID="botaoPadrao1" runat="server" ID="cmdSalvar"
                            Text="Salvar" OnClick="cmdSalvar_Click" />&nbsp;
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>

    <asp:UpdatePanel ID="upCobrancaDetalhe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <ajaxToolkit:ModalPopupExtender ID="mpeOperacoesManuais" runat="server" PopupControlID="pnlOperacoesManuais" CancelControlID="cmdFecharOpManuais" TargetControlID="targetOpMan">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlOperacoesManuais" EnableViewState="false" runat="server">
                <asp:LinkButton runat="server" EnableViewState="false" ID="targetOpMan" />
                <table width="500" cellpadding="2" cellspacing="4" style="border-top:solid 1px gray;border-left:solid 1px gray;border-right:solid 1px gray;background-color:white">
                    <tr>
                        <td align="center" valign="middle" style="background-color:white;">
                            <div class="alert alert-info"><h4>Operação manual</h4></div>
                        </td>
                    </tr>
                </table>
                <table width="500px" cellpadding="2" cellspacing="4" style="border-left:solid 1px gray;border-right:solid 1px gray;background-color:white">
                    <tr>
                        <td width="100px"><b>Tipo</b></td>
                        <td>
                            <asp:RadioButton ID="optCredito" Text="Crédito" GroupName="cred" runat="server" />
                            &nbsp;
                            <asp:RadioButton ID="optDebito" Text="Débito" GroupName="cred" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Valor</b></td>
                        <td>
                            <asp:TextBox ID="txtOpManualValor" Width="45px" Text="0,00" SkinID="textboxSkin" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Comentário</b></td>
                        <td>
                            <asp:TextBox ID="txtOpManualObs" SkinID="textboxSkin" Width="400px" MaxLength="455" runat="server" />
                        </td>
                    </tr>
                </table>
                <table width="500px" cellpadding="2" cellspacing="4" style="border-bottom:solid 1px gray;border-left:solid 1px gray;border-right:solid 1px gray;background-color:white">
                    <tr>
                        <td align="center">
                            <asp:Button Text="Fechar" SkinID="botaoPadraoINFO_Small" ID="cmdFecharOpManuais" runat="server" EnableViewState="false" />
                            &nbsp;
                            <asp:Button Text="Salvar" SkinID="botaoPadraoINFO_Small" ID="cmdSalvarOpManuais" runat="server" EnableViewState="false" OnClick="cmdSalvarOpManuais_Click" OnClientClick="return confirm('Deseja realmente prosseguir');" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>


            <ajaxToolkit:ModalPopupExtender ID="mpeCobrancaDetalhe" runat="server" PopupControlID="pnlCobrancaDetalhe" CancelControlID="cmdFecharCobrancaDetalhe" TargetControlID="target"></ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCobrancaDetalhe" EnableViewState="true" runat="server">
                <asp:LinkButton runat="server" EnableViewState="false" ID="target" />
                <asp:TextBox ID="txtIdCobrancaEmDetalhe" Visible="false" runat="server" />
                <table width="400px" cellpadding="2" cellspacing="4" style="border:solid 4px gray;background-color:white">
                    <tr>
                        <td colspan="2" align="center" height='30' valign="middle" style="background-color:Gray;color:White">
                            <asp:Literal ID="litTitulo" runat="server" EnableViewState="true" />
                        </td>
                    </tr>
                    <tr runat="server" visible="false" enableviewstate="false">
                        <td colspan="2" align="center" height='20' valign="middle" style="background-color:Whitesmoke;">
                            <b>Composição da Parcela</b>&nbsp;&nbsp;
                            <asp:ImageButton ID="cmdRecalcularComposicao" OnClientClick="return confirm('Deseja recalcular a composição da parcela?');" ImageAlign="AbsMiddle" ImageUrl="~/images/refresh.png" ToolTip="recalcular composição da parcela" OnClick="cmdRecalcularComposicao_Click" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr runat="server" visible="false" enableviewstate="false">
                        <td colspan="2">
                            <asp:GridView ID="gridComposicao" runat="server" SkinID="gridPadrao"
                                AutoGenerateColumns="False" Width="500"
                                AllowPaging="false" EnableViewState="false">
                                <Columns>
                                    <asp:BoundField HeaderText="Tipo" DataField="StrTipo" />
                                    <asp:BoundField HeaderText="Beneficiário" DataField="BeneficiarioNome" />
                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:N2}" />
                                </Columns>
                                <HeaderStyle HorizontalAlign="Left" />
                                <RowStyle Height="15px" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr style="background-color:Whitesmoke;"><td colspan="2" height="10"></td></tr>
                    <tr valign="bottom" style="background-color:Whitesmoke;border-bottom:solid 1px Gray">
                        <td colspan="2" align="center"><asp:CheckBox Font-Size="14px" ForeColor="gray" id="chkAltCobrancaValor" Checked="true" Text="alterar valor manualmente" runat="server" AutoPostBack="true" OnCheckedChanged="chkAltCobrancaValor_CheckedChanged" /></td>
                    </tr>
                    <tr>
                        <td height='25' >
                            <b>Vidas</b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltCobrancaVidas" Width="90px" MaxLength="4" SkinID="txtPadrao" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td height='25'>
                            <b>Valor</b>
                        </td>
                        <td width="85%" >
                            <asp:TextBox ID="txtAltCobrancaValor" Width="90px" MaxLength="18" SkinID="txtPadrao" runat="server" onkeypress="return filtro_SoNumeros(event);" />
                        </td>
                    </tr>
                    <tr>
                        <td height='25' >
                            <b>Vencto.</b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltCobrancaVencto" Width="90px" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"  />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td height="25">
                            <asp:CheckBox ID="chkAltCobrancaCancelada" Text="Cancelada" ForeColor="Red" runat="server" />
                        </td>
                    </tr>
                    <asp:Literal ID="litBaixa" runat="server" EnableViewState="false" />
                    <asp:Literal ID="litNegociacao" runat="server" EnableViewState="false" />
                    <tr height='10'><td colspan="2" height='10' style="background-color:Whitesmoke;border-top:solid 1px Gray"></td></tr>
                    <tr>
                        <td colspan="2" align="center" style="background-color:Whitesmoke;">
                            <asp:Button Text="Fechar" SkinID="botaoPadraoWarning_Small" ID="cmdFecharCobrancaDetalhe" runat="server" EnableViewState="false" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button Text="Salvar" SkinID="botaoPadraoINFO_Small" ID="cmdSalvarCobrancaDetalhe" OnClick="cmdSalvarCobrancaDetalhe_click" OnClientClick="return confirm('Confirma as as alterações?')" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <style type="text/css">
        .ajax__calendar_container { z-index : 9999999999999999999000 ; }
    </style>

    <script type="text/javascript">

        $(document).ready(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(configAutocomplete);

            configAutocomplete(null, null);
        });

        //https://stackoverflow.com/questions/3693560/how-do-i-pass-an-extra-parameter-to-jquery-autocomplete-field

        function configAutocomplete(sender, args) {
            $("#<%= txtContratoPJ.ClientID %>").autocomplete
            ({
                source: function (request, response) {

                    $.ajax({
                        url: "../../proxy/proxyCarregaContrato.aspx?estip=" + $("#<%= cboEstipulante.ClientID %>").val(),
                        dataType: "json",
                        extraParams: { estip: function () { return $("#<%= cboEstipulante.ClientID %>").val(); } },
                        data: {
                            featureClass: "P",
                            style: "full",
                            maxRows: 12,
                            name_startsWith: request.term
                        },
                        success: function (data) {
                            response($.map(data.Contratos, function (item) {
                                //alert(item.Titular);
                                return {

                                    label: item.Titular,
                                    value: item.Titular,
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
            if (item != null && item != undefined) {
                document.getElementById('<%= txtContratoPJId.ClientID %>').value = item.data.ID;
            }
            else {
                document.getElementById('<%= txtContratoPJId.ClientID %>').value = '';
            }
        }
    </script>
</asp:Content>
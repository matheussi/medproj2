<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucBeneficiarioForm.ascx.cs" Inherits="MedProj.www.usercontrols.ucBeneficiarioForm" %>
<script language="javascript" type="text/javascript">
    function semNumeroConfirm(campoID) {
        var valor = document.getElementById(campoID).value;
        if (valor == '') {
            return confirm('Deseja cadastrar o endereço sem número?');
        }
    }
</script>
<asp:Literal runat="server" ID="litFechar" />
<asp:Panel ID="pnlEnriquecimento" runat="server" Visible="false">
    <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1;background-color:#EFF3FB">
        <tr>
            <td><strong><span style="color:#507CD1;font-size:9pt">Há informações de enriquecimento para confimarção:</span></strong></td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="gridEnriquecimento" runat="server" SkinID="gridViewSkin" 
                    OnRowDataBound="gridEnriquecimento_RowDataBound" OnRowCommand="gridEnriquecimento_RowCommand"
                    DataKeyNames="id_telMail,id_beneficiario" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="beneficiario_nome" HeaderText="Nome" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="tipo" HeaderText="Tipo" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="ddd" HeaderText="DDD" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="telefone" HeaderText="Número" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="ramal" HeaderText="Ramal" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="email" HeaderText="E-mail" Visible="false" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        
                        <asp:BoundField ItemStyle-Wrap="false" DataField="dado" HeaderText="Dado" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        
                        <asp:ButtonField Text="<img src='images/delete.png' title='inválido' alt='inválido' border='0' />" CommandName="invalido">
                            
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='images/tick.png' title='confirmar' alt='confirmar' border='0' />" CommandName="ok">
                            
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
</asp:Panel>
<ajaxToolkit:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="p1">
        <HeaderTemplate><font color="black">Dados comuns</font></HeaderTemplate>
        <ContentTemplate>
            <div class="form-group">
                <label class="col-md-2 control-label">Tipo</label>
                <div class="col-md-8" style="margin-top:5px">
                    <asp:RadioButton ID="optFisica" Text="Pessoa física" GroupName="tipo" runat="server" Checked="true" AutoPostBack="true"  OnCheckedChanged="optTipo_CheckedChanged"/>
                    &nbsp;
                    <asp:RadioButton ID="optJuridica" Text="Pessoa jurídica" GroupName="tipo" runat="server" AutoPostBack="true"  OnCheckedChanged="optTipo_CheckedChanged"/>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-2 control-label">Matriz</label>
                <div class="col-xs-8"><asp:DropDownList ID="cboBeneficiario" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label">Nome</label>
                <div class="col-xs-8"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtNome" Width="100%" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label"><asp:Literal ID="lblDataNasc_Data" Text="Data de Nascimento" runat="server"/></label>
                <div class="col-xs-2">
                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtDataNascimento" Width="100px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                </div>
                <label class="col-xs-1 control-label"><asp:Literal ID="lblRG_IE" Text="RG" runat="server" /></label>
                <div class="col-xs-2"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtRG" Width="150px" /></div>

                <label class="col-xs-1 control-label"><asp:Literal ID="lblCPF_CNPJ" Text="CPF" runat="server" /></label>
                <div class="col-xs-2">
                    <asp:TextBox runat="server" SkinID="txtPadrao" ID="txtCPF" Width="150px" onkeypress="filtro_SoNumeros(event);" MaxLength="14" />
                </div>
            </div>

            <asp:Panel ID="pnlRGOrgao_RgUF_Sexo" runat="server" Visible="true">
                <div class="form-group">
                    <label class="col-xs-2 control-label">RG órgão expedidor</label>
                    <div class="col-xs-2"><asp:TextBox ID="txtRgOrgao" runat="server" SkinID="txtPadrao" Width="100px"></asp:TextBox></div>
                    <label class="col-xs-1 control-label">RG UF</label>
                    <div class="col-xs-2"><asp:TextBox ID="txtRgUF" runat="server" SkinID="txtPadrao" MaxLength="2" Width="45px" /></div>
                    <label class="col-xs-1 control-label">Sexo</label>
                    <div class="col-xs-2"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboSexo" /></div>
                </div>
            </asp:Panel>

            <asp:Panel ID="trParentesco" runat="server" Visible="false" EnableViewState="false">
                <div class="form-group">
                    <label class="col-xs-2 control-label">Parentesco</label>
                    <div class="col-xs-4"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboParentesco" /></div>
                </div>
            </asp:Panel>

            <div class="form-group">
                <label class="col-xs-2 control-label"><asp:Literal ID="lblNomeMae_NomeContato" Text="Nome da mãe" runat="server"/></label>
                <div class="col-xs-8"><asp:TextBox runat="server" SkinID="txtPadrao" ID="txtNomeMae" Width="100%" /></div>
            </div>

            <asp:Panel ID="pnlDeclNascVivo_CNS" runat="server" Visible="true">
                <div class="form-group">
                    <label class="col-xs-2 control-label">Decl. nasc. vivo</label>
                    <div class="col-xs-2"><asp:TextBox ID="txtDeclaracaoNascimentoVivo" runat="server" SkinID="txtPadrao" Width="100%" /></div>
                    <label class="col-xs-1 control-label">CNS</label>
                    <div class="col-xs-2"><asp:TextBox ID="txtCNS" runat="server" SkinID="txtPadrao" Width="90px" /></div>
                </div>
            </asp:Panel>
            <div class="form-group">
                <label class="col-xs-2 control-label">
                    Dados de contato &nbsp;
                </label>
                <div class="col-xs-1">
                    <asp:ImageButton runat="server" Visible="False" 
                        ToolTip="Puxar dados de contato do Titular da proposta" 
                        ImageUrl="~/images/duplicar.png" ID="cmdPuxarFonesDoTitular" 
                        OnClick="cmdPuxarFonesDoTitular_Click" />
                </div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label">DDD</label>
                <div class="col-xs-1">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDD1" Width="100%" MaxLength="2"  onkeypress="filtro_SoNumeros(event);" />
                </div>

                <label class="col-xs-1 control-label">Fone</label>
                <div class="col-xs-2">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtFone1" Width="100%" MaxLength="9" />
                </div>
                <label class="col-xs-1 control-label">ramal</label>
                <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtRamal1" Width="100%" MaxLength="5" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label">DDD</label>
                <div class="col-xs-1">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDD2" Width="100%" MaxLength="2"  onkeypress="filtro_SoNumeros(event);" />
                </div>

                <label class="col-xs-1 control-label">Fone</label>
                <div class="col-xs-2">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtFone2" Width="100%" MaxLength="9" />
                </div>
                <label class="col-xs-1 control-label">ramal</label>
                <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtRamal2" Width="100%" MaxLength="5" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label">DDD</label>
                <div class="col-xs-1">
                   <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtDDDCelular" Width="100%" MaxLength="2"  onkeypress="filtro_SoNumeros(event);" />
                </div>

                <label class="col-xs-1 control-label">Celular</label>
                <div class="col-xs-2">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCelular" Width="100%" MaxLength="10" />
                </div>
                <label class="col-xs-1 control-label">Operadora</label>
                <div class="col-xs-3"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCelularOperadora" Width="100%" MaxLength="99" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label">E-mail</label>
                <div class="col-xs-8"><asp:TextBox SkinID="txtPadrao" Width="100%" runat="server" ID="txtEmail" MaxLength="85" /></div>
            </div>
            
            <table>
                <tr>
                    <td valign="top"></td>
                    <td valign="top" align="left">
                        <asp:Panel ID="pnlEnriquecimentoValido" Visible="False" runat="server">
                            <table>
                                <tr>
                                    <asp:DataList CellPadding="0" ID="dlEnriquecimento" DataKeyField="id_telMail"
                                        RepeatDirection="Horizontal" runat="server" 
                                        OnItemCommand="dlEnriquecimento_ItemCommand" RepeatColumns="3">
                                        <ItemTemplate>
                                            <td valign="top" align="left">
                                                <table cellpadding="2" border="0" style="border: solid 1px #507CD1;background-color:#EFF3FB">
                                                    <tr>
                                                        <td>Tipo:</td>
                                                        <td nowap><asp:Literal ID="litTipo" Text='<%# DataBinder.Eval(Container.DataItem, "tipo") %>' runat="server" /></td>
                                                        <td align="right"><asp:ImageButton ID="cmdFechar" ImageUrl="~/images/close.png" ImageAlign="Top" hspacing='0' ToolTip="fechar" OnClientClick="return confirm('Deseja fechar o item?');" runat="server" CommandName="fechar" CommandArgument='<%# Eval("id_telMail") %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Dado:</td>
                                                        <td colspan="2" nowap><asp:Literal ID="Literal1" Text='<%# DataBinder.Eval(Container.DataItem, "dado") %>' runat="server" /></td>
                                                    </tr>
                                                    <%--<tr>
                                                        <td>DDD</td>
                                                        <td colspan="2"><asp:Literal ID="litDDD" Text='<%# DataBinder.Eval(Container.DataItem, "ddd") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Número</td>
                                                        <td colspan="2"><asp:Literal ID="litNumero" Text='<%# DataBinder.Eval(Container.DataItem, "telefone") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Ramal</td>
                                                        <td colspan="2"><asp:Literal ID="litRamal" Text='<%# DataBinder.Eval(Container.DataItem, "ramal") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>E-mail</td>
                                                        <td colspan="2"><asp:Literal ID="litEmail" Text='<%# DataBinder.Eval(Container.DataItem, "email") %>' runat="server" /></td>
                                                    </tr>--%>
                                                </table>
                                            </td>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="p2">
        <HeaderTemplate><font color="black">Endereços</font></HeaderTemplate>
        <ContentTemplate>
            <div class="form-group">
                <label class="col-xs-2 control-label" style="margin-top:-5px">CEP</label>
                <div class="col-xs-2">
                    <asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCEP" Width="100%" MaxLength="9" />
                    <ajaxToolkit:MaskedEditExtender TargetControlID="txtCEP" Mask="99999-999" runat="server" ID="meeCEP" ClearMaskOnLostFocus="true" Enabled="True" />
                </div>
                <div class="col-xs-1"><asp:ImageButton runat="server" EnableViewState="false" ToolTip="checar CEP" ImageUrl="~/images/endereco.png" ID="cmdBuscaEndereco" OnClick="cmdBuscaEndereco_Click" />&nbsp;</div>
                <div class="col-xs-1"><asp:ImageButton runat="server" Visible="false" ToolTip="Puxar endereços do Titular da propsta" ImageUrl="~/images/duplicar.png" ID="cmdPuxarEnderecosDoTitular" OnClick="cmdPuxarEnderecosDoTitular_Click" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label" style="margin-top:-5px">Logradouro</label>
                <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtLogradouro" Width="100%" MaxLength="300" /></div>
                <label class="col-xs-1 control-label" style="margin-top:-5px">Número</label>
                <div class="col-xs-1"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtNumero" Width="100%" MaxLength="9" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label" style="margin-top:-5px">Complemento</label>
                <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtComplemento" Width="100%" MaxLength="250" /></div>
                <label class="col-xs-1 control-label" style="margin-top:-5px">Bairro</label>
                <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtBairro" Width="100%" MaxLength="300" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label" style="margin-top:-5px">Cidade</label>
                <div class="col-xs-4"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></div>
                <label class="col-xs-1 control-label" style="margin-top:-5px">UF</label>
                <div class="col-xs-2"><asp:TextBox SkinID="txtPadrao" runat="server" ID="txtUF" Width="40px" MaxLength="2" /></div>
            </div>

            <div class="form-group">
                <label class="col-xs-2 control-label" style="margin-top:-5px">Tipo</label>
                <div class="col-xs-5">
                    <asp:DropDownList Width="100%" runat="server" ID="cboTipoEndereco" SkinID="comboPadrao1">
                        <asp:ListItem Text="RESIDENCIAL" Value="0" Selected="True" />
                        <asp:ListItem Text="COMERCIAL" Value="1" />
                    </asp:DropDownList>
                </div>
                <div class="col-xs-2 text-right">
                    <asp:Button runat="server" SkinID="botaoPadraoINFO" EnableViewState="false" Text="adicionar" ID="cmdAddEndereco" OnClick="cmdAddEndereco_Click" />
                </div>
            </div>

            <div class="form-group">
                <div class="col-xs-9">
                    <hr />
                </div>
            </div>
            <span runat="server" id="spanEnderecosCadastrados"><b>Endereços cadastrados</b></span> <br />
            <asp:GridView Width="76%" ID="gridEnderecos" runat="server" 
                AutoGenerateColumns="False" SkinID="gridPadrao" DataKeyNames="ID" OnRowCommand="gridEnderecos_RowCommand" OnRowDataBound="gridEnderecos_RowDataBound">
                <Columns>
                    <asp:BoundField Visible="false" DataField="ID" HeaderText="Código">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Logradouro" HeaderText="Logradouro">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="alterar" Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />">
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:ButtonField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../../images/delete.png' title='excluir' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:ButtonField>
                </Columns>
                
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="p3" Visible="false">
        <HeaderTemplate><asp:Literal ID="litP3" runat="server" Visible="false"><font color="black">Ficha de Saúde</font></asp:Literal></HeaderTemplate>
        <ContentTemplate>
            <asp:DataList CellPadding="0" CellSpacing="0" ID="dlFicha" DataKeyField="ID" runat="server" OnItemCommand="dlFicha_ItemCommand" OnItemDataBound="dlFicha_ItemDataBound">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <br />
                <table cellpadding="3" cellspacing="0" width="600">
                    <tr>
                        <td colspan="2" bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="left">
                            <asp:Label ID="lblQuesta" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoTexto") %>' runat="server" />
                            <asp:Literal ID="litItemDeclaracaoID" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoID") %>' runat="server" Visible="false" />
                        </td>
                        <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center" width="1%">
                            <asp:CheckBox OnCheckedChanged="checkboxSkin_Changed2" AutoPostBack="true" SkinID="checkboxSkin" ID="chkFSim" runat="server" Checked='<%# Bind("Sim") %>' />
                        </td>
                    </tr>
                    <tr runat="server" id="tr1Ficha" visible="false">
                        <td style="border-left: solid 1px #507CD1">Data</td>
                        <td width="90%" colspan="2" style="border-right: solid 1px #507CD1">Descrição</td>
                    </tr>
                    <tr runat="server" id="tr2Ficha" visible="false">
                        <td style="border-left: solid 1px #507CD1;border-bottom: solid 1px #507CD1">
                            <asp:TextBox SkinID="textboxSkin" Width="66px" runat="server" ID="txtFichaSaudeData" MaxLength="10" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' />
                            <ajaxToolkit:MaskedEditExtender MaskType="Date" EnableViewState="false" TargetControlID="txtFichaSaudeData" Mask="99/99/9999" runat="server" ID="meeFichaSaudeData" />
                        </td>
                         <td width="90%" colspan="2" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">
                            <asp:TextBox ID="txtFichaSaudeDescricao" Width="99%" SkinID="textboxSkin" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' />
                        </td>
                    </tr>
                    <tr runat="server" id="tr3Ficha" visible="false">
                        <td colspan="3" style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                            <asp:Button ID="cmdSalvarFicha" SkinID="botaoPequeno" Text="salvar" runat="server" CommandName="salvar" CommandArgument="<%# Container.ItemIndex %>" /><asp:Literal runat="server" EnableViewState="false" ID="litFichaAviso" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:DataList>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="p4" Visible="false">
        <HeaderTemplate><asp:Literal ID="litP4" runat="server" Visible="false"><font color="black">Adicionais</font></asp:Literal></HeaderTemplate>
        <ContentTemplate>
            <asp:GridView ID="gridAdicional" runat="server" SkinID="gridViewSkin" 
                DataKeyNames="AdicionalID,ID" AutoGenerateColumns="False"
                width="650px" OnRowDataBound="gridAdicional_OnRowDataBound" 
                onrowcommand="gridAdicional_RowCommand">
                <Columns>
                    <asp:BoundField DataField="AdicionalDescricao" HeaderText="Produto">
                        <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox oncheckedchanged="checkboxGridAdicional_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<table cellpadding="2" width="65%">
    <tr id="trUsar" runat="server" visible="false">
        <td colspan="2" align="center" bgcolor="whitesmoke">
            <asp:LinkButton Text="Já existe um beneficiário com este nome cadastrado no sistema. Quero usá-lo!" ID="lnkUsar" runat="server" OnClick="lnkUsar_Click" ForeColor="Red" />
        </td>
    </tr>
    <tr>
        <td align="left"><asp:Button Width="130" OnClick="cmdVoltar_Click" SkinID="botaoPadrao1" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
        <td align="right"><asp:Button Width="130" OnClick="cmdSalvar_Click" SkinID="botaoPadrao1" runat="server" EnableViewState="true" ID="cmdSalvar" Text="Salvar" /></td>
    </tr>
</table>
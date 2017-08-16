<%@ Page Title=""  EnableEventValidation="false" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="prestador.aspx.cs" Inherits="MedProj.www.credenciamento.prestadores.prestador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Prestador
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <input type="hidden" name="txtClienteId" id="txtClienteId" runat="server" />

            <div class="panel panel-default">
                <div class="panel-body">

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Prestador</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtNomePrestador" runat="server" SkinID="txtPadrao" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Segmento</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboSegmento" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Observações</label>
                        <div class="col-xs-10"><asp:TextBox ID="txtObs" TextMode="MultiLine" runat="server" SkinID="txtPadrao" /></div>
                    </div>

                    <asp:Panel ID="pnlBotoes" runat="server" Visible="false">
                        <div class="form-group">
                            <div class="col-xs-2 text-center">&nbsp;</div>
                            <div class="col-xs-3 text-left"><asp:Button ID="cmdContratos" Text="Contratos" runat="server" OnClick="cmdContratos_Click" SkinID="botaoPadraoWarning" EnableViewState="false" /></div>
                            <div class="col-xs-2 text-left"><asp:Button ID="cmdEspecialidades" Text="Especialidades" runat="server" OnClick="cmdEspecialidades_Click"  SkinID="botaoPadraoINFO" EnableViewState="false" /></div><%----%>
                            <div class="col-xs-3 text-center"><asp:Button ID="cmdDadosBancarios" Text="Dados bancários" runat="server" OnClick="cmdDadosBancarios_Click" SkinID="botaoPadraoSUCCESS" EnableViewState="false" /></div>
                            <div class="col-xs-2 text-right"><asp:Button ID="cmdDadosAcesso" Text="Dados de acesso" runat="server" OnClick="cmdDadosAcesso_Click"       SkinID="botaoPadraoDANGER" EnableViewState="false" /></div>
                        </div>
                    </asp:Panel>

                    <asp:panel ID="pnlContratos" runat="server" CssClass="alert alert-warning" Visible="false">

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Tipo</label>
                            <div class="col-xs-10">
                                <asp:DropDownList ID="cboTipo" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_SelectedIndexChanged" runat="server" SkinID="comboPadrao1" Width="100%">
                                    <asp:ListItem Text="Pessoa jurídica" Value="0" Selected="True" />
                                    <asp:ListItem Text="Pessoa física" Value="1" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label"><asp:Literal ID="litNome" runat="server" Text="Razão social" /></label>
                            <div class="col-xs-4"><asp:TextBox ID="txtNome" runat="server" SkinID="txtPadrao" /></div>
                            <label class="col-xs-2 control-label"><asp:Literal ID="litDocumento" runat="server" Text="CNPJ" /></label>
                            <div class="col-xs-4"><asp:TextBox ID="txtDocumento" runat="server" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Pagamento</label>
                            <div class="col-xs-10">
                                <asp:DropDownList ID="cboPagamento" runat="server" SkinID="comboPadrao1" Width="100%">
                                    <asp:ListItem Text="Mensalmente" Value="0" Selected="True" />
                                    <asp:ListItem Text="Quinzenalmente" Value="1" />
                                    <asp:ListItem Text="Semanalmente" Value="2" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Telefone</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtFone" runat="server" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DDDFONEFAX9(this, event)" MaxLength="14" /></div>
                            <label class="col-xs-2 control-label">Celular</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtCel" runat="server" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DDDFONEFAX9(this, event)" MaxLength="14" /></div>
                            <label class="col-xs-2 control-label">E-mail</label>
                            <div class="col-xs-2"><asp:TextBox ID="txtEmail" runat="server" SkinID="txtPadrao" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">CEP</label>
                            <div class="col-xs-5">
                                        
                                <div class="input-group">
                                    <asp:TextBox ID="txtCEP" SkinID="txtPadrao" runat="server" Width="70%" MaxLength="9" onkeypress="filtro_SoNumeros(event); mascara_CEP(this,event);" />
                                    <asp:LinkButton ID="cmdCEP" runat="server" OnClick="cmdCEP_Click"><span class="input-group-addon glyphicon glyphicon-search"></span></asp:LinkButton>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Endereço</label>
                            <div class="col-xs-8"><asp:TextBox ID="txtEndereco" SkinID="txtPadrao" runat="server" Width="99.5%" MaxLength="75" /></div>
                            <div class="col-xs-2">
                                <div class="input-group">
                                    <span class="input-group-addon">nº</span>
                                    <asp:TextBox ID="txtNumero" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                </div>
                            </div>
                        </div>

                        <div class="col-xs-offset-2 col-xs-10">
                            <div class="row">
                                <div class="row" style="margin:-10px;">
                                    <div class="col-xs-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">Compl.</span>
                                            <asp:TextBox ID="txtComplemento" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                    <div class="col-xs-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">Bairro</span>
                                            <asp:TextBox ID="txtBairro" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                    <div class="col-xs-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">Cidade</span>
                                            <asp:TextBox ID="txtCidade" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                    <div class="col-xs-2">
                                        <div class="input-group">
                                            <span class="input-group-addon">Estado</span>
                                            <asp:TextBox ID="txtEstado" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%----%>
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Coordenadas</label>
                            <div class="row">
                                <div class="row">
                                    <div class="col-xs-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">Latitude</span>
                                            <asp:TextBox ID="txtLatitude" BackColor="#EEEEEE" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                    <div class="col-xs-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">Longitude</span>
                                            <asp:TextBox ID="txtLongitude" BackColor="#EEEEEE" SkinID="txtPadrao" runat="server" Width="100%" MaxLength="75" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-md-2 control-label">Observações</label>
                            <div class="col-xs-10">
                                <asp:TextBox ID="txtContratoObservacoes" TextMode="MultiLine" SkinID="txtPadrao" runat="server" Rows="6" Width="100%" MaxLength="7000" />
                            </div>
                        </div>


                        <asp:Panel ID="pnlProcedimentosDetalhamento" runat="server">

                        <div class="form-group">
                            <label class="col-xs-2 control-label" style="padding-top:3px">Tabela de preço</label>
                            <div class="col-xs-10 text-left"><asp:DropDownList ID="cboTabelaPreco" runat="server" DataTextField="Nome" DataValueField="ID" SkinID="comboPadrao1" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboTabelaPreco_SelectedIndexChanged"/></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label" style="padding-top:3px">Procedimentos</label>
                            <div class="col-xs-10 text-left"><asp:DropDownList ID="cboTabelaProcedimentos" AutoPostBack="true" OnSelectedIndexChanged="cboTabelaProcedimentos_SelectedIndexChanged" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label" style="padding-top:3px">Especialidade</label>
                            <div class="col-xs-10 text-left"><asp:DropDownList ID="cboEspecialidadePro" AutoPostBack="true" OnSelectedIndexChanged="cboEspecialidadePro_SelectedIndexChanged" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label" style="padding-top:3px">Categoria</label>
                            <div class="col-xs-10 text-left"><asp:DropDownList ID="cboCategoria" AutoPostBack="true" OnSelectedIndexChanged="cboCategoria_SelectedIndexChanged" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label" style="padding-top:3px">ou pesquisar</label>
                            <div class="col-xs-9 text-left"><asp:TextBox ID="txtPesquisarProcedimento" runat="server" SkinID="txtPadrao" Width="100%"/></div>
                            <div class="col-xs-1 text-left"><asp:LinkButton ID="cmdPesquisarProcedimento" runat="server" OnClick="cmdPesquisarProcedimento_Click" ToolTip="localizar..."><span class="glyphicon glyphicon-search"></span></asp:LinkButton></div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-2"></div>
                            <div class="col-xs-10 text-left">
                                <div style="font-size:9pt;height:200px;overflow-y: scroll;border:1px solid gray;background-color:whitesmoke" runat="server">
                                    <asp:GridView ID="gridProcedimentosParaAdd" DataKeyNames="ID" SkinID="gridPadrao" Width="100%" runat="server" OnRowCreated="gridProcedimentosParaAdd_RowCreated" OnRowDataBound="gridProcedimentosParaAdd_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkChec" runat="server" />
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="chkChecHeader" Visible="false" AutoPostBack="true" OnCheckedChanged="chkChecHeader_CheckedChanged" runat="server" />
                                                </HeaderTemplate>
                                                <ItemStyle Width="1%" />
                                                <HeaderStyle Width="1%" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                                            <%--<asp:BoundField DataField="Especialidade" HeaderText="Especialidade" />
                                            <asp:BoundField DataField="Categoria" HeaderText="Categoria" />--%>
                                            <asp:TemplateField HeaderText="Tabela" >
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="cboTabela" runat="server" Width="200px" SkinID="comboPadrao1" AutoPostBack="true" OnSelectedIndexChanged="cboTabela_SelectedIndexChanged" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valor" >
                                                <ItemTemplate>
                                                    <asp:TextBox Width="80px" MaxLength="15" ID="txtValor" SkinID="txtPadrao" ReadOnly="true" runat="server" Text='<%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "CH")).ToString("N2") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField >
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgCadeado" OnClientClick="return confirm('Alterar forma de precificar?');" ImageUrl="~/Images/cadeado.png" runat="server" OnClick="imgCadeado_Click" />
                                                </ItemTemplate>
                                                <HeaderStyle Width="1%" />
                                                <ItemStyle Width="1%" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <RowStyle BackColor="WhiteSmoke" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-12 text-center">
                                <asp:Button ID="cmdAddProcedimento" Text="adicionar procedimentos" SkinID="botaoPadraoINFO_Small" runat="server" OnClick="cmdAddProcedimento_Click" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-12">
                                <hr />
                            </div>
                        </div>

                        </asp:panel>

                        <asp:Panel ID="pnlProcedimentosAdicionados" runat="server" Visible="false">
                            <div class="form-group">
                                <div class="col-xs-2">
                                    Localizar:<br />
                                    <asp:TextBox ID="txtLocalizarProcedimentoAdicionado" placeholder="por nome ou código" SkinID="txtPadrao" Width="100%" MaxLength="35" EnableViewState="false" runat="server" />
                                    <asp:Button ID="cmdLocalizarProcedimentoAdicionado" Width="100%" SkinID="botaoPadraoINFO_Small" Text="pesquisar" runat="server" EnableViewState="false" OnClick="cmdLocalizarProcedimentoAdicionado_Click" />
                                    <asp:Button ID="cmdExibirTodosProcedimentoAdicionado" Width="100%" SkinID="botaoPadraoWarning_Small" Text="exibir todos" runat="server" EnableViewState="false" OnClick="cmdExibirTodosProcedimentoAdicionado_Click" />
                                </div>
                                <div class="col-xs-10 text-left">
                                    <b>Procedimentos adicionados:</b>
                                    <div style="font-size:9pt;height:150px;overflow-y: scroll;border:1px solid gray;background-color:whitesmoke">
                                        <asp:GridView ID="gridProcedimentosAdicionados" DataKeyNames="ID" SkinID="gridPadrao" Width="100%" runat="server" OnRowDataBound="gridProcedimentosAdicionados_RowDataBound" OnRowCommand="gridProcedimentosAdicionados_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Procedimento" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litProcedimento" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Nome")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Código" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litCodigo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Codigo")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Especialidade" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litEspecialidade" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Especialidade")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Categoria" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litCategoria" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Categoria")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tabela" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litTabela" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TabelaPreco.Nome")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ValorCalculado" HeaderText="Valor" DataFormatString="{0:N2}" />
                                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                                    <ItemStyle Width="1%" />
                                                    <ControlStyle Width="1%" />
                                                    <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <RowStyle BackColor="WhiteSmoke" />
                                        </asp:GridView>
                                        <asp:CheckBoxList ID="chklProcedimento" runat="server" DataValueField="ID" DataTextField="Nome" ></asp:CheckBoxList><%----%>
                                    </div>
                                </div>
                            </div>
                            
                        
                            <div class="form-group">
                                <div class="col-xs-12">
                                    <hr />
                                </div>
                            </div>
                        </asp:Panel>

                        <div class="form-group">
                            <div class="col-xs-12 text-center">
                                <asp:Button ID="cmdAddContratos" Text="Gravar contrato" SkinID="botaoPadraoSUCCESS" runat="server" OnClick="cmdAddContratos_Click" />
                            </div>
                        </div>

                        
                        <div class="form-group">
                            <div class="col-xs-12">
                                <hr />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-12">
                                <b>Contratos existentes:</b>
                                <asp:GridView ID="gridContratos" DataKeyNames="ID" SkinID="gridPadrao" Width="100%" runat="server" OnRowCommand="gridContratos_RowCommand" OnRowDataBound="gridContratos_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Nome" HeaderText="Nome" />
                                        <asp:TemplateField HeaderText="Tabela" Visible="false" >
                                            <ItemTemplate>
                                                <asp:Literal runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TabelaPreco.Nome")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Região" Visible="false">
                                            <ItemTemplate>
                                                <asp:Literal runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Regiao.Nome")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                                        <asp:BoundField DataField="Email" HeaderText="E-mail" />

                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                        </asp:ButtonField>
                                    </Columns>
                                    <RowStyle BackColor="WhiteSmoke" />
                                </asp:GridView>
                            </div>
                        </div>

                    </asp:panel>

                    <asp:Panel ID="pnlEspecialidades" Visible="false" CssClass="alert alert-info" runat="server">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Contrato</label>
                            <div class="col-xs-9 text-left"><asp:DropDownList ID="cboContrato" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Especialidade</label>
                            <div class="col-xs-9 text-left"><asp:DropDownList ID="cboEspecialidade" runat="server" SkinID="comboPadrao1" Width="100%"/></div>
                            <div class="col-xs-1"><asp:Button ID="cmdAddEspecialidade" Text="+" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdAddEspecialidade_Click" /></div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-9">
                                <hr />
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-9">
                                <asp:GridView ID="gridEspecialidades" DataKeyNames="ID" SkinID="gridPadrao" Width="100%" runat="server" OnRowCommand="gridEspecialidades_RowCommand" OnRowDataBound="gridEspecialidades_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Contrato" >
                                            <ItemTemplate>
                                                <asp:Literal runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ContratoNome")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Especialidade">
                                            <ItemTemplate>
                                                <asp:Literal runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EspecialidadeNome")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                    </Columns>
                                    <RowStyle BackColor="WhiteSmoke" />
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlDadosBancarios" Visible="false" CssClass="alert alert-info" runat="server">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Contrato</label>
                            <div class="col-xs-9 text-left"><asp:DropDownList ID="cboContrato_Bancarios" runat="server" SkinID="comboPadrao1" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboContrato_Bancarios_SelectedIndexChanged"/></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Banco</label>
                            <div class="col-xs-4 text-left"><asp:TextBox ID="txtBanco" SkinID="txtPadrao" MaxLength="120" Width="100%" runat="server" /></div>
                            <label class="col-xs-2 control-label">Tipo</label>
                            <div class="col-xs-3 text-left">
                                <asp:DropDownList ID="cboTipoConta" SkinID="comboPadrao1" Width="100%" runat="server">
                                    <asp:ListItem Text="Conta Corrente" Value="0" Selected="True" />
                                    <asp:ListItem Text="Conta Poupança" Value="1" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Número</label>
                            <div class="col-xs-2 text-left">
                                <asp:TextBox ID="txtBancoNumero" onkeypress="filtro_SoNumeros(event);" SkinID="txtPadrao" MaxLength="3" Width="100%" runat="server" />
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Agência</label>
                            <div class="col-xs-3 text-left"><asp:TextBox ID="txtAgencia" onkeypress="filtro_SoNumeros(event);" SkinID="txtPadrao" MaxLength="20" Width="100%" runat="server" /></div>
                            <label class="col-xs-2 control-label">Agência DV</label>
                            <div class="col-xs-2 text-left">
                                <asp:TextBox ID="txtAgenciaDV" SkinID="txtPadrao" MaxLength="1" Width="50px" runat="server" />
                            </div>
                            
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Num. Conta</label>
                            <div class="col-xs-3 text-left"><asp:TextBox ID="txtNumConta" SkinID="txtPadrao" MaxLength="30" Width="100%" runat="server" /></div>
                            <label class="col-xs-2 control-label">Conta DV</label>
                            <div class="col-xs-2 text-left">
                                <asp:TextBox ID="txtNumContaDV" SkinID="txtPadrao" MaxLength="2" Width="50px" runat="server" />
                            </div>
                            <div class="col-xs-1 text-right" style="padding-top:5px"><asp:LinkButton ID="lnkSalvarDadosBancarios" runat="server" ToolTip="salvar dados bancários" CssClass="glyphicon glyphicon-floppy-disk" OnClick="lnkSalvarDadosBancarios_Click" /></div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlAcesso" Visible="false" CssClass="alert alert-danger" runat="server">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Contrato</label>
                            <div class="col-xs-9 text-left"><asp:DropDownList ID="cboContrato_Acesso" runat="server" SkinID="comboPadrao1" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboContrato_Acesso_SelectedIndexChanged"/></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Login</label>
                            <div class="col-xs-5 text-left"><asp:TextBox ID="txtLogin" SkinID="txtPadrao" Width="100%" MaxLength="100" runat="server" /></div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Senha</label>
                            <div class="col-xs-4 text-left"><asp:TextBox ID="txtSenha" TextMode="Password" SkinID="txtPadrao" Width="100%" MaxLength="25" runat="server" /></div>
                            <div class="col-xs-4 text-left" style="padding-top:5px"><asp:LinkButton ID="lnkSalvarDadosAcesso" runat="server" ToolTip="salvar dados de acesso" CssClass="glyphicon glyphicon-floppy-disk" OnClick="lnkSalvarDadosAcesso_Click" /></div>
                        </div>
                    </asp:Panel>
                    
                    <hr />
                    <div class="col-xs-12 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" SkinID="botaoPadrao1" EnableViewState="false" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" SkinID="botaoPadrao1" EnableViewState="false" />
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

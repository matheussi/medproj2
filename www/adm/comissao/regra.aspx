<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="regra.aspx.cs" Inherits="MedProj.www.adm.comissao.regra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Regra de comissionamento
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
                        <label class="col-xs-2 control-label">Descrição</label>
                        <div class="col-xs-8">
                            <asp:TextBox ID="txtNome" runat="server" Width="100%" MaxLength="149" SkinID="txtPadrao" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-8"><asp:DropDownList ID="cboAssociadoPJ" runat="server" Width="100%" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-8 col-xs-offset-2">
                            <asp:CheckBox ID="chkAtivo" Checked="true" Text="Ativo" runat="server" />
                        </div>
                    </div>


                    <asp:panel ID="pnlParcelas" runat="server" CssClass="alert alert-warning">

                        <div class="form-group">
                            <label class="col-xs-10 text-center"><strong>Parcelas e percentuais</strong></label>
                        </div>

                        <div class="form-group">
                            <label class="col-md-2 control-label">Parcela</label>
                            <div class="col-md-2"><asp:TextBox ID="txtParcela" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                            <label class="col-md-2 control-label">Percentual</label>
                            <div class="col-md-2"><asp:TextBox ID="txtPercentual" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                            <div class="col-md-3"><asp:CheckBox ID="chkVitalicio" Text="Vitalício a partir desta" runat="server" /></div>
                        </div>
                        <div class="form-group">
                            <label class="col-md-2 control-label">Corretor</label>
                            <div class="col-md-8">
                                <asp:DropDownList ID="cboItemCorretores" runat="server" SkinID="comboPadrao1" Width="100%" />
                            </div>
                            <div class="col-md-1"><asp:LinkButton id="cmdAddVig" runat="server" OnClick="cmdAddVig_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div>
                        </div>


                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="gridItens" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridVig_RowCommand" OnRowDataBound="gridVig_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Parcela" HeaderText="Parcela"/>
                                        <asp:BoundField DataField="Percentual" HeaderText="Percentual" DataFormatString="{0:N2}" />

                                        <%----%>
                                        <%--<asp:TemplateField HeaderText="Corretor" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkCorretor" Text='<%# DataBinder.Eval(Container.DataItem, "Corretor.Nome")%>' CommandArgument='<%# Eval("Corretor.ID") %>' CommandName="Excecao" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="Corretor" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCorretor" Text='<%# DataBinder.Eval(Container.DataItem, "Corretor.Nome")%>'  runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Vitalício" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litVitalicio" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar" Visible="true">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                    </asp:panel>

                    <asp:panel ID="pnlCorretores" runat="server" CssClass="alert alert-info" EnableViewState="false" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-10 text-center"><strong>Corretores da tabela</strong></label>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Corretor</label>
                            <div class="col-xs-8">
                                <asp:DropDownList runat="server" SkinID="comboPadrao1" ID="cboCorretor" Width="100%" />
                            </div>
                            <div class="col-xs-2">
                                <asp:LinkButton id="lnkAddCorretor" runat="server" OnClick="cmdAddCorretor_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="gridCorretor" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridCorretor_RowCommand" OnRowDataBound="gridCorretor_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Corretor" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCorretor" Text='<%#DataBinder.Eval(Container.DataItem, "Corretor.Nome")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar" Visible="false">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:panel>

                    <asp:panel ID="pnlContratos" runat="server" CssClass="alert alert-success">
                        <div class="form-group">
                            <label class="col-xs-10 text-center"><strong>Contratos da tabela</strong></label>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-2 control-label">Cartão</label>
                            <div class="col-xs-2"  style="padding-left:0px;">
                                <asp:TextBox ID="txtCartao" Width="100%" SkinID="txtPadrao" runat="server" EnableViewState="false" />
                            </div>
                            <label class="col-xs-2 control-label">Nome</label>
                            <div class="col-xs-2"  style="padding-left:0px;">
                                <asp:TextBox ID="txtNomeBeneficiario" Width="100%" SkinID="txtPadrao" runat="server" EnableViewState="false" />
                            </div>
                            <div class="col-xs-3">
                                <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadraoINFO_Small" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                <asp:Button ID="cmdInserirTodos" Text="Inserir todos" SkinID="botaoPadraoDANGER_Small" EnableViewState="false" runat="server" OnClick="cmdInserirTodos_Click" OnClientClick="return confirm('Deseja realmente inserior TODOS os contratos do projeto selecionado?\nContratos ja inseridos serão ignorados.\nEssa operação não poderá ser desfeita');" />
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="gridContrato" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridContrato_RowCommand" OnRowDataBound="gridContrato_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Numero" HeaderText="Número">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <%----%>
                                        <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Titular">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>

                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Adicionar">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-plus" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar" Visible="false">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-10 text-center">Contratos ja adicionados à tabela</label>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-8">
                                <asp:GridView ID="gridContratoAdicionado" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridContratoAdicionado_RowCommand" OnRowDataBound="gridContratoAdicionado_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Numero" HeaderText="Número">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Titular" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkContrato" Text='<%# DataBinder.Eval(Container.DataItem, "BeneficiarioTitularNome")%>' CommandArgument='<%# Eval("ID") %>' CommandName="Excecao" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                    </asp:panel>


                    <!-------------------------------------------->



                    <hr />
                    <div class="col-xs-12 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                </div>
            </div>


            <!-- Itens de Exceção -->
            <asp:Panel ID="pnlExcecao" runat="server" Visible="false" >
                <div style="background:rgba(0,0,0,0.5); width:100%; height:100%; position:fixed; z-index:99999; top:0; left:0;">
                    <div style="position:absolute; background:white; border-radius:10px; width:69%; margin-left:-500px; top:25%; left:50%; padding:30px;">

                        <div class="form-group alert alert-danger">
                            <label class="col-xs-12 text-center">
                                <strong><asp:Literal ID="litContratoExcecao" runat="server" /> Parcelas e percentuais - Exceção</strong>
                            </label>
                        </div>

                        <div class="form-group">
                            <div class="col-md-10 col-md-offset-1 text-center">
                                <asp:RadioButton ID="optComissionadoExcecao" ForeColor="Red" Text=" Comissionado" runat="server" GroupName="exc" Checked="true" AutoPostBack="true" OnCheckedChanged="opt_changed" />
                                &nbsp;
                                <asp:RadioButton ID="optNaoComissionadoExcecao" ForeColor="Red" Text=" Não Comissionado" runat="server" GroupName="exc"  AutoPostBack="true" OnCheckedChanged="opt_changed"  />
                            </div>
                        </div>
                        <asp:Panel ID="pnlComissionavel" runat="server">
                            <div class="form-group">
                                <label class="col-md-2 control-label">Parcela</label>
                                <div class="col-md-2"><asp:TextBox ID="txtParcelaExcecao" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                                <label class="col-md-2 control-label">Percentual</label>
                                <div class="col-md-2"><asp:TextBox ID="txtPercentualExcecao" onkeypress="filtro_SoNumeros(event);" runat="server" Width="80%" SkinID="txtPadrao" MaxLength="15" /></div>
                                <div class="col-md-3"><asp:CheckBox ID="chkVitalicioExcecao" Text="Vitalício a partir desta" runat="server" /></div>
                            </div>
                        </asp:Panel>
                        <div class="form-group">
                            <label class="col-md-2 control-label">Corretor</label>
                            <div class="col-md-9">
                                <asp:DropDownList ID="cboCorretorExcecao" runat="server" SkinID="comboPadrao1" Width="100%" />
                            </div>
                            <div class="col-md-1"><asp:LinkButton id="lnkAddParcelaExecao" runat="server" OnClick="cmdAddParcelaExcecao_Click" ToolTip="salvar..."><span class="glyphicon glyphicon-floppy-disk" /></asp:LinkButton></div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-9">
                                <asp:GridView ID="gridItensExcecao" runat="server" SkinID="gridPadraoProp" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridItemExcecao_RowCommand" OnRowDataBound="gridItemExcecao_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Parcela" HeaderText="Parcela"/>
                                        <asp:BoundField DataField="Percentual" HeaderText="Percentual" DataFormatString="{0:N2}" />
                                        <asp:TemplateField HeaderText="Corretor" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCorretor" Text='<%# DataBinder.Eval(Container.DataItem, "Corretor.Nome")%>'  runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Vitalício" HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litVitalicio" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar" Visible="false">
                                            <ItemStyle Width="1%" />
                                            <ControlStyle Width="1%" />
                                            <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-12 text-center">
                                <asp:Button ID="cmdSairItensExecao" Text="Fechar" SkinID="botaoPadraoDANGER_Small" runat="server" OnClick="cmdSairItensExecao_Click" /> 
                            </div>
                        </div>

                    </div>
                </div>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

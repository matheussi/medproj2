<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cancelar.aspx.cs" Inherits="MedProj.www.atendimento.cancelar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">Cancelar atendimento</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="panel panel-default" style="position:relative; top:-70px">
        <div class="panel-heading text-left" style="position:relative;">
            <div class="col-xs-12">
                <div class="row">
                    <b>Cancelar atendimento<asp:Literal ID="litPrestador" Visible="false" runat="server" EnableViewState="false" /></b>
                    <asp:Literal ID="litTopoResumo" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <asp:UpdatePanel ID="up" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlHide" Visible="false" EnableViewState="false" runat="server">
                    <div class="row">
                        <div class="text-left col-xs-12">
                            <asp:Literal ID="litNomeUnidade" runat="server" EnableViewState="false" /><br />
                        </div>
                    </div>
                    <hr />
                    </asp:Panel>
                    <div class="form-group">
                        <div class="col-xs-2"><b>Filtro</b></div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-2"  style="padding-left:0px;">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                    de
                                    </span>
                                    <asp:TextBox ID="txtDataDe" Width="80%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                </div>
                            </div>
                            <div class="col-md-2" style="padding-left:0px;">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                    até
                                    </span>
                                    <asp:TextBox ID="txtDataAte" Width="80%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                </div>
                            </div>
                            <div class="col-md-3" style="padding-left:0px;">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                    Cartão
                                    </span>
                                    <asp:TextBox ID="txtNumeroCartao" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="18" onkeypress="filtro_SoNumeros(event);" />
                                </div>
                            </div>
                            <div class="col-md-5" style="padding-left:0px;"></div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            
                            <div class="col-md-4" style="padding-left:0px;">
                                <div class="input-group">
                                    <span class="input-group-addon">Nome</span>
                                    <asp:TextBox ID="txtNome" Width="90%" SkinID="txtPadrao" runat="server" MaxLength="150" />
                                </div>
                            </div>
                            <div class="col-md-3" style="padding-left:0px;">
                                <div class="input-group">
                                    <span class="input-group-addon">CPF&nbsp;&nbsp;&nbsp;&nbsp;</span>
                                    <asp:TextBox ID="txtCPF" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="18" onkeypress="filtro_SoNumeros(event);" />
                                </div>
                            </div>
                            <div class="col-md-5" style="padding-left:0px;margin-top:-2px">
                                <asp:Button ID="cmdLocalizar" Text="pesquisar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdLocalizar_Click" />
                            </div>
                        </div>
                    </div>



                    <div class="form-group">
                        <div class="col-xs-7">
                            <asp:Literal ID="litResultado" runat="server" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-10">
                            <asp:GridView ID="grid" Width="100%" SkinID="gridPadrao"
                                runat="server" AllowPaging="False" AutoGenerateColumns="False"  
                                DataKeyNames="ID" onrowcommand="grid_RowCommand"
                                onrowdatabound="grid_RowDataBound" PageSize="25"
                                OnPageIndexChanging="grid_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="Titular" HeaderText="Nome">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Numero" HeaderText="Cartão">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Wrap="false" Width="1%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Qtd" HeaderText="Qtd" >
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Wrap="false" Width="1%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Wrap="false" Width="1%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FormaPagto" HeaderText="Forma.Pagto">
                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" />
                                        <ItemStyle Wrap="false" />
                                    </asp:BoundField>
                                    <asp:ButtonField ButtonType="Link" CommandName="detalhe" >
                                        <ItemStyle Width="1%" />
                                        <ControlStyle Width="1%" />
                                        <ControlStyle CssClass="glyphicon glyphicon-search" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <!--Modal Detalhe do Atendimento - DESATIVADO-->
                    <div class="modal fade" id="modalDetalhe" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header text-left">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                    <h2 class="modal-title">Detalhe do atendimento para cancelamento</h2>
                                </div>
                                <div class="modal-body">
                                    
                                    <div class="form-group">
                                        <div class="col-xs-12">
                                            <asp:GridView ID="gridDetalhe" Width="100%" SkinID="gridPadrao" runat="server" DataKeyNames="ID">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Especialidade" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Especialidade.Nome")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Código" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Codigo")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Procedimento" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Nome")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:N2}">
                                                        <ItemStyle Wrap="false" Width="1%" />
                                                    </asp:BoundField>

                                                    <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                                        <ItemStyle Width="1%" />
                                                        <ControlStyle Width="1%" />
                                                        <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <!--
                                    <div class="form-group">
                                        <div class="col-xs-12">
                                            <div class="row">
                                                <asp:Literal ID="litAtendimentoEfetivado" EnableViewState="true" runat="server" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:LinkButton CssClass="alert-link" ID="lnkEmail" runat="server" EnableViewState="false" ToolTip="enviar por e-mail" OnClientClick="if(confirm('Deseja enviar o comprovante por e-mail?')) { document.getElementById('cmdFecharModal').click(); } else { return false; }" OnClick="lnkEmail_Click"><span class="glyphicon glyphicon-envelope"></span></asp:LinkButton>
                                                &nbsp;&nbsp;
                                                <asp:LinkButton CssClass="alert-link" ID="lnkPrint" runat="server" EnableViewState="true" ToolTip="versão para impressão"><span class="glyphicon glyphicon-print"></span></asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                    -->
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-info" data-dismiss="modal" id="cmdFecharModal">Fechar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!--FIM - Modal Detalhe do Atendimento-->

                    <asp:Panel ID="pnlDetalhe2" runat="server" Visible="false" >
                        <div style="background:rgba(0,0,0,0.5); width:100%; height:100%; position:fixed; z-index:99999; top:0; left:0;">
                            <div style="position:absolute; background:white; border-radius:10px; width:800px; margin-left:-300px; top:5%; left:50%; padding:30px;">
                                <div class="form-group">
                                    <div class="col-xs-7">
                                        <strong>Detalhe do atendimento para cancelamento</strong>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-7">
                                        <asp:Literal ID="litDetalhe" runat="server" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-12">
                                        <asp:GridView ID="gridDetalhe2" Width="100%" SkinID="gridPadrao" runat="server" DataKeyNames="ID" OnRowCommand="gridDetalhe2_RowCommand" OnRowDataBound="gridDetalhe2_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Especialidade" HeaderStyle-Wrap="false" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Especialidade")%>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Código" HeaderStyle-Wrap="false" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Codigo")%>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Procedimento" HeaderStyle-Wrap="false" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litEspecialidade" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Nome")%>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:N2}">
                                                    <ItemStyle Wrap="false" Width="1%" />
                                                </asp:BoundField>

                                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Excluir">
                                                    <ItemStyle Width="1%" />
                                                    <ControlStyle Width="1%" />
                                                    <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-7">
                                        <asp:Literal ID="litTotal" runat="server" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-7">
                                        <asp:Button id="cmdFecharPnl" class="btn btn-info" runat="server" Text="Fechar" OnClick="cmdFecharPnl_Click" />
                                    </div>
                                </div>
                               

                            </div>
                        </div>

                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script>
        function showModal() {
            $('#modalDetalhe').modal('show');
        }
    </script>
</asp:Content>

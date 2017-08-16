<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="importacoesProcedimento.aspx.cs" Inherits="MedProj.www.adm.importacao.importacoesProcedimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Agenda de atribuição de procedimentos
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Nova agenda" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Filtro:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-2"  style="padding-left:0px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                        de
                                        </span>
                                        <asp:TextBox ID="txtDataDe" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                    </div>
                                </div>
                                <div class="col-md-2" style="padding-left:0px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                        até
                                        </span>
                                        <asp:TextBox ID="txtDataAte" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="10" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" OnClick="cmdProcurar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="text-right col-md-12"></div>
                        <div class="space"></div>

                        <asp:GridView ID="grid" runat="server" SkinID="gridPadraoProp" Width="100%" 
                            AutoGenerateColumns="False" AllowPaging="true" PageSize="100" 
                            OnPageIndexChanging="grid_PageIndexChanging" DataKeyNames="ID"
                            OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="DataCriacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />

                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-Width="40%" />

                                <asp:TemplateField HeaderText="Tabela" HeaderStyle-Wrap="false" >
                                    <ItemTemplate>
                                        <asp:Literal ID="litOperadora" Text='<%#DataBinder.Eval(Container.DataItem, "Tabela.Nome")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="DataProcessamento" HeaderText="Ocorrer em" HeaderStyle-Wrap="false" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="DataConclusao" HeaderText="Conclusão" DataFormatString="{0:dd/MM/yyyy}" />

                                <%--<asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton CssClass="glyphicon glyphicon-envelope" CommandName="Email" CommandArgument='<%# Container.DataItemIndex %>' ID="lnkEmail" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>

                                <asp:ButtonField ButtonType="Link" Text="Excluir" CommandName="Excluir" Visible="false">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Link" Text="" CommandName="Editar">
                                    <ItemStyle Width="1%" />
                                    <ControlStyle Width="1%" />
                                    <ControlStyle CssClass="glyphicon glyphicon-pencil" />
                                </asp:ButtonField>

                                <asp:ButtonField ButtonType="Image" CommandName="Log" ImageUrl="~/Images/excel.png" Visible="false">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <asp:Literal ID="litMensagem" EnableViewState="false" runat="server" />
                   
                    </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="regras.aspx.cs" Inherits="MedProj.www.adm.comissao.regras" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Regras de comissionamento
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>


            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="space"></div>

                    <asp:GridView ID="grid" Width="100%" SkinID="gridPadrao"
                        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                        DataKeyNames="ID" onrowcommand="grid_RowCommand" 
                        onrowdatabound="grid_RowDataBound" PageSize="50"
                        OnPageIndexChanging="grid_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Nome" HeaderText="Descrição">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Associado" HeaderStyle-Wrap="false" >
                                <ItemTemplate>
                                    <asp:Literal ID="litAssoc" Text='<%#DataBinder.Eval(Container.DataItem, "Estipulante.Nome")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:ButtonField Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="Editar" >
                                <ItemStyle Height="16px" Width="16px" />
                                <ControlStyle Height="16px" Width="16px" />
                            </asp:ButtonField>
                            <asp:ButtonField Text="<img src='../../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                                <ItemStyle Height="16px" Width="16px" />
                                <ControlStyle Height="16px" Width="16px" /> 
                            </asp:ButtonField>
                        </Columns>
                    </asp:GridView>
                    <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

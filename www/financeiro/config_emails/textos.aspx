﻿<%@ Page Theme="padrao" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="textos.aspx.cs" Inherits="MedProj.www.financeiro.config_emails.textos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Textos para avisos aos clientes
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading text-right" style="position:relative;">
                    <div style="position:absolute; right:0; top:-70px;"><asp:Button ID="lnkNovo" Text="Novo" runat="server" EnableViewState="false" SkinID="botaoPadrao1" OnClick="lnkNovo_Click" /></div>
                    <div style="position:absolute; right:80px; top:-70px;"><asp:Button ID="lnkVoltar" Text="Voltar" runat="server" EnableViewState="false" SkinID="botaoPadraoWarning" OnClick="lnkVoltar_Click" /></div>
                    <asp:Panel ID="pnlFiltro" Visible="false" EnableViewState="false" runat="server">
                    <div class="col-md-12">
                        <div class="row">
                            <label class="col-md-4 text-left">Associado PJ:</label>
                            <label class="col-md-3 text-left">Contrato Adm:</label>
                            <label class="col-md-5 text-left">Contrato:</label>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-4"  style="padding-left:0px;">
                                      <asp:DropDownList ID="cboAssociadoPJ" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" />
                                </div>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="cboContratoADM" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" />
                                </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="cboContrato" Width="100%" SkinID="comboPadrao1" runat="server" />
                                </div>
                                <div class="col-md-1">
                                    <asp:Button ID="cmdProcurar" Text="Procurar" SkinID="botaoPadrao1" EnableViewState="false" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div><%----%>
                    <div class="clearfix"></div>
                    </asp:Panel>
                </div>
                <div class="panel-body">
                    <div class="space"></div>

                    <asp:GridView ID="grid" Width="100%" SkinID="gridPadrao"
                        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                        DataKeyNames="ID" onrowcommand="grid_RowCommand" 
                        onrowdatabound="grid_RowDataBound" PageSize="50"
                        OnPageIndexChanging="grid_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:ButtonField Text="<img src='../../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="Editar" >
                                <ItemStyle Height="16px" Width="16px" />
                                <ControlStyle Height="16px" Width="16px" />
                            </asp:ButtonField>
                            <asp:ButtonField Visible="false" Text="<img src='../../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
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

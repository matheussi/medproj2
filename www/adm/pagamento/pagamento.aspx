<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="pagamento.aspx.cs" Inherits="MedProj.www.adm.pagamento.pagamento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Agenda de pagamento
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">

    <asp:UpdatePanel ID="up" runat="server">
        <%--<Triggers>
            <asp:PostBackTrigger ControlID="lnkArquivoLog" />
        </Triggers>--%>
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkArquivo" />
        </Triggers>
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">&nbsp;</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Descrição</label>
                        <div class="col-xs-8"><asp:TextBox ID="txtDescricao" runat="server" Width="100%" MaxLength="149" SkinID="txtPadrao" /></div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Executar em</label>
                        <div class="col-xs-2"><asp:TextBox ID="txtExecutarEm" runat="server" Width="100%" MaxLength="10" SkinID="txtPadrao" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);"/></div>
                    </div>
                    <asp:Panel ID="pnlPrestador" EnableViewState="false" Visible="false" runat="server">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Prestador</label>
                            <div class="col-xs-8"><asp:DropDownList ID="cboPrestador" runat="server" Width="100%" MaxLength="10" SkinID="comboPadrao1" /></div>
                        </div>
                    </asp:Panel>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Período</label>
                        <div class="col-xs-2">
                            <div class="input-group">
                                <span class="input-group-addon">de</span>
                                <asp:TextBox ID="txtDe" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                            </div>
                        </div>
                        <div class="col-xs-2">
                            <div class="input-group">
                                <span class="input-group-addon">até</span>
                                <asp:TextBox ID="txtAte" Width="100%" SkinID="txtPadrao" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Tipo</label>
                        <div class="col-xs-3">
                            <asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboTipo">
                                <asp:ListItem Text="selecione" Value="-1" Selected="True" />
                                <asp:ListItem Text="Mensal" Value="0" />
                                <asp:ListItem Text="Quinzenal" Value="1" />
                                <asp:ListItem Text="Semanal" Value="2" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <asp:Panel ID="pnlUpload" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Arquivo CSV</label>
                            <div class="col-xs-8">
                                <div style="border:1px solid #dedede; border-radius:5px; padding:5px;">
                                    <input type="file" id="fileUploadImage" style="width:100%" runat="server" visible="false" />
                                    <asp:HiddenField runat="server" ID="listaNovosNomes" />
                                    <asp:HiddenField runat="server" ID="listaNomes" />
                                    <div style="float:left; padding-right:15px;">
                                        <input id="sendAnexos" name="sendAnexos" type="file" />
                                    </div>
                                    <div id='listAnexos' style="float:left;"></div>
                                    <div style="clear:both"></div>
                                </div>
                            </div>
                            <div class="col-xs-2" style="text-align:left; margin:-2px;">
                                <asp:Button ID="btnUpload" runat="server" Text="Enviar" Width="100%" SkinID="botaoPadraoINFO" Visible="false" />
                            </div>
                            <div class="clearfix"></div>
                            <div class="space"></div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlDetalhe" Visible="false" runat="server">
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:Literal ID="litDownload" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:LinkButton ID="lnkArquivo" Text="Clique para abrir o arquivo gerado" runat="server" OnClick="lnkArquivo_Click" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:CheckBox ID="chkInativo" Text="Inativa" runat="server" ForeColor="Red" />
                                <asp:Literal ID="litErro" runat="server" />
                            </div>
                        </div>
                    </asp:Panel>

                    <hr />
                    <div class="col-xs-12 text-right">
                        <asp:Button ID="cmdVoltar" Text="Voltar" runat="server" OnClick="cmdVoltar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                        <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" EnableViewState="false" SkinID="botaoPadrao1" />
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
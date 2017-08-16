<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="importacao.aspx.cs" Inherits="MedProj.www.adm.importacao.importacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8">
    <link rel='stylesheet' href='<% Response.Write(ResolveClientUrl("~/css/uploadify.css")); %>' /> 
    <script src='<% Response.Write(ResolveClientUrl("~/Scripts/swfobject/swfobject.js")); %>'  type="text/javascript"></script>
    <script src='<% Response.Write(ResolveClientUrl("~/Scripts/uploadify/jquery.uploadify.v2.1.0.min.js")); %>'  type="text/javascript"></script>

    <script type="text/javascript">
        
        // <![CDATA[

        function chamada() {
            $('#sendAnexos').uploadify({
                'uploader': '<% Response.Write(ResolveClientUrl("~/Scripts/uploadify/uploadify.swf")); %>',
                'script': '<% Response.Write(ResolveClientUrl("~/proxy/UploadMult.ashx")); %>',
                'scriptData': {},
                'cancelImg': '<% Response.Write(ResolveClientUrl("~/Images/cancelar_upload.png")); %>',
                'auto': true,
                'multi': false,
                'queueSizeLimit': 90,
                'fileDesc': "Files",
                'queueSizeLimit': 1073741824,
                'sizeLimit': 1073741824,
                'buttonText': 'Selecione',
                'buttonImg': '<% Response.Write(ResolveClientUrl("~/Images/bgBotaoAnexo.jpg")); %>',
                'folder': '<% Response.Write(ResolveClientUrl(ConfigurationManager.AppSettings["AnexosTemp"])); %>',
                'onComplete': function (event, queueID, fileObj, response, data) {
                    splNomes = response.split("|");
                    if (document.getElementById("<%= listaNomes.ClientID %>")) {
                        if (document.getElementById("<%= listaNomes.ClientID %>").value.indexOf(splNomes[1]) == -1) {
                            document.getElementById("<%= listaNomes.ClientID %>").value = splNomes[1];
                            document.getElementById("<%= listaNovosNomes.ClientID %>").value = splNomes[0];
                        }
                    }
                },
                'onAllComplete': function (event, queueID, fileObj, response, data) {
                    montaAnexos();
                },
                'onError': function (event, queueID, fileObj, errorObj) {
                    alert(errorObj.info);
                    //alert("Error: "+errorObj.type+"      Info: "+errorObj.info);
                }
            });
        }

        function montaAnexos() {
            if (document.getElementById("<%= listaNomes.ClientID %>")) {
                splAnexos = document.getElementById("<%= listaNomes.ClientID %>").value.split(",");

                document.getElementById("listAnexos").innerHTML = "";

                for (i = 0; i < splAnexos.length; i++) {
                    if (splAnexos[i] != "")
                        document.getElementById("listAnexos").innerHTML +=
                            "<div style='float:left; height:20px; margin-right:15px;'>" +
                            "   <table border='0' cellpadding='0' cellspacing='0'>" +
                            "       <tr>" +
                            "           <td style='height:16px;'><img src='<% Response.Write(ResolveClientUrl("~/Images/icon-novo.gif")); %>' alt='' border='0' /></td>" +
                            "           <td style='height:16px; padding:2px;'><span class='fonte11'>" + splAnexos[i] + "&nbsp;&nbsp;</span></td>" +
                            "           <td style='height:16px;'><img src='<% Response.Write(ResolveClientUrl("~/Images/cancel.png")); %>' alt='' border='0' style='cursor:pointer;' onclick='if (confirm(\"Confirma Exclusão?\")) { excluir_arquivo(" + i + ") } else { return false; }' /></td>" +
                            "       </tr>" +
                            "   </table>" +
                            "</div>";
                }
            }
        }

        function excluir_arquivo(indice)
        {
            arrAnexos = $('#<%= listaNomes.ClientID %>').val().split(",");
            arrAnexosNovosNomes = $('#<%= listaNovosNomes.ClientID %>').val().split(",");

            document.getElementById("<%= listaNomes.ClientID %>").value = "";
            document.getElementById("<%= listaNovosNomes.ClientID %>").value = "";

            for (i = 0; i < arrAnexos.length - 1; i++) {
                if (i != indice) {
                    document.getElementById("<%= listaNomes.ClientID %>").value += arrAnexos[i] + ",";
                    document.getElementById("<%= listaNovosNomes.ClientID %>").value += arrAnexosNovosNomes[i] + ",";
                }
            }

            montaAnexos();
        }
        // ]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Agenda de importação
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">

    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdSalvar" />
            <asp:PostBackTrigger ControlID="lnkArquivoLog" />
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

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Filial</label>
                        <div class="col-xs-3"><asp:DropDownList ID="cboFilial" runat="server" Width="100%" MaxLength="10" SkinID="comboPadrao1" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Associado PJ</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboEstipulante" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_SelectedIndexChanged" /></div>
                        <label class="col-xs-2 control-label">Operadora</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="True" Width="100%" runat="server" ID="cboOperadora" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" /></div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-2 control-label">Contrato</label>
                        <div class="col-xs-3"><asp:DropDownList SkinID="comboPadrao1" AutoPostBack="True" Width="100%" runat="server" ID="cboContrato" OnSelectedIndexChanged="cboContrato_SelectedIndexChanged" /></div>
                        <label class="col-xs-2 control-label">Plano</label>
                        <div class="col-xs-3">
                            <asp:DropDownList SkinID="comboPadrao1" Width="100%" runat="server" ID="cboPlano" AutoPostBack="false" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-8 col-xs-offset-2">
                            <asp:CheckBox ID="chkNaoCriticarCpf" Text="Não criticar cpf" runat="server" ForeColor="Red" />
                        </div>
                    </div>

                    <%--<div class="form-group">
                        <label class="col-xs-2 control-label">Acomodação</label>
                        <div class="col-xs-2"><asp:DropDownList SkinID="comboPadrao1" Width="395px" runat="server" ID="cboAcomodacao" /></div>
                    </div>--%>

                    <asp:Panel ID="pnlUpload" runat="server">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Arquivo CSV</label>
                            <%----%>
                            <div class="col-xs-10">
                                <asp:FileUpload ID="fuArquivoCSV" Width="100%" runat="server" />
                                <asp:HiddenField runat="server" ID="listaNovosNomes" />
                                <asp:HiddenField runat="server" ID="listaNomes" />
                            </div> 
                            <%--
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
                                <asp:Button ID="btnUpload" runat="server" Text="Enviar" Width="100%" OnClick="btnUpload_Click" SkinID="botaoPadraoINFO" Visible="false" />
                            </div>
                            --%>
                            <div class="clearfix"></div>
                            <div class="space"></div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlDownload" Visible="false" runat="server">
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:Literal ID="litDownload" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-8 col-xs-offset-2">
                                <asp:LinkButton ID="lnkArquivoLog" Text="Clique para abrir o arquivo de log" runat="server" OnClick="lnkArquivoLog_Click" />
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
    <script type="text/javascript">
        /*
        $(document).ready(function ()
        {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(setupUploadFile);

            setupUploadFile(null, null);
        });

        function setupUploadFile(sender, args)
        {
            chamada();
        }
        */
    </script>
</asp:Content>
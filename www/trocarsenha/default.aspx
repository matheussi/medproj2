<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MedProj.www.trocarsenha._default" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Clube Azul - Alterar Senha</title>
    <meta id="Meta1" charset="iso-8859-1"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <link href="../css/typica-login.css" rel="stylesheet" />
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
    <link href="../css/bootstrap-responsive.min.css" rel="stylesheet" />
    <script src="<%# Page.ResolveClientUrl("~/Scripts/common.js") %>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="sm" runat="server" EnableScriptGlobalization="true"></ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="up" runat="server">
            <ContentTemplate>

            <div class="navbar navbar-fixed-top">
                <div class="navbar-inner" style="background-color:#E1EDF3;border-bottom: solid 1px #003366">
                    <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </a>
                    <div class="col-xs-4">
                        <a class="brand" target="_blank" href="http://www.clubeazul.org.br"><img src="../Images/clubeazul.png" alt="" height='70'></a>
                    </div>
                    <div class="col-xs-4" style="font-size:10px; text-align:center; font-weight:normal; text-transform:capitalize;">
                        <b>Atendimento:</b><br />Segunda a Quinta das 08:30 às 18h - Sexta das 08:30 às 17h<br />
                        Ligações do Rio de Janeiro: (21) 3916-7277<br />Demais localidades: 4020-1610<br />
                        E-mail: atendimento@clubeazul.org.br
                    </div>
                    <div class="col-xs-4" style="text-align:right;">
                        <div style="margin:-10px">
                            <a href="../login.aspx?ac=1" class="btn btn-info">área administrativa</a>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>

            <div class="container">
                <div id="login-wraper" style="width:850px; margin-left:-400px;height:auto">
            
                    <div>
                        <span style="font-size:32px; font-family:Lato;">Alterar Senha</span>
                    </div>
                    <div class="body">
                        <br />
                        <div class="form-group">
                            <label class="col-xs-12 control-label text-left">Por favor, informe o número do cartão.</label>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-5">
                                <asp:TextBox ID="txtNumeroContrato" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="16" EnableViewState="false" />
                            </div>
                            <div class="col-xs-7 text-left" style="top:-3px">
                                <asp:Button ID="cmdValidarNumeroContrato" Text="Validar" SkinID="botaoPadraoSUCCESS" OnClick="cmdValidarNumeroContrato_Click" runat="server" />
                            </div>
                        </div>
                        <div class='clearfix'></div>
                        
                        <asp:Panel ID="pnlResult" Visible="false" runat="server">
                            <div class='clearfix'></div><br />
                            <div class="alert alert-success" role="alert">

                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">Nome</label>
                                    <div class="col-xs-10 text-left">
                                        <asp:Literal ID="litNome" runat="server" />
                                    </div>
                                </div>
                                <div class='clearfix'></div>
                                <div class="form-group text-left" >
                                    <label class="col-xs-2 control-label" style="font-size:14px">Data Nasc.</label>
                                    <div class="col-xs-10 text-left">
                                        <asp:Literal ID="litData" runat="server" />
                                    </div>
                                </div>
                                <div class='clearfix'></div>
                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">CPF</label>
                                    <div class="col-xs-10 text-left">
                                        <asp:Literal ID="litCPF" runat="server" />
                                    </div>
                                </div>
                                <div class='clearfix'></div>
                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">E-mail</label>
                                    <div class="col-xs-10 text-left">
                                        <asp:Literal ID="litEmail" runat="server" />
                                    </div>
                                </div>

                                <div class='clearfix'></div><br />

                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">Senha atual</label>
                                    <div class="col-xs-3 text-left" style="top:-4px">
                                        <asp:TextBox ID="txtSenhaAtual" SkinID="txtPadrao" TextMode="Password" Width="100%" MaxLength="6" runat="server" />
                                    </div>
                                </div>
                                <div class='clearfix'></div>
                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">Nova senha</label>
                                    <div class="col-xs-3 text-left" style="top:-4px">
                                        <asp:TextBox ID="txtNovaSenha1" onkeypress="filtro_SoNumeros(event);" SkinID="txtPadrao" TextMode="Password" Width="100%" MaxLength="6" runat="server" />
                                    </div>
                                    <div class="col-xs-7 text-left" style="left:-19px;top:-1px;font-size:12px">
                                        A senha deve ser numérica, ter 6 dígitos, e não deve iniciar em zero (0)
                                    </div>
                                </div>
                                <div class='clearfix'></div>
                                <div class="form-group text-left">
                                    <label class="col-xs-2 control-label" style="font-size:14px">Confirme</label>
                                    <div class="col-xs-3 text-left" style="top:-4px">
                                        <asp:TextBox ID="txtNovaSenha2" onkeypress="filtro_SoNumeros(event);" SkinID="txtPadrao" TextMode="Password" Width="100%" MaxLength="6" runat="server" />
                                    </div>
                                    <div class="col-xs-3 text-left" style="top:-9px">
                                        <asp:Button ID="cmdConfirmar" Text="Confirmar" OnClientClick="return confirm('Deseja realmente alterar sua senha?');" SkinID="botaoPadraoSUCCESS" OnClick="cmdConfirmar_Click" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>

    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="../Scripts/jquery-1.10.2.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/backstretch.min.js"></script>
    <script src="../Scripts/typica-login.js"></script>
</body>
</html>

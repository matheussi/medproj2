<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="MedProj.www.login" %>
<!DOCTYPE html>
<html lang="pt">
  <head runat="server">
    <title>Clube Azul - Entrar</title>
    <meta id="Meta1" charset="iso-8859-1"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

    <link href="css/typica-login.css" rel="stylesheet">
    <link href="css/bootstrap.min.css" rel="stylesheet">
    <link href="css/bootstrap-responsive.min.css" rel="stylesheet">

  </head>

  <body>
<form class="form login-form" runat="server" id="form1">
    <div class="navbar navbar-fixed-top">
      <div class="navbar-inner" style="background-color:#E1EDF3;border-bottom: solid 1px #003366">
      <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </a>
      <a class="brand" target="_blank" href="http://www.clubeazul.org.br"><img src="Images/clubeazul.png" alt="" height='70'></a>
    </div>
    </div>



    <div class="container">
        <div style="position:absolute; width:420px; left:50%; margin-left: -235px; text-align:center; top:450px; ">
            <br /><b>Atendimento:</b><br />Segunda a Quinta das 08:30 às 18h - Sexta das 08:30 às 17h<br />
            Ligações do Rio de Janeiro: (21) 3916-7277<br />Demais localidades: 4020-1610<%--Telefone: (21) 4020-1610--%><br />
            E-mail: atendimento@clubeazul.org.br<br />
        </div>
        <div id="login-wraper">
            
            <legend>Entrar no Sistema<span class="blue"></span></legend>
            <div class="body">
                <div class="form-group">
                    <label for="inputEmail3" class="col-sm-2 control-label">Login</label>
                    <div class="col-sm-10">
                        <%--<input type="email" class="form-control" id="inputEmail3" placeholder="email">--%>
                        <input type="text"  class="form-control" id="txtLogin" placeholder="Email" runat="server" />
                    </div>
                </div>
                <div class='clearfix'></div>
                <div class="form-group" style='margin-top:10px;'>
                    <label for="inputEmail3" class="col-sm-2 control-label">Senha</label>
                    <div class="col-sm-10">
                    <%--<input type="email" class="form-control" id="Email1" placeholder="Senha">--%>
                        <input type="password" class="form-control" id='txtSenha' runat="server" placeholder="Password" />
                    </div>
                </div>
                <div class='clearfix'></div>
            </div>

            
            
            <div class="footer">
                <label class="checkbox inline">
                    <span class='fonte11'><font color='red'><b><asp:Literal ID="litErro" runat="server" /></b></font></span>
                </label>
                <div class="pull-right">
                    <asp:Button ID="cmdEntrar" class="btn btn-info" runat="server" OnClick="cmdEntrar_Click" Text="Entrar" EnableViewState="false"/>
                </div>
                <%--<button type="submit" class="btn btn-success">Login</button>--%>
            </div>
        </div>

    </div>
     
    <footer class="white navbar-fixed-bottom" style="border-top: solid 1px #003366">
      <div class='pull-right' style="margin:-12px 10px 10px 10px;"><%--<img src='images/site/link.png' height="36" width="62">--%></div>
        <div style="margin:-10px">
            Esqueceu sua senha, <a href="#" onclick="$('#modalSenha').modal('show');" class="btn btn-info">clique aqui!</a>
        </div>
    </footer>

    <div class="modal fade" id="modalSenha" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header text-left">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h2 class="modal-title" id="H1">Reenvio de senha</h2>
                </div>
                <div class="modal-body">
                        <table class="table table-bordered table-striped">
                        <tr>
                            <td width="30%"><strong>Login:</strong></td>
                            <td align="left"><asp:TextBox ID="EmailSenha" runat="server" SkinID="txtPadrao" MaxLength="85" /></td>
                        </tr>
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal" id="cmdFecharModal">Fechar</button>
                    <asp:Button ID="cmdEnviarSenha" Text="Enviar" SkinID="botaoPadrao1" OnClick="cmdEnviarSenha_Click" runat="server" OnClientClick="if(getElementById('EmailSenha').value == '') { alert('Informe seu login.'); return false;} document.getElementById('cmdFecharModal').click();" />
                </div>
            </div>
        </div>
    </div>
</form>

    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/backstretch.min.js"></script>
    <script src="Scripts/typica-login.js"></script>

  </body>
</html>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_testEmail.aspx.cs" Inherits="MedProj.www._testEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td>From</td>
                <td><asp:TextBox ID="txtFrom" runat="server" /></td>
            </tr>
            <tr>
                <td>To</td>
                <td><asp:TextBox ID="txtTo" runat="server" Text="matheussi@gmail.com" /></td>
            </tr>
            <tr>
                <td>SMTP</td>
                <td><asp:TextBox ID="txtSmtp" runat="server" Text="localhost" /></td>
            </tr>
            <tr>
                <td>Porta</td>
                <td><asp:TextBox ID="txtPorta" runat="server" /></td>
            </tr>
            <tr>
                <td>Login</td>
                <td><asp:TextBox ID="txtLogin" runat="server" /></td>
            </tr>
            <tr>
                <td>Senha</td>
                <td><asp:TextBox ID="txtSenha" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button ID="cmdEnviar" runat="server" OnClick="cmdEnviar_Click" /></td>
            </tr>
        </table>
    </form>
</body>
</html>

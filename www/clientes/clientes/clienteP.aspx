<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clienteP.aspx.cs" Inherits="MedProj.www.clientes.clientes.clienteP" %>
<%@ Register Src="~/usercontrols/ucBeneficiarioForm.ascx" TagPrefix="uc1" TagName="ucBeneficiarioForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pessoa - Detalhe</title>
    <link href="../../css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../../css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../css/style.css" rel="stylesheet" />
    <link href="../../css/typica-login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" role="form" class="form-horizontal" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="sm" runat="server"></ajaxToolkit:ToolkitScriptManager>

        <asp:UpdatePanel ID="up" runat="server" >
            <ContentTemplate>
                <uc1:ucBeneficiarioForm runat="server" ID="ucBeneficiarioForm" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>

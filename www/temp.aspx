<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="temp.aspx.cs" Inherits="MedProj.www.temp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
        <br />
        <ajaxToolkit:TabContainer BorderStyle="None" BorderWidth="0" Width="50%" ID="TabContainer1" runat="server" ActiveTabIndex="0" Visible="true">
            <ajaxToolkit:TabPanel runat="server"  ID="TabPanel1" Visible="true">
                <HeaderTemplate>teste</HeaderTemplate>
                <ContentTemplate>
                    asdfasdf
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel runat="server"  ID="TabPanel2" Visible="true">
                <HeaderTemplate>endereço</HeaderTemplate>
                <ContentTemplate>
                    blalala
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

        </ajaxToolkit:TabContainer>
    
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="pessoa.aspx.cs" Inherits="MedProj.www.clientes.pessoas.pessoa" %>
<%@ Register Src="~/usercontrols/ucBeneficiarioForm.ascx" TagPrefix="uc1" TagName="ucBeneficiarioForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../css/style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Pessoa
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <uc1:ucBeneficiarioForm CarregarDeInicio="true" runat="server" ID="ucBeneficiarioForm1" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
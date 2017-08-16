<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MedProj.www.orientador._default" %>

<!DOCTYPE html>

<html lang="pt">
  <head id="Head1" runat="server">
    <title>Clube Azul - Orientador</title>
    <meta id="Meta1" charset="iso-8859-1"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

    <link href="../css/typica-login.css" rel="stylesheet" />
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
    <link href="../css/bootstrap-responsive.min.css" rel="stylesheet" />

  </head>

    <body>

        <form class="form login-form" runat="server" id="form1">

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
                        <span style="font-size:32px; font-family:Lato;">Orientador</span><br />
                        <span style="font-size:14px;">Por favor, utilize o filtro abaixo e localize um prestador perto de você.</span><span class="blue"></span>
                    </div>
                    <div class="body">
                        <div class="form-group">
                            <label class="col-sm-2 control-label">Especialidade</label>
                            <div class="col-sm-10">
                                <asp:DropDownList ID="cboEspecialidade" Width="100%" SkinID="comboPadrao1" runat="server" />
                            </div>
                        </div>
                        <div class='clearfix'></div>
                        <div class="form-group">
                            <label class="col-sm-2 control-label">Estado</label>
                            <div class="col-sm-2">
                                <asp:DropDownList ID="cboUf" Width="100%" SkinID="comboPadrao1" runat="server" OnSelectedIndexChanged="cboUf_SelectedIndexChanged" AutoPostBack="true" />
                            </div>
                            <label class="col-sm-2 control-label">Cidade</label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="cboCidade" Width="100%" SkinID="comboPadrao1" runat="server" OnSelectedIndexChanged="cboCidade_SelectedIndexChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class='clearfix'></div>
                        <div class="form-group">
                            <label class="col-sm-2 control-label">Bairro</label>
                            <div class="col-sm-8">
                                <asp:DropDownList ID="cboBairro" Width="100%" SkinID="comboPadrao1" runat="server" />
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="cmdPesquisar" class="btn btn-info" runat="server" Text="Pesquisar" Width="100%" OnClick="cmdPesquisar_Click" EnableViewState="false"/>
                            </div>
                        </div>
                        <div class='clearfix'></div>

                        <asp:Panel ID="pnlResult" Visible="false" runat="server">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <br />
                                    <asp:GridView ID="grid" Width="100%" SkinID="gridPadrao"
                                        runat="server" AllowPaging="False" AutoGenerateColumns="False"  
                                        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound" PageSize="25" OnPageIndexChanging="grid_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="Prestador" HeaderText="Prestador">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Endereco" HeaderText="Endereço">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle Wrap="false"  />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Bairro" HeaderText="Bairro">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle Wrap="false"  />
                                            </asp:BoundField>
                                    
                                            <asp:BoundField DataField="Cidade" HeaderText="Cidade">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle Wrap="false"  />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="UF" HeaderText="UF">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle Wrap="false" Width="1%"  />
                                            </asp:BoundField>
                                    
                                            <asp:BoundField DataField="Telefone" HeaderText="Telefone">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlNoResult" Visible="false" runat="server">
                            <div class="form-group">
                                <label class="col-sm-12 control-label" style="font-size:14px">Nenhum prestador pôde ser localizado</label>
                            </div>
                        </asp:Panel>
                    </div>

            
            
                    <%--<div class="footer">
                        <label class="checkbox inline">
                            <span class='fonte11'><font color='red'><b><asp:Literal ID="litErro" runat="server" /></b></font></span>
                        </label>
                        <div class="pull-right">
                            botao
                        </div>
                    </div>--%>
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

<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="default.aspx.cs" Inherits="MedProj.www._default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script src="Scripts/highcharts/highcharts.js" ></script>--%>
    <script src="http://code.highcharts.com/highcharts.js"></script>
    <script src="http://code.highcharts.com/modules/exporting.js"></script>

    <%--<script src="http://canvg.googlecode.com/svn/trunk/rgbcolor.js" ></script>
    <script src="http://canvg.googlecode.com/svn/trunk/StackBlur.js" ></script>
    <script src="http://canvg.googlecode.com/svn/trunk/canvg.js" ></script>--%>
 </asp:Content>

<asp:Content ID="topo" ContentPlaceHolderID="title" runat="server">


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">

    <asp:Panel ID="pnlDadosPrestador" runat="server" Visible="false">
        <div class="panel panel-default" style="position:relative; top:-70px">
            <div class="panel-heading text-left" style="position:relative;">
                <div class="col-md-12">
                    <div class="row">
                        <asp:Literal ID="litPrestador" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="text-left col-md-12">
                        <asp:Literal ID="litNomeUnidade" runat="server" EnableViewState="false" /><br />
                        <asp:Literal ID="litEnderecoUnidade" runat="server" EnableViewState="false" /><br />
                        <asp:Literal ID="litContatoUnidade" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnl" runat="server" Visible="false">
    <div style="position:absolute;margin-top:-60px;margin-left:20px; width:30%; min-width:450px;right:30px">
        <div class="pull-right">
            <div class="form-group">
                <div class="col-xs-5">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">De</span>
                        <asp:TextBox SkinID="txtPadrao" runat="server" MaxLength="10" ID="txtDe" Width="100%" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                    </div>
                </div>
                <div class="col-xs-5">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">Até</span>
                        <asp:TextBox SkinID="txtPadrao" runat="server" MaxLength="10" ID="txtAte" Width="100%" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);"  />
                    </div>
                </div> 

                <div class="col-xs-2" style="margin:-2px;">
                    <asp:Button Text="Filtrar" ID="cmdFiltrar" SkinID="botaoPadraoINFO" runat="server" OnClick="cmdFiltrar_Click" />
                </div>
            </div>
        </div>
    </div>


    <!--main content start-->

    <section id="main-content">
        <!--tiles start-->
        <div class="row">
            <div class="col-md-3 col-sm-6">
                <div class="dashboard-tile detail tile-red">
                    <div class="content">
                        <a id="aBox1" runat="server" href="#">
                            <h1 runat="server" id="h1Prioritarias" class="text-left timer" data-from="0" data-speed="2500" enableviewstate="true"></h1>
                            <p>Retornos não realizados<br /></p>
                        </a>
                    </div>
                    <div class="icon"><i class="fa fa-users"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3 col-sm-6">
                <div class="dashboard-tile detail tile-red">
                    <div class="content">
                        <a id="aBox2" runat="server" href="#">
                            <h1 runat="server" id="h1Todas" class="text-left timer" data-from="0" data-speed="2500" enableviewstate="true"></h1>
                            <p>E-mails não visualizados</p>
                        </a>
                    </div>
                    <div class="icon"><i class="fa fa-comments"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3 col-sm-6">
                <div class="dashboard-tile detail tile-blue">
                    <div class="content">
                        <a id="aBox3" runat="server" href="#">
                            <h1 runat="server" id="h1Novas" class="text-left timer" data-from="0" data-to="32" data-speed="2500" enableviewstate="true"></h1>
                            <p>Novas propostas</p>
                        </a>
                    </div>
                    <div class="icon"><i class="fa fa fa-envelope"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3 col-sm-6">
                <div class="dashboard-tile detail tile-purple">
                    <div class="content">
                        <a id="aBox4" runat="server" href="#">
                            <h1 runat="server" id="h1Fechadas" class="text-left timer" data-to="105" data-speed="2500" enableviewstate="true"></h1>
                            <p>Propostas fechadas</p>
                        </a>
                    </div>
                    <div class="icon"><i class="fa fa-bar-chart-o"></i>
                    </div>
                </div>
            </div>
        </div>
        <!--tiles end-->


        <!--dashboard charts and map start-->
        <!--
        <div class="row">
            <div class="col-md-6">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Sales for 2014</h3>
                        <div class="actions pull-right">
                            <i class="fa fa-chevron-down"></i>
                            <i class="fa fa-times"></i>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div id="sales-chart" style="height: 250px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Office locations</h3>
                        <div class="actions pull-right">
                            <i class="fa fa-chevron-down"></i>
                            <i class="fa fa-times"></i>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div class="map" id="map" style="height: 250px;"></div>
                    </div>
                </div>
            </div>
        </div>
        -->
        <!--dashboard charts and map end-->
        <!--ToDo start-->
        <!--
        <div class="row">
            <div class="col-md-4">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">To do list</h3>
                        <div class="actions pull-right">
                            <i class="fa fa-chevron-down"></i>
                            <i class="fa fa-times"></i>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <input id="new-todo" type="text" class="form-control" placeholder="What needs to be done?">
                                    <section id='main'>
                                        <ul id='todo-list'></ul>
                                    </section>
                                </div>
                                <div class="form-group">
                                    <button id="todo-enter" class="btn btn-primary pull-right">Submit</button>
                                    <div id='todo-count'></div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title">Server Status</h3>
                                <div class="actions pull-right">
                                    <i class="fa fa-chevron-down"></i>
                                    <i class="fa fa-times"></i>
                                </div>
                            </div>
                            <div class="panel-body">

                            <span class="sublabel">Memory Usage</span>
                            <div class="progress progress-striped">
                            <div class="progress-bar progress-bar-info" style="width: 20%">20%</div>
                            </div><!-- progress --

                            <span class="sublabel">CPU Usage </span>
                            <div class="progress progress-striped">
                            <div class="progress-bar progress-bar-default" style="width: 60%">60%</div>
                            </div><!-- progress --

                            <span class="sublabel">Disk Usage </span>
                            <div class="progress progress-striped">
                            <div class="progress-bar progress-bar-primary" style="width: 80%">80%</div>
                            </div><!-- progress --
                  
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="panel panel-solid-info">
                    <div class="panel-heading">
                        <h3 class="panel-title">Weather</h3>
                        <div class="actions pull-right">
                            <i class="fa fa-chevron-down"></i>
                            <i class="fa fa-times"></i>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h3 class="text-center small-thin uppercase">Today</h3>
                                <div class="text-center">
                                    <canvas id="clear-day" width="110" height="110"></canvas>
                                    <h4>62°C</h4>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <h3 class="text-center small-thin uppercase">Tonight</h3>
                                <div class="text-center">
                                    <canvas id="partly-cloudy-night" width="110" height="110"></canvas>
                                    <h4>44°C</h4>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <div class="row">
                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Mon</h6>
                                <div class="text-center">
                                    <canvas id="partly-cloudy-day" width="32" height="32"></canvas>
                                    <span>48°C</span>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Mon</h6>
                                <div class="text-center">
                                    <canvas id="rain" width="32" height="32"></canvas>
                                    <span>39°C</span>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Tue</h6>
                                <div class="text-center">
                                    <canvas id="sleet" width="32" height="32"></canvas>
                                    <span>32°C</span>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Wed</h6>
                                <div class="text-center">
                                    <canvas id="snow" width="32" height="32"></canvas>
                                    <span>28°C</span>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Thu</h6>
                                <div class="text-center">
                                    <canvas id="wind" width="32" height="32"></canvas>
                                    <span>40°C</span>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <h6 class="text-center small-thin uppercase">Fri</h6>
                                <div class="text-center">
                                    <canvas id="fog" width="32" height="32"></canvas>
                                    <span>42°C</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4>Browser Summary</h4>
                        <div id="donut-example"></div>
                    </div>
                </div>
            </div>
        </div>
        -->
        <!--ToDo end-->

        <div class="row" style="background-color:white;border:solid 1px gray">
            <div class="col-xs-3">
                <div id="g1">
                </div>
            </div>
            <div class="col-xs-6">
                <div id="g2">
                </div>
            </div>
            <div class="col-xs-3">
                <div id="g3">
                </div>
            </div>
        </div>

        <!-- lembretes -->
        <div class="row">
            <div class="col-xs-12">
                <asp:DataList ID="dlLembrete" DataKeyField="ID" runat="server" RepeatDirection="Vertical" Width="100%">
                    <ItemTemplate>
                        <table width="100%" class="table table-bordered table-striped">
                            <tr>
                                <th align="left" width="53%">Cliente</th>
                                <th align="left">Dono</th>
                                <th align="left">Telefone</th>
                                <th align="left" width="15%">Data</th>
                            </tr>
                            <tr>
                                <td><%#DataBinder.Eval(Container.DataItem, "Proposta.Cliente.Nome")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "Proposta.Usuario.Nome")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "Proposta.Cliente.Telefone")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "Data", "{0:dd/MM/yyyy}")%></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <%# DataBinder.Eval(Container.DataItem, "Texto").ToString().Replace("\n", "<br/>") %>
                                    &nbsp;
                                    <asp:LinkButton ID="lnkAbrir" CssClass="glyphicon glyphicon-search"  CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Proposta.ID")%>' runat="server" OnClick="lnkAbrir_Click" />
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </div>
    </section>
    <!--main content end-->


    <asp:GridView ID="grid" runat="server" SkinID="gridPadrao" Width="100%" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="Cliente">
                <ItemTemplate>
                    <asp:Literal ID="litUsuario" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Proposta.Cliente.Nome")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Telefone">
                <ItemTemplate>
                    <asp:Literal ID="litUsuario" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Proposta.Cliente.Telefone")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Texto" HeaderText="Info" />

            <asp:ButtonField ButtonType="Link" Text="Excluir" CommandName="Excluir" Visible="false">
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField ButtonType="Link" Text="Abrir" CommandName="Abrir">
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>

   <script type='text/javascript'>

       $('#g2').highcharts({
           chart: {
               type: 'column'
           },
           title: {
               text: 'Comparação Enviadas x Fechadas'
           },
           subtitle: {
               text: '(Valores em reais)'
           },
           xAxis: {
               categories: [
                   <% Response.Write(this.G2_Meses); %>
               ]
           },
           yAxis: {
               min: 0,
               title: {
                   text: '<% Response.Write(this.G2_Periodo); %>'
               }
           },
           tooltip: {
               headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
               pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                   '<td style="padding:0"><b>{point.y:.2f} reais</b></td></tr>',
               footerFormat: '</table>',
               shared: true,
               useHTML: true
           },
           plotOptions: {
               column: {
                   pointPadding: 0.2,
                   borderWidth: 0
               }
           },
           series: [ <% Response.Write(this.G2); %> ]
       });

           $('#g1').highcharts({
               chart: {
                   type: 'column'
               },
               title: {
                   text: 'Distribuição por status'
               },
               subtitle: {
                   text: '(valores em reais)'
               },
               xAxis: {
                   categories: [
                       'Status'
                   ]
               },
               yAxis: {
                   min: 0,
                   title: {
                       text: '<% Response.Write(this.G1_Periodo); %>'
                   }
               },
               tooltip: {
                   headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                   pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                       '<td style="padding:0"><b>{point.y:.2f} reais</b></td></tr>',
                   footerFormat: '</table>',
                   shared: true,
                   useHTML: true
               },
               plotOptions: {
                   column: {
                       pointPadding: 0.2,
                       borderWidth: 0
                   }
               },
               series: [
                   <% Response.Write(this.G1); %>
               ],
               dataLabels: {
                   enabled: true,
                   rotation: -90,
                   color: '#FFFFFF',
                   align: 'right',
                   x: 4,
                   y: 10,
                   style: {
                       fontSize: '13px',
                       fontFamily: 'Verdana, sans-serif',
                       textShadow: '0 0 3px black'
                   }
               }
           });

           $('#g3').highcharts({
               chart: {
                   plotBackgroundColor: null,
                   plotBorderWidth: null,
                   plotShadow: false
               },
               plotOptions: {
                   pie: {
                       allowPointSelect: true,
                       cursor: 'pointer',
                       dataLabels: {
                           enabled: false,
                           format: '<span style="color:{point.color}">{point.name} ({point.y})</span>: <b>{point.percentage:.1f}%</b><br/>'
                       },
                       showInLegend: true
                   }
               },
               tooltip: {
                   headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                   pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.percentage:.1f}%</b><br/>'
               },
               title: {
                   text: 'Distribuição por origem'
               },
               series: [{
                   type: 'pie',
                   name: 'Origem',
                   data: [
                       <% Response.Write(this.G3); %>
                   ]
               }]
           });
    </script>

    </asp:Panel>

    <asp:Panel ID="pnlOrientador" Visible="false" runat="server">

        <asp:UpdatePanel ID="upOrientador" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <div class="form-group">
                    <label class="col-xs-4 control-label" style="font-size:16px">Pesquisa por prestadores e procedimentos</label>
                </div>

                <div class="form-group">
                    <label class="col-md-2 control-label aria-label">Segmento</label>
                    <div class="col-md-6">
                        <asp:DropDownList ID="cboSegmento" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboSegmento_SelectedIndexChanged" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-2 control-label aria-label">Busca avançada</label>
                    <div class="col-md-4"><asp:TextBox ID="txtBuscaAvancada" SkinID="txtPadrao" runat="server" /></div>
                    <div class="col-md-2"><asp:Button ID="cmdBuscaAvancada" Text="localizar" runat="server" SkinID="botaoPadraoWarning_Small" OnClick="cmdBusca2_Click" /></div>
                </div>
                <asp:Panel ID="pnlBuscaAvancada" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="col-xs-11">
                            <br />
                            <asp:GridView ID="gridBuscaAvancada" Width="90%" SkinID="gridPadrao"
                                runat="server" AllowPaging="False" AutoGenerateColumns="False"  DataKeyNames="ID"
                                OnRowCommand="gridBuscaAvancada_RowCommand" PageSize="25" OnPageIndexChanging="gridBuscaAvancada_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="Especialidade" HeaderText="Especialidade">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:ButtonField ButtonType="Link" CommandName="sel" DataTextField="Codigo" HeaderText="Código">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:ButtonField>

                                    <asp:ButtonField ButtonType="Link" CommandName="sel" DataTextField="Nome" HeaderText="Procedimento">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>

                <div class="form-group">
                    <label class="col-xs-2 control-label">Especialidade</label>
                    <div class="col-xs-6">
                        <asp:DropDownList ID="cboEspecialidade" Width="100%" SkinID="comboPadrao1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboEspecialidade_SelectedIndexChanged" />
                    </div>
                </div>
                <asp:Panel ID="pnlDivProcedimentos" runat="server" Visible="false">
                    <div class="form-group" runat="server" enableviewstate="false" visible="false">
                        <label class="col-md-2 control-label" style="font-weight:normal">localizar procedimento</label>
                        <div class="col-md-4"><asp:TextBox ID="txtBusca" SkinID="txtPadrao" runat="server" /></div>
                        <div class="col-md-2"><asp:Button ID="cmdBusca" Text="localizar" runat="server" SkinID="botaoPadraoWarning_Small" OnClick="cmdBusca_Click" /></div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Procedimento</label>
                        <div class="col-xs-6">
                            <asp:DropDownList ID="cboProcedimento" Width="100%" SkinID="comboPadrao1" runat="server" />
                        </div>
                    </div>
                </asp:Panel>
                <div class="form-group">
                    <label class="col-xs-2 control-label">Estado</label>
                    <div class="col-xs-2">
                        <asp:DropDownList ID="cboUf" Width="100%" SkinID="comboPadrao1" runat="server" OnSelectedIndexChanged="cboUf_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <label class="col-xs-1 control-label">Cidade</label>
                    <div class="col-xs-3">
                        <asp:DropDownList ID="cboCidade" Width="100%" SkinID="comboPadrao1" runat="server" OnSelectedIndexChanged="cboCidade_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">Bairro</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="cboBairro" Width="100%" SkinID="comboPadrao1" runat="server" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="cmdPesquisar" class="btn btn-info" runat="server" Text="Pesquisar" Width="100%" OnClick="cmdPesquisar_Click" EnableViewState="false"/>
                    </div>
                </div>

                <asp:Panel ID="pnlResult" Visible="false" runat="server">
                    <div class="form-group">
                        <div class="col-xs-12">
                            <br />
                            <asp:GridView ID="grid2" Width="100%" SkinID="gridPadrao"
                                runat="server" AllowPaging="False" AutoGenerateColumns="False"  
                                onrowcommand="grid2_RowCommand" onrowdatabound="grid2_RowDataBound" PageSize="25" OnPageIndexChanging="grid2_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="Fantasia" HeaderText="Fantasia">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
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

                                    <asp:BoundField DataField="Valor" HeaderText="Valor">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlNoResult" Visible="false" runat="server">
                    <div class="form-group">
                        <label class="col-xs-4 control-label" style="font-size:13px">Nenhum prestador pôde ser localizado</label>
                    </div>
                </asp:Panel>
                </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>



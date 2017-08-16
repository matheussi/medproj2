<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="atendimento.aspx.cs" Inherits="MedProj.www.atendimento.atendimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <style type="text/css">
      <!--
      .oculto {display:none}
      -->
    </style>
    <script language="javascript" type="text/javascript">
      <!--
        function Shift()
        {
            if (document.getElementById(26).value == "a")
            {
                ind = 49;
                inicio = 1;
                fim = 49;
            }
            else
            {
                ind = -49;
                inicio = 50;
                fim = 98;
            }

            for (chr = inicio; chr <= fim; chr++)
            {
                aux = document.getElementById(chr).value;
                document.getElementById(chr).value = document.getElementById(chr + ind).value;
                document.getElementById(chr + ind).value = aux;
            }
        }
        function Digitar(tecla)
        {
            document.getElementById('ctl00_content_txtSenhaContrato').value += tecla
        }
        //-->
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="panel panel-default" style="position:relative; top:-70px">
        <div class="panel-heading text-left" style="position:relative;">
            <div class="col-xs-12">
                <div class="row">
                    <b><asp:Literal ID="litPrestador" runat="server" EnableViewState="false" /></b>
                    <asp:Literal ID="litTopoResumo" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <asp:UpdatePanel ID="up" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlHide" Visible="false" EnableViewState="false" runat="server">
                        <div class="row">
                            <div class="text-left col-xs-12">
                                <asp:Literal ID="litNomeUnidade" runat="server" EnableViewState="false" /><br />
                                <asp:Literal ID="litEnderecoUnidade" runat="server" EnableViewState="false" /><br />
                                <asp:Literal ID="litContatoUnidade" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <hr />
                    </asp:Panel>
                    <div class="form-group">
                        <div class="col-md-10"><strong>Caro usuário, preencher um dos campos abaixo para iniciar o atendimento</strong></div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4">Informe o número do cartão Clube Azul</div><br />
                        <div class="col-xs-6">
                            <asp:TextBox ID="txtNumeroCartao" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="18" onkeypress="filtro_SoNumeros(event);" />
                        </div>
                        <div class="col-xs-1">
                            <asp:ImageButton ID="cmdValidarNumero" ImageUrl="~/Images/tick.png" ToolTip="validar cartão" runat="server" OnClick="cmdValidarNumero_Click" />
                            <asp:ImageButton ID="cmdCancelarAtendimento" ImageUrl="~/Images/delete.png" ToolTip="cancelar atendimento" runat="server" OnClick="cmdCancelarAtendimento_Click" OnClientClick="return confirm('Deseja cancelar o atendimento?'); " Visible="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-10"><strong>OU</strong></div><br />
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4">Informe o número do CPF do titular do cartão</div><br />
                        <div class="col-xs-6">
                            <asp:TextBox ID="txtCPFTitularCartao" Width="100%" SkinID="txtPadrao" runat="server" MaxLength="11" onkeypress="filtro_SoNumeros(event);" />
                        </div>
                        <div class="col-xs-1">
                            <asp:ImageButton ID="cmdValidarCPFTitular" ImageUrl="~/Images/tick.png" ToolTip="validar cartão" runat="server" OnClick="cmdValidarCPFTitular_Click" />
                            <asp:ImageButton ID="cmdCancelarAtendimento2" ImageUrl="~/Images/delete.png" ToolTip="cancelar atendimento" runat="server" OnClick="cmdCancelarAtendimento_Click" OnClientClick="return confirm('Deseja cancelar o atendimento?'); " Visible="false" />
                        </div>
                    </div>

                    <asp:Panel ID="pnlMaisDeUmContrato" runat="server" Visible="false" EnableViewState="false">
                        <div class="form-group">
                            <div class="col-xs-6">
                                <div class="alert alert-warning" style="font-size:12px"><strong>O CPF informado possui mais de um cartão.</strong><br/>Por favor, queira selecionar um, clicando sobre o número:<br /><br />
                                    <asp:GridView ID="gridMaisDeUmContrato" BackColor="White" Width="100%" SkinID="gridPadrao2"
                                        runat="server" AllowPaging="False" AutoGenerateColumns="False"  
                                        DataKeyNames="ID" onrowcommand="gridMaisDeUmContrato_RowCommand">
                                        <Columns>
                                            <asp:ButtonField ButtonType="Link" DataTextField="Numero" HeaderText="Número do cartão" CommandName="Selecionar" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <div class="form-group">
                        <div class="col-xs-8">
                            <asp:Literal ID="litResultadoValidacao" runat="server" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlespec" runat="server" Visible="false" EnableViewState="false">
                        <div class="form-group">
                            <div class="col-xs-2">Especialidade</div>
                            <div class="col-xs-6">
                                <asp:DropDownList ID="cboEspecialidade" runat="server" SkinID="comboPadrao1" Width="100%"/>
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="form-group">
                        <div class="col-xs-2">Procedimento</div>
                        <div class="col-xs-6">
                            <asp:TextBox ID="txtProcedimentos" runat="server" SkinID="txtPadrao" Width="100%" />
                            <asp:DropDownList ID="cboProcedimentos" runat="server" SkinID="comboPadrao1" Width="1%" Visible="false" EnableViewState="false"/>
                        </div>
                        <div class="col-xs-1" style="margin-left:-20px;margin-top:-1px">
                            <input type="hidden" name="txtProcedimentoId" id="txtProcedimentoId" runat="server" />
                            <asp:Button id="imgAdd" Text="incluir" SkinID="botaoPadraoINFO_Small" runat="server" OnClick="imgAdd_Click" />
                            <%--<asp:ImageButton ID="imgAdd" ImageUrl="~/Images/seta_baixa.png" ToolTip="adicionar" runat="server" OnClick="imgAdd_Click" />--%>
                        </div>
                        <div class="col-xs-3" style="margin-left:-20px;margin-top:-1px">
                            <asp:Button ID="lnkTodosProcedimentos" SkinID="botaoPadraoWarning_Small" Text="ver procedimentos" OnClick="lnkTodosProcedimentos_Click" runat="server" ></asp:Button>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-2"></div>
                        <div class="col-xs-6">
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-8">
                            <asp:GridView ID="grid" Width="100%" SkinID="gridPadrao"
                                runat="server" AllowPaging="False" AutoGenerateColumns="False"  
                                DataKeyNames="Id,EspecId" onrowcommand="grid_RowCommand" 
                                onrowdatabound="grid_RowDataBound" PageSize="25"
                                OnPageIndexChanging="grid_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="EspecNome" HeaderText="Especialidade">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Nome" HeaderText="Procedimento">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Codigo" HeaderText="Código">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField ButtonType="Link"  CommandName="excluir" >
                                        <ItemStyle Width="1%" />
                                        <ControlStyle Width="1%" />
                                        <ControlStyle CssClass="glyphicon glyphicon-remove" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <asp:Panel ID="pnlGravar" runat="server" Visible="false">
                        <div class="form-group">
                            <label class="col-xs-1" style="margin-top:2px"></label>
                            <div class="col-xs-3"></div>
                            <div class="col-xs-4 text-right">
                                <asp:Button ID="cmdGravar" Text="Confirmar lançamento" SkinID="botaoPadraoSUCCESS" runat="server" OnClick="cmdGravar_Click" Visible="true" />
                            </div>
                        </div>
                    </asp:Panel>


                    <!--Modal Confirmação-->
                    <div class="modal fade" id="modalConfirm" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header text-left">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                    <h2 class="modal-title">Confirmação de atendimento</h2>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <div class="col-xs-12">
                                            <div class="row">
                                                <asp:Literal ID="litAtendimentoEfetivado" EnableViewState="true" runat="server" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:LinkButton CssClass="alert-link" ID="lnkEmail" runat="server" EnableViewState="false" ToolTip="enviar por e-mail" OnClientClick="if(confirm('Deseja enviar o comprovante por e-mail?')) { document.getElementById('cmdFecharModal').click(); } else { return false; }" OnClick="lnkEmail_Click"><span class="glyphicon glyphicon-envelope"></span></asp:LinkButton>
                                                &nbsp;&nbsp;
                                                <asp:LinkButton CssClass="alert-link" ID="lnkPrint" runat="server" EnableViewState="true" ToolTip="versão para impressão"><span class="glyphicon glyphicon-print"></span></asp:LinkButton>
                                                <asp:Literal ID="litAtendimentoEfetivadoFechaDiv" EnableViewState="true" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <asp:Panel ID="pnlCampoEmail" runat="server" Visible="false">
                                        <div class="form-group">
                                            <div class="col-xs-1"></div>
                                            <label class="col-xs-1" style="margin-top:2px">E-mail</label>
                                            <div class="col-xs-6">
                                                <asp:TextBox ID="txtEmail" SkinID="txtPadrao" Width="60%" MaxLength="85" runat="server" />
                                            <%--</div>
                                            <div class="col-xs-2">--%>
                                                <asp:Button ID="cmdEmail" SkinID="botaoPadraoINFO_Small" runat="server" EnableViewState="false" Text="E-mail" OnClientClick="if(confirm('Deseja enviar o comprovante por e-mail?')) { document.getElementById('cmdFecharModal').click(); } else { return false; }" OnClick="lnkEmail_Click" />
                                            <%--</div>
                                            <div class="col-xs-2">--%>
                                                <asp:Button ID="cmdPrint" SkinID="botaoPadraoINFO_Small" runat="server" EnableViewState="true" Text="Impressão" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSenha" runat="server">
                                        <div class="form-group">
                                            <div class="col-xs-12">
                                                <asp:Literal ID="litComprovante" runat="server" EnableViewState="false" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-xs-2" style="margin-top:2px">Forma pagto</label>
                                            <div class="col-xs-6">
                                                <asp:RadioButton ID="optCartao" Text=" Débito no CARTÃO PRÉ PAGO Clube Azul Vida Saudável" Checked="true" GroupName="pagto" runat="server" />
                                                
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-offset-2 col-md-6">
                                                <asp:RadioButton ID="optDinheiro" Text=" PAGAMENTO IMEDIATO (dinheiro, cheque, cartão de crédito, etc)" GroupName="pagto" runat="server" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-xs-2" style="margin-top:2px">Senha</label>
                                            <div class="col-xs-2">
                                                <asp:TextBox ID="txtSenhaContrato" MaxLength="20" TextMode="Password" runat="server" ReadOnly="false" SkinID="txtPadrao" Width="100%" />
                                            </div>
                                            <div class="col-xs-1">
                                                <asp:Button ID="cmdValidar" Text="Validar" SkinID="botaoPadraoINFO_Small" runat="server" OnClick="cmdValidar_Click" OnClientClick="if(!confirm('ATENÇÃO: Deseja prosseguir com a efetivação do atendimento?\nEssa ação irá efetivar o atendimento no cartão informado.')) { return false; } else { document.getElementById('cmdFecharModal').click(); }"/>
                                                <asp:Button ID="cmdValidar2" Visible="false" Text="Validar" SkinID="botaoPadraoINFO_Small" runat="server" OnClick="cmdValidar_Click" OnClientClick="if(!confirm('ATENÇÃO: Deseja prosseguir com a efetivação do atendimento?\nA forma de pagamento selecionada é: dinheiro à vista.')) { return false; } else { document.getElementById('cmdFecharModal').click(); }"/>
                                            </div>
                                            <div class="col-xs-1">
                                                <button type="button" class="btn btn-warning btn-sm" onclick="document.getElementById('ctl00_content_txtSenhaContrato').value = '';return false;">Limpar</button>
                                            </div>
                                            <div class="col-xs-7">


<%--    <form name="teclas" action="#">
      <div style="background-color:whitesmoke;border:solid 1px gray;width:85%;padding-top:15px;padding-bottom:12px" class="text-center">
          <input type="button" value="'" onclick="Digitar(this.value)" id="1" />&nbsp;
          <input type="button" value="1" onclick="Digitar(this.value)" id="2" />&nbsp;
          <input type="button" value="2" onclick="Digitar(this.value)" id="3" />&nbsp;
          <input type="button" value="3" onclick="Digitar(this.value)" id="4" />&nbsp;
          <input type="button" value="4" onclick="Digitar(this.value)" id="5" />&nbsp;
          <input type="button" value="5" onclick="Digitar(this.value)" id="6" />&nbsp;
          <input type="button" value="6" onclick="Digitar(this.value)" id="7" />&nbsp;
          <input type="button" value="7" onclick="Digitar(this.value)" id="8" />&nbsp;
          <input type="button" value="8" onclick="Digitar(this.value)" id="9" />&nbsp;
          <input type="button" value="9" onclick="Digitar(this.value)" id="10" />&nbsp;
          <input type="button" value="0" onclick="Digitar(this.value)" id="11" />&nbsp;
          <input type="button" value="-" onclick="Digitar(this.value)" id="12" />&nbsp;
          <input type="button" value="=" onclick="Digitar(this.value)" id="13" />&nbsp;<br /><br />
          &nbsp;&nbsp;&nbsp;&nbsp;<input type="button" value="q" onclick="Digitar(this.value)" id="14" />&nbsp;
          <input type="button" value="w" onclick="Digitar(this.value)" id="15" />&nbsp;
          <input type="button" value="e" onclick="Digitar(this.value)" id="16" />&nbsp;
          <input type="button" value="r" onclick="Digitar(this.value)" id="17" />&nbsp;
          <input type="button" value="t" onclick="Digitar(this.value)" id="18" />&nbsp;
          <input type="button" value="y" onclick="Digitar(this.value)" id="19" />&nbsp;
          <input type="button" value="u" onclick="Digitar(this.value)" id="20" />&nbsp;
          <input type="button" value="i" onclick="Digitar(this.value)" id="21" />&nbsp;
          <input type="button" value="o" onclick="Digitar(this.value)" id="22" />&nbsp;
          <input type="button" value="p" onclick="Digitar(this.value)" id="23" />&nbsp;
          <input type="button" value="´" onclick="Digitar(this.value)" id="24" />&nbsp;
          <input type="button" value="[" onclick="Digitar(this.value)" id="25" /><br /><br />
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="button" value="a" onclick="Digitar(this.value)" id="26" />&nbsp;
          <input type="button" value="s" onclick="Digitar(this.value)" id="27" />&nbsp;
          <input type="button" value="d" onclick="Digitar(this.value)" id="28" />&nbsp;
          <input type="button" value="f" onclick="Digitar(this.value)" id="29" />&nbsp;
          <input type="button" value="g" onclick="Digitar(this.value)" id="30" />&nbsp;
          <input type="button" value="h" onclick="Digitar(this.value)" id="31" />&nbsp;
          <input type="button" value="j" onclick="Digitar(this.value)" id="32" />&nbsp;
          <input type="button" value="k" onclick="Digitar(this.value)" id="33" />&nbsp;
          <input type="button" value="l" onclick="Digitar(this.value)" id="34" />&nbsp;
          <input type="button" value="ç" onclick="Digitar(this.value)" id="35" />&nbsp;
          <input type="button" value="~" onclick="Digitar(this.value)" id="36" />&nbsp;
          <input type="button" value="]" onclick="Digitar(this.value)" id="37" /><br /><br />
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="button" value="\" onclick="Digitar(this.value)" id="38" />
          <input type="button" value="z" onclick="Digitar(this.value)" id="39" />&nbsp;
          <input type="button" value="x" onclick="Digitar(this.value)" id="40" />&nbsp;
          <input type="button" value="c" onclick="Digitar(this.value)" id="41" />&nbsp;
          <input type="button" value="v" onclick="Digitar(this.value)" id="42" />&nbsp;
          <input type="button" value="b" onclick="Digitar(this.value)" id="43" />&nbsp;
          <input type="button" value="n" onclick="Digitar(this.value)" id="44" />&nbsp;
          <input type="button" value="m" onclick="Digitar(this.value)" id="45" />&nbsp;
          <input type="button" value="," onclick="Digitar(this.value)" id="46" />&nbsp;
          <input type="button" value="." onclick="Digitar(this.value)" id="47" />&nbsp;
          <input type="button" value=";" onclick="Digitar(this.value)" id="48" />&nbsp;
          <input type="button" value="/" onclick="Digitar(this.value)" id="49" />&nbsp;
          <input type="button" value="shift" onclick="Shift()" /><br /><br />
          <input type="button" value=" " onclick="Digitar(this.value)" style="width: 230px; height: 22px; margin-left: 100px" />
      
          <!-- - - - - Início dos campos ocultos - - - - - -->
          <input type="button" value='"' onclick="Digitar(this.value)" class="oculto" id="50" />
          <input type="button" value="!" onclick="Digitar(this.value)" class="oculto" id="51" />
          <input type="button" value="@" onclick="Digitar(this.value)" class="oculto" id="52" />
          <input type="button" value="#" onclick="Digitar(this.value)" class="oculto" id="53" />
          <input type="button" value="$" onclick="Digitar(this.value)" class="oculto" id="54" />
          <input type="button" value="%" onclick="Digitar(this.value)" class="oculto" id="55" />
          <input type="button" value="¨" onclick="Digitar(this.value)" class="oculto" id="56" />
          <input type="button" value="&amp;" onclick="Digitar(this.value)" class="oculto" id="57" />
          <input type="button" value="*" onclick="Digitar(this.value)" class="oculto" id="58" />
          <input type="button" value="(" onclick="Digitar(this.value)" class="oculto" id="59" />
          <input type="button" value=")" onclick="Digitar(this.value)" class="oculto" id="60" />
          <input type="button" value="_" onclick="Digitar(this.value)" class="oculto" id="61" />
          <input type="button" value="+" onclick="Digitar(this.value)" class="oculto" id="62" />
          <input type="button" value="Q" onclick="Digitar(this.value)" class="oculto" id="63" />
          <input type="button" value="W" onclick="Digitar(this.value)" class="oculto" id="64" />
          <input type="button" value="E" onclick="Digitar(this.value)" class="oculto" id="65" />
          <input type="button" value="R" onclick="Digitar(this.value)" class="oculto" id="66" />
          <input type="button" value="T" onclick="Digitar(this.value)" class="oculto" id="67" />
          <input type="button" value="Y" onclick="Digitar(this.value)" class="oculto" id="68" />
          <input type="button" value="U" onclick="Digitar(this.value)" class="oculto" id="69" />
          <input type="button" value="I" onclick="Digitar(this.value)" class="oculto" id="70" />
          <input type="button" value="O" onclick="Digitar(this.value)" class="oculto" id="71" />
          <input type="button" value="P" onclick="Digitar(this.value)" class="oculto" id="72" />
          <input type="button" value="`" onclick="Digitar(this.value)" class="oculto" id="73" />
          <input type="button" value="{" onclick="Digitar(this.value)" class="oculto" id="74" />
          <input type="button" value="A" onclick="Digitar(this.value)" class="oculto" id="75" />
          <input type="button" value="S" onclick="Digitar(this.value)" class="oculto" id="76" />
          <input type="button" value="D" onclick="Digitar(this.value)" class="oculto" id="77" />
          <input type="button" value="F" onclick="Digitar(this.value)" class="oculto" id="78" />
          <input type="button" value="G" onclick="Digitar(this.value)" class="oculto" id="79" />
          <input type="button" value="H" onclick="Digitar(this.value)" class="oculto" id="80" />
          <input type="button" value="J" onclick="Digitar(this.value)" class="oculto" id="81" />
          <input type="button" value="K" onclick="Digitar(this.value)" class="oculto" id="82" />
          <input type="button" value="L" onclick="Digitar(this.value)" class="oculto" id="83" />
          <input type="button" value="Ç" onclick="Digitar(this.value)" class="oculto" id="84" />
          <input type="button" value="^" onclick="Digitar(this.value)" class="oculto" id="85" />
          <input type="button" value="}" onclick="Digitar(this.value)" class="oculto" id="86" />
          <input type="button" value="|" onclick="Digitar(this.value)" class="oculto" id="87" />
          <input type="button" value="Z" onclick="Digitar(this.value)" class="oculto" id="88" />
          <input type="button" value="X" onclick="Digitar(this.value)" class="oculto" id="89" />
          <input type="button" value="C" onclick="Digitar(this.value)" class="oculto" id="90" />
          <input type="button" value="V" onclick="Digitar(this.value)" class="oculto" id="91" />
          <input type="button" value="B" onclick="Digitar(this.value)" class="oculto" id="92" />
          <input type="button" value="N" onclick="Digitar(this.value)" class="oculto" id="93" />
          <input type="button" value="M" onclick="Digitar(this.value)" class="oculto" id="94" />
          <input type="button" value="<" onclick="Digitar(this.value)" class="oculto" id="95" />
          <input type="button" value=">" onclick="Digitar(this.value)" class="oculto" id="96" />
          <input type="button" value=":" onclick="Digitar(this.value)" class="oculto" id="97" />
          <input type="button" value="?" onclick="Digitar(this.value)" class="oculto" id="98" />
      <!-- Fim dos campos ocultos -->
       </div>
    </form>--%>


                                            </div>
                                        </div>
                                    </asp:Panel>



                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-info" data-dismiss="modal" id="cmdFecharModal">Fechar</button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Confirmacao de procedimento duplicado -->
                    <asp:Panel ID="pnlConfirmProc" runat="server" Visible="false" >
                        <div style="background:rgba(0,0,0,0.5); width:100%; height:100%; position:fixed; z-index:99999; top:0; left:0;">
                            <div style="position:absolute; background:white; border-radius:10px; width:400px; margin-left:-200px; top:25%; left:50%; padding:30px;">
                                
                                <div class="form-group">
                                    <div class="col-xs-12">
                                        <h4>Procedimento já adicionado.<br />Deseja prosseguir?</h4>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-xs-6">
                                        <asp:Button id="cmdNao" class="btn btn-danger" runat="server" Text="Não adicionar" OnClick="cmdNao_Click" />
                                    </div>
                                    <div class="col-xs-6">
                                        <asp:Button id="cmdSim" class="btn btn-info" runat="server" Text="Sim, adicionar" OnClick="cmdSim_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlTodosProcedimentos" Visible="false" runat="server">
                        <div style="background:rgba(0,0,0,0.5); width:100%; height:100%; position:fixed; z-index:99999; top:0; left:0;">
                            <div style="position:absolute; overflow:auto; background:white; border-radius:10px; width:650px; margin-left:-200px; top:2%; left:50%; padding:30px;">
                                <div class="form-group">
                                    <!--<div class="col-xs-12">
                                        <h4>Procedimento cadastrados</h4>
                                    </div>-->
                                    <div class="form-group">
                                        <div class="col-xs-12" style="height: 550px; overflow-y: scroll;">
                                            <asp:GridView ID="gridTodosProcedimentos" Font-Size="11px" AllowPaging="true" PageSize="100" OnRowCommand="gridTodosProcedimentos_RowCommand" OnRowDataBound="gridTodosProcedimentos_RowDataBound" OnPageIndexChanging="gridTodosProcedimentos_PageIndexChanging" Width="100%" SkinID="gridPadrao" runat="server" DataKeyNames="ID">
                                                <Columns>
                                                    <asp:TemplateField Visible="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litId" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.ID")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:ButtonField ButtonType="Link" Text="" CommandName="Incluir">
                                                        <ItemStyle Width="1%" />
                                                        <ControlStyle Width="1%" />
                                                        <ControlStyle Font-Size="16px" CssClass="glyphicon glyphicon-ok-circle" />
                                                    </asp:ButtonField>

                                                    <asp:TemplateField HeaderText="Código" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litCodigo" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Codigo")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Procedimento" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litProc" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Nome")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Valor" DataField="ValorCalculado" DataFormatString="{0:N2}">
                                                        <ItemStyle Wrap="false" Width="1%" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Especialidade" HeaderStyle-Wrap="false" >
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litProc" Text='<%#DataBinder.Eval(Container.DataItem, "Procedimento.Especialidade")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                    <div class="col-xs-12">
                                        <asp:Button id="cmdFecharShadowProcedimentos" SkinID="botaoPadraoINFO_Small" runat="server" Text="fechar" OnClick="cmdFecharShadowProcedimentos_Click" />
                                    </div>
                                </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>


                   <%--
                   <asp:Panel runat="server" id="divConfirm">
                        <div class="OverlayBackground"></div>
                        <div style="border-radius:15px; padding:5px;position:fixed;left:50%;top:10%;margin-left:-315px;width:630px; height:430px;z-index:9999; ">
                            <div onclick="$('#<%= divConfirm.ClientID %>').css('display', 'none');" class="ButtonDefault" style="right:0px; top:-50px; position:absolute; padding:10px; font-size:12px;">Fechar [ x ]</div>
                            <div class="form-group">
                                <div class="form-group">
                                    <div class="col-xs-1"></div>
                                    <label class="col-xs-1" style="margin-top:2px">Senha</label>
                                    <div class="col-xs-2">
                                        <asp:TextBox ID="txtSenhaContrato_" MaxLength="20" TextMode="Password" runat="server" SkinID="txtPadrao" Width="100%" />
                                    </div>
                                    <div class="col-xs-1">
                                        <asp:LinkButton ID="lnkEmail_" runat="server" EnableViewState="false" ToolTip="enviar por e-mail" OnClientClick="if(!confirm('Deseja enviar o comprovante por e-mail?')) { return false; }"><span class="glyphicon glyphicon-envelope"></span></asp:LinkButton>
                                    </div>
                                    <div class="col-xs-1">
                                        <asp:LinkButton ID="lnkPrint_" runat="server" EnableViewState="false" ToolTip="versão para impressão" OnClientClick="if(!confirm('Deseja abrir o comprovante para impressão?')) { return false; }"><span class="glyphicon glyphicon-print"></span></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                   </asp:Panel>--%>


                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">

        $(document).ready(function ()
        {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(configAutocomplete);

            configAutocomplete(null, null);
        });

        function configAutocomplete(sender, args)
        {
            $("#<%= txtProcedimentos.ClientID %>").autocomplete
            ({
                source: function (request, response) {

                    $.ajax({
                        url: "../proxy/proxyCarregaProcedimentosCredenciado.aspx",
                        dataType: "json",
                        data: {
                            featureClass: "P",
                            style: "full",
                            maxRows: 12,
                            name_startsWith: request.term
                        },
                        success: function (data) {
                            response($.map(data.Procs, function (item) {
                                return {

                                    label: item.Nome + ' (' + item.Valor + ' - ' + item.Espec + ')',
                                    value: item.Nome,
                                    data: item
                                }
                            }));
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    showItem(ui.item ? ui.item : undefined);
                },
                search: function (event, ui) {
                    showItem(ui.item ? ui.item : undefined);
                }
            });
        }

        function showItem(item)
        {
            if (item != null && item != undefined)
            {
                document.getElementById('<%= txtProcedimentoId.ClientID %>').value = item.data.ID;
            }
            else
            {
                document.getElementById('<%= txtProcedimentoId.ClientID %>').value = '';
            }
        }

        function showModal()
        {
            $('#modalConfirm').modal('show');
        }
    </script>
</asp:Content>

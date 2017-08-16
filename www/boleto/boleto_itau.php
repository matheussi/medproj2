<?php

   $key = "passwordDR0wSS@P6660juht";
   $iv  = "password";

   $cipher = mcrypt_module_open(MCRYPT_3DES, '', 'cbc', '');

   $cript = postget('param'); //parametro ainda encriptado


   $link = SimpleTripleDesDecrypt($cript);

   $parsed_url = parse_url($link);

   $url_query = $parsed_url['query'];
   parse_str($url_query, $params);

   $email = $params['mailto'];

// DADOS DO BOLETO PARA O CLIENTE

$dadosboleto["nosso_numero"]        = $params['nossonum'];                                                                                                         //'12345678';  // Nosso numero - REGRA: Máximo de 8 caracteres!
$dadosboleto["numero_documento"]    = $params['numdoc2'];                                                                                                          //'0123';	// Num do pedido ou nosso numero
$dadosboleto["data_vencimento"]     = $params['v_dia'] . '/' . $params['v_mes'] . '/' . $params['v_ano'];                                                          //$data_venc; // Data de Vencimento do Boleto - REGRA: Formato DD/MM/AAAA
$dadosboleto["data_documento"]      = $params['d_dia'] . '/' . $params['d_mes'] . '/' . $params['d_ano'];                                                          //date("d/m/Y"); // Data de emissão do Boleto
$dadosboleto["data_processamento"]  = $params['p_dia'] . '/' . $params['p_mes'] . '/' . $params['p_ano'];                                                          //date("d/m/Y"); // Data de processamento do boleto (opcional)
$dadosboleto["valor_boleto"]        = str_replace('.','',$params['valor']);                                                                                    //number_format(,2,',','') $valor_boleto; 	// Valor do Boleto - REGRA: Com vírgula e sempre com duas casas depois da virgula




// DADOS DO SEU CLIENTE
$dadosboleto["sacado"]    = $params['nome']; //utf8_decode(postget('nome'));                                                                                                             //"Nome do seu Cliente";
$dadosboleto["endereco1"] = $params['end1']; //utf8_decode(postget('end1'));                                                                                                             //"Endereço do seu Cliente";
$dadosboleto["endereco2"] = $params['end2']; //utf8_decode(postget('end2'));                                                                                                             //"Cidade - Estado -  CEP: 00000-000";

// INFORMACOES PARA O CLIENTE
$dadosboleto["demonstrativo1"] = ''; //$params['demonst1']; //utf8_decode(postget('demonst1'));                                                                                                 //"Pagamento de Compra na Loja Nonononono";
$dadosboleto["demonstrativo2"] = '';                                                                                                                               //"Mensalidade referente a nonon nonooon nononon<br>Taxa bancária - R$ ".number_format($taxa_boleto, 2, ',', '');
$dadosboleto["demonstrativo3"] = '';                                                                                                                               //"BoletoPhp - http://www.boletophp.com.br";

// INFORMACOES PARA O CAIXA
$dadosboleto["instrucoes1"]    = $params['instr']; //utf8_decode(postget('instr1'));                                                                                                   //"- Sr. Caixa, cobrar multa de 2% após o vencimento";
$dadosboleto["instrucoes2"]    = '';                                                                                                                               //"- Receber até 10 dias após o vencimento";
$dadosboleto["instrucoes3"]    = '';                                                                                                                               //"- Em caso de dúvidas entre em contato conosco: xxxx@xxxx.com.br";
$dadosboleto["instrucoes4"]    = '';                                                                                                                               //"&nbsp; Emitido pelo sistema Projeto BoletoPhp - www.boletophp.com.br";

// DADOS OPCIONAIS DE ACORDO COM O BANCO OU CLIENTE
$dadosboleto["quantidade"]     = "";
$dadosboleto["valor_unitario"] = "";
$dadosboleto["aceite"]         = "";		
$dadosboleto["especie"]        = "R$";
$dadosboleto["especie_doc"]    = "";


// ---------------------- DADOS FIXOS DE CONFIGURAÇÃO DO SEU BOLETO --------------- //


// DADOS DA SUA CONTA - ITAÚ
$dadosboleto["agencia"]  = "9108";  // Num da agencia, sem digito
$dadosboleto["conta"]    = "09660";	// Num da conta, sem digito
$dadosboleto["conta_dv"] = "4"; 	  // Digito do Num da conta [agencia_codigo]

// DADOS PERSONALIZADOS - ITAÚ
$dadosboleto["carteira"] = "175";  // Código da Carteira: pode ser 175, 174, 104, 109, 178, ou 157

// SEUS DADOS
$dadosboleto["identificacao"] = ""; //BoletoPhp - Código Aberto de Sistema de Boletos
$dadosboleto["cpf_cnpj"]  = ""; //03.609.855/0001-02
$dadosboleto["endereco"]  = ""; //Alameda Santos, 415
$dadosboleto["cidade_uf"] = ""; //São Paulo / SP - CEP: 01419-000
$dadosboleto["cedente"]   = "JHM ADMINISTRADORA";

include("include/funcoes_itau.php"); 
include("include/layout_itau.php");



function SimpleTripleDes($buffer) 
{
  global $key, $iv, $cipher;
  //printvar($buffer, 'Encrypting:');

  // get the amount of bytes to pad
  $extra = 8 - (strlen($buffer) % 8);
  //printvar($extra, 'Padding with n zeros');

  // add the zero padding
  if($extra > 0) {
    for($i = 0; $i < $extra; $i++) {
      $buffer .= "\0";
    }
  }

  mcrypt_generic_init($cipher, $key, $iv);
  $result = bin2hex(mcrypt_generic($cipher, $buffer));
  mcrypt_generic_deinit($cipher);
  return $result;
}

function SimpleTripleDesDecrypt($buffer) 
{
  global $key, $iv, $cipher;

  mcrypt_generic_init($cipher, $key, $iv);

  $result = rtrim(mdecrypt_generic($cipher, hex2bin($buffer)), "\0");

  mcrypt_generic_deinit($cipher);
  return utf8_decode($result);
}

// HELPER FUNCTIONS

function printvar($var, $label="") 
{
    print "<pre style=\"border: 1px solid #999; background-color: #f7f7f7; color: #000; overflow: auto; width: auto; text-align: left; padding: 1em;\">" .
        (
            (
                strlen(
                    trim($label)
                )
            ) ? htmlentities($label)."\n===================\n" : ""
        ) .
        htmlentities(print_r($var, TRUE)) . "</pre>";
}

function postget($str) 
{
     
   if (isset($_POST[$str])) 
   {
      return $_POST[$str];
   }
   elseif (isset($_GET[$str]))
   {
      return $_GET[$str];
   }
}
?>
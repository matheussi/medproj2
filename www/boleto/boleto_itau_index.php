<?php

   //http://www.sanity-free.org/131/triple_des_between_php_and_csharp.html
   //http://www.sanity-free.org/forum/viewtopic.php?id=133
   
   //http://www.codediesel.com/php/building-get-query-strings-php/

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



   $key = "passwordDR0wSS@P6660juht";
   $iv  = "password";

   $cipher = mcrypt_module_open(MCRYPT_3DES, '', 'cbc', '');

   $link = SimpleTripleDesDecrypt(postget(param));

   //header('location:' . utf8_encode($link));

   echo $link;

   die();

   //// ENCRYPTING
   //printvar(
   //  SimpleTripleDes(postget(param)), //SimpleTripleDes('Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.'),
   //  'Encrypted:'
   //);
   
   // DECRYPTING
   printvar(
     SimpleTripleDesDecrypt(postget(param)), //SimpleTripleDesDecrypt('51196a80db5c51b8523220383de600fd116a947e00500d6b9101ed820d29f198c705000791c07ecc1e090213c688a4c7a421eae9c534b5eff91794ee079b15ecb862a22581c246e15333179302a7664d4be2e2384dc49dace30eba36546793be'),
     'Decrypted:'
   );
   
   function SimpleTripleDes($buffer) {
     global $key, $iv, $cipher;
     printvar($buffer, 'Encrypting:');
   
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
   
   function SimpleTripleDesDecrypt($buffer) {
     global $key, $iv, $cipher;
     //printvar($buffer, 'Decrypting:');
   
     mcrypt_generic_init($cipher, $key, $iv);
     $result = rtrim(mdecrypt_generic($cipher, hex2bin($buffer)), "\0");
     mcrypt_generic_deinit($cipher);
     return utf8_decode($result);
   }
   
   function hex2bin($data)
   {
     $len = strlen($data);
     return pack("H" . $len, $data);
   } 
   
   // HELPER FUNCTIONS
   
   function printvar($var, $label="") {
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

?>
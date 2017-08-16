namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.IO;
    using System.Data;
    using System.Text;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.BusinessLayer;

    using System.Data.SqlClient;

    [DBTable("layoutArquivoCustomizado")]
    public class LayoutArquivoCustomizado : EntityBase, IPersisteableEntity
    {
        String TirarAcentos(String texto)
        {
            string textor = "";

            for (int i = 0; i < texto.Length; i++)
            {
                if (texto[i].ToString() == "ã") textor += "a";
                else if (texto[i].ToString() == "á") textor += "a";
                else if (texto[i].ToString() == "à") textor += "a";
                else if (texto[i].ToString() == "â") textor += "a";
                else if (texto[i].ToString() == "ä") textor += "a";
                else if (texto[i].ToString() == "é") textor += "e";
                else if (texto[i].ToString() == "è") textor += "e";
                else if (texto[i].ToString() == "ê") textor += "e";
                else if (texto[i].ToString() == "ë") textor += "e";
                else if (texto[i].ToString() == "í") textor += "i";
                else if (texto[i].ToString() == "ì") textor += "i";
                else if (texto[i].ToString() == "ï") textor += "i";
                else if (texto[i].ToString() == "õ") textor += "o";
                else if (texto[i].ToString() == "ó") textor += "o";
                else if (texto[i].ToString() == "ò") textor += "o";
                else if (texto[i].ToString() == "ö") textor += "o";
                else if (texto[i].ToString() == "ú") textor += "u";
                else if (texto[i].ToString() == "ù") textor += "u";
                else if (texto[i].ToString() == "ü") textor += "u";
                else if (texto[i].ToString() == "ç") textor += "c";
                else if (texto[i].ToString() == "Ã") textor += "A";
                else if (texto[i].ToString() == "Á") textor += "A";
                else if (texto[i].ToString() == "À") textor += "A";
                else if (texto[i].ToString() == "Â") textor += "A";
                else if (texto[i].ToString() == "Ä") textor += "A";
                else if (texto[i].ToString() == "É") textor += "E";
                else if (texto[i].ToString() == "È") textor += "E";
                else if (texto[i].ToString() == "Ê") textor += "E";
                else if (texto[i].ToString() == "Ë") textor += "E";
                else if (texto[i].ToString() == "Í") textor += "I";
                else if (texto[i].ToString() == "Ì") textor += "I";
                else if (texto[i].ToString() == "Ï") textor += "I";
                else if (texto[i].ToString() == "Õ") textor += "O";
                else if (texto[i].ToString() == "Ó") textor += "O";
                else if (texto[i].ToString() == "Ò") textor += "O";
                else if (texto[i].ToString() == "Ö") textor += "O";
                else if (texto[i].ToString() == "Ú") textor += "U";
                else if (texto[i].ToString() == "Ù") textor += "U";
                else if (texto[i].ToString() == "Ü") textor += "U";
                else if (texto[i].ToString() == "Ç") textor += "C";
                else textor += texto[i];
            }
            return textor;
        }

        /// <summary>
        /// Caminho do diretório que contém os arquivos gerados.
        /// </summary>
        public static String ReservatoryPath
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("/") + ConfigurationManager.AppSettings["transactcustom_file"].Replace("/", @"\"); }
        }

        #region enuns 

        public enum eTipoTransacao : int
        {
            Inclusao,
            AlteracaoCadastral,
            AdicionaBeneficiario,
            CancelaBeneficiario,
            MudancaPlano,
            SegundaViaCartao,
            CancelaContrato,
            SINCRONIZACAO_SEG
        }
        public enum eFormatoArquivo : int
        {
            TxtFormatado,
            TxtDelimitado,
            Xls,
            Xml
        }
        public enum eBeneficiariosInclusos : int
        {
            ApenasTitular,
            //Todos,
            Especifico,
            ApenasInativos,
            ApenasAtivos,
            Novos
        }

        #endregion

        #region fields 

        Hashtable _dicPropotaCPFTitular;

        Object _id;
        Object _operadoraId;
        String _nome;
        Int32 _tipo;
        Int32 _formato;
        Int32 _beneficiariosInclusos;
        String _delimitador;
        DateTime _data;

        #endregion

        #region properties 

        [DBFieldInfo("lac_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("lac_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("lac_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        /// <summary>
        /// Tipo de transação (enum LayoutArquivoCustomizado.eTipoTransacao)
        /// </summary>
        [DBFieldInfo("lac_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("lac_beneficiariosInclusos", FieldType.Single)]
        public Int32 BeneficiariosInclusos
        {
            get { return _beneficiariosInclusos; }
            set { _beneficiariosInclusos= value; }
        }

        [DBFieldInfo("lac_formato", FieldType.Single)]
        public Int32 Formato
        {
            get { return _formato; }
            set { _formato= value; }
        }

        [DBFieldInfo("lac_delimitador", FieldType.Single)]
        public String Delimitador
        {
            get { return _delimitador; }
            set { _delimitador= value; }
        }

        [DBFieldInfo("lac_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        public LayoutArquivoCustomizado() { _data = DateTime.Now; }
        public LayoutArquivoCustomizado(Object id) : this() { _id = id; }

        #region EntityBase methods 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar(IList<ItemLayoutArquivoCustomizado> itens)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                pm.Save(this);

                if (itens != null)
                {
                    foreach (ItemLayoutArquivoCustomizado item in itens)
                    {
                        item.LayoutID = this.ID;
                        pm.Save(item);
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm = null;
            }
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<LayoutArquivoCustomizado> Carregar(Object operadoraId, eTipoTransacao tipo)
        {
            String qry = "* FROM layoutArquivoCustomizado WHERE lac_operadoraId=" + operadoraId + " AND lac_tipo=" + Convert.ToInt32(tipo);
            return LocatorHelper.Instance.ExecuteQuery<LayoutArquivoCustomizado>(qry, typeof(LayoutArquivoCustomizado));
        }

        internal String ToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return TirarAcentos(Convert.ToString(param));
        }
        internal String TrataFormato(String formato)
        {
            if (formato.ToUpper() == "DD/MM/YYYY")
                return "dd/MM/yyyy";
            else if (formato.ToUpper() == "DDMMYYYY")
                return "ddMMyyyy";
            if (formato.ToUpper() == "DD/MM/YY")
                return "dd/MM/yy";
            if (formato.ToUpper() == "DD/MM/YY")
                return "dd/MM/yy";
            if (formato.ToUpper() == "YYYYMMDD")
                return "yyyyMMdd";
            else
                return formato;
        }

        internal void Escreve(IList<ItemLayoutArquivoCustomizado> itens, ref StringBuilder content, DataRow row, Int32 contador)
        {
            if (itens == null || itens.Count == 0) { return; }
            String aux = ""; char pad = ' ';
            int i = 0;
            foreach (ItemLayoutArquivoCustomizado item in itens)
            {
                if (i > 0 && !String.IsNullOrEmpty(this._delimitador))
                {
                    content.Append(this._delimitador);
                }

                if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.FonteDeDados) == item.Behavior)
                {
                    #region guarda o cpf do titular 

                    if (ToString(row["contratobeneficiario_tipo"]) == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular).ToString())
                    {
                        if (!_dicPropotaCPFTitular.ContainsKey(ToString(row["contrato_id"])))
                        {
                            _dicPropotaCPFTitular.Add(ToString(row["contrato_id"]), ToString(row["beneficiario_cpf"]));
                        }
                    }
                    #endregion

                    //se o dependente nao tem cpf, usa o do titular
                    if (item.Valor.ToLower() == "beneficiario_cpf_tit")// && (ToString(row["beneficiario_cpf"]) == "" || ToString(row["beneficiario_cpf"]) == "99999999999"))
                    {
                        //if (ToString(row["contratobeneficiario_tipo"]) != "0") //nao é titular
                        //{
                            aux = ToString(_dicPropotaCPFTitular[ToString(row["contrato_id"])]);
                        //}
                    }
                    else if (item.Valor.ToLower() == "titx_dep0")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = "0";
                        else
                            aux = "X";
                    }
                    else if (item.Valor.ToLower() == "tit00_dep01")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = "01";
                        else
                            aux = "00";
                    }
                    else if (item.Valor.ToLower() == "tipoendref_lograd_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                        {
                            aux = ToString(row["tipoendref_lograd"]);
                            if (aux.Split(' ').Length > 0) { aux = aux.Split(' ')[0]; }
                        }
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_lograd_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                        {
                            aux = ToString(row["EndRef_Lograd"]);

                            if (aux.Split(' ').Length > 0)
                            {
                                String temp = "";
                                for (int j = 0; j < aux.Split(' ').Length; j++)
                                {
                                    if (j == 0) { continue; }
                                    temp += aux.Split(' ')[j] + " ";
                                }
                                aux = temp;
                            }
                        }
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_numero_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = ToString(row["EndRef_Numero"]);
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_compl_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = ToString(row["EndRef_Compl"]);
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_bairro_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = ToString(row["EndRef_Bairro"]);
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_cidade_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = ToString(row["EndRef_Cidade"]);
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_uf_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            aux = ToString(row["EndRef_UF"]);
                        else
                            aux = "";
                    }
                    else if (item.Valor.ToLower() == "endref_cep_tit")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                            if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoValor.Numerico) == item.TipoValor)
                                aux = Convert.ToInt32(row["EndRef_CEP"]).ToString();
                            else
                                aux = ToString(row["EndRef_CEP"]);
                        else
                            aux = "";
                    }
                    else
                    {
                        if (!row.Table.Columns.Contains(item.Valor)) { continue; }
                        aux = ToString(row[item.Valor]);
                    }
                    ///////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "beneficiario_sexo" && ToString(row[item.Valor]) == "1") { aux = "M"; }
                    else if (item.Valor.ToLower() == "beneficiario_sexo" && ToString(row[item.Valor]) == "2") { aux = "F"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (aux == "99999999999" && item.Valor.ToLower() == "beneficiario_cpf") { aux = ""; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if ((item.Valor.ToLower() == "endcobr_numero" || item.Valor.ToLower() == "endref_numero") && aux == "0") { aux = "s/n"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: 0 de parentesco quando titular///////////////////////////////////////////
                    if (item.Valor.ToLower() == "contratoadmparentescoagregado_parentescocodigo")
                    {
                        if (ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                        {
                            aux = "0";
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "tipoendref_lograd" || item.Valor.ToLower() == "tipoendcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0) { aux = aux.Split(' ')[0]; }
                    }

                    if (item.Valor.ToLower() == "endref_lograd" || item.Valor.ToLower() == "endcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0)
                        {
                            String temp = "";
                            for (int j = 0; j < aux.Split(' ').Length; j++)
                            {
                                if (j == 0) { continue; }
                                temp += aux.Split(' ')[j] + " ";
                            }
                            aux = temp;
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "plano_codigo")
                    {
                        String temp = ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = ToString(row["plano_codigo"]);
                        }
                        else
                        {
                            aux = ToString(row["plano_codigoParticular"]);
                        }
                    }
                    if (item.Valor.ToLower() == "plano_subplano")
                    {
                        String temp = ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = ToString(row["plano_subplano"]);
                        }
                        else
                        {
                            aux = ToString(row["plano_subplanoParticular"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: código do adicional ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "adicional_codtitular")
                    {
                        String temp = ToString(row["contratobeneficiario_tipo"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                        {
                            aux = ToString(row["adicional_codtitular"]);
                        }
                        else if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Agregado))
                        {
                            aux = ToString(row["adicional_codagregado"]);
                        }
                        else if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Dependente))
                        {
                            aux = ToString(row["adicional_coddependente"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: contrato_obs ///////////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "contrato_obs")
                    {
                        if (!String.IsNullOrEmpty(aux))
                        {
                            String[] arr = aux.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            aux = "";
                            foreach (String _arrItem in arr)
                            {
                                aux = aux + "{" + _arrItem + "} "; 
                            }
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoDado.Data) == item.TipoDado)
                    {
                        try
                        {
                            DateTime data = Convert.ToDateTime(aux, new System.Globalization.CultureInfo("pt-Br"));
                            if (!String.IsNullOrEmpty(item.Formato))
                                aux = data.ToString(TrataFormato(item.Formato));
                            else
                                aux = ToString(data);
                        }
                        catch { }
                    }

                    if (item.Tamanho > 0)
                    {
                        pad = ' ';
                        if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoValor.Numerico) == item.TipoValor)
                            pad = '0';
                        else
                        {
                            if (aux.Length > item.Tamanho)
                            { aux = aux.Substring(0, item.Tamanho); }
                        }

                        if (String.IsNullOrEmpty(this._delimitador))
                        {
                            if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoPreenchimento.Direita) == item.TipoPreenchimento)
                            { aux = aux.PadRight(item.Tamanho, pad); }
                            else
                            { aux = aux.PadLeft(item.Tamanho, pad); }
                        }
                    }
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.QuebraDeLinha) == item.Behavior)
                {
                    content.Append(Environment.NewLine);
                    i = 0;
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Contador) == item.Behavior)
                {
                    aux = contador.ToString();

                    if (item.Tamanho > 0)
                    {
                        pad = ' ';
                        if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoValor.Numerico) == item.TipoValor)
                            pad = '0';

                        if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoPreenchimento.Direita) == item.TipoPreenchimento)
                            aux = aux.PadRight(item.Tamanho, pad);
                        else
                            aux = aux.PadLeft(item.Tamanho, pad);
                    }
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.DataCorrente) == item.Behavior)
                {
                    if (!String.IsNullOrEmpty(item.Formato))
                        aux = DateTime.Now.ToString(TrataFormato(item.Formato));
                    else
                        aux = ToString(DateTime.Now);
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Literal) == item.Behavior)
                {
                    aux = item.Valor;

                    if (item.Tamanho > 0)
                    {
                        pad = ' ';
                        if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoValor.Numerico) == item.TipoValor)
                            pad = '0';

                        if (String.IsNullOrEmpty(this._delimitador))
                        {
                            if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoPreenchimento.Direita) == item.TipoPreenchimento)
                            { aux = aux.PadRight(item.Tamanho, pad); }
                            else
                            { aux = aux.PadLeft(item.Tamanho, pad); }
                        }
                    }
                }

                content.Append(aux);
                i++;
            }
        }
        internal void EscreveCabecalhoXLS(IList<ItemLayoutArquivoCustomizado> itens, ref StringBuilder content)
        {
            content.Append("<table>");
            content.Append("<tr>");

            if (itens == null || itens.Count == 0) { return; }

            foreach (ItemLayoutArquivoCustomizado item in itens)
            {
                if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Literal) == item.Behavior)
                {
                    content.Append("<th>");
                    content.Append(item.Valor);
                    content.Append("</th>");
                }
            }

            content.Append("</tr>");
        }
        internal void EscreveDetalheXLS(IList<ItemLayoutArquivoCustomizado> itens, ref StringBuilder content, DataRow row)
        {
            if (itens == null || itens.Count == 0) { return; }

            String aux = "";
            content.Append("<tr>");

            foreach (ItemLayoutArquivoCustomizado item in itens)
            {
                if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.FonteDeDados) == item.Behavior)
                {
                    aux = ToString(row[item.Valor]);

                    //TODO: Fone ddd ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "beneficiario_telefoneddd" && row["beneficiario_telefoneDDD"] != DBNull.Value)
                    {
                        String temp = Convert.ToString(row["beneficiario_telefoneDDD"]).Split(')')[0] + ")";
                        if (temp.Length == 4)
                            aux = temp.Trim();
                        else
                            aux = "";
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: Fone Num ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "beneficiario_telefonenumero" && row["beneficiario_telefoneNumero"] != DBNull.Value)
                    {
                        String[] temp = Convert.ToString(row["beneficiario_telefone"]).Split(')');
                        if (temp.Length == 2)
                            aux = temp[1].Trim();
                        else
                            aux = "";
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: manobra até a criação das FÓRMULAS/////////////////////////////////////
                    if (item.Valor.ToLower() == "beneficiario_sexo" && Convert.ToString(row[item.Valor]) == "1") { aux = "M"; }
                    else if (item.Valor.ToLower() == "beneficiario_sexo" && Convert.ToString(row[item.Valor]) == "2") { aux = "F"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: se cpf 99999999999, enviar "" //////////////////////////////////////////
                    if (aux == "99999999999" && item.Valor.ToLower() == "beneficiario_cpf") { aux = ""; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if ((item.Valor.ToLower() == "endcobr_numero" || item.Valor.ToLower() == "endref_numero") && aux == "0") { aux = "s/n"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "contratobeneficiario_tipo")
                    {
                        if (aux == "0") { aux = "T"; }
                        else { aux = "D"; }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: 0 de parentesco quando titular///////////////////////////////////////////
                    if (item.Valor.ToLower() == "contratoadmparentescoagregado_parentescocodigo")
                    {
                        if (Convert.ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                        {
                            aux = "0";
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "tipoendref_lograd" || item.Valor.ToLower() == "tipoendcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0) { aux = aux.Split(' ')[0]; }
                    }

                    if (item.Valor.ToLower() == "endref_lograd" || item.Valor.ToLower() == "endcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0)
                        {
                            String temp = "";
                            for (int i = 0; i < aux.Split(' ').Length; i++)
                            {
                                if (i == 0) { continue; }
                                temp += aux.Split(' ')[i] + " ";
                            }
                            aux = temp;
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: código do plano qdo houver apenas uma coluna (particular ou coletivo?)///
                    if (item.Valor.ToLower() == "plano_codigo")
                    {
                        String temp = Convert.ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = Convert.ToString(row["plano_codigo"]);
                        }
                        else
                        {
                            aux = Convert.ToString(row["plano_codigoParticular"]);
                        }
                    }
                    if (item.Valor.ToLower() == "plano_subplano")
                    {
                        String temp = Convert.ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = Convert.ToString(row["plano_subplano"]);
                        }
                        else
                        {
                            aux = Convert.ToString(row["plano_subplanoParticular"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: código do adicional ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "adicional_codtitular")
                    {
                        String temp = Convert.ToString(row["contratobeneficiario_tipo"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                        {
                            aux = Convert.ToString(row["adicional_codtitular"]);
                        }
                        else if(Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Agregado))
                        {
                            aux = Convert.ToString(row["adicional_codagregado"]);
                        }
                        else if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Dependente))
                        {
                            aux = Convert.ToString(row["adicional_coddependente"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoDado.Data) == item.TipoDado)
                    {
                        try
                        {
                            DateTime data = Convert.ToDateTime(aux, new System.Globalization.CultureInfo("pt-Br"));
                            if (!String.IsNullOrEmpty(item.Formato))
                                aux = data.ToString(TrataFormato(item.Formato));
                            else
                                aux = Convert.ToString(data);
                        }
                        catch { }
                    }
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Literal) == item.Behavior)
                {
                    aux = item.Valor;
                }

                
                content.Append("<td>");
                content.Append(aux);
                content.Append("</td>");
            }

            content.Append("</tr>");
        }
        internal void EscreveDetalheXML(IList<ItemLayoutArquivoCustomizado> itens, ref StringBuilder content, DataRow row)
        {
            if (itens == null || itens.Count == 0) { return; }

            String aux = "";

            foreach (ItemLayoutArquivoCustomizado item in itens)
            {
                if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.FonteDeDados) == item.Behavior)
                {
                    aux = ToString(row[item.Valor]);

                    //TODO: Fone ddd ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "beneficiario_telefoneddd" && row["beneficiario_telefoneDDD"] != DBNull.Value)
                    {
                        String temp = Convert.ToString(row["beneficiario_telefoneDDD"]).Split(')')[0] + ")";
                        if (temp.Length == 4)
                            aux = temp.Trim();
                        else
                            aux = "";
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: Fone Num ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "beneficiario_telefonenumero" && row["beneficiario_telefoneNumero"] != DBNull.Value)
                    {
                        String[] temp = Convert.ToString(row["beneficiario_telefone"]).Split(')');
                        if (temp.Length == 2)
                            aux = temp[1].Trim();
                        else
                            aux = "";
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "beneficiario_sexo" && Convert.ToString(row[item.Valor]) == "1") { aux = "M"; }
                    else if (item.Valor.ToLower() == "beneficiario_sexo" && Convert.ToString(row[item.Valor]) == "2") { aux = "F"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: se cpf 99999999999, enviar "" //////////////////////////////////////////
                    if (aux == "99999999999" && item.Valor.ToLower() == "beneficiario_cpf") { aux = ""; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if ((item.Valor.ToLower() == "endcobr_numero" || item.Valor.ToLower() == "endref_numero") && aux == "0") { aux = "s/n"; }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "contratobeneficiario_tipo")
                    {
                        if (aux == "0") { aux = "T"; }
                        else { aux = "D"; }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: 0 de parentesco quando titular///////////////////////////////////////////
                    if (item.Valor.ToLower() == "contratoadmparentescoagregado_parentescocodigo")
                    {
                        if (Convert.ToString(row["contratobeneficiario_tipo"]) == "0") //titular
                        {
                            aux = "0";
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (item.Valor.ToLower() == "tipoendref_lograd" || item.Valor.ToLower() == "tipoendcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0) { aux = aux.Split(' ')[0]; }
                    }

                    if (item.Valor.ToLower() == "endref_lograd" || item.Valor.ToLower() == "endcobr_lograd")
                    {
                        if (aux.Split(' ').Length > 0)
                        {
                            String temp = "";
                            for (int i = 0; i < aux.Split(' ').Length; i++)
                            {
                                if (i == 0) { continue; }
                                temp += aux.Split(' ')[i] + " ";
                            }
                            aux = temp;
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: código do plano qdo houver apenas uma coluna (particular ou coletivo?)///
                    if (item.Valor.ToLower() == "plano_codigo")
                    {
                        String temp = Convert.ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = Convert.ToString(row["plano_codigo"]);
                        }
                        else
                        {
                            aux = Convert.ToString(row["plano_codigoParticular"]);
                        }
                    }
                    if (item.Valor.ToLower() == "plano_subplano")
                    {
                        String temp = Convert.ToString(row["contrato_tipoacomodacao"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun))
                        {
                            aux = Convert.ToString(row["plano_subplano"]);
                        }
                        else
                        {
                            aux = Convert.ToString(row["plano_subplanoParticular"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    //TODO: código do adicional ////////////////////////////////////////////////////
                    if (item.Valor.ToLower() == "adicional_codtitular")
                    {
                        String temp = Convert.ToString(row["contratobeneficiario_tipo"]);
                        if (Convert.ToInt32(temp) == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                        {
                            aux = Convert.ToString(row["adicional_codtitular"]);
                        }
                        else if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Agregado))
                        {
                            aux = Convert.ToString(row["adicional_codagregado"]);
                        }
                        else if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Dependente))
                        {
                            aux = Convert.ToString(row["adicional_coddependente"]);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eTipoDado.Data) == item.TipoDado)
                    {
                        try
                        {
                            DateTime data = Convert.ToDateTime(aux, new System.Globalization.CultureInfo("pt-Br"));
                            if (!String.IsNullOrEmpty(item.Formato))
                                aux = data.ToString(TrataFormato(item.Formato));
                            else
                                aux = Convert.ToString(data);
                        }
                        catch { }
                    }
                }
                else if (Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Literal) == item.Behavior)
                {
                    aux = item.Valor;
                }

                content.Append(String.Concat("<", item.Valor.ToLower(), ">"));
                content.Append(aux);
                content.Append(String.Concat("</", item.Valor.ToLower(), ">"));
            }
        }

        public static void __RegerarArquivo(String[] contratoBeneficiarioIds)
        {
            #region query 

            String qry = String.Concat("SELECT lote_layout_customizado_id, contrato.*, plano.*, operadora_nome, beneficiario.*, estadocivil_codigo, endCobr.endereco_logradouro AS EndCobr_Lograd, endCobr.endereco_numero AS EndCobr_Numero, endCobr.endereco_complemento AS EndCobr_Compl, endCobr.endereco_bairro AS EndCobr_Bairro, endCobr.endereco_cidade AS EndCobr_Cidade, endCobr.endereco_uf AS EndCobr_UF, endCobr.endereco_cep AS EndCobr_CEP, endRef.endereco_logradouro AS EndRef_Lograd, endRef.endereco_numero AS EndRef_Numero, endRef.endereco_complemento AS EndRef_Compl, endRef.endereco_bairro AS EndRef_Bairro, endRef.endereco_cidade AS EndRef_Cidade, endRef.endereco_uf AS EndRef_UF, endRef.endereco_cep AS EndRef_CEP, contratobeneficiario_numeroSequencia, contratobeneficiario_peso, contratobeneficiario_altura, contratobeneficiario_dataCasamento, contratobeneficiario_status, contratobeneficiario_tipo ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                "       INNER JOIN arquivo_transacional_lote_item ON item_contrato_id=contratobeneficiario_contratoId AND item_beneficiario_id=contratobeneficiario_beneficiarioId AND item_ativo=1 ",
                "       INNER JOIN arquivo_transacional_lote ON lote_id=item_lote_id",
                "   WHERE ",
                "       contratobeneficiario_id IN (", String.Join(",", contratoBeneficiarioIds), ") AND lote_layout_customizado_id IS NOT NULL ",
                "   ORDER BY lote_layout_customizado_id,contrato_id");
                #endregion

            Int32 i = 0;
            DataTable dt = null;
            StringBuilder content = null;
            ArqTransacionalLote lote = null;
            ArqTransacionalLoteItem item = null;
            LayoutArquivoCustomizado layout = null;
            IList<ItemLayoutArquivoCustomizado> itens = null;
            PersistenceManager pm = new PersistenceManager();
            IList<ItemLayoutArquivoCustomizado> itensSecao = null;
            pm.BeginTransactionContext();

            try
            {
                dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
                if (dt.Rows.Count == 0) { return; }

                String fileName = null;
                String filePath = null;
                if (!Directory.Exists(LayoutArquivoCustomizado.ReservatoryPath)) { Directory.CreateDirectory(LayoutArquivoCustomizado.ReservatoryPath); }

                List<String> layoutsIDs  = new List<String>();

                foreach (DataRow row in dt.Rows)
                {
                    if (layoutsIDs.Contains(Convert.ToString(row["lote_layout_customizado_id"]))) { continue; }
                    layoutsIDs.Add(Convert.ToString(row["lote_layout_customizado_id"]));
                }

                DataRow[] rows = null;

                foreach (String layoutId in layoutsIDs)
                {
                    lote = new ArqTransacionalLote();
                    fileName = String.Concat("_mov_", DateTime.Now.ToString("ddMMyyHHmmfff"), ".txt");
                    filePath = LayoutArquivoCustomizado.ReservatoryPath;

                    layout = new LayoutArquivoCustomizado(layoutId);
                    pm.Load(layout);
                    itens = ItemLayoutArquivoCustomizado.Carregar(layoutId, pm);
                    if (itens == null) { continue; }

                    content = new StringBuilder();
                    rows = dt.Select("lote_layout_customizado_id=" + layoutId, "contrato_id ASC, contratobeneficiario_tipo ASC");
                    if (rows == null || rows.Length == 0) { continue; }

                    //CABEÇALHO 
                    itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Header);
                    if (((eFormatoArquivo)layout._formato) == eFormatoArquivo.Xls)
                    {
                        layout.EscreveCabecalhoXLS(itensSecao, ref content);
                        fileName = fileName.Replace("_mov_", "_movXLS_");
                    }
                    else
                    {
                        layout.Escreve(itensSecao, ref content, null, i);
                        content.Append(Environment.NewLine);
                    }

                    //CABEÇALHO DO DETALHE
                    itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.HeaderDetail);

                    if (((eFormatoArquivo)layout._formato) == eFormatoArquivo.Xls)
                        layout.EscreveDetalheXLS(itensSecao, ref content, dt.Rows[0]);
                    else
                    {
                        layout.Escreve(itensSecao, ref content, dt.Rows[0], i);
                        content.Append(Environment.NewLine);
                    }

                    //DETALHE
                    itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Detail);
                    i = 0;
                    foreach (DataRow row in rows)
                    {
                        if (((eFormatoArquivo)layout._formato) == eFormatoArquivo.Xls)
                            layout.EscreveDetalheXLS(itensSecao, ref content, row);
                        else
                        {
                            if (i > 0) { content.Append(Environment.NewLine); }
                            layout.Escreve(itensSecao, ref content, row, i);
                        }

                        item = new ArqTransacionalLoteItem();
                        item.BeneficiarioID = row["beneficiario_id"];
                        item.BeneficiarioSequencia = Convert.ToInt32(row["contratobeneficiario_numeroSequencia"]);
                        item.ContratoID = row["contrato_id"];
                        item.Ativo = true;
                        lote.Itens.Add(item);

                        //Altera o status do beneficiario
                        ContratoBeneficiario.SetaStatusDevolvidoParaContratoBeneficiario(((ContratoBeneficiario.eStatus)Convert.ToInt32(row["contratobeneficiario_status"])), row["contrato_id"], row["beneficiario_id"], pm);

                        i++;
                    }

                    //RODAPÉ
                    if (((eFormatoArquivo)layout._formato) == eFormatoArquivo.Xls)
                        content.Append("</table>");
                    else
                    {
                        itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Trailler);
                        if (itensSecao != null && itensSecao.Count > 0) { content.Append(Environment.NewLine); }
                        layout.Escreve(itensSecao, ref content, null, i);
                    }

                    if (lote.Itens.Count > 0)
                    {
                        lote.Arquivo      = fileName;
                        lote.Numeracao    = 0;
                        lote.DataCriacao  = DateTime.Now;
                        lote.Movimentacao = LayoutArquivoCustomizado.TraduzMovimentacao((eTipoTransacao)layout._tipo);
                        lote.OperadoraID  = layout._operadoraId;
                        lote.Quantidade   = rows.Length;
                        lote.TipoMovimentacao = LayoutArquivoCustomizado.TraduzTipoMovimentacao((eTipoTransacao)layout._tipo);
                        lote.LayoutCustomizadoID = layout._id;
                        lote.Salvar(false, pm);
                    }

                    layout.EscreveArquivo(fileName, content.ToString());
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                if (dt != null) { dt.Dispose(); }
                pm = null;
            }
        }

        void EscreveArquivo(String nome, String conteudo)
        {
            File.WriteAllText(LayoutArquivoCustomizado.ReservatoryPath + nome, 
                conteudo, System.Text.Encoding.GetEncoding("iso-8859-1"));
        }

        /// <summary>
        /// Gera o conteúdo do arquivo com base no layout customizado.
        /// </summary>
        /// <param name="contratoIds">Lista com os ids dos contratos.</param>
        /// <param name="contratoBeneficiarios">A chave será o id do contrato e o valor será uma lista (String) com os ids dos beneficiários participantes.</param>
        /// <param name="tipoInclusao">Forma de inclusão de beneficiários na query que busca do banco de dados os dados que farão parte do conteúdo do arquivo.</param>
        /// <returns>Conteúdo do arquivo</returns>
        public String GeraConteudoDoArquivo(ref String fileName, ref String filePath, String[] contratoAdmIDs, String[] adicionaisIDs, DateTime vigencia, Boolean exportacao, DateTime de, DateTime ate, Boolean somenteComAdicional, ref String errMsg)
        {
            if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xml)
            {
                return GeraConteudoDoArquivoXML(ref fileName, ref filePath, contratoAdmIDs, adicionaisIDs, vigencia, exportacao, de, ate, somenteComAdicional, ref errMsg);
            }

            eBeneficiariosInclusos tipoInclusao = traduzParaTipoInclusaoBeneficiario((eTipoTransacao)this._tipo);
            ContratoBeneficiario.eStatus benefStatus = traduzParaContratoBeneficiarioStatus((eTipoTransacao)this._tipo);
            ArqTransacionalLote lote = new ArqTransacionalLote();

            fileName = String.Concat("_mov_", DateTime.Now.ToString("ddMMyyHHmmfff"), ".txt");
            filePath = LayoutArquivoCustomizado.ReservatoryPath;
            if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                _dicPropotaCPFTitular = new Hashtable();

                #region condição de vigencia 

                String qry = "";
                String vigenciaCondition = "";
                if (vigencia != DateTime.MinValue)
                {
                    if (eTipoTransacao.Inclusao == (eTipoTransacao)this._tipo)
                    {
                        vigenciaCondition = " AND contrato_vigencia = '" + vigencia.ToString("yyyy-MM-dd 00:00:00") + "'";
                    }
                    else if (eTipoTransacao.AdicionaBeneficiario == (eTipoTransacao)this._tipo)
                    {
                        vigenciaCondition = " AND contratobeneficiario_vigencia = '" + vigencia.ToString("yyyy-MM-dd 00:00:00") + "'";
                    }
                }
                #endregion

                #region comentado 
                ////if (tipoInclusao == eBeneficiariosInclusos.Especifico)
                ////{
                //qry = String.Concat("contratobeneficiario_id, contratobeneficiario_contratoId ",
                //    "   FROM contrato_beneficiario ",
                //    "       INNER JOIN contrato ON contratobeneficiario_contratoId=contrato_id ",
                //    "   WHERE ",
                //    "       contratobeneficiario_ativo=1 AND ",
                //    "       contrato_operadoraId=", this._operadoraId, " AND ",
                //    "       contratobeneficiario_status=", Convert.ToInt32(benefStatus), " AND ",
                //    "       contrato_cancelado=0 AND contrato_rascunho=0 AND contrato_adimplente=1",
                //    "   ORDER BY contratobeneficiario_contratoId");

                //DataTable dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                //if (dtAux.Rows.Count == 0) { return null; }
                //contratoIds = new List<String>();
                //contratoBeneficiariosEspecificosIDs = new List<String>();

                //foreach (DataRow row in dtAux.Rows)
                //{
                //    contratoBeneficiariosEspecificosIDs.Add(Convert.ToString(row[0]));
                //    if (!contratoIds.Contains(Convert.ToString(row[1])))
                //    {
                //        contratoIds.Add(Convert.ToString(row[1]));
                //    }
                //}

                //dtAux.Dispose();
                ////}
                #endregion

                IList<ItemLayoutArquivoCustomizado> itens = ItemLayoutArquivoCustomizado.Carregar(this._id, pm);
                if (itens == null) { return null; } //throw exception

                StringBuilder content = new StringBuilder();

                //ATENCAO:  problema no retorno de beneficiarios (relacao 1 ou n para cada contrato)
                //          para saber quais retornar, utilizar a enum eBeneficiariosInclusos
                DataTable dt = null;
                ArqTransacionalLoteItem item = null;
                Int32 i = 0;

                //HEADER
                IList<ItemLayoutArquivoCustomizado> itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Header);

                if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xls)
                {
                    this.EscreveCabecalhoXLS(itensSecao, ref content);
                    fileName = fileName.Replace("_mov_", "_movXLS_");
                }
                if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xml)
                {
                    content.Append("<?xml version=\"1.0\"?>");
                    fileName = fileName.Replace("_mov_", "_movXML_");
                }
                else
                {
                    this.Escreve(itensSecao, ref content, null, i);
                    if (itensSecao != null && itensSecao.Count > 0)
                    {
                        content.Append(Environment.NewLine);//////////////////////////////////////////////////
                    }
                }

                #region query 

                #region processa segundo enum eBeneficiariosInclusos 

                String condicaoContrato = "";
                if (this._tipo == (Int32)eTipoTransacao.Inclusao)
                    condicaoContrato = " contrato_pendente <> 1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.AdicionaBeneficiario)
                    condicaoContrato = " contrato_adimplente=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.AlteracaoCadastral)
                    condicaoContrato = " contrato_adimplente=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG)
                    condicaoContrato = " contrato_rascunho <> 1 AND ";

                String campos = "SELECT contrato.*, estipulante_descricao, contratoadm_descricao, contratoadm_numero, tipocontrato_descricao, usuario_documento1, usuario_nome, plano.*, operadora_nome, operadora_cnpj, beneficiario.*, beneficiario_telefone as beneficiario_telefoneDDD, beneficiario_telefone as beneficiario_telefoneNumero, estadocivil_descricao, estadocivil_codigo, endCobr.endereco_logradouro AS EndCobr_Lograd, endCobr.endereco_logradouro AS TipoEndCobr_Lograd, endCobr.endereco_numero AS EndCobr_Numero, endCobr.endereco_complemento AS EndCobr_Compl, endCobr.endereco_bairro AS EndCobr_Bairro, endCobr.endereco_cidade AS EndCobr_Cidade, endCobr.endereco_uf AS EndCobr_UF, endCobr.endereco_cep AS EndCobr_CEP, endRef.endereco_logradouro AS EndRef_Lograd, endRef.endereco_logradouro AS TipoEndRef_Lograd,endRef.endereco_numero AS EndRef_Numero, endRef.endereco_complemento AS EndRef_Compl, endRef.endereco_bairro AS EndRef_Bairro, endRef.endereco_cidade AS EndRef_Cidade, endRef.endereco_uf AS EndRef_UF, endRef.endereco_cep AS EndRef_CEP, contratobeneficiario_numeroSequencia, contratobeneficiario_peso, contratobeneficiario_altura, contratobeneficiario_dataCasamento, contratobeneficiario_status, contratobeneficiario_data, contratobeneficiario_tipo, contratobeneficiario_carenciaCodigo, contratoAdmparentescoagregado_parentescoCodigo, contratoAdmparentescoagregado_parentescoTipo, contratoAdmparentescoagregado_parentescoDescricao ";

                if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG || exportacao)//Sincronização SEG ou exportacao
                {
                    String between = "";

                    if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG)//só para sincronizacao seg
                    {
                        //if (!exportacaoNOVOS)
                        //    between = String.Concat(" AND contrato_alteracao BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999'");
                        //else
                            between = String.Concat(" AND ((contrato_data BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999') OR (contrato_alteracao BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999'))");
                    }

                    String fielAdicional = "";
                    String joinAdicional = "";
                    String condAdicional = "";

                    #region Query Adicional 

                    if (adicionaisIDs != null && adicionaisIDs.Length > 0)
                    {
                        fielAdicional = ", adicional_codTitular,adicional_codAgregado,adicional_codDependente, adicional_descricao ";

                        if(!somenteComAdicional)
                            joinAdicional = " LEFT JOIN adicional_beneficiario ON beneficiario_id=adicionalbeneficiario_beneficiarioid AND adicionalbeneficiario_propostaId=contrato_id left JOIN adicional ON adicional_id=adicionalbeneficiario_adicionalId ";
                        else
                            joinAdicional = " INNER JOIN adicional_beneficiario ON beneficiario_id=adicionalbeneficiario_beneficiarioid AND adicionalbeneficiario_propostaId=contrato_id left JOIN adicional ON adicional_id=adicionalbeneficiario_adicionalId ";

                        condAdicional = String.Concat(" (adicionalbeneficiario_adicionalId IS NULL OR adicionalbeneficiario_adicionalId IN (", String.Join(",", adicionaisIDs), ")) AND ");
                    }
                    #endregion

                    //condição para nao enviar beneficiários excluidos.
                    String joinBeneficiarioCondition = String.Concat("(", Convert.ToInt32(ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora), ",", Convert.ToInt32(ContratoBeneficiario.eStatus.Excluido), ",", Convert.ToInt32(ContratoBeneficiario.eStatus.ExclusaoPendente), ") ");

                    //Localiza propostas cadastradas/alteradas no intervalo de data fornecido
                    qry = String.Concat(campos, fielAdicional,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_status NOT IN ", joinBeneficiarioCondition, //AND contratobeneficiario_ativo=1 
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 AND contratobeneficiario_status NOT IN ",joinBeneficiarioCondition, //AND contratobeneficiario_ativo=1 
                        joinAdicional,
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato, condAdicional,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, between, vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasAtivos)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasInativos)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=0 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=0 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasTitular)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=0 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_tipo=0 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.Especifico)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.Novos) //para novos beneficiarios
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo),
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo),
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato, 
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                #endregion

                #endregion

                dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];

                if (dt.Rows.Count == 0) { return null; }

                //CABEÇALHO DO DETALHE
                itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.HeaderDetail);

                if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xls)
                    this.EscreveDetalheXLS(itensSecao, ref content, dt.Rows[0]);
                //else if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xml)
                //{
                //    content.Append("<cabecalho>");
                //    this.EscreveDetalheXML(itensSecao, ref content, dt.Rows[0]);
                //    content.Append("</cabecalho>");
                //}
                else
                {
                    this.Escreve(itensSecao, ref content, dt.Rows[0], i);

                    if (itensSecao != null && itensSecao.Count > 0)
                    {
                        content.Append(Environment.NewLine);//////////////////////////////////////////////////
                    }
                }

                //DETALHE
                itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Detail);
                i = 1;
                foreach (DataRow row in dt.Rows)
                {
                    if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xls)
                        this.EscreveDetalheXLS(itensSecao, ref content, row);
                    else
                    {
                        if (i > 1) { content.Append(Environment.NewLine); }
                        this.Escreve(itensSecao, ref content, row, i);
                    }

                    item                        = new ArqTransacionalLoteItem();
                    item.BeneficiarioID         = row["beneficiario_id"];
                    item.BeneficiarioSequencia  = Convert.ToInt32(row["contratobeneficiario_numeroSequencia"]);
                    item.ContratoID             = row["contrato_id"];
                    item.Ativo                  = true;
                    lote.Itens.Add(item);
                    i++;
                }

                if (((eFormatoArquivo)this._formato) == eFormatoArquivo.Xls)
                    content.Append("</table>");
                else if (((eFormatoArquivo)this._formato) != eFormatoArquivo.Xml)
                {
                    i++;
                    itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Trailler);
                    if (itensSecao != null && itensSecao.Count > 0) { content.Append(Environment.NewLine); }
                    this.Escreve(itensSecao, ref content, null, i);
                }

                if (lote.Itens.Count > 0 && !exportacao)
                {
                    lote.Arquivo             = fileName;
                    lote.Numeracao           = 0;
                    lote.DataCriacao         = DateTime.Now;
                    lote.DataVigencia        = vigencia;
                    lote.Movimentacao        = LayoutArquivoCustomizado.TraduzMovimentacao((eTipoTransacao)this._tipo);
                    lote.OperadoraID         = this._operadoraId;
                    lote.Quantidade          = dt.Rows.Count;
                    lote.TipoMovimentacao    = LayoutArquivoCustomizado.TraduzTipoMovimentacao((eTipoTransacao)this._tipo);
                    lote.LayoutCustomizadoID = this._id;
                    lote.Exportacao          = exportacao;
                    lote.Salvar(false, pm);
                }

                dt.Dispose();
                pm.Commit();
                return content.ToString();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                errMsg = ex.Message;
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        String GeraConteudoDoArquivoXML(ref String fileName, ref String filePath, String[] contratoAdmIDs, String[] adicionaisIDs, DateTime vigencia, Boolean exportacao, DateTime de, DateTime ate, Boolean somenteComAdicional, ref String errMsg)
        {
            eBeneficiariosInclusos tipoInclusao = traduzParaTipoInclusaoBeneficiario((eTipoTransacao)this._tipo);
            ContratoBeneficiario.eStatus benefStatus = traduzParaContratoBeneficiarioStatus((eTipoTransacao)this._tipo);
            ArqTransacionalLote lote = new ArqTransacionalLote();
            ArqTransacionalLoteItem item;

            fileName = String.Concat("_movXML_", DateTime.Now.ToString("ddMMyyHHmmfff"), ".xml");
            filePath = LayoutArquivoCustomizado.ReservatoryPath;

            if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                _dicPropotaCPFTitular = new Hashtable();

                #region condição de vigencia

                String qry = "";
                String vigenciaCondition = "";
                if (vigencia != DateTime.MinValue)
                {
                    if (eTipoTransacao.Inclusao == (eTipoTransacao)this._tipo)
                    {
                        vigenciaCondition = " AND contrato_vigencia = '" + vigencia.ToString("yyyy-MM-dd 00:00:00") + "'";
                    }
                    else if (eTipoTransacao.AdicionaBeneficiario == (eTipoTransacao)this._tipo)
                    {
                        vigenciaCondition = " AND contratobeneficiario_vigencia = '" + vigencia.ToString("yyyy-MM-dd 00:00:00") + "'";
                    }
                }
                #endregion

                IList<ItemLayoutArquivoCustomizado> itens = ItemLayoutArquivoCustomizado.Carregar(this._id, pm);
                if (itens == null) { return null; } //throw exception

                StringBuilder content = new StringBuilder();
                DataTable dt = null;

                #region query

                #region processa segundo enum eBeneficiariosInclusos

                String condicaoContrato = "";
                if (this._tipo == (Int32)eTipoTransacao.Inclusao)
                    condicaoContrato = " contrato_pendente <> 1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.AdicionaBeneficiario)
                    condicaoContrato = " contrato_adimplente=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.AlteracaoCadastral)
                    condicaoContrato = " contrato_adimplente=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND ";
                else if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG)
                    condicaoContrato = " contrato_rascunho <> 1 AND ";

                String campos = "SELECT contrato.*, estipulante_descricao, contratoadm_descricao, contratoadm_numero, tipocontrato_descricao, usuario_documento1, usuario_nome, plano.*, operadora_nome, operadora_cnpj, beneficiario.*, beneficiario_telefone as beneficiario_telefoneDDD, beneficiario_telefone as beneficiario_telefoneNumero, estadocivil_descricao, estadocivil_codigo, endCobr.endereco_logradouro AS EndCobr_Lograd, endCobr.endereco_logradouro AS TipoEndCobr_Lograd, endCobr.endereco_numero AS EndCobr_Numero, endCobr.endereco_complemento AS EndCobr_Compl, endCobr.endereco_bairro AS EndCobr_Bairro, endCobr.endereco_cidade AS EndCobr_Cidade, endCobr.endereco_uf AS EndCobr_UF, endCobr.endereco_cep AS EndCobr_CEP, endRef.endereco_logradouro AS EndRef_Lograd, endRef.endereco_logradouro AS TipoEndRef_Lograd,endRef.endereco_numero AS EndRef_Numero, endRef.endereco_complemento AS EndRef_Compl, endRef.endereco_bairro AS EndRef_Bairro, endRef.endereco_cidade AS EndRef_Cidade, endRef.endereco_uf AS EndRef_UF, endRef.endereco_cep AS EndRef_CEP, contratobeneficiario_numeroSequencia, contratobeneficiario_peso, contratobeneficiario_altura, contratobeneficiario_dataCasamento, contratobeneficiario_status, contratobeneficiario_data, contratobeneficiario_tipo, contratobeneficiario_carenciaCodigo, contratoAdmparentescoagregado_parentescoCodigo, contratoAdmparentescoagregado_parentescoTipo, contratoAdmparentescoagregado_parentescoDescricao ";

                if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG || exportacao)//Sincronização SEG ou exportacao
                {
                    String between = "";

                    if (this._tipo == (Int32)eTipoTransacao.SINCRONIZACAO_SEG)//só para sincronizacao seg
                    {
                        //if (!exportacaoNOVOS)
                        //    between = String.Concat(" AND contrato_alteracao BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999'");
                        //else
                        between = String.Concat(" AND ((contrato_data BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999') OR (contrato_alteracao BETWEEN '", de.ToString("yyyy-MM-dd HH:mm"), ":00:000' AND '", ate.ToString("yyyy-MM-dd HH:mm"), ":59:999'))");
                    }

                    String fielAdicional = "";
                    String joinAdicional = "";
                    String condAdicional = "";

                    #region Query Adicional

                    if (adicionaisIDs != null && adicionaisIDs.Length > 0)
                    {
                        fielAdicional = ", adicional_codTitular,adicional_codAgregado,adicional_codDependente, adicional_descricao ";

                        if (!somenteComAdicional)
                            joinAdicional = " LEFT JOIN adicional_beneficiario ON beneficiario_id=adicionalbeneficiario_beneficiarioid AND adicionalbeneficiario_propostaId=contrato_id left JOIN adicional ON adicional_id=adicionalbeneficiario_adicionalId ";
                        else
                            joinAdicional = " INNER JOIN adicional_beneficiario ON beneficiario_id=adicionalbeneficiario_beneficiarioid AND adicionalbeneficiario_propostaId=contrato_id left JOIN adicional ON adicional_id=adicionalbeneficiario_adicionalId ";

                        condAdicional = String.Concat(" (adicionalbeneficiario_adicionalId IS NULL OR adicionalbeneficiario_adicionalId IN (", String.Join(",", adicionaisIDs), ")) AND ");
                    }
                    #endregion

                    //condição para nao enviar beneficiários excluidos.
                    String joinBeneficiarioCondition = String.Concat("(", Convert.ToInt32(ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora), ",", Convert.ToInt32(ContratoBeneficiario.eStatus.Excluido), ",", Convert.ToInt32(ContratoBeneficiario.eStatus.ExclusaoPendente), ") ");

                    //Localiza propostas cadastradas/alteradas no intervalo de data fornecido
                    qry = String.Concat(campos, fielAdicional,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_status NOT IN ", joinBeneficiarioCondition, //AND contratobeneficiario_ativo=1 
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 AND contratobeneficiario_status NOT IN ", joinBeneficiarioCondition, //AND contratobeneficiario_ativo=1 
                        joinAdicional,
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato, condAdicional,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, between, vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasAtivos)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasInativos)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=0 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=0 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.ApenasTitular)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=0 ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_tipo=0 ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.Especifico)
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId ",
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                else if (tipoInclusao == eBeneficiariosInclusos.Novos) //para novos beneficiarios
                {
                    qry = String.Concat(campos,
                        "   FROM contrato ",
                        "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
                        "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
                        "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
                        "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
                        "       INNER JOIN plano ON contrato_planoId=plano_id ",
                        "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                        "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo),
                        "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo),
                        "       INNER JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId",
                        "       INNER JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId",
                        "       INNER JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                        "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
                        "   WHERE ", condicaoContrato,
                        "       contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") AND ",
                        "       contrato_operadoraId=", this._operadoraId, " AND ",
                        "       contratobeneficiario_ativo=1 AND contratobeneficiario_status=", Convert.ToInt32(benefStatus), vigenciaCondition,
                        "   ORDER BY contrato_id, contratobeneficiario_tipo");
                }
                #endregion

                #endregion

                dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];

                if (dt.Rows.Count == 0) { return null; }

                ArrayList propostas = new ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    if (propostas.Contains(Convert.ToString(row["contrato_id"]))) { continue; }

                    propostas.Add(Convert.ToString(row["contrato_id"]));
                }

                content.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                content.AppendLine();
                content.Append("<corpo>");
                content.AppendLine();

                IList<ItemLayoutArquivoCustomizado> itensSecao = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.Detail);
                IList<ItemLayoutArquivoCustomizado> itensSecaoCabecalho = ItemLayoutArquivoCustomizado.RetornarItensDaSecao(itens, ItemLayoutArquivoCustomizado.eSecao.HeaderDetail);
                DataRow[] rows = null;

                foreach (String contratoId in propostas)
                {
                    content.Append("<proposta>");
                    content.AppendLine();

                    rows = dt.Select("contrato_id=" + contratoId);

                    //CABEÇALHO DO DETALHE
                    if (itensSecaoCabecalho != null)
                    {
                        content.Append("<cabecalho>");
                        content.AppendLine();
                        this.EscreveDetalheXML(itensSecaoCabecalho, ref content, rows[0]);
                        content.AppendLine();
                        content.Append("</cabecalho>");
                        content.AppendLine();
                    }

                    //DETALHE
                    content.Append("<detalhe>");
                    content.AppendLine();
                    foreach (DataRow row in rows)
                    {
                        this.EscreveDetalheXML(itensSecao, ref content, row);
                        content.AppendLine();

                        item = new ArqTransacionalLoteItem();
                        item.BeneficiarioID = row["beneficiario_id"];
                        item.BeneficiarioSequencia = Convert.ToInt32(row["contratobeneficiario_numeroSequencia"]);
                        item.ContratoID = row["contrato_id"];
                        item.Ativo = true;
                        lote.Itens.Add(item);
                    }
                    content.Append("</detalhe>");

                    content.AppendLine();
                    content.Append("</proposta>");
                    content.AppendLine();
                }

                content.Append("</corpo>");

                if (lote.Itens.Count > 0 && !exportacao)
                {
                    lote.Arquivo = fileName;
                    lote.Numeracao = 0;
                    lote.DataCriacao = DateTime.Now;
                    lote.DataVigencia = vigencia;
                    lote.Movimentacao = LayoutArquivoCustomizado.TraduzMovimentacao((eTipoTransacao)this._tipo);
                    lote.OperadoraID = this._operadoraId;
                    lote.Quantidade = dt.Rows.Count;
                    lote.TipoMovimentacao = LayoutArquivoCustomizado.TraduzTipoMovimentacao((eTipoTransacao)this._tipo);
                    lote.LayoutCustomizadoID = this._id;
                    lote.Exportacao = exportacao;
                    lote.Salvar(false, pm);
                }

                dt.Dispose();
                pm.Commit();
                return content.ToString();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                errMsg = ex.Message;
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        public static String TraduzMovimentacao(eTipoTransacao tipo)
        {
            switch (tipo)
            {
                case eTipoTransacao.Inclusao :
                case eTipoTransacao.AdicionaBeneficiario:
                {
                    return ArqTransacionalUnimed.Movimentacao.InclusaoBeneficiario;
                }
                case eTipoTransacao.AlteracaoCadastral:
                {
                    return ArqTransacionalUnimed.Movimentacao.AlteracaoBeneficiario;
                }
                case eTipoTransacao.MudancaPlano:
                {
                    return ArqTransacionalUnimed.Movimentacao.MudancaDePlano;
                }
                case eTipoTransacao.SegundaViaCartao:
                {
                    return ArqTransacionalUnimed.Movimentacao.SegundaViaCartaoBeneficiario;
                }
                case eTipoTransacao.CancelaContrato:
                {
                    return ArqTransacionalUnimed.Movimentacao.CancelamentoContrato;
                }
                case eTipoTransacao.CancelaBeneficiario:
                {
                    return ArqTransacionalUnimed.Movimentacao.ExclusaoBeneficiario;
                }
                default:
                {
                    return String.Empty;
                }
            }
        }
        public static String TraduzTipoMovimentacao(eTipoTransacao tipo)
        {
            switch (tipo)
            {
                case eTipoTransacao.Inclusao:
                case eTipoTransacao.AdicionaBeneficiario:
                {
                    return "I";
                }
                case eTipoTransacao.AlteracaoCadastral:
                case eTipoTransacao.MudancaPlano:
                {
                    return "A";
                }
                case eTipoTransacao.SegundaViaCartao:
                {
                    return "C";
                }
                case eTipoTransacao.CancelaContrato:
                case eTipoTransacao.CancelaBeneficiario:
                {
                    return "E";
                }
                default:
                {
                    return String.Empty;
                }
            }
        }

        static ContratoBeneficiario.eStatus traduzParaContratoBeneficiarioStatus(eTipoTransacao tipo)
        {
            switch (tipo)
            {
                case eTipoTransacao.Inclusao:
                case eTipoTransacao.AdicionaBeneficiario:
                {
                    return ContratoBeneficiario.eStatus.Novo;
                }
                case eTipoTransacao.AlteracaoCadastral:
                {
                    return ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
                }
                case eTipoTransacao.MudancaPlano:
                {
                    return ContratoBeneficiario.eStatus.MudancaPlanoPendente;
                }
                case eTipoTransacao.SegundaViaCartao:
                {
                    return ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
                }
                case eTipoTransacao.CancelaContrato:
                {
                    return ContratoBeneficiario.eStatus.CancelamentoPendente;
                }
                case eTipoTransacao.CancelaBeneficiario:
                {
                    return ContratoBeneficiario.eStatus.ExclusaoPendente;
                }
                case eTipoTransacao.SINCRONIZACAO_SEG:
                {
                    return ContratoBeneficiario.eStatus.Desconhecido;
                }
                default:
                {
                    throw new ArgumentException("Tipo de transação inválido.", "eTipoTransacao tipo");
                }
            }
        }
        static eBeneficiariosInclusos traduzParaTipoInclusaoBeneficiario(eTipoTransacao tipo)
        {
            switch (tipo)
            {
                case eTipoTransacao.Inclusao:
                case eTipoTransacao.AdicionaBeneficiario:
                {
                    return eBeneficiariosInclusos.Novos;
                }
                case eTipoTransacao.AlteracaoCadastral:
                {
                    return eBeneficiariosInclusos.Especifico;
                }
                case eTipoTransacao.MudancaPlano:
                {
                    return eBeneficiariosInclusos.ApenasTitular;
                }
                case eTipoTransacao.SegundaViaCartao:
                {
                    return eBeneficiariosInclusos.Especifico;
                }
                case eTipoTransacao.CancelaContrato:
                {
                    return eBeneficiariosInclusos.ApenasTitular;
                }
                case eTipoTransacao.CancelaBeneficiario:
                {
                    return eBeneficiariosInclusos.Especifico;
                }
                case eTipoTransacao.SINCRONIZACAO_SEG:
                {
                    return eBeneficiariosInclusos.Especifico;
                }
                default:
                {
                    throw new ArgumentException("Tipo de transação inválido.", "eTipoTransacao tipo");
                }
            }
        }
    }

    //TODO: pensar em 'formulas' para tratar campos especiais...
    [Serializable]
    [DBTable("itemLayoutArquivoCustomizado")]
    public class ItemLayoutArquivoCustomizado : EntityBase, IPersisteableEntity
    {
        #region enuns 

        public enum eSecao : int
        {
            Header,
            HeaderDetail,
            Trailler,
            Detail,
        }

        public enum eTipoValor : int
        {
            Alfanumerico,
            Numerico
        }

        public enum eTipoDado : int
        {
            SemEspecificacao,
            Monetario,
            Data
        }

        public enum eBehavior : int
        {
            FonteDeDados,
            Literal,
            QuebraDeLinha,
            Contador,
            DataCorrente
        }

        public enum eTipoPreenchimento : int
        {
            Direita,
            Esquerda
        }

        #endregion

        #region fields 

        Object _id;
        Object _layoutId;
        Int32 _secao;
        Int32 _tipoValor; //numerico ou alfa
        Int32 _tipoDado; //monetario, data, etc
        String _formato;
        Int32 _behavior; //literal ou fonte de dados
        Int32 _tipoPreenchimento; //a direita, ou a esquerda
        Int32 _tamanho;
        String _valor;
        String _valorRotulo;

        #endregion

        #region properties 

        [DBFieldInfo("ilac_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("ilac_layoutId", FieldType.Single)]
        public Object LayoutID
        {
            get { return _layoutId; }
            set { _layoutId= value; }
        }

        [DBFieldInfo("ilac_secao", FieldType.Single)]
        public Int32 Secao
        {
            get { return _secao; }
            set { _secao= value; }
        }

        /// <summary>
        /// Alfanumérico ou Numérico
        /// </summary>
        [DBFieldInfo("ilac_tipoValor", FieldType.Single)]
        public Int32 TipoValor
        {
            get { return _tipoValor; }
            set { _tipoValor= value; }
        }

        /// <summary>
        /// Se monetário, data, etc.
        /// </summary>
        [DBFieldInfo("ilac_tipoDado", FieldType.Single)]
        public Int32 TipoDado
        {
            get { return _tipoDado; }
            set { _tipoDado= value; }
        }

        [DBFieldInfo("ilac_formato", FieldType.Single)]
        public String Formato
        {
            get { return _formato; }
            set { _formato= value; }
        }

        /// <summary>
        /// Como se comporta o campo: literal ou oriundo de fonte de dados
        /// </summary>
        [DBFieldInfo("ilac_behavior", FieldType.Single)]
        public Int32 Behavior
        {
            get { return _behavior; }
            set { _behavior= value; }
        }

        [DBFieldInfo("ilac_tipoPreenchimento", FieldType.Single)]
        public Int32 TipoPreenchimento
        {
            get { return _tipoPreenchimento; }
            set { _tipoPreenchimento= value; }
        }

        /// <summary>
        /// Se 0, não há um tamanho a obdecer;
        /// </summary>
        [DBFieldInfo("ilac_tamanho", FieldType.Single)]
        public Int32 Tamanho
        {
            get { return _tamanho; }
            set { _tamanho= value; }
        }

        [DBFieldInfo("ilac_valor", FieldType.Single)]
        public String Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [DBFieldInfo("ilac_valorRotulo", FieldType.Single)]
        public String ValorRotulo
        {
            get { return _valorRotulo; }
            set { _valorRotulo= value; }
        }

        public String strSecao
        {
            get
            {
                if (((eSecao)_secao) == eSecao.HeaderDetail)
                    return "Cabeçalho do Detalhe";
                if (((eSecao)_secao) == eSecao.Detail)
                    return "Detalhe";
                else if (((eSecao)_secao) == eSecao.Header)
                    return "Cabeçalho";
                else
                    return "Rodapé";
            }
        }

        public String strTipoValor
        {
            get
            {
                if (((eBehavior)_behavior) == eBehavior.QuebraDeLinha)
                    return "------";

                if (((eTipoValor)_tipoValor) == eTipoValor.Alfanumerico)
                    return "Alfanumérico";
                else if (((eTipoValor)_tipoValor) == eTipoValor.Numerico)
                    return "Numérico";
                else
                    return "";
            }
        }

        public String Campo
        {
            get
            {
                if (((eBehavior)_behavior) == eBehavior.Literal)
                    return _valor;
                else if (((eBehavior)_behavior) == eBehavior.FonteDeDados)
                    return _valorRotulo;// String.Concat(_valorRotulo, " (", _valor, ")");
                else if (((eBehavior)_behavior) == eBehavior.QuebraDeLinha)
                    return "[Quebra de Linha]";
                else if (((eBehavior)_behavior) == eBehavior.Contador)
                    return "[Contador da Linha]";
                else
                    return "[Data Corrente]";
            }
        }

        #endregion

        public ItemLayoutArquivoCustomizado(Object id) : this() { _id = id; }
        public ItemLayoutArquivoCustomizado() { _tipoDado = 0; _tipoValor = 0; _behavior = 0; _tipoPreenchimento = 0; _tamanho = -1; }

        #region EntityBase methods 

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<ItemLayoutArquivoCustomizado> Carregar(Object arquivoLayoutID)
        {
            return Carregar(arquivoLayoutID, null);
        }
        public static IList<ItemLayoutArquivoCustomizado> Carregar(Object arquivoLayoutID, PersistenceManager pm)
        {
            String qry = "* FROM itemLayoutArquivoCustomizado WHERE ilac_layoutId=" + arquivoLayoutID;
            return LocatorHelper.Instance.ExecuteQuery<ItemLayoutArquivoCustomizado>(qry, typeof(ItemLayoutArquivoCustomizado), pm);
        }

        public static IList<ItemLayoutArquivoCustomizado> RetornarItensDaSecao(IList<ItemLayoutArquivoCustomizado> itens, eSecao secao)
        {
            if (itens == null) { return null; }
            List<ItemLayoutArquivoCustomizado> retornar = new List<ItemLayoutArquivoCustomizado>();

            foreach (ItemLayoutArquivoCustomizado item in itens)
            {
                if (item.Secao == Convert.ToInt32(secao))
                    retornar.Add(item);
            }

            return retornar;
        }
    }
}

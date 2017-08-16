namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    public enum SearchMatchType : int
    {
        QualquerParteDoCampo,
        InicioDoCampo,
        CampoInteiro
    }

    public enum eTipoPessoa
    {
        Fisica,
        Juridica,
        Juridica2 //Foi adicionado o tipo 2 pelo sistema do Clube
    }

    [DBTable("beneficiario")]
    public class Beneficiario : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _matrizid;
        String _nome;
        String _cpf;
        String _rg;
        String _rgUF;
        String _rgOrgaoExp;
        Object _estadoCivilId;
        DateTime _dataNascimento;
        DateTime _dataCasamento;
        String _fone;
        String _ramal1;
        String _fone2;
        String _ramal2;
        String _cel;
        String _celOperadora;
        String _email;
        String _nomeMae;
        String _sexo;
        Decimal _peso;
        Decimal _altura;
        String _declaracaoNascimentoVivo;
        String _cns;
        int _tipo;

        String _contratoNumero;
        int _tipoParticipacaoContrato;
        int _importId;
        Object _enriquecimentoId;

        #region propriedades 

        [DBFieldInfo("beneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("beneficiario_matrizId", FieldType.Single)]
        public Object MatrizID
        {
            get { return _matrizid; }
            set { _matrizid= value; }
        }

        [DBFieldInfo("beneficiario_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        [DBFieldInfo("beneficiario_cpf", FieldType.Single)]
        public String CPF
        {
            get { return _cpf; }
            set { _cpf= value; }
        }

        [DBFieldInfo("beneficiario_rg", FieldType.Single)]
        public String RG
        {
            get { return _rg; }
            set { _rg= value; }
        }

        [DBFieldInfo("beneficiario_rgUF", FieldType.Single)]
        public String RgUF
        {
            get { return _rgUF; }
            set { _rgUF= value; }
        }

        [DBFieldInfo("beneficiario_rgOrgaoExp", FieldType.Single)]
        public String RgOrgaoExp
        {
            get { return _rgOrgaoExp; }
            set { _rgOrgaoExp= value; }
        }

        //[DBFieldInfo("beneficiario_estadoCivilId", FieldType.Single)]
        //Object EstadoCivilID/////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _estadoCivilId; }
        //    set { _estadoCivilId= value; }
        //}

        [DBFieldInfo("beneficiario_dataNascimento", FieldType.Single)]
        public DateTime DataNascimento
        {
            get { return _dataNascimento; }
            set { _dataNascimento= value; }
        }

        //[DBFieldInfo("beneficiario_dataCasamento", FieldType.Single)]
        //DateTime DataCasamento/////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _dataCasamento; }
        //    set { _dataCasamento= value; }
        //}

        [DBFieldInfo("beneficiario_nomeMae", FieldType.Single)]
        public String NomeMae
        {
            get { return _nomeMae; }
            set { _nomeMae= value; }
        }

        [DBFieldInfo("beneficiario_telefone", FieldType.Single)]
        public String Telefone
        {
            get { return _fone; }
            set { _fone= value; }
        }

        public String FTelefone
        {
            get
            {
                return base.FormataTelefone(_fone);
            }
        }

        [DBFieldInfo("beneficiario_ramal", FieldType.Single)]
        public String Ramal
        {
            get { return _ramal1; }
            set { _ramal1= value; }
        }

        [DBFieldInfo("beneficiario_telefone2", FieldType.Single)]
        public String Telefone2
        {
            get { return _fone2; }
            set { _fone2= value; }
        }

        [DBFieldInfo("beneficiario_ramal2", FieldType.Single)]
        public String Ramal2
        {
            get { return _ramal2; }
            set { _ramal2= value; }
        }

        [DBFieldInfo("beneficiario_celular", FieldType.Single)]
        public String Celular
        {
            get { return _cel; }
            set { _cel= value; }
        }

        [DBFieldInfo("beneficiario_celularOperadora", FieldType.Single)]
        public String CelularOperadora
        {
            get { return _celOperadora; }
            set { _celOperadora= value; }
        }

        public String FCelular
        {
            get
            {
                return base.FormataTelefone(_cel);
            }
        }

        [DBFieldInfo("beneficiario_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        [DBFieldInfo("beneficiario_sexo", FieldType.Single)]
        public String Sexo
        {
            get { return _sexo; }
            set { _sexo= value; }
        }

        //[DBFieldInfo("beneficiario_peso", FieldType.Single)]
        //Decimal Peso//////////////////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _peso; }
        //    set { _peso= value; }
        //}

        //[DBFieldInfo("beneficiario_altura", FieldType.Single)]
        //Decimal Altura/////////////////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _altura; }
        //    set { _altura= value; }
        //}

        [DBFieldInfo("beneficiario_declaracaoNascimentoVivo", FieldType.Single)]
        public String DeclaracaoNascimentoVivo
        {
            get { return _declaracaoNascimentoVivo; }
            set { _declaracaoNascimentoVivo= value; }
        }

        [DBFieldInfo("beneficiario_cns", FieldType.Single)]
        public String CNS
        {
            get { return _cns; }
            set { _cns= value; }
        }

        /// <summary>
        /// Tipo de pessoa, Fisica ou Juridica. Enum Entity.eTipoPessoa
        /// </summary>
        [DBFieldInfo("beneficiario_tipo", FieldType.Single)]
        public int Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero= value; }
        }

        [Joinned("contratobeneficiario_tipo")]
        public int TipoParticipacaoContrato
        {
            get { return _tipoParticipacaoContrato; }
            set { _tipoParticipacaoContrato= value; }
        }

        [DBFieldInfo("importId", FieldType.Single)]
        public int ImportID
        {
            get { return _importId; }
            set { _importId= value; }
        }

        [Joinned("enriquecimentoId")]
        public Object EnriquecimentoID
        {
            get { return _enriquecimentoId; }
            set { _enriquecimentoId= value; }
        }


        #endregion

        #region métodos EntityBase

        public void Salvar()
        {
            Beneficiario.limpaCPF(ref _cpf);
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Carregar_DataNascimento(PersistenceManager pm)
        {
            String qry = "SELECT beneficiario_dataNascimento FROM beneficiario WHERE beneficiario_id=" + this._id;
            IList<Beneficiario> benef = LocatorHelper.Instance.ExecuteQuery<Beneficiario>(qry, typeof(Beneficiario), pm);

            if (benef == null)
                return;
            else
                this._dataNascimento =  benef[0].DataNascimento;
        }
        #endregion

        

        /// <summary>
        /// Método para sinalizar o Beneficiário em um contrato. (Mudança de Status)
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="Status">Status a ser sinalizado.</param>
        private void DisparaEventoParaGeracaoArquivo(Object ContratoID, ContratoBeneficiario.eStatus Status)
        {
            if (ContratoID != null)
            {
                try
                {
                    ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID, this._id, Status);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("ID de Contrato é nulo.");
        }

        private static String DateDiff(int interval, DateTime data)
        {
            return DateDiff(interval, data, DateTime.Now);
        }

        private static String DateDiff(int interval, DateTime data, DateTime dataReferencia)
        {
            String retorno = "";

            TimeSpan tsDuration;
            tsDuration = dataReferencia - data;

            Int32 dias = 0;
            Decimal iMeses = 0;
            Int32 meses = 0;
            Decimal iAnos = 0;
            Int32 anos = 0;

            if (interval == 1)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;
                iMeses = Convert.ToDecimal((iAnos - anos) * 12);
                meses = (int)iMeses;
                dias = (int)((iMeses - meses) * 24);

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = Convert.ToString(anos + "a " + meses + "m " + tsDurationDia.Days + "d");
            }
            else if (interval == 2)
            {
                //retorno = Convert.ToString(Convert.ToInt32(tsDuration.Days / 365));

                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;

                retorno = anos.ToString();
            }

            return retorno;
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Alteração de Cadastro.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaAlteracaoCadastro(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.AlteracaoCadastroPendente);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Exclusão de Cadastro.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaExclusaoCadastro(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.ExclusaoPendente);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Segunda Via de Cartão Magnético.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaSegundaViaCartao(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.SegundaViaCartaoPendente);
            }
            catch (Exception) { throw; }
        }

        public static int CalculaIdade(DateTime dataNascimento)
        {
            return Convert.ToInt32(DateDiff(2, dataNascimento));
        }

        public static int CalculaIdade(DateTime dataNascimento, DateTime dataReferencia)
        {
            int anos = Convert.ToInt32(DateDiff(2, dataNascimento, dataReferencia));
            return anos;
            //int anos = dataReferencia.Year - dataNascimento.Year;

            //if (dataReferencia.Month < dataNascimento.Month || (dataReferencia.Month == dataNascimento.Month && dataReferencia.Day < dataNascimento.Day))
            //    anos--;

            //return anos;
        }

        /// <summary>
        /// Verifica se matriz de uma filial é também a filial da mesma lhe tenta atribuir como matriz. 
        /// </summary>
        public static bool VerificaPossibilidadeDeMatriz(int matriz, int filial, int id)
        {
            string sql = string.Concat("select beneficiario_id from beneficiario where beneficiario_matrizId=", filial,
                " and beneficiario_id=", matriz);

            object ret = LocatorHelper.Instance.ExecuteScalar(sql, null, null);

            if (ret == null || ret == DBNull.Value)
                return true;
            else
                return false;
        }

        public Beneficiario() { _dataCasamento = DateTime.MinValue; _tipo = (int)eTipoPessoa.Fisica; }
        public Beneficiario(Object id) : this() { _id = id; }

        public static IList<Beneficiario> CarregarTodos()
        {
            return null;
        }

        static void limpaCPF(ref String cpf)
        {
            if (!String.IsNullOrEmpty(cpf))
            {
                cpf = cpf.Replace("_", ""); 
                cpf = cpf.Replace(".", ""); 
                cpf = cpf.Replace("-", "");
            }
        }

        public static IList<Beneficiario> CarregarPorParametro(String nome, String cpf, String rg, SearchMatchType smtype)
        {
            limpaCPF(ref cpf);

            StringBuilder query = new StringBuilder();
            query.Append("TOP 100 beneficiario.*, (select top 1 id_telMail from mailing where id_beneficiario=beneficiario.beneficiario_id and concluido=0) as enriquecimentoId FROM beneficiario ");

            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            String startChar = "";
            String finisChar = "";
            String operatorChar = "";

            if (smtype == SearchMatchType.InicioDoCampo || smtype == SearchMatchType.QualquerParteDoCampo)
            {
                finisChar = "%";
                operatorChar = " LIKE ";
            }
            if (smtype == SearchMatchType.QualquerParteDoCampo)
            {
                startChar = "%"; 
            }
            else if (smtype == SearchMatchType.CampoInteiro)
            {
                operatorChar = "=";
            }

            StringBuilder whereAnd = new StringBuilder();
            if (!String.IsNullOrEmpty(nome))
            {
                whereAnd.Append(" WHERE beneficiario_nome");
                whereAnd.Append(operatorChar);
                whereAnd.Append("@beneficiario_nome");

                parameterAndValues.Add("@beneficiario_nome", startChar + nome + finisChar);
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                if (whereAnd.Length > 0) { whereAnd.Append(" AND "); }
                else { whereAnd.Append(" WHERE "); }

                whereAnd.Append(" beneficiario_cpf = @beneficiario_cpf");
                parameterAndValues.Add("@beneficiario_cpf", cpf);
            }

            if (!String.IsNullOrEmpty(rg))
            {
                if (whereAnd.Length > 0) { whereAnd.Append(" AND "); }
                else { whereAnd.Append(" WHERE "); }

                whereAnd.Append(" beneficiario_rg = @beneficiario_rg");
                parameterAndValues.Add("@beneficiario_rg", rg);
            }

            query.Append(whereAnd.ToString());

            query.Append(" ORDER BY beneficiario_nome");

            if (parameterAndValues.Count > 0)
            {
                String[] _params = new String[parameterAndValues.Count];
                String[] _values = new String[parameterAndValues.Count];

                int i = 0;
                foreach (System.Collections.DictionaryEntry item in parameterAndValues)
                {
                    _params[i] = Convert.ToString(item.Key);
                    _values[i] = Convert.ToString(item.Value);
                    i++;
                }

                return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario));
            }
            else
            {
                return LocatorHelper.Instance.ExecuteQuery<Beneficiario>(query.ToString(), typeof(Beneficiario));
            }
        }

        /// <summary>
        /// Carrega um ou mais beneficiários com a mesma data de nascimento, nome, mas cpf DIFERENTE do informado
        /// </summary>
        public static IList<Beneficiario> CarregarPorParametro(DateTime nascimento, String nome, String cpf)
        {
            String qry = "* FROM beneficiario WHERE beneficiario_cpf = @cpf AND beneficiario_nome=@nome";

            if (nascimento != DateTime.MinValue)
            {
                qry = String.Concat(qry, " AND CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='", nascimento.ToString("dd/MM/yyyy"), "'");
            }

            String[] _params = new String[2] { "@cpf", "@nome" };
            String[] _values = new String[2] { cpf, nome };

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(qry, _params, _values, typeof(Beneficiario));
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento)
        {
            return CarregarPorParametro(cpf, nascimento, null);
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("* FROM beneficiario ");

            query.Append(" WHERE beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento != DateTime.MinValue)
            {
                query.Append(" AND  CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='");
                query.Append(nascimento.ToString("dd/MM/yyyy"));
                query.Append("'");
            }

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario), pm);
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento, String nomeMae, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("* FROM beneficiario ");

            query.Append(" WHERE beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento != DateTime.MinValue)
            {
                query.Append(" AND  CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='");
                query.Append(nascimento.ToString("dd/MM/yyyy"));
                query.Append("'");
            }

            query.Append(" AND beneficiario_nomeMae=@beneficiario_nomeMae ");
            parameterAndValues.Add("@beneficiario_nomeMae", nomeMae);

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario), pm);
        }

        public static Object CarregarPorParametro(String nome, String nomeMae, PersistenceManager pm, DateTime nascimento, String cpf)
        {
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("SELECT beneficiario_id FROM beneficiario ");

            query.Append(" WHERE beneficiario_nome=@beneficiario_nome ");
            parameterAndValues.Add("@beneficiario_nome", nome);

            query.Append(" AND beneficiario_nomeMae=@beneficiario_nomeMae ");
            parameterAndValues.Add("@beneficiario_nomeMae", nomeMae);

            query.Append(" AND beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento.Year > 1753)
            {
                query.Append(" AND CONVERT(varchar(20), beneficiario_dataNascimento, 103)=@beneficiario_dataNascimento ");
                parameterAndValues.Add("@beneficiario_dataNascimento", nascimento.ToString("dd/MM/yyyy"));
            }

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteScalar(query.ToString(), _params, _values, pm);
        }

        public static Object CarregarPorParametro(Object importId, PersistenceManager pm)
        {
            return LocatorHelper.Instance.ExecuteScalar("SELECT beneficiario_id FROM beneficiario WHERE importId=" + importId, null, null, pm);
        }

        public static IList<Beneficiario> CarregarPorContratoId(Object contratoId)
        {
            return null;
        }

        public static IList<Beneficiario> CarregarPorOperadoraId(Object operadoraId, String nome, String cpf, String rg)
        {
            limpaCPF(ref cpf);

            StringBuilder query = new StringBuilder();
            query.Append("beneficiario.*, contrato_numero, contratobeneficiario_tipo FROM beneficiario INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId INNER JOIN plano ON plano_id = contrato_planoId INNER JOIN contratoadm ON plano_contratoId = contratoadm_id WHERE contratoadm_operadoraid= ");
            query.Append(operadoraId);

            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();
            StringBuilder whereAnd = new StringBuilder();

            if (!String.IsNullOrEmpty(nome))
            {
                query.Append(" AND beneficiario_nome LIKE @beneficiario_nome");
                parameterAndValues.Add("@beneficiario_nome", "%" + nome + "%");
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                query.Append(" AND beneficiario_cpf = @beneficiario_cpf");
                parameterAndValues.Add("@beneficiario_cpf", cpf);
            }

            if (!String.IsNullOrEmpty(rg))
            {
                query.Append(" AND beneficiario_rg = @beneficiario_rg");
                parameterAndValues.Add("@beneficiario_rg", rg);
            }

            query.Append(" ORDER BY beneficiario_nome, contrato_numero");

            if (parameterAndValues.Count > 0)
            {
                String[] _params = new String[parameterAndValues.Count];
                String[] _values = new String[parameterAndValues.Count];

                int i = 0;
                foreach (System.Collections.DictionaryEntry item in parameterAndValues)
                {
                    _params[i] = Convert.ToString(item.Key);
                    _values[i] = Convert.ToString(item.Value);
                    i++;
                }

                return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario));
            }
            else
            {
                return LocatorHelper.Instance.ExecuteQuery<Beneficiario>(query.ToString(), typeof(Beneficiario));
            }
        }

        /// <summary>
        /// Checa se o cpf em questão está em uso. També faz a checagem quanto a validade do 
        /// cpf informado.
        /// </summary>
        public static Boolean ChecaCpf(Object beneficiarioId, String cpf)
        {
            limpaCPF(ref cpf);
            return ValidaCpf(cpf);
            //if (!ValidaCpf(cpf)) { return false; } else { return true; }

            //String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";
            //if (beneficiarioId != null)
            //    query += " AND beneficiario_id <> " + beneficiarioId;

            //System.Data.DataTable dt = LocatorHelper.Instance.
            //    ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            //return dt == null || dt.Rows.Count == 0;
        }

        public static Boolean ChecaCpfEmUso(Object beneficiarioId, String cpf)
        {
            limpaCPF(ref cpf);
            if (!ValidaCpf(cpf)) { return false; }

            if (cpf == "99999999999") { return false; }

            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";
            if (beneficiarioId != null)
                query += " AND beneficiario_id <> " + beneficiarioId;

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            return dt == null || dt.Rows.Count == 0;
        }

        public static bool ValidaCpf(String vrCPF)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"].ToUpper() == "Y")
            {
                return true;
            }

            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");
            valor = valor.Replace("_", "");

            if (valor.Length != 11)
                return false;

            if (valor == "99999999999") { return false; }

            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                  valor[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                    return false;

            return true;
        }

        public static Object VerificaExistenciaCPF(String cpf)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        /// <summary>
        /// Verifica a existência do Beneficiário por CPF e Data de Nascimento.
        /// </summary>
        /// <param name="cpf">CPF.</param>
        /// <param name="dataNascimento">Data de Nascimento.</param>
        /// <returns>Retorna o ID do Beneficiário.</returns>
        public static Object VerificaExistenciaCPF(String cpf, DateTime dataNascimento)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF AND beneficiario_dataNascimento = @dataNascimento";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF", "@dataNascimento" }, new String[] { cpf, dataNascimento.ToString("yyyy-MM-dd")}).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        public static Object VerificaExistenciaCPF(String cpf, DateTime dataNascimento, String nomeMae, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF AND beneficiario_dataNascimento = @dataNascimento AND beneficiario_nomeMae=@nomeMae";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF", "@dataNascimento", "@nomeMae" }, new String[] { cpf, dataNascimento.ToString("yyyy-MM-dd"), nomeMae }, pm).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        public Endereco Endereco
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Importar os Beneficiários de um MDB. A versão tem que ser 2003. No caso de Windows 7 colocar o 
        /// MDB em uma pasta que tenha permissão, não utilize o Desktop.
        /// </summary>
        /// <param name="MDBPath">Caminho do MDB.</param>
        public static void Importar(String MDBPath)
        {
            #region Table Header

            const String NomeColumn = "NOME";
            const String SexoColumn = "SEXO";
            const String CPFColumn = "CPF";
            const String RGColumn = "RG";
            const String DataNascimentoColumn = "NASCIMENTO";
            const String EmailColumn = "EMAIL";
            const String NomeMaeColumn = "NOMEMAE";
            const String DDD1Column = "DDD1";
            const String Telefone1Column = "TEL1";
            const String Ramal1Column = "RAMAL1";
            const String DDD2Column = "DDD2";
            const String Telefone2Column = "TEL2";
            const String Ramal2Column = "RAMAL2";
            const String CelDDDColumn = "CEL_DDD";
            const String CelColumn = "CEL";
            const String CelOperadoraColumn = "CEL_OPERADORA";
            const String TipoLogr1Column = "TIPO_LOGR1";
            const String Logr1Column = "LOGRADOURO1";
            const String NumLogr1Column = "NUM_LOGR1";
            const String ComplLogr1Column = "COMPL_LOGR1";
            const String Bairro1Column = "BAIRRO1";
            const String Cidade1Column = "CIDADE1";
            const String UF1Column = "UF1";
            const String CEP1Column = "CEP1";
            const String TipoEnd1Column = "TIPO_END1";
            const String TipoLogr2Column = "TIPO_LOGR2";
            const String Logr2Column = "LOGRADOURO2";
            const String NumLogr2Column = "NUM_LOGR2";
            const String ComplLogr2Column = "COMPL_LOGR2";
            const String Bairro2Column = "BAIRRO2";
            const String Cidade2Column = "CIDADE2";
            const String UF2Column = "UF2";
            const String CEP2Column = "CEP2";
            const String TipoEnd2Column = "TIPO_END2";

            #endregion

            String mdbFilePath      = MDBPath;
            String connectionString = String.Concat("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", mdbFilePath, ";User Id=Admin;");

            OleDbConnection connectionMDB = new OleDbConnection(connectionString);

            try
            {
                connectionMDB.Open();
            }
            catch (Exception) { throw; }

            OleDbCommand cmdMDB = connectionMDB.CreateCommand();

            cmdMDB.CommandType = CommandType.Text;
            cmdMDB.CommandText = "SELECT * FROM beneficiarios";

            OleDbDataReader drBeneficiario = cmdMDB.ExecuteReader();

            #region BeneficiarioImportVars

            String beneficiarioNome;
            String beneficiarioSexo;
            String beneficiarioCPF;
            String beneficiarioRG;
            String beneficiarioDataNascimento;
            String beneficiarioEmail;
            String beneficiarioNomeMae;
            String beneficiarioDDD1;
            String beneficiarioTelefone1;
            String beneficiarioRamal1;
            String beneficiarioDDD2;
            String beneficiarioTelefone2;
            String beneficiarioRamal2;
            String beneficiarioCelDDD;
            String beneficiarioCel;
            String beneficiarioCelOperadora;
            String beneficiarioTipoLogr1;
            String beneficiarioLogr1;
            String beneficiarioNumLogr1;
            String beneficiarioComplLogr1;
            String beneficiarioBairro1;
            String beneficiarioCidade1;
            String beneficiarioUF1;
            String beneficiarioCEP1;
            String beneficiarioTipoEnd1;
            String beneficiarioTipoLogr2;
            String beneficiarioLogr2;
            String beneficiarioNumLogr2;
            String beneficiarioComplLogr2;
            String beneficiarioBairro2;
            String beneficiarioCidade2;
            String beneficiarioUF2;
            String beneficiarioCEP2;
            String beneficiarioTipoEnd2;
            Beneficiario beneficiario = null;
            Endereco beneficiarioEndereco1 = null;
            Endereco beneficiarioEndereco2 = null;

            #endregion

            Int32 i = 0;

            PersistenceManager PMTransaction = new PersistenceManager();
            PMTransaction.BeginTransactionContext();

            while (drBeneficiario.HasRows && drBeneficiario.Read())
            {
                beneficiario                = new Beneficiario();
                beneficiarioEndereco1       = new Endereco();
                beneficiarioEndereco2       = new Endereco();

                beneficiarioNome           = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)).ToString() : null;
                beneficiarioSexo           = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)).ToString() : null;
                beneficiarioCPF            = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)).ToString() : null;
                beneficiarioRG             = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)).ToString() : null;
                beneficiarioDataNascimento = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)).ToString() : null;

                try
                {
                    beneficiarioEmail = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioEmail = String.Empty; }

                try
                {
                    beneficiarioNomeMae = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioNomeMae = String.Empty; }

                beneficiarioDDD1         = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)).ToString() : null;
                beneficiarioTelefone1    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)).ToString() : null;
                beneficiarioRamal1       = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)).ToString() : null;
                beneficiarioDDD2         = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)).ToString() : null;
                beneficiarioTelefone2    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)).ToString() : null;
                beneficiarioRamal2       = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)).ToString() : null;
                beneficiarioCelDDD       = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)).ToString() : null;
                beneficiarioCel          = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)).ToString() : null;
                beneficiarioCelOperadora = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)).ToString() : null;

                beneficiarioTipoLogr1  = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)).ToString() : null;
                beneficiarioLogr1      = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)).ToString() : null;
                beneficiarioNumLogr1   = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)).ToString() : null;
                beneficiarioComplLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)).ToString() : null;
                beneficiarioBairro1    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)).ToString() : null;
                beneficiarioCidade1    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)).ToString() : null;
                beneficiarioUF1        = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)).ToString() : null;
                beneficiarioCEP1       = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)).ToString() : null;
                beneficiarioTipoEnd1   = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)).ToString() : null;

                beneficiarioTipoLogr2  = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)).ToString() : null;
                beneficiarioLogr2      = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)).ToString() : null;
                beneficiarioNumLogr2   = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)).ToString() : null;
                beneficiarioComplLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)).ToString() : null;
                beneficiarioBairro2    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)).ToString() : null;
                beneficiarioCidade2    = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)).ToString() : null;
                beneficiarioUF2        = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)).ToString() : null;
                beneficiarioCEP2       = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)).ToString() : null;
                beneficiarioTipoEnd2   = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)).ToString() : null;

                beneficiario.ID = Beneficiario.VerificaExistenciaCPF(beneficiarioCPF, Convert.ToDateTime(beneficiarioDataNascimento));

                beneficiario.Nome           = beneficiarioNome;
                beneficiario.CPF            = beneficiarioCPF;
                beneficiario.Sexo           = (!String.IsNullOrEmpty(beneficiarioSexo)) ? (beneficiarioSexo.Equals("M")) ? "1" : "2" : null;
                beneficiario.RG             = beneficiarioRG;
                beneficiario.DataNascimento = Convert.ToDateTime(beneficiarioDataNascimento);
                beneficiario.Email          = beneficiarioEmail;
                beneficiario.NomeMae        = beneficiarioNomeMae;

                if (!String.IsNullOrEmpty(beneficiarioDDD1) && !String.IsNullOrEmpty(beneficiarioTelefone1))
                {
                    beneficiario.Telefone = String.Concat("(", Convert.ToInt32(beneficiarioDDD1).ToString(), ") ", beneficiarioTelefone1);
                    beneficiario.Ramal    = beneficiarioRamal1;
                }

                if (!String.IsNullOrEmpty(beneficiarioDDD2) && !String.IsNullOrEmpty(beneficiarioTelefone2))
                {
                    beneficiario.Telefone2 = String.Concat("(", Convert.ToInt32(beneficiarioDDD2).ToString(), ") ", beneficiarioTelefone2);
                    beneficiario.Ramal2    = beneficiarioRamal2;
                }

                if (!String.IsNullOrEmpty(beneficiarioCelDDD) && !String.IsNullOrEmpty(beneficiarioCel) && !String.IsNullOrEmpty(beneficiarioCelOperadora))
                {
                    beneficiario.Celular          = String.Concat("(", Convert.ToInt32(beneficiarioCelDDD).ToString(), ") ", beneficiarioCel);
                    beneficiario.CelularOperadora = beneficiarioCelOperadora;
                }

                try
                {
                    PMTransaction.Save(beneficiario);
                }
                catch (Exception)
                {
                    PMTransaction.Rollback();
                    throw;
                }

                beneficiarioEndereco1.DonoId      = beneficiario.ID;
                beneficiarioEndereco1.DonoTipo    = (int)Endereco.TipoDono.Beneficiario;
                beneficiarioEndereco1.Logradouro  = String.Concat(beneficiarioTipoLogr1.Replace(":", String.Empty), " ", beneficiarioLogr1);
                beneficiarioEndereco1.Numero      = beneficiarioNumLogr1;
                beneficiarioEndereco1.Complemento = beneficiarioComplLogr1;
                beneficiarioEndereco1.Bairro      = beneficiarioBairro1;
                beneficiarioEndereco1.Cidade      = beneficiarioCidade1;
                beneficiarioEndereco1.UF          = beneficiarioUF1;
                beneficiarioEndereco1.CEP         = beneficiarioCEP1;
                beneficiarioEndereco1.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd1)) ? (beneficiarioTipoEnd1.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                try
                {
                    beneficiarioEndereco1.Importar(PMTransaction);
                }
                catch (Exception)
                {
                    PMTransaction.Rollback();
                    throw;
                }

                if (!String.IsNullOrEmpty(beneficiarioLogr2))
                {
                    beneficiarioEndereco2.DonoId      = beneficiario.ID;
                    beneficiarioEndereco2.DonoTipo    = (int)Endereco.TipoDono.Beneficiario;
                    beneficiarioEndereco2.Logradouro  = String.Concat(beneficiarioTipoLogr2.Replace(":", String.Empty), " ", beneficiarioLogr2);
                    beneficiarioEndereco2.Numero      = beneficiarioNumLogr2;
                    beneficiarioEndereco2.Complemento = beneficiarioComplLogr2;
                    beneficiarioEndereco2.Bairro      = beneficiarioBairro2;
                    beneficiarioEndereco2.Cidade      = beneficiarioCidade2;
                    beneficiarioEndereco2.UF          = beneficiarioUF2;
                    beneficiarioEndereco2.CEP         = beneficiarioCEP2;
                    beneficiarioEndereco2.Tipo        = (!String.IsNullOrEmpty(beneficiarioTipoEnd2)) ? (beneficiarioTipoEnd2.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                    try
                    {
                        beneficiarioEndereco2.Importar(PMTransaction);
                    }
                    catch (Exception)
                    {
                        PMTransaction.Rollback();
                        throw;
                    }
                }

                i++;

                PMTransaction.Commit();
            }

            PMTransaction.Dispose();
            PMTransaction = null;

            drBeneficiario.Close();
            drBeneficiario.Dispose();
            drBeneficiario = null;

            cmdMDB.Dispose();
            cmdMDB = null;

            connectionMDB.Close();
            connectionMDB.Dispose();
            connectionMDB = null;
        }
    }
}
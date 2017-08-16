namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("endereco")]
    public class Endereco : EntityBase, IPersisteableEntity
    {
        public enum TipoDono : int
        {
            Beneficiario,
            CorretorOuSupervisor,
            Operadora,
            Filial,
            Produtor
        }

        public enum TipoEndereco : int
        {
            Residencial,
            Comercial
        }

        Object _id;
        Object _donoId;
        Int32 _donoTipo;
        String _logradouro;
        String _numero;
        String _complemento;
        String _bairro;
        String _cidade;
        String _uf;
        String _cep;
        Int32 _tipo;

        #region Propriedades 

        [DBFieldInfo("endereco_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("endereco_donoId", FieldType.Single)]
        public Object DonoId
        {
            get { return _donoId; }
            set { _donoId= value; }
        }

        [DBFieldInfo("endereco_donoTipo", FieldType.Single)]
        public Int32 DonoTipo
        {
            get { return _donoTipo; }
            set { _donoTipo= value; }
        }

        [DBFieldInfo("endereco_logradouro", FieldType.Single)]
        public String Logradouro
        {
            get { return _logradouro; }
            set { _logradouro= value; }
        }

        [DBFieldInfo("endereco_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero= value; }
        }

        [DBFieldInfo("endereco_complemento", FieldType.Single)]
        public String Complemento
        {
            get { return _complemento; }
            set { _complemento= value; }
        }

        [DBFieldInfo("endereco_bairro", FieldType.Single)]
        public String Bairro
        {
            get { return _bairro; }
            set { _bairro= value; }
        }

        [DBFieldInfo("endereco_cidade", FieldType.Single)]
        public String Cidade
        {
            get { return _cidade; }
            set { _cidade= value; }
        }

        [DBFieldInfo("endereco_uf", FieldType.Single)]
        public String UF
        {
            get { return _uf; }
            set { _uf= value; }
        }

        [DBFieldInfo("endereco_cep", FieldType.Single)]
        public String CEP
        {
            get { return _cep; }
            set { _cep= value; }
        }

        [DBFieldInfo("endereco_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        #endregion

        public Endereco(Object id) : this() { _id = id; }
        public Endereco() { _tipo = (Int32)TipoEndereco.Residencial; }

        public void Salvar()
        {
            if (_cep != null) { _cep = _cep.Replace("-", ""); }
            base.Salvar(this);
        }

        /// <summary>
        /// Importar Endereço.
        /// </summary>
        public void Importar()
        {
            this.Importar(null);
        }

        /// <summary>
        /// Importar Endereço. Se duplicado por Completo não Importa.
        /// </summary>
        /// <param name="PM">Instância do Persistence Manager.</param>
        public void Importar(PersistenceManager PM)
        {
            if (PM == null) PM = new PersistenceManager();

            #region Parameters

            String[] Params = new String[10];
            String[] Values = new String[10];

            Params[0] = "@donoId";
            Params[1] = "@donoTipo";
            Params[2] = "@logradouro";
            Params[3] = "@numero";
            Params[4] = "@complemento";
            Params[5] = "@bairro";
            Params[6] = "@cidade";
            Params[7] = "@uf";
            Params[8] = "@cep";
            Params[9] = "@tipo";

            Values[0] = (this._donoId != null && this._donoId.ToString().Length > 0) ? this._donoId.ToString() : String.Empty;
            Values[1] = (this._donoTipo > -1) ? this._donoTipo.ToString() : "0";
            Values[2] = (!String.IsNullOrEmpty(this._logradouro)) ? this._logradouro : String.Empty;
            Values[3] = (!String.IsNullOrEmpty(this._numero)) ? this._numero : String.Empty;
            Values[4] = (!String.IsNullOrEmpty(this._complemento)) ? this._complemento : String.Empty;
            Values[5] = (!String.IsNullOrEmpty(this._bairro)) ? this._bairro : String.Empty;
            Values[6] = (!String.IsNullOrEmpty(this._cidade)) ? this._cidade : String.Empty;
            Values[7] = (!String.IsNullOrEmpty(this._uf)) ? this._uf : String.Empty;
            Values[8] = (!String.IsNullOrEmpty(this._cep)) ? this._cep : String.Empty;
            Values[9] = (this._tipo > -1) ? this._tipo.ToString() : "0"; 

            #endregion

            String strSQL = String.Concat("SELECT ",
                                          "      endereco_id ", 
                                          "FROM endereco ",
                                          "WHERE endereco_donoId = @donoId AND endereco_donoTipo = @donoTipo AND endereco_logradouro = @logradouro AND endereco_numero = @numero AND ",
                                          "      endereco_complemento = @complemento AND endereco_bairro = @bairro AND endereco_cidade = @cidade AND ",
                                          "      endereco_uf = @uf AND endereco_cep = @cep AND endereco_tipo = @tipo");
            try
            {
                IList<Endereco> lstEndereco = LocatorHelper.Instance.ExecuteParametrizedQuery<Endereco>(strSQL, Params, Values, typeof(Endereco), PM);

                if (lstEndereco == null || lstEndereco.Count == 0)
                    PM.Save(this);
            }
            catch (Exception) { throw; }
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Carregar(PersistenceManager pm)
        {
            pm.Load(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public static IList<Endereco> CarregarPorDono(Object donoID, Endereco.TipoDono tipoDono)
        {
            return CarregarPorDono(donoID, tipoDono, null);
        }

        public static IList<Endereco> CarregarPorDono(Object donoID, Endereco.TipoDono tipoDono, PersistenceManager pm)
        {
            String query = "* FROM endereco WHERE endereco_donoid=" + donoID + " AND endereco_donotipo=" + Convert.ToInt32(tipoDono);

            IList<Endereco> lista = LocatorHelper.Instance.ExecuteQuery<Endereco>(query, typeof(Endereco), pm);

            return lista;
        }

        public static IList<Endereco> Carregar(ArrayList donoIDs)
        {
            String query = "* FROM endereco WHERE endereco_id IN (";

            String inClausule = "";

            foreach (Object id in donoIDs)
            {
                if (inClausule.Length > 0) { inClausule += ","; }
                inClausule += id;
            }
            
            query += inClausule + ")";

            IList<Endereco> lista = LocatorHelper.Instance.ExecuteQuery<Endereco>(query, typeof(Endereco));

            return lista;
        }
    }
}

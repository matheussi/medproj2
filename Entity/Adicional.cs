namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa um produto adicional passivo de adesão por beneficiários.
    /// </summary>
    [Serializable]
    [DBTable("adicional")]
    public class Adicional : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _operadoraId;
        String _descricao;
        String _codTitular;
        Boolean _ativo;
        Decimal _valorUnico;
        Boolean _paraTodaProposta;
        DateTime _data;
        Boolean _dental;

        #endregion

        #region propriedades 

        [DBFieldInfo("adicional_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("adicional_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("adicional_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("adicional_codTitular", FieldType.Single)]
        public String CodTitular
        {
            get { return _codTitular; }
            set { _codTitular= value; }
        }

        [DBFieldInfo("adicional_paraTodaProposta", FieldType.Single)]
        public Boolean ParaTodaProposta
        {
            get { return _paraTodaProposta; }
            set { _paraTodaProposta= value; }
        }

        [DBFieldInfo("adicional_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("adicional_dental", FieldType.Single)]
        public Boolean Dental
        {
            get { return _dental; }
            set { _dental= value; }
        }

        public AdicionalFaixa AdicionalFaixa
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public AdicionalRegra AdicionalRegra
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        //[DBFieldInfo("adicional_valorUnico", FieldType.Single)]
        //public Decimal ValorUnico
        //{
        //    get { return _valorUnico; }
        //    set { _valorUnico= value; }
        //}

        //[DBFieldInfo("adicional_data", FieldType.Single)]
        //public DateTime Data
        //{
        //    get { return _data; }
        //    set { _data= value; }
        //}

        #endregion

        public Adicional() { _ativo = true; _data = DateTime.Now; }
        public Adicional(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<Adicional> CarregarPorOperadoraID(Object operadoraId)
        {
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_operadoraId=" + operadoraId + " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional));
        }

        public static Adicional CarregarPorOperadoraID(Object operadoraId, String adicionalDescricao, PersistenceManager pm)
        {
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_operadoraId=" + operadoraId + " AND adicional_descricao='" + adicionalDescricao + "'";
            IList<Adicional> lista = LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<Adicional> Carregar(Object[] ids)
        {
            String inClausule = String.Join(",", (String[])ids);
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_id IN (" + inClausule + ") ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional));
        }

        public static Object CarregarIDPorCodigoTitular(String codigoTitular, Object operadoraId, PersistenceManager pm)
        {
            String query = "SELECT adicional_id FROM adicional WHERE adicional_codTitular='" + codigoTitular + "' AND adicional_operadoraID=" + operadoraId;
            return LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);
        }

        public static Decimal CalculaValor(Object adicionalId, Object beneficiarioId, Int32 beneficiarioIdade)
        {
            return CalculaValor(adicionalId, beneficiarioId, beneficiarioIdade, null, null);
        }
        public static Decimal CalculaValor(Object adicionalId, Object beneficiarioId, Int32 beneficiarioIdade, DateTime? dataReferencia, PersistenceManager pm)
        {
            Adicional adicional = new Adicional();
            adicional.ID = adicionalId;
            adicional.Carregar();

            if (adicional.ID == null) { return -1; }

            IList<AdicionalFaixa> faixa = AdicionalFaixa.CarregarPorTabela(adicionalId, dataReferencia, pm);

            if (faixa != null && faixa.Count > 0)
            {
                if (beneficiarioIdade == -1)
                {
                    Beneficiario beneficiario = new Beneficiario();
                    beneficiario.ID = beneficiarioId;
                    if (pm == null)
                        beneficiario.Carregar();
                    else
                        pm.Load(beneficiario);
                    if (beneficiario.ID == null) { return -1; }
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.DataNascimento);
                }

                foreach (AdicionalFaixa _item in faixa)
                {
                    if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                    {
                        return _item.Valor;
                    }
                    else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                    {
                        return _item.Valor;
                    }
                }

                return 0;
            }
            else
                return 0;
        }
    }

    [Serializable]
    [DBTable("adicional_faixa")]
    public class AdicionalFaixa : EntityBase, IPersisteableEntity
    {
        Object   _id;
        Object   _adicionalId;
        int      _idadeInicio;
        int      _idadeFim;
        Decimal  _valor;
        DateTime _vigencia;

        #region propriedades 

        [DBFieldInfo("adicionalfaixa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("adicionalfaixa_adicionalid", FieldType.Single)]
        public Object AdicionalId
        {
            get { return _adicionalId; }
            set { _adicionalId= value; }
        }

        [DBFieldInfo("adicionalfaixa_idadeInicio", FieldType.Single)]
        public int IdadeInicio
        {
            get { return _idadeInicio; }
            set { _idadeInicio= value; }
        }

        [DBFieldInfo("adicionalfaixa_idadeFim", FieldType.Single)]
        public int IdadeFim
        {
            get { return _idadeFim; }
            set { _idadeFim= value; }
        }

        [DBFieldInfo("adicionalfaixa_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [DBFieldInfo("adicionalfaixa_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia= value; }
        }

        public String strVigencia
        {
            get 
            {
                if (_vigencia == DateTime.MinValue)
                    return "";
                else
                    return _vigencia.ToString("dd/MM/yyyy");
            }
        }

        public String FaixaEtaria
        {
            get
            {
                String ret = "";

                if (_idadeFim > 0)
                    ret = String.Concat("de ", _idadeInicio, " a ", _idadeFim);
                else
                    ret = String.Concat("de ", _idadeInicio, " em diante");

                return ret;
            }
        }

        #endregion

        public AdicionalFaixa() { }

        #region métodos EntityBase 

        public void Salvar()
        {
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
        #endregion

        public static IList<AdicionalFaixa> CarregarPorTabela(Object adicionalId, DateTime? dataReferencia)
        {
            return CarregarPorTabela(adicionalId, null, null);
        }
        public static IList<AdicionalFaixa> CarregarPorTabela(Object adicionalId, DateTime? vigencia, PersistenceManager pm)
        {
            String query = "* FROM adicional_faixa WHERE adicionalfaixa_adicionalid=" + adicionalId;

            if (vigencia != null)
            {
                query += " and adicionalfaixa_vigencia <= '" + vigencia.Value.ToString("yyyy-MM-dd") + "'";
            }
            
            query += " ORDER BY adicionalfaixa_vigencia DESC, adicionalfaixa_idadeInicio"; //and adicionalfaixa_vigencia = (select max(adicionalfaixa_vigencia) FROM adicional_faixa WHERE adicionalfaixa_adicionalid=2)

            if (vigencia == null)
                return LocatorHelper.Instance.ExecuteQuery<AdicionalFaixa>(query, typeof(AdicionalFaixa), pm);
            else
            {
                IList<AdicionalFaixa> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalFaixa>(query, typeof(AdicionalFaixa), pm);

                if (lista == null) { return null; }

                DateTime vigenciaRetornada = lista[0]._vigencia;

                List<AdicionalFaixa> ret = new List<AdicionalFaixa>();
                foreach (AdicionalFaixa af in lista)
                {
                    if (af._vigencia == vigenciaRetornada) { ret.Add(af); }
                }

                return ret;
            }
        }
    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("plano")]
    public class Plano : EntityBase, IPersisteableEntity
    {
        public Plano() { _ativo = true; }
        public Plano(Object id) : this() { _id = id; }

        #region Campos 

        Object _id;
        Object _contratoId;
        Object _tabelaValorAtualId;
        String _descricao;
        String _codigo;
        String _subplano;
        DateTime _inicioColetivo;
        String _codigoParticular;
        String _subplanoParticular;
        DateTime _inicioParticular;
        String _caracteristicas;
        Boolean _ativo;
        Boolean _quartoParticular;
        Boolean _quartoComum;
        String _ansComum;
        String _ansParticular;

        #endregion

        #region Propriedades 

        [DBFieldInfo("plano_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("plano_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("plano_tabelaValorAtualId", FieldType.Single)]
        public Object TabelaValoreAtualID
        {
            get { return _tabelaValorAtualId; }
            set { _tabelaValorAtualId= value; }
        }

        [DBFieldInfo("plano_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        public String DescricaoPlanoSubPlano
        {
            get
            {
                String ret = "";

                if (!String.IsNullOrEmpty(_codigo)) { ret += "Cod " + _codigo; }
                if (!String.IsNullOrEmpty(_subplano))
                {
                    if (ret.Length > 0) { ret += " "; }
                    ret += "Sub " + _subplano;
                }

                if (!String.IsNullOrEmpty(_codigoParticular))
                {
                    if (ret.Length > 0) { ret += " - "; }
                    ret += "Cod " + _codigoParticular;
                }

                if (!String.IsNullOrEmpty(_subplanoParticular))
                {
                    if (ret.Length > 0) { ret += " "; }
                    ret += "Sub " + _subplanoParticular;
                }

                return String.Concat(_descricao, " (", ret, ")");
            }
        }

        [DBFieldInfo("plano_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo= value; }
        }

        [DBFieldInfo("plano_subplano", FieldType.Single)]
        public String SubPlano
        {
            get { return _subplano; }
            set { _subplano= value; }
        }

        [DBFieldInfo("plano_inicioColetivo", FieldType.Single)]
        public DateTime InicioColetivo
        {
            get { return _inicioColetivo; }
            set { _inicioColetivo= value; }
        }

        [DBFieldInfo("plano_codigoParticular", FieldType.Single)]
        public String CodigoParticular
        {
            get { return _codigoParticular; }
            set { _codigoParticular= value; }
        }

        [DBFieldInfo("plano_subplanoParticular", FieldType.Single)]
        public String SubPlanoParticular
        {
            get { return _subplanoParticular; }
            set { _subplanoParticular= value; }
        }

        [DBFieldInfo("plano_inicioParticular", FieldType.Single)]
        public DateTime InicioParticular
        {
            get { return _inicioParticular; }
            set { _inicioParticular= value; }
        }

        [DBFieldInfo("plano_caracteristica", FieldType.Single)]
        public String Caracteristicas
        {
            get { return _caracteristicas; }
            set { _caracteristicas= value; }
        }

        [DBFieldInfo("plano_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("plano_quartoComum", FieldType.Single)]
        public Boolean QuartoComum
        {
            get { return _quartoComum; }
            set { _quartoComum= value; }
        }

        [DBFieldInfo("plano_quartoParticular", FieldType.Single)]
        public Boolean QuartoParticular
        {
            get { return _quartoParticular; }
            set { _quartoParticular= value; }
        }

        [DBFieldInfo("plano_quartoComunAns", FieldType.Single)]
        public String AnsQuartoComum
        {
            get { return _ansComum; }
            set { _ansComum= value; }
        }

        [DBFieldInfo("plano_quartoParticularAns", FieldType.Single)]
        public String AnsQuartoParticular
        {
            get { return _ansParticular; }
            set { _ansParticular= value; }
        }

        public String strDatasInicio
        {
            get
            {
                String ret = "";

                if (_inicioColetivo != DateTime.MinValue)
                    ret += "coletivo: " + _inicioColetivo.ToString("dd/MM/yyyy");

                if (_inicioParticular != DateTime.MinValue)
                {
                    if (ret.Length > 0) { ret += " e "; }

                    ret += "particular: " + _inicioParticular.ToString("dd/MM/yyyy");
                }

                return ret;
            }
        }
        #endregion

        public TabelaValor TabelaValor
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #region persistence methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }
        #endregion

        public static IList<Plano> CarregarPorContratoID(Object contratoID)
        {
            return Plano.CarregarPorContratoID(contratoID, false);
        }

        public static IList<Plano> CarregarPorContratoID(Object contratoID, Boolean apenasAtivos)
        {
            String query = "* FROM plano WHERE plano_contratoId=" + contratoID;
            if (apenasAtivos) { query += " AND plano_ativo=1"; }
            query +=" ORDER BY plano_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Plano>(query, typeof(Plano));
        }

        public static Object CarregarID(Object contratoAdmID, String codigo, String subPlano, PersistenceManager pm)
        {
            String qry = "SELECT plano_id FROM plano WHERE plano_contratoId=" + contratoAdmID + " AND (plano_codigo='" + codigo + "' OR plano_codigoParticular='" + codigo + "') AND (plano_subplano='" + subPlano + "' OR plano_subplanoParticular='" + subPlano + "')";

            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Plano Carregar(Object contratoAdmID, String codigo, String subPlano, PersistenceManager pm)
        {
            String qry = "SELECT * FROM plano WHERE plano_contratoId=" + contratoAdmID + " AND (plano_codigo='" + codigo + "' OR plano_codigoParticular='" + codigo + "') AND (plano_subplano='" + subPlano + "' OR plano_subplanoParticular='" + subPlano + "')";

            IList<Plano> list = LocatorHelper.Instance.ExecuteQuery<Plano>(qry, typeof(Plano), pm);
            if (list == null || list.Count == 0)
                return null;
            else
                return list[0];
        }

        public static IList<Plano> CarregarPorOperadoraID(Object operadoraId)
        {
            String query = "plano.* FROM plano INNER JOIN contratoadm ON plano_contratoId= contratoadm_id INNER JOIN operadora on operadora_id = contratoadm_operadoraId WHERE operadora_inativa=0 AND contratoadm_ativo=1 AND plano_ativo=1 AND operadora_id=" + operadoraId + " ORDER BY plano_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Plano>(query, typeof(Plano));
        }

        public static IList<Plano> CarregaPlanosDaTabelaDeValor(Object tabelaDeValorId)
        {
            String qry = "DISTINCT(tabelavaloritem_planoId) as plano_id FROM tabela_valor_item WHERE tabelavaloritem_tabelaid=" + tabelaDeValorId;
            return LocatorHelper.Instance.ExecuteQuery<Plano>(qry, typeof(Plano));
        }

        public static Boolean Existe(Object contratoId, Object planoId, String planoDescricao, String qcCodigo, String qcSubPlano, String qpCodigo, String qpSubPlano)
        {
            String qry = "SELECT COUNT(*) FROM plano WHERE plano_descricao='" + planoDescricao + "' AND plano_contratoId=" + contratoId;

            #region TODO: parametrizar a frase sql 

            if (!String.IsNullOrEmpty(qcCodigo))
            {
                qry += " AND plano_codigo='" + qcCodigo + "'";
            }
            if (!String.IsNullOrEmpty(qpSubPlano))
            {
                qry += " AND plano_subplano='" + qcSubPlano + "'";
            }
            if (!String.IsNullOrEmpty(qpCodigo))
            {
                qry += " AND plano_codigoParticular='" + qpCodigo + "'";
            }
            if (!String.IsNullOrEmpty(qpSubPlano))
            {
                qry += " AND plano_subplanoParticular='" + qpSubPlano + "'";
            }
            #endregion

            if (planoId != null)
            {
                qry += " AND plano_id <> " + planoId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public static void AlterarStatus(Object PlanoID, Boolean ativo)
        {
            String command = "UPDATE plano SET plano_ativo=" + Convert.ToInt32(ativo) + " WHERE plano_id=" + PlanoID;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        public static void SetaTabelaValorAutal(Object planoId, Object tabelaValorId, PersistenceManager pm)
        {
            String command = "UPDATE plano SET plano_tabelaValorAtualId=" + tabelaValorId + " WHERE plano_id=" + planoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public void SetaTabelaValorAutal(Object tabelaValorId, PersistenceManager pm)
        {
            Plano.SetaTabelaValorAutal(this._id, tabelaValorId, pm);
        }
    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    public enum eTipoAtendimentoItem
    {
        Reclamacao,
        SegundaViaBoleto,
        CancelamentoContrato,
        SegundaViaCartao,
        AlteracaoCadastro,
        MudancaDePlano,
        AdicionarBeneficiarios,
        CancelarBeneficiarios
    }

    [DBTable("atendimento_item")]
    public class AtendimentoItem : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Int32 _atendimento_id;
        Int32 _tipo;
        Int32 _plano_id;
        Int32 _acomodacao_id;
        String _texto;
        String _beneficiario_ids;
        String _parentesco_ids;
        DateTime _datahora;
        Int32 _status;
        DateTime _prazo;

        #endregion

        public AtendimentoItem() { _status = (Int32)Atendimento.eStatus.Pendente; }
        public AtendimentoItem(Object atendimento_id) { _id = atendimento_id; }

        #region propriedades 

        [DBFieldInfo("item_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("item_atendimento_id", FieldType.Single)]
        public Int32 AtendimentoId
        {
            get { return _atendimento_id; }
            set { _atendimento_id = value; }
        }

        [DBFieldInfo("item_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        [DBFieldInfo("item_plano_id", FieldType.Single)]
        public Int32 PlanoId
        {
            get { return _plano_id; }
            set { _plano_id = value; }
        }

        [DBFieldInfo("item_acomodacao_id", FieldType.Single)]
        public Int32 AcomodacaoId
        {
            get { return _acomodacao_id; }
            set { _acomodacao_id = value; }
        }

        [DBFieldInfo("item_texto", FieldType.Single)]
        public String Texto
        {
            get { return _texto; }
            set { _texto = value; }
        }

        [DBFieldInfo("item_beneficiario_ids", FieldType.Single)]
        public String BeneficiarioIds
        {
            get { return _beneficiario_ids; }
            set { _beneficiario_ids = value; }
        }

        [DBFieldInfo("item_parentesco_ids", FieldType.Single)]
        public String ParentescoIds
        {
            get { return _parentesco_ids; }
            set { _parentesco_ids = value; }
        }

        [DBFieldInfo("item_datahora", FieldType.Single)]
        public DateTime DataHora
        {
            get { return _datahora; }
            set { _datahora = value; }
        }

        [DBFieldInfo("item_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status= value; }
        }

        [DBFieldInfo("item_prazo", FieldType.Single)]
        public DateTime Prazo
        {
            get { return _prazo; }
            set { _prazo = value; }
        }
        #endregion

        #region métodos EntityBase 

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

        public static IList<AtendimentoItem> CarregaAtendimentoItens(Object atendimentoId)
        {
            String query = String.Concat("",
                "SELECT * ",
                "   FROM atendimento_item ",
                "   WHERE item_atendimento_id = ", atendimentoId,
                "   ORDER BY item_datahora ASC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<AtendimentoItem>
                (query, null, null, typeof(AtendimentoItem));
        }

        public static IList<AtendimentoItem> CarregaAtendimentoItens(List<String> atendimentoIDs, PersistenceManager pm)
        {
            String qry = String.Concat("* FROM atendimento INNER JOIN atendimento_item ON atendimento_id = item_atendimento_id WHERE item_id IN (",
                String.Join(",", atendimentoIDs.ToArray()), ")");

            return LocatorHelper.Instance.ExecuteQuery<AtendimentoItem>(qry, typeof(AtendimentoItem), pm);
        }

    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa uma entrada no histórico de planos de uma proposta.
    /// </summary>
    [DBTable("contrato_plano_historico")]
    public class ContratoPlano : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _contratoId;
        Object _planoId;
        Int32 _tipoAcomodacao;
        DateTime _data;

        String _planoDescricao;

        #endregion

        #region propriedades 

        [DBFieldInfo("contratoplano_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratoplano_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("contratoplano_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId= value; }
        }

        [DBFieldInfo("contratoplano_tipoAcomodacao", FieldType.Single)]
        public Int32 TipoAcomodacao
        {
            get { return _tipoAcomodacao; }
            set { _tipoAcomodacao= value; }
        }

        /// <summary>
        /// Data de admissão.
        /// </summary>
        [DBFieldInfo("contratoplano_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("plano_descricao")]
        public String PlanoDescricao
        {
            get { return _planoDescricao; }
            set { _planoDescricao= value; }
        }

        #endregion

        public ContratoPlano() { }

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

        public static IList<ContratoPlano> Carregar(Object contratoId)
        {
            String query = "contrato_plano_historico.*, plano_descricao FROM contrato_plano_historico INNER JOIN contrato ON contratoplano_contratoId = contrato_id INNER JOIN plano ON contratoplano_planoId = plano_id WHERE contrato_id=" + contratoId + " ORDER BY contratoplano_data DESC, contratoplano_id DESC";

            return LocatorHelper.Instance.ExecuteQuery<ContratoPlano>(query, typeof(ContratoPlano));
        }

        /// <summary>
        /// Método para Carregar o Último ConratoPlano Ativo.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <returns>Retorna um ContratoPlano preenchido.</returns>
        public static ContratoPlano CarregarUltimo(Object ContratoID)
        {
            if (ContratoID != null)
            {
                IList<ContratoPlano> lstContratoPlano = null;

                try
                {
                    lstContratoPlano = Carregar(ContratoID);
                }
                catch (Exception) { throw; }

                if (lstContratoPlano != null && lstContratoPlano.Count > 1)
                {
                    return lstContratoPlano[1];
                }
                else
                    return null;
            }
            else
                throw new ArgumentNullException("O ID do Contrato não pode ser nulo");
        }

        public static ContratoPlano CarregarPenultimo(Object contratoId, PersistenceManager pm)
        {
            String query = "TOP 2 contrato_plano_historico.*, plano_descricao FROM contrato_plano_historico INNER JOIN contrato ON contratoplano_contratoId = contrato_id INNER JOIN plano ON contratoplano_planoId = plano_id WHERE contrato_id=" + contratoId + " ORDER BY contratoplano_data DESC, contratoplano_id DESC";

            IList<ContratoPlano> obj = LocatorHelper.Instance.ExecuteQuery<ContratoPlano>(query, typeof(ContratoPlano), pm);

            if (obj == null)
                return null;
            else if (obj.Count == 1)
                return null;
            else
                return obj[1];
        }

        public static ContratoPlano CarregarAtual(Object contratoId, PersistenceManager pm)
        {
            String query = "TOP 1 contrato_plano_historico.*, plano_descricao FROM contrato_plano_historico INNER JOIN contrato ON contratoplano_contratoId = contrato_id INNER JOIN plano ON contratoplano_planoId = plano_id WHERE contrato_id=" + contratoId + " ORDER BY contratoplano_data DESC, contratoplano_id DESC";

            IList<ContratoPlano> obj = LocatorHelper.Instance.ExecuteQuery<ContratoPlano>(query, typeof(ContratoPlano), pm);

            if (obj == null)
                return null;
            else
                return obj[0];
        }
    }
}
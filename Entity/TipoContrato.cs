namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("tipo_contrato")]
    public class TipoContrato : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            private UI() { }

            public static void FillComboWithTiposContrato(System.Web.UI.WebControls.ListBox cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Normal", "1"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Carência", "4"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Migração", "3"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Administrativa", "2"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Especial", "5"));
            }

            public static TipoComissionamentoProdutorOuOperadora TraduzTipoContratoComissionamento(Int32 tipo)
            {
                if (tipo == 1) { return TipoComissionamentoProdutorOuOperadora.Normal; }
                if (tipo == 2) { return TipoComissionamentoProdutorOuOperadora.Administrativa; }
                if (tipo == 3) { return TipoComissionamentoProdutorOuOperadora.Migracao; }
                if (tipo == 4) { return TipoComissionamentoProdutorOuOperadora.Carencia; }
                if (tipo == 2) { return TipoComissionamentoProdutorOuOperadora.Especial; }

                return TipoComissionamentoProdutorOuOperadora.Normal;
            }
        }

        public enum TipoComissionamentoProdutorOuOperadora : int
        {
            /// <summary>
            /// 0
            /// </summary>
            Normal,
            /// <summary>
            /// 1
            /// </summary>
            Carencia,
            /// <summary>
            /// 2
            /// </summary>
            Migracao,
            /// <summary>
            /// 3
            /// </summary>
            Administrativa,
            /// <summary>
            /// 4
            /// </summary>
            Especial,
            /// <summary>
            /// 5
            /// </summary>
            Idade
        }

        Object _id;
        String _descricao;
        Boolean _ativo;
        Boolean _solicitarInfoAnterior;
        Int32 _tipoComissionamento;

        #region propriedades 

        [DBFieldInfo("tipocontrato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("tipocontrato_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("tipocontrato_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("tipocontrato_solicitaInfoAnterior", FieldType.Single)]
        public Boolean SolicitarInfoAnterior
        {
            get { return _solicitarInfoAnterior; }
            set { _solicitarInfoAnterior= value; }
        }

        [DBFieldInfo("tipocontrato_tipoComissionamento", FieldType.Single)]
        public Int32 TipoComissionamento
        {
            get { return _tipoComissionamento; }
            set { _tipoComissionamento= value; }
        }

        #endregion

        public TipoContrato() { }

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

        public static Hashtable CarregaTiposDeComissionamento()
        {
            Hashtable ht = new Hashtable();

            ht.Add(1, "Normal");
            ht.Add(4, "Carência");
            ht.Add(3, "Migração");
            ht.Add(2, "Administrativa");
            ht.Add(5, "Especial");

            return ht;
        }

        public static IList<TipoContrato> Carregar(Boolean apenasAtivos)
        {
            String query = "* FROM tipo_contrato";
            if (apenasAtivos) { query += " WHERE tipocontrato_ativo=1"; }
            query += " ORDER BY tipocontrato_descricao";

            return LocatorHelper.Instance.ExecuteQuery<TipoContrato>(query, typeof(TipoContrato));
        }
    }
}
namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("filial")]
    public class Filial : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _nome;
        String _email;
        String _telefone;
        Boolean _ativa;

        Endereco _endereco;

        public Filial() { _ativa = true; }

        #region propriedades 

        [DBFieldInfo("filial_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("filial_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("filial_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        

        [DBFieldInfo("filial_telefone", FieldType.Single)]
        public String Telefone
        {
            get { return _telefone; }
            set { _telefone= value; }
        }

        public String FTelefone
        {
            get
            {
                return base.FormataTelefone(_telefone);
            }
        }

        [DBFieldInfo("filial_ativa", FieldType.Single)]
        public Boolean Ativa
        {
            get { return _ativa; }
            set { _ativa= value; }
        }

        public Endereco Endereco
        {
            get { return _endereco; }
            set { _endereco= value; }
        }
        #endregion

        public Agente Agente
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #region métodos EntityBase 

        public void Salvar()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            this._endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Filial);
            try
            {
                pm.Save(this);

                this._endereco.DonoId = this.ID;
                pm.Save(this._endereco);

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                pm = null;
                throw ex;
            }
        }

        public void Remover()
        {
            this.Carregar();
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                pm.Remove(this);
                if (this._endereco != null) { pm.Remove(this._endereco); }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                pm = null;
                throw ex;
            }
        }

        public void Carregar()
        {
            base.Carregar(this);

            IList<Endereco> lista = Endereco.CarregarPorDono(this.ID, Endereco.TipoDono.Filial);
            if (lista != null) { this._endereco = lista[0]; }
        }
        #endregion

        public static IList<Filial> CarregarTodas(Boolean apenasAtivas)
        {
            String query = "* FROM filial ";
            if (apenasAtivas) { query += "WHERE filial_ativa=1 "; }
            query += "ORDER BY filial_nome";

            return LocatorHelper.Instance.ExecuteQuery<Filial>(query, typeof(Filial));
        }

        public static Boolean Duplicado(Object filialId, String nome)
        {
            String qry = "SELECT DISTINCT(filial_id) FROM filial WHERE filial_nome=@nome";

            if (filialId != null)
            {
                qry += " AND filial_id <> " + filialId;
            }

            String[] pNames  = new String[] { "@nome" };
            String[] pValues = new String[] { nome };

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, pNames, pValues);

            if (returned == null || returned == DBNull.Value)
                return false;
            else
                return true;
        }
    }
}
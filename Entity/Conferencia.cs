namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Data;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable]
    [DBTable("conferencia")]
    public class Conferencia : EntityBase, IPersisteableEntity
    {
        //public enum eDepartamento : int
        //{
        //    Conferencia=1,
        //    Cadastro,
        //    Corretor
        //}

        #region campos 

        Object _id;
        String _titularCpf;
        String _titularNome;
        Object _titular_beneficiarioId;
        DateTime _titularDataNascimento;
        List<String> _adicionalIDs;
        Decimal _titularAltura;
        Decimal _titularPeso;
        Decimal _titularValor;
        String _cep;
        String _propostaNumero;
        DateTime _propostaData;
        Object _filialId;
        Object _estipulanteId;
        Object _operadoraId;
        Object _contratoAdmId;
        Object _planoId;
        Object _acomodacaoId;
        String _corretorNome;
        Object _corretorId;
        Object _tipoContratoId;
        Boolean _tipoContratoExplicito;
        String _obs;
        Int32 _departamento;
        DateTime _prazo;
        DateTime _data;
        DateTime _admissao;

        String _corretorTerceiroNome;
        String _corretorTerceiroCPF;
        String _superiorTerceiroNome;
        String _superiorTerceiroCPF;

        #endregion

        #region propriedades 

        [DBFieldInfo("conferencia_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("conferencia_titularCpf", FieldType.Single)]
        public String TitularCPF
        {
            get { return _titularCpf; }
            set { _titularCpf = value; }
        }

        [DBFieldInfo("conferencia_titularNome", FieldType.Single)]
        public String TitularNome
        {
            get { return _titularNome; }
            set { _titularNome = value; }
        }

        [DBFieldInfo("conferencia_titularBeneficiarioId", FieldType.Single)]
        public Object Titular_BeneficiarioID
        {
            get { return _titular_beneficiarioId; }
            set { _titular_beneficiarioId= value; }
        }

        [DBFieldInfo("conferencia_titularDataNascimento", FieldType.Single)]
        public DateTime TitularDataNascimento
        {
            get { return _titularDataNascimento; }
            set { _titularDataNascimento= value; }
        }

        public List<String> AdicionalIDs
        {
            get { return _adicionalIDs; }
            set { _adicionalIDs= value; }
        }

        [DBFieldInfo("conferencia_titularAdicionais", FieldType.Single)]
        public String strAdicionalIDs
        {
            get
            {
                if (_adicionalIDs == null) { return null; }
                String _result = "";
                foreach (String ad in _adicionalIDs)
                {
                    if (_result.Length > 0) { _result += ","; }
                    _result += ad;
                }

                return _result;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) { return; }
                if (_adicionalIDs == null) { _adicionalIDs = new List<String>(); }

                String[] ids = value.Split(',');

                foreach (String id in ids)
                {
                    _adicionalIDs.Add(id);
                }
            }
        }

        [DBFieldInfo("conferencia_titularAltura", FieldType.Single)]
        public Decimal TitularAltura
        {
            get { return _titularAltura; }
            set { _titularAltura= value; }
        }

        [DBFieldInfo("conferencia_titularPeso", FieldType.Single)]
        public Decimal TitularPeso
        {
            get { return _titularPeso; }
            set { _titularPeso= value; }
        }

        [DBFieldInfo("conferencia_titularValor", FieldType.Single)]
        public Decimal TitularValor
        {
            get { return _titularValor; }
            set { _titularValor= value; }
        }

        [DBFieldInfo("conferencia_cep", FieldType.Single)]
        public String CEP
        {
            get { return _cep; }
            set { _cep = value; }
        }

        [DBFieldInfo("conferencia_propostaNumero", FieldType.Single)]
        public String PropostaNumero
        {
            get { return _propostaNumero; }
            set { _propostaNumero= value; }
        }

        [DBFieldInfo("conferencia_propostaData", FieldType.Single)]
        public DateTime PropostaData
        {
            get { return _propostaData; }
            set { _propostaData= value; }
        }

        [DBFieldInfo("conferencia_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("conferencia_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId= value; }
        }

        [DBFieldInfo("conferencia_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("conferencia_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        [DBFieldInfo("conferencia_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId = value; }
        }

        [DBFieldInfo("conferencia_acomodacaoId", FieldType.Single)]
        public Object AcomodacaoID
        {
            get { return _acomodacaoId; }
            set { _acomodacaoId= value; }
        }

        public String CorretorNome
        {
            get { return _corretorNome; }
            set { _corretorNome = value; }
        }

        [DBFieldInfo("conferencia_corretorId", FieldType.Single)]
        public Object CorretorID
        {
            get { return _corretorId; }
            set { _corretorId = value; }
        }

        [DBFieldInfo("conferencia_tipoContratoId", FieldType.Single)]
        public Object TipoContratoID
        {
            get { return _tipoContratoId; }
            set { _tipoContratoId = value; }
        }

        [DBFieldInfo("conferencia_tipoContratoExplicito", FieldType.Single)]
        public Boolean TipoContratoExplicito
        {
            get { return _tipoContratoExplicito; }
            set { _tipoContratoExplicito= value; }
        }

        [DBFieldInfo("conferencia_obs", FieldType.Single)]
        public String OBS
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("conferencia_depto", FieldType.Single)]
        public Int32 Departamento
        {
            get { return _departamento; }
            set { _departamento= value; }
        }

        [DBFieldInfo("conferencia_prazo", FieldType.Single)]
        public DateTime Prazo
        {
            get { return _prazo; }
            set { _prazo= value; }
        }

        [DBFieldInfo("conferencia_admissao", FieldType.Single)]
        public DateTime Admissao
        {
            get { return _admissao; }
            set { _admissao= value; }
        }

        [DBFieldInfo("conferencia_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [DBFieldInfo("conferencia_corretorTerceiroNome", FieldType.Single)]
        public String CorretorTerceiroNome
        {
            get { return _corretorTerceiroNome; }
            set { _corretorTerceiroNome = value; }
        }
        [DBFieldInfo("conferencia_corretorTerceiroCPF", FieldType.Single)]
        public String CorretorTerceiroCPF
        {
            get { return _corretorTerceiroCPF; }
            set { _corretorTerceiroCPF = value; }
        }

        [DBFieldInfo("conferencia_superiorTerceiroNome", FieldType.Single)]
        public String SuperiorTerceiroNome
        {
            get { return _superiorTerceiroNome; }
            set { _superiorTerceiroNome = value; }
        }
        [DBFieldInfo("conferencia_superiorTerceiroCPF", FieldType.Single)]
        public String SuperiorTerceiroCPF
        {
            get { return _superiorTerceiroCPF; }
            set { _superiorTerceiroCPF = value; }
        }

        #endregion

        public Conferencia() { _data = DateTime.Now; _propostaData = _data; _departamento = Convert.ToInt32(ContratoStatusHistorico.eStatus.EmConferencia); }
        public Conferencia(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                String command = "DELETE FROM conferencia_checklist WHERE conferenciachecklist_conferenciaId=" + this.ID;
                NonQueryHelper.Instance.ExecuteNonQuery(command, pm);

                command = "DELETE FROM conferencia_itemSaudeInstancia WHERE conferenciaItemSaudeInstancia_conferenciaId=" + this.ID;
                NonQueryHelper.Instance.ExecuteNonQuery(command, pm);

                command = "DELETE FROM conferenciaBeneficiario WHERE conferenciabeneficiario_conferenciaId=" + this.ID;
                NonQueryHelper.Instance.ExecuteNonQuery(command, pm);

                pm.Remove(this);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static IList<Conferencia> Carregar(Object operadoraId, String propostaNumero, ContratoStatusHistorico.eStatus[] departamentos)
        {
            String[] strDeptos = new String[departamentos.Length];
            for(int i = 0; i < departamentos.Length;i++)
                strDeptos[i] = Convert.ToString(Convert.ToInt32(departamentos[i]));

            String[] names = null, values = null;

            String query = "conferencia.* FROM conferencia WHERE conferencia_depto IN (" + String.Join(",", strDeptos) + ")";
            query += String.Concat(" and year(conferencia_data) >= ", 
                DateTime.Now.Year, " and month(conferencia_data) >=  ", (DateTime.Now.Month - 1));

            if (operadoraId != null) { query += " AND conferencia_operadoraId=" + operadoraId; }
            if (!String.IsNullOrEmpty(propostaNumero))
            {
                query += " AND conferencia_propostaNumero=@conferencia_propostaNumero";
                names  = new String[1] { "@conferencia_propostaNumero" };
                values = new String[1] { propostaNumero };
            }
            query += " ORDER BY conferencia_prazo, conferencia_data";

            if (String.IsNullOrEmpty(propostaNumero))
                return LocatorHelper.Instance.ExecuteQuery<Conferencia>(query, typeof(Conferencia));
            else
                return LocatorHelper.Instance.ExecuteParametrizedQuery<Conferencia>(query, names, values, typeof(Conferencia));
        }

        public static Boolean AlteraEstagio(Object operadoraId, List<String> ids, List<String> numeros, ContratoStatusHistorico.eStatus estagio)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                String letra = ""; Int32 numero = -1; Int32 qtdZeros = -1; Object ret = null;
                String qry = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero='";

                for (int i =0; i < ids.Count; i++)
                {
                    ret = LocatorHelper.Instance.ExecuteScalar(qry + numeros[i] + "'", null, null, pm);
                    if (ret != null && ret != DBNull.Value)
                    {
                        pm.Commit(); pm = null;
                        return false;
                    }

                    NonQueryHelper.Instance.ExecuteNonQuery("UPDATE conferencia SET conferencia_prazo=null, conferencia_depto=" + Convert.ToInt32(estagio).ToString() + " WHERE conferencia_id=" + ids[i], pm);

                    letra = EntityBase.PrimeiraPosicaoELetra(numeros[i]);
                    if (letra != "")
                    {
                        numero = Convert.ToInt32(numeros[i].Replace(letra, ""));
                        qtdZeros = numeros[i].Replace(letra, "").Length;
                    }
                    else
                    {
                        numero = Convert.ToInt32(numeros[i]);
                        qtdZeros = numeros[i].Length;
                    }
                    
                    ContratoStatusHistorico.Salvar(numero, qtdZeros, letra, operadoraId, estagio, pm);
                }

                pm.Commit();
                return true;
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        public static DataTable CarregarPorParametros(String[] operadoraIds, Object status, DateTime de, DateTime ate)
        {
            String deptoCond = "", cadastradosJoin = "";

            if (Convert.ToString(status) != "6") //propostas ja cadastradas 
            {
                deptoCond = " and conferencia_depto=" + Convert.ToString(status);
            }
            else
            {
                cadastradosJoin = " inner join contrato on contrato_operadoraId=conferencia_operadoraId and contrato_numero=conferencia_propostaNumero ";
                //deptoCond = " and conferencia_depto >= 5 ";
            }

            String qry = String.Concat(
                "select conferencia_id as ID, conferencia_titularNome as TitularNome, '' as TitularIdade, conferencia_titularDataNascimento, filial_nome as FilialNome, ",
                "       operadora_nome as OperadoraNome, estipulante_descricao as EstipulanteNome, conferencia_titularValor, conferencia_data as Data, conferencia_admissao as Admissao, ",
                "       case conferencia_depto ",
                "           when 1 then 'Com Corretor' ",
                "           when 3 then 'Em Conferência' ",
                "           when 4 then 'No Cadastro' ",
                "           when 5 then 'Em análise técnica' ",
                "           when 6 then 'Cadastrado' ",
                "       end as Status, ",
                "       conferencia_propostaNumero as ContratoNumero, ",
                "       conferencia_titularCpf as TitularCpf, ",
                "       (select sum(conferenciabeneficiario_valor) from conferenciaBeneficiario where conferenciabeneficiario_conferenciaid=conferencia_id) as Total, ",
                "       (select count(conferenciabeneficiario_id) from conferenciaBeneficiario where conferenciabeneficiario_conferenciaid=conferencia_id) as QtdVidas",
                "   from conferencia ",
                cadastradosJoin,
                "       inner join filial on filial_id          = conferencia_filialId",
                "       inner join operadora on operadora_id    = conferencia_operadoraId",
                "       inner join estipulante on estipulante_id= conferencia_estipulanteId",
                "   where ",
                "       conferencia_propostaData between '", de.ToString("yyyy-MM-dd"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:990"), "' ",
                "       and operadora_id in (", String.Join(",", operadoraIds), ")", deptoCond);
//              "       and conferencia_depto=", status);

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
            dt.Columns.Add("ValorFinal");

            foreach (DataRow row in dt.Rows)
            {
                if (row["conferencia_titularDataNascimento"] != null &&
                    row["conferencia_titularDataNascimento"] != DBNull.Value &&
                    EntityBase.ToDateTime(row["conferencia_titularDataNascimento"]) != DateTime.MinValue)
                {
                    row["QtdVidas"] = Convert.ToInt32(row["QtdVidas"]) + 1; //acrescenta o titular
                    row["TitularIdade"] = Beneficiario.CalculaIdade(EntityBase.ToDateTime(row["conferencia_titularDataNascimento"]));
                }

                if (Convert.ToString(status) == "6") { row["Status"] = "Cadastrado"; }

                row["ValorFinal"] = EntityBase.CToDecimal(row["conferencia_titularValor"]) + EntityBase.CToDecimal(row["Total"]);
            }

            return dt;
        }
    }

    [Serializable]
    [DBTable("conferencia_checklist")]
    public class ConferenciaCheckList : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _conferenciaId;
        Object _itemCheckListId;
        Boolean _valor;

        String _itemCheckListDescricao;

        #region propriedades 

        [DBFieldInfo("conferenciachecklist_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("conferenciachecklist_conferenciaId", FieldType.Single)]
        public Object ConferenciaID
        {
            get { return _conferenciaId; }
            set { _conferenciaId = value; }
        }

        [DBFieldInfo("conferenciachecklist_itemCheckListId", FieldType.Single)]
        public Object ItemCheckListID
        {
            get { return _itemCheckListId; }
            set { _itemCheckListId = value; }
        }

        [DBFieldInfo("conferenciachecklist_valor", FieldType.Single)]
        public Boolean Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [Joinned("checklist_descricao")]
        public String ItemCheckListDescricao
        {
            get { return _itemCheckListDescricao; }
            set { _itemCheckListDescricao = value; }
        }

        #endregion

        public ConferenciaCheckList() { }
        public ConferenciaCheckList(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public static void Salvar(IList<ConferenciaCheckList> lista)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                String query = null; Object returned = null;
                foreach (ConferenciaCheckList item in lista)
                {
                    if (item.ID == null)
                    {
                        query = String.Concat("SELECT conferenciachecklist_id FROM conferencia_checklist WHERE conferenciachecklist_conferenciaId = ", item.ConferenciaID, " AND conferenciachecklist_itemCheckListId=", item.ItemCheckListID);
                        returned = LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);

                        item.ID = returned;
                    }
                    pm.Save(item);
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
                pm = null;
            }
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

        public static IList<ConferenciaCheckList> Carregar(Object conferenciaId)
        {
            String sql = String.Concat("SELECT conferencia_checklist.*, checklist_descricao FROM conferencia_checklist ",
                "INNER JOIN checklist ON conferenciachecklist_itemCheckListId=checklist_id ",
                "WHERE conferenciachecklist_conferenciaId=", conferenciaId,
                " ORDER BY checklist_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ConferenciaCheckList>(sql, typeof(ConferenciaCheckList));
        }
    }

    [Serializable]
    [DBTable("conferenciaBeneficiario")]
    public class ConferenciaBeneficiario : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _beneficiarioId;
        Object _conferenciaId;
        DateTime _dataNascimento;
        String _cpf;
        String _nome;
        Object _parentescoId;
        Object _estadoCivilId;
        DateTime _dataCasamento;
        Decimal _peso;
        Decimal _altura;
        Decimal _valor;
        List<String> _adicionalIDs;

        DateTime _dataProposta;

        String _parentescoDescricao;
        #endregion

        #region properties 

        [DBFieldInfo("conferenciabeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("conferenciabeneficiario_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_conferenciaId", FieldType.Single)]
        public Object ConferenciaID
        {
            get { return _conferenciaId; }
            set { _conferenciaId= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_dataNascimento", FieldType.Single)]
        public DateTime DataNascimento
        {
            get { return _dataNascimento; }
            set { _dataNascimento = value; }
        }

        [DBFieldInfo("conferenciabeneficiario_cpf", FieldType.Single)]
        public String CPF
        {
            get { return _cpf; }
            set { _cpf= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_parentescoId", FieldType.Single)]
        public Object ParentescoID
        {
            get { return _parentescoId; }
            set { _parentescoId = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        [DBFieldInfo("conferenciabeneficiario_estadoCivilId", FieldType.Single)]
        public Object EstadoCivilID
        {
            get { return _estadoCivilId; }
            set { _estadoCivilId= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_dataCasamento", FieldType.Single)]
        public DateTime DataCasamento
        {
            get { return _dataCasamento; }
            set { _dataCasamento= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_peso", FieldType.Single)]
        public Decimal Peso
        {
            get { return _peso; }
            set { _peso = value; }
        }

        [DBFieldInfo("conferenciabeneficiario_altura", FieldType.Single)]
        public Decimal Altura
        {
            get { return _altura; }
            set { _altura = value; }
        }

        [DBFieldInfo("conferenciabeneficiario_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [DBFieldInfo("conferenciabeneficiario_dataProposta", FieldType.Single)]
        public DateTime PropostaData
        {
            get { return _dataProposta; }
            set { _dataProposta = value; }
        }

        public List<String> AdicionalIDs
        {
            get { return _adicionalIDs; }
            set { _adicionalIDs= value; }
        }

        [DBFieldInfo("conferenciabeneficiario_adicionais", FieldType.Single)]
        public String strAdicionalIDs
        {
            get
            {
                String _result = "";
                foreach (String ad in _adicionalIDs)
                {
                    if (_result.Length > 0) { _result += ","; }
                    _result += ad;
                }

                return _result;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) { return; }
                String[] ids = value.Split(',');

                foreach (String id in ids)
                {
                    _adicionalIDs.Add(id);
                }
            }
        }

        public String Idade
        {
            get 
            {
                int idade = Beneficiario.CalculaIdade(_dataNascimento, _dataProposta);
                if (idade < 1)
                    return "0 anos";
                else if (idade == 1)
                    return "1 ano";
                else
                    return idade.ToString() + " anos";
            }
        }

        [Joinned("contratoAdmparentescoagregado_parentescoDescricao")]
        public String ParentescoDescricao
        {
            get { return _parentescoDescricao; }
            set { _parentescoDescricao= value; }
        }
        #endregion

        public ConferenciaBeneficiario() { _adicionalIDs = new List<String>(); }
        public ConferenciaBeneficiario(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public static void Salvar(IList<ConferenciaBeneficiario> lista)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                //String query = null; Object returned = null;
                foreach (ConferenciaBeneficiario beneficiario in lista)
                {
                    if (Convert.ToString(beneficiario.ParentescoID) == "-1") { continue; } //foi uma emergencia. TODO: consertar isso com urgência
                    if (beneficiario.ID == null)
                    {
                        //query = String.Concat("SELECT conferenciabeneficiario_id FROM conferenciaBeneficiario WHERE conferenciabeneficiario_conferenciaId = ", beneficiario.ConferenciaID, " AND conferenciabeneficiario_cpf='", beneficiario.CPF, "'");
                        //returned = LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);

                        //beneficiario.ID = returned;
                    }

                    pm.Save(beneficiario);
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
                pm = null;
            }
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

        public static IList<ConferenciaBeneficiario> Carregar(Object conferenciaId)
        {
            String sql = String.Concat("conferenciaBeneficiario.*, contratoAdmparentescoagregado_parentescoDescricao FROM conferenciaBeneficiario LEFT JOIN contratoADM_parentesco_agregado ON conferenciabeneficiario_parentescoId=contratoAdmparentescoagregado_id WHERE conferenciabeneficiario_conferenciaId=", conferenciaId);
            return LocatorHelper.Instance.ExecuteQuery<ConferenciaBeneficiario>(sql, typeof(ConferenciaBeneficiario));
        }
    }

    [Serializable]
    [DBTable("conferencia_itemSaudeInstancia")]
    public class ConferenciaItemSaudeInstancia : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _conferenciaId;
        String _beneficiarioCpf;
        Object _itemSaudeId;
        Boolean _valor;

        String _itemSaudeDescricao;

        #region propriedades

        [DBFieldInfo("conferenciaItemSaudeInstancia_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("conferenciaItemSaudeInstancia_conferenciaId", FieldType.Single)]
        public Object ConferenciaID
        {
            get { return _conferenciaId; }
            set { _conferenciaId= value; }
        }

        [DBFieldInfo("conferenciaItemSaudeInstancia_beneficiarioCpf", FieldType.Single)]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf= value; }
        }

        [Joinned("itemsaude_id")]
        [DBFieldInfo("conferenciaItemSaudeInstancia_itemSaudeId", FieldType.Single)]
        public Object ItemSaudeID
        {
            get { return _itemSaudeId; }
            set { _itemSaudeId= value; }
        }

        [DBFieldInfo("conferenciaItemSaudeInstancia_valor", FieldType.Single)]
        public Boolean Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [Joinned("itemsaude_descricao")]
        public String ItemCheckListDescricao
        {
            get { return _itemSaudeDescricao; }
            set { _itemSaudeDescricao= value; }
        }

        #endregion

        public ConferenciaItemSaudeInstancia() { }
        public ConferenciaItemSaudeInstancia(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public static void Salvar(IList<ConferenciaItemSaudeInstancia> lista)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                String query = null; Object returned = null;
                foreach (ConferenciaItemSaudeInstancia item in lista)
                {
                    if (item.ID == null)
                    {
                        query = String.Concat("SELECT conferenciaItemSaudeInstancia_id FROM conferencia_itemSaudeInstancia WHERE conferenciaItemSaudeInstancia_conferenciaId = ", item.ConferenciaID, " AND conferenciaItemSaudeInstancia_itemSaudeId=", item.ItemSaudeID);
                        returned = LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);

                        item.ID = returned;
                    }

                    pm.Save(item);
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
                pm = null;
            }
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

        public static IList<ConferenciaItemSaudeInstancia> Carregar(Object conferenciaId, String beneficiarioCpf)
        {
            String sql = String.Concat("SELECT conferencia_itemSaudeInstancia.*, itemsaude_id, itemsaude_descricao FROM conferencia_itemSaudeInstancia ",
                "INNER JOIN conferencia_itemSaude ON conferenciaItemSaudeInstancia_itemSaudeId=itemsaude_id ",
                "WHERE conferenciaItemSaudeInstancia_conferenciaId=", conferenciaId, " AND conferenciaItemSaudeInstancia_beneficiarioCpf='", beneficiarioCpf,
                "' ORDER BY itemsaude_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ConferenciaItemSaudeInstancia>(sql, typeof(ConferenciaItemSaudeInstancia));
        }
    }

    [Serializable]
    public class MedicalReport : IPersisteableEntity
    {
        #region Campos 

        Object _id;
        Object _operadoraId;
        Object _contratoId;
        Object _itemDeclaracaoId;
        String _contratoNumero;
        Object _beneficiarioId;
        String _beneficiarioNome;
        String _beneficiarioSexo;
        DateTime _beneficiarioDataNascimento;
        String _itemSaudeDescricao;
        String _itemSaudeInstanciaDescricao;
        DateTime _itemSaudeInstanciaData;
        String _cidInicial;
        String _cidFinal;
        Boolean _aprovadoMedico;
        String _obsMedico;
        DateTime _dataAprovadoPeloMedico;
        Decimal _peso;
        Decimal _altura;

        Boolean _aprovadoDeptoTecnico;
        String _obsDeptoTecnico;
        DateTime _dataAprovadoDeptoTecnico;

        #endregion

        #region Propriedades 

        [Joinned("itemDeclaracaoSaudeInstancia_id")]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [Joinned("itemdeclaracaosaude_operadoraid")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [Joinned("itemdeclaracaosaudeinstancia_beneficiarioId")]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId= value; }
        }

        /// <summary>
        /// ID do item da declaração de saúde
        /// </summary>
        [Joinned("itemdeclaracaosaudeinstancia_itemdeclaracaoid")]
        public Object ItemSaudeID
        {
            get { return _itemDeclaracaoId; }
            set { _itemDeclaracaoId= value; }
        }

        /// <summary>
        /// ID da proposta.
        /// </summary>
        [Joinned("contratobeneficiario_contratoId")]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero= value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { if (_beneficiarioNome == null) { return null; } else { return _beneficiarioNome.Split(' ')[0]; } }
            set { _beneficiarioNome= value; }
        }

        [Joinned("beneficiario_sexo")]
        public String BeneficiarioSexo
        {
            get { if (_beneficiarioSexo == "1") { return "Masculino"; } else { return "Feminino"; } }
            set { _beneficiarioSexo = value; }
        }

        public Int32 BeneficiarioSexoID
        {
            get { if (BeneficiarioSexo == "Masculino") { return 1; } else { return 2; } }
        }

        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioDataNascimento
        {
            get { return _beneficiarioDataNascimento; }
            set { _beneficiarioDataNascimento= value; }
        }

        [Joinned("contratobeneficiario_peso")]
        public Decimal Peso
        {
            get { return _peso; }
            set { _peso= value; }
        }

        [Joinned("contratobeneficiario_altura")]
        public Decimal Altura
        {
            get { return _altura; }
            set { _altura= value; }
        }

        public String BeneficiarioAlturaPeso
        {
            get
            {
                return this._altura.ToString("N2") + " " + _peso.ToString("N3");
            }
        }

        public Int32 BeneficiarioIdade
        {
            get
            {
                if (_beneficiarioDataNascimento == DateTime.MinValue) { return 0; }
                return Beneficiario.CalculaIdade(_beneficiarioDataNascimento);
            }
        }

        [Joinned("itemdeclaracaosaude_texto")]
        public String ItemSaudeDescricao
        {
            get { return _itemSaudeDescricao; }
            set { _itemSaudeDescricao= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_descricao")]
        public String ItemSaudeInstanciaDescricao
        {
            get { return _itemSaudeInstanciaDescricao; }
            set { _itemSaudeInstanciaDescricao= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_data")]
        public DateTime ItemSaudeInstanciaData
        {
            get { return _itemSaudeInstanciaData; }
            set { _itemSaudeInstanciaData= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_cidInicial")]
        public String CIDInicial
        {
            get { return _cidInicial; }
            set { _cidInicial= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_cidFinal")]
        public String CIDFinal
        {
            get { return _cidFinal; }
            set { _cidFinal= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_aprovadoMedico")]
        public Boolean AprovadoMedico
        {
            get { return _aprovadoMedico; }
            set { _aprovadoMedico= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_dataAprovadoMedico")]
        public DateTime DataAprovadoPeloMedico
        {
            get { return _dataAprovadoPeloMedico; }
            set { _dataAprovadoPeloMedico= value; }
        }

        public String strDataAprovadoPeloMedico
        {
            get { return _dataAprovadoPeloMedico.ToString("dd/MM/yyyy"); }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_obs")]
        public String OBSMedico
        {
            get { return _obsMedico; }
            set { _obsMedico= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_parecerDeptoTecnico")]
        public Boolean AprovadoDeptoTecnico
        {
            get { return _aprovadoDeptoTecnico; }
            set { _aprovadoDeptoTecnico= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_obsDeptoTecnico")]
        public String OBSDeptoTecnico
        {
            get { return _obsDeptoTecnico; }
            set { _obsDeptoTecnico= value; }
        }

        [Joinned("itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico")]
        public DateTime DataAprovadoDeptoTecnico
        {
            get { return _dataAprovadoDeptoTecnico; }
            set { _dataAprovadoDeptoTecnico= value; }
        }

        #endregion

        public MedicalReport() { }

        public static IList<MedicalReport> CarregarParaMedico(DateTime? preenchidoEm)
        {
            String condicao = "";

            if (!preenchidoEm.HasValue)
            {
                condicao = "AND (itemDeclaracaoSaudeInstancia_cidInicial IS NULL OR itemDeclaracaoSaudeInstancia_cidFinal IS NULL OR itemDeclaracaoSaudeInstancia_aprovadoMedico IS NULL)"; // OR itemDeclaracaoSaudeInstancia_aprovadoMedico=0
            }
            else
            {
                condicao = "AND  CONVERT(VARCHAR(20), itemDeclaracaoSaudeInstancia_dataAprovadoMedico, 103)='" + preenchidoEm.Value.ToString("dd/MM/yyyy") + "'";
            }

            String query = String.Concat("SELECT contratobeneficiario_peso, contratobeneficiario_altura, itemDeclaracaoSaudeInstancia_id,contrato_numero, beneficiario_nome, beneficiario_sexo, beneficiario_dataNascimento, itemdeclaracaosaude_texto, itemDeclaracaoSaudeInstancia_descricao, itemDeclaracaoSaudeInstancia_data, itemDeclaracaoSaudeInstancia_cidInicial, itemDeclaracaoSaudeInstancia_cidFinal, itemDeclaracaoSaudeInstancia_aprovadoMedico, itemDeclaracaoSaudeInstancia_obs ",
                "FROM contrato_beneficiario ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN contrato ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN declaracao_saude_item_instancia ON itemdeclaracaosaudeinstancia_beneficiarioId=contratobeneficiario_beneficiarioId ",
                "INNER JOIN declaracao_saude_item ON itemdeclaracaosaudeinstancia_itemdeclaracaoid=itemdeclaracaosaude_id ",
                "WHERE itemdeclaracaosaudeinstancia_sim=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 ", condicao,
                " ORDER BY contratobeneficiario_contratoId, beneficiario_nome");

            return LocatorHelper.Instance.ExecuteQuery<MedicalReport>(query, typeof(MedicalReport));
        }

        public static IList<MedicalReport> CarregarParaTecnico(Nullable<DateTime> preenchidoEm)
        {
            String condicao = String.Concat("AND (itemDeclaracaoSaudeInstancia_dataAprovadoMedico IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidInicial IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidFinal IS NOT NULL AND (itemDeclaracaoSaudeInstancia_aprovadoMedico=0 OR itemDeclaracaoSaudeInstancia_aprovadoMedico=1)) AND (contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo), " OR contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Devolvido), ") ");

            if (preenchidoEm.HasValue)
            {
                condicao += " AND  CONVERT(VARCHAR(20), itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico, 103)='" + preenchidoEm.Value.ToString("dd/MM/yyyy") + "'";
            }
            else
            {
                condicao += " AND itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico IS NULL ";
            }

            String query = String.Concat("DISTINCT(itemdeclaracaosaudeinstancia_beneficiarioId), itemDeclaracaoSaudeInstancia_parecerDeptoTecnico, contrato_numero, contratobeneficiario_contratoId, beneficiario_nome, beneficiario_sexo, beneficiario_dataNascimento, itemdeclaracaosaude_operadoraid ",
                "FROM contrato_beneficiario ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN contrato ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN declaracao_saude_item_instancia ON itemdeclaracaosaudeinstancia_beneficiarioId=contratobeneficiario_beneficiarioId ",
                "INNER JOIN declaracao_saude_item ON itemdeclaracaosaudeinstancia_itemdeclaracaoid=itemdeclaracaosaude_id ",
                "WHERE itemdeclaracaosaudeinstancia_sim=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 ", condicao,
                " ORDER BY contratobeneficiario_contratoId, beneficiario_nome");

            return LocatorHelper.Instance.ExecuteQuery<MedicalReport>(query, typeof(MedicalReport));
        }

        public static IList<MedicalReport> CarregarParaTecnico(Object beneficiarioId, Object contratoId)
        {
            String condicao = String.Concat("AND (itemDeclaracaoSaudeInstancia_dataAprovadoMedico IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidInicial IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidFinal IS NOT NULL AND (itemDeclaracaoSaudeInstancia_aprovadoMedico=0 OR itemDeclaracaoSaudeInstancia_aprovadoMedico=1)) AND itemdeclaracaosaudeinstancia_beneficiarioId=", beneficiarioId, " AND contratobeneficiario_contratoId=", contratoId);

            String query = String.Concat("SELECT itemDeclaracaoSaudeInstancia_id,contrato_numero, beneficiario_nome, beneficiario_sexo, beneficiario_dataNascimento, itemdeclaracaosaude_texto, itemdeclaracaosaude_operadoraid, itemDeclaracaoSaudeInstancia_descricao, itemDeclaracaoSaudeInstancia_data, itemDeclaracaoSaudeInstancia_cidInicial, itemDeclaracaoSaudeInstancia_cidFinal, itemDeclaracaoSaudeInstancia_aprovadoMedico, itemDeclaracaoSaudeInstancia_dataAprovadoMedico, itemDeclaracaoSaudeInstancia_obs, itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico,itemDeclaracaoSaudeInstancia_parecerDeptoTecnico,itemDeclaracaoSaudeInstancia_obsDeptoTecnico, contratobeneficiario_contratoId, contratobeneficiario_beneficiarioId ",
                "FROM contrato_beneficiario ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN contrato ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN declaracao_saude_item_instancia ON itemdeclaracaosaudeinstancia_beneficiarioId=contratobeneficiario_beneficiarioId ",
                "INNER JOIN declaracao_saude_item ON itemdeclaracaosaudeinstancia_itemdeclaracaoid=itemdeclaracaosaude_id ",
                "WHERE itemdeclaracaosaudeinstancia_sim=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 ", condicao);

            return LocatorHelper.Instance.ExecuteQuery<MedicalReport>(query, typeof(MedicalReport));
        }

        public static MedicalReport CarregarParaTecnico(Object id)
        {
            String condicao = String.Concat("AND (itemDeclaracaoSaudeInstancia_dataAprovadoMedico IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidInicial IS NOT NULL AND itemDeclaracaoSaudeInstancia_cidFinal IS NOT NULL AND (itemDeclaracaoSaudeInstancia_aprovadoMedico=0 OR itemDeclaracaoSaudeInstancia_aprovadoMedico=1)) AND itemdeclaracaosaudeinstancia_id=", id, " AND (contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Novo), " OR contratobeneficiario_status=", Convert.ToInt32(ContratoBeneficiario.eStatus.Devolvido), ") ");

            String query = String.Concat("SELECT itemdeclaracaosaudeinstancia_itemdeclaracaoid, itemDeclaracaoSaudeInstancia_id,contrato_numero, beneficiario_nome, beneficiario_sexo, beneficiario_dataNascimento, itemdeclaracaosaude_texto, itemdeclaracaosaude_operadoraid, itemDeclaracaoSaudeInstancia_descricao, itemDeclaracaoSaudeInstancia_data, itemDeclaracaoSaudeInstancia_cidInicial, itemDeclaracaoSaudeInstancia_cidFinal, itemDeclaracaoSaudeInstancia_aprovadoMedico, itemDeclaracaoSaudeInstancia_dataAprovadoMedico, itemDeclaracaoSaudeInstancia_obs, itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico,itemDeclaracaoSaudeInstancia_parecerDeptoTecnico,itemDeclaracaoSaudeInstancia_obsDeptoTecnico, contratobeneficiario_contratoId, contratobeneficiario_beneficiarioId ",
                "FROM contrato_beneficiario ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN contrato ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN declaracao_saude_item_instancia ON itemdeclaracaosaudeinstancia_beneficiarioId=contratobeneficiario_beneficiarioId ",
                "INNER JOIN declaracao_saude_item ON itemdeclaracaosaudeinstancia_itemdeclaracaoid=itemdeclaracaosaude_id ",
                "WHERE itemdeclaracaosaudeinstancia_sim=1 AND contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 ", condicao);

            IList<MedicalReport> ret = LocatorHelper.Instance.ExecuteQuery<MedicalReport>(query, typeof(MedicalReport));

            if (ret == null)
                return null;
            else
                return ret[0];
        }
    }
}
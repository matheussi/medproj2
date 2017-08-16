using System;
using System.Collections.Generic;
using System.Text;

using LC.Framework.Phantom;

namespace LC.Web.PadraoSeguros.Entity
{
    /// <summary>
    /// Representa os Dados de Configuração de um Arquivo Transacional da Unimed. Seção U01.
    /// </summary>
    [DBTable("arquivo_transacional_conf")]
    public sealed class ArqTransacionalUnimedConf : AbastractArqTransacionalConf, IPersisteableEntity
    {
        #region Private Fields

        private String _arqc_cod_operadora_singular;
        private String _arqc_cod_operadora_naunimed;
        private Int32  _arqc_tipo_identificacao;
        private String _arqc_cei_operadora;

        #endregion

        #region Public Members
        /// <summary>
        /// ID.
        /// </summary>
        [DBFieldInfo("arqc_id", FieldType.PrimaryKeyAndIdentity)]
        public sealed override Object ID
        {
            get { return this._arqc_id; }
            set { this._arqc_id = value; }
        }

        /// <summary>
        /// ID da Operadora.
        /// </summary>
        [DBFieldInfo("arqc_operadora_id", FieldType.Single)]
        public sealed override Object OperadoraID
        {
            get { return this._arqc_operadora_id; }
            set { this._arqc_operadora_id = value; }
        }

        /// <summary>
        /// Código da Unimed Singular
        /// </summary>
        [DBFieldInfo("arqc_cod_operadora_singular", FieldType.Single)]
        public String OperadoraCodSingular
        {
            get { return this._arqc_cod_operadora_singular; }
            set { this._arqc_cod_operadora_singular = value; }
        }

        /// <summary>
        /// Código da Empresa na Unimed
        /// </summary>
        [DBFieldInfo("arqc_cod_operadora_naunimed", FieldType.Single)]
        public String OperadoraNaUnimed
        {
            get { return this._arqc_cod_operadora_naunimed; }
            set { this._arqc_cod_operadora_naunimed = value; }
        }

        /// <summary>
        /// Nome da Operadora.
        /// </summary>
        [DBFieldInfo("arqc_nome_operadora", FieldType.Single)]
        public sealed override String OperadoraNome
        {
            get { return this._arqc_nome_operadora; }
            set { this._arqc_nome_operadora = value; }
        }

        /// <summary>
        /// CNPJ da Operadora.
        /// </summary>
        [DBFieldInfo("arqc_cnpj_operadora", FieldType.Single)]
        public sealed override String OperadoraCNPJ
        {
            get { return this._arqc_cnpj_operadora; }
            set { this._arqc_cnpj_operadora = value; }
        }

        /// <summary>
        /// Versão do Arquivo.
        /// </summary>
        [DBFieldInfo("arqc_versao", FieldType.Single)]
        public sealed override String ArqVersao
        {
            get { return this._arqc_versao; }
            set { this._arqc_versao = value; }
        }

        /// <summary>
        /// Tipo de Identificação. 1 - CPF, 2 - Registor de Trabalho.
        /// </summary>
        [DBFieldInfo("arqc_tipo_identificacao", FieldType.Single)]
        public Int32 TipoIdentificacao
        {
            get { return this._arqc_tipo_identificacao; }
            set { this._arqc_tipo_identificacao = value; }
        }

        /// <summary>
        /// CEI da Empresa.
        /// </summary>
        [DBFieldInfo("arqc_cei_operadora", FieldType.Single)]
        public String OperadoraCEI
        {
            get { return this._arqc_cei_operadora; }
            set { this._arqc_cei_operadora = value; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Construtor Default.
        /// </summary>
        public ArqTransacionalUnimedConf() { }

        /// <summary>
        /// Construtor com a possibilidade de passar o ID do Arquivo de Configuração.
        /// </summary>
        /// <param name="ID"></param>
        public ArqTransacionalUnimedConf(Object ID)
        {
            this._arqc_id = ID;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Método para Carregar as Configurações pela ID da Operadora.
        /// </summary>
        /// <param name="OperadoraID"></param>
        public void CarregarPorOperadora(Object OperadoraID)
        {
            CarregarPorOperadora(OperadoraID, null);
        }

        /// <summary>
        /// Método para Carregar as Configurações pela ID da Operadora.
        /// </summary>
        public void CarregarPorOperadora(Object OperadoraID, PersistenceManager pm)
        {
            if (OperadoraID != null)
            {
                String[] strParam = new String[1];
                String[] strValue = new String[1];

                strParam[0] = "@operadora_id";
                strValue[0] = OperadoraID.ToString();

                String strSQL = "SELECT * FROM arquivo_transacional_conf WHERE arqc_operadora_id = @operadora_id";
                IList<ArqTransacionalUnimedConf> lstArqTransConf = LocatorHelper.Instance.ExecuteParametrizedQuery<ArqTransacionalUnimedConf>(strSQL, strParam, strValue, typeof(ArqTransacionalUnimedConf), pm);

                if (lstArqTransConf != null && lstArqTransConf.Count > 0)
                {
                    this._arqc_id = lstArqTransConf[0]._arqc_id;
                    this._arqc_operadora_id = lstArqTransConf[0]._arqc_operadora_id;
                    this._arqc_cod_operadora_singular = lstArqTransConf[0]._arqc_cod_operadora_singular;
                    this._arqc_cod_operadora_naunimed = lstArqTransConf[0]._arqc_cod_operadora_naunimed;
                    this._arqc_nome_operadora = lstArqTransConf[0]._arqc_nome_operadora;
                    this._arqc_cnpj_operadora = lstArqTransConf[0]._arqc_cnpj_operadora;
                    this._arqc_tipo_identificacao = lstArqTransConf[0]._arqc_tipo_identificacao;
                    this._arqc_versao = lstArqTransConf[0]._arqc_versao;
                    this._arqc_cei_operadora = lstArqTransConf[0]._arqc_cei_operadora;

                    lstArqTransConf.Clear();
                }

                lstArqTransConf = null;
            }
            else
                throw new ArgumentNullException("ID de Operadora é nulo");
        }

        #endregion
    }
}

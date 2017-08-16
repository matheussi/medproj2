using System;
using System.Collections.Generic;
using System.Text;

namespace LC.Web.PadraoSeguros.Entity
{
    /// <summary>
    /// Representa um arquivo de configuração abstrato para os arquivos transacionais.
    /// </summary>
    public abstract class AbastractArqTransacionalConf : EntityBase
    {
        protected Object _arqc_id;
        protected Object _arqc_operadora_id;
        protected String _arqc_nome_operadora;
        protected String _arqc_cnpj_operadora;
        protected String _arqc_versao;

        /// <summary>
        /// ID.
        /// </summary>
        public virtual Object ID
        {
            get { return this._arqc_id; }
            set { this._arqc_id = value; }
        }

        /// <summary>
        /// ID da Operadora.
        /// </summary>
        public virtual Object OperadoraID
        {
            get { return this._arqc_operadora_id; }
            set { this._arqc_operadora_id = value; }
        }

        /// <summary>
        /// Nome da Operadora.
        /// </summary>
        public virtual String OperadoraNome
        {
            get { return this._arqc_nome_operadora; }
            set { this._arqc_nome_operadora = value; }
        }

        /// <summary>
        /// CNPJ da Operadora.
        /// </summary>
        public virtual String OperadoraCNPJ
        {
            get { return this._arqc_cnpj_operadora; }
            set { this._arqc_cnpj_operadora = value; }
        }

        /// <summary>
        /// Versão do Arquivo.
        /// </summary>
        public virtual String ArqVersao
        {
            get { return this._arqc_versao; }
            set { this._arqc_versao = value; }
        }
    }
}

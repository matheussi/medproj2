using System;
using System.Collections.Generic;
using System.Text;

namespace LC.Web.PadraoSeguros.Entity.Filtro
{
    /// <summary>
    /// Representa um Filtro de Comissão por Produção.
    /// </summary>
    [Serializable()]
    public class ComissaoProducaoFiltro : Filtro
    {
        #region Private Fields

        private Object   _intervalo_inicial;
        private Object   _intervalo_final;
        private Object   _novo_valor;
        private DateTime _vigencia;

        #endregion

        #region Public Members

        /// <summary>
        /// Valor Inicial do Intervalo. (De)
        /// </summary>
        public Object IntervaloInicial
        {
            get { return this._intervalo_inicial; }
            set { this._intervalo_inicial = value; }
        }

        /// <summary>
        /// Valor Final do Intervalo. (Até)
        /// </summary>
        public Object IntervaloFinal
        {
            get { return this._intervalo_final; }
            set { this._intervalo_final = value; }
        }

        /// <summary>
        /// Novo Valor.
        /// </summary>
        public Object NovoValor
        {
            get { return this._novo_valor; }
            set { this._novo_valor = value; }
        }

        /// <summary>
        /// Data de Vigência para o Novo Valor.
        /// </summary>
        public DateTime Vigencia
        {
            get { return this._vigencia; }
            set { this._vigencia = value; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Construtor Default. Inicializa interavalos com -1.
        /// </summary>
        public ComissaoProducaoFiltro() 
        {
            this._intervalo_inicial = null;
            this._intervalo_final   = null;
            this._novo_valor        = null;
        }

        /// <summary>
        /// Construtor para Inicializar o Intervalo Inicial.
        /// </summary>
        /// <param name="IntervaloInicial"></param>
        public ComissaoProducaoFiltro(Object IntervaloInicial)
            : this()
        {
            this._intervalo_inicial = IntervaloInicial;
        }

        /// <summary>
        /// Contrutor para Inicializar o Intervalo Inicial e Final.
        /// </summary>
        /// <param name="IntervaloInicial">Valor Inicial. (De)</param>
        /// <param name="IntervaloFinal">Valor Final. (Até)</param>
        public ComissaoProducaoFiltro(Object IntervaloInicial, Object IntervaloFinal)
            : this(IntervaloInicial)
        {
            this._intervalo_final = IntervaloFinal;
        }

        /// <summary>
        /// Construtor para Inicializar o Intervalo Inicial, Final e Data Vigência.
        /// </summary>
        /// <param name="IntervaloInicial">Valor Inicial. (De)</param>
        /// <param name="IntervaloFinal">Valor Final. (Até)</param>
        /// <param name="Vigencia">Data de Vigência.</param>
        public ComissaoProducaoFiltro(Object IntervaloInicial, Object IntervaloFinal, DateTime Vigencia)
            : this(IntervaloInicial, IntervaloFinal)
        {
            this._vigencia = Vigencia;
        }

        /// <summary>
        /// Construtor para Inicializar o Intervalo Inicial, Final e Data Vigência.
        /// </summary>
        /// <param name="IntervaloInicial">Valor Inicial. (De)</param>
        /// <param name="IntervaloFinal">Valor Final. (Até)</param>
        /// <param name="Vigencia">Data de Vigência.</param>
        /// <param name="NovoValor">Novo Valor.</param>
        public ComissaoProducaoFiltro(Object IntervaloInicial, Object IntervaloFinal, DateTime Vigencia, Object NovoValor)
            : this(IntervaloInicial, IntervaloFinal, Vigencia)
        {
            this._novo_valor = NovoValor;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Método para Validar as restrições do Filtro.
        /// </summary>
        /// <returns>True se o Filtro estiver correto e False para incorreto e valores iguais a NULL.</returns>
        public override Boolean IsValid()
        {
            return true;
        }

        /// <summary>
        /// Método para checar se um valor se aplica ao Filtro.
        /// </summary>
        /// <param name="Value">Valor a ser checado.</param>
        /// <returns>True para valores que se aplicam. False para valores que não se aplicam.</returns>
        public Boolean CheckValue(Object Value)
        {
            return false;
        }

        /// <summary>
        /// Método para checar se um valor se aplica ao Filtro.
        /// </summary>
        /// <param name="Value">Valor a ser checado.</param>
        /// <returns>True para valores que se aplicam. False para valores que não se aplicam.</returns>
        public Boolean CheckValue(Int32 Value)
        {
            return (Value >= Convert.ToInt32(this._intervalo_inicial) && Value <= Convert.ToInt32(this._intervalo_final)) ? true : false;
        }

        #endregion
    }
}

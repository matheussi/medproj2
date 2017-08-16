namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class NumeroCartao : EntidadeBase
    {
        public NumeroCartao()
        {
            this.Via = 1;
            this.Ativo = true;
            this.Data = DateTime.Now;

            this.GerarCV();
        }

        public virtual string Numero { get; set; }
        /// <summary>
        /// Código de segurança
        /// </summary>
        public virtual string CV { get; protected set; }
        public virtual int Via { get; set; }
        /// <summary>
        /// Dígito verificador
        /// </summary>
        public virtual int DV { get; protected set; }
        public virtual DateTime Data { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual Contrato Contrato { get; set; }

        public virtual void GerarCV()
        {
            Random r1 = new Random(DateTime.Now.Millisecond);

            CV = string.Concat(r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));

            r1 = null;
        }

        public virtual string GerarNumeroInicial()
        {
            this.Numero = System.Configuration.ConfigurationManager.AppSettings["cartaoNumeroInicial"];

            this.Via = 1;

            this.GerarDigitoVerificador();

            return this.Numero;
        }

        public virtual string NumeroCompletoSemCV
        {
            get
            {
                return string.Concat(this.Numero, this.Via, this.DV);
            }
        }

        public virtual string NumeroCompletoComCV
        {
            get
            {
                return string.Concat(this.Numero, this.Via, this.DV, this.CV);
            }
        }

        public virtual int GerarDigitoVerificador()
        {
            long intNumero = Convert.ToInt64(Numero);
            //long intNumero = Convert.ToInt64(Numero + Via.ToString());

            int[] intPesos = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7, 8, 9 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;
            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }
            int intResto = (intSoma * 10) % 11;
            int intDigito = intResto;

            if (intDigito >= 10)
                intDigito = 0;

            this.DV = intDigito;

            return intDigito;
        }
    }
}

namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("numero_contrato")]
    public class NumeroCartao : IPersisteableEntity
    {
        public NumeroCartao()
        {
            this.Via = 1;
            this.Ativo = true;
            this.Data = DateTime.Now;

            this.GerarCV();
        }

        [DBFieldInfo("numerocontrato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("numerocontrato_numero", FieldType.Single)]
        public string Numero { get; set; }

        [DBFieldInfo("numerocontrato_data", FieldType.Single)]
        public DateTime Data { get; set; }

        [DBFieldInfo("numerocontrato_ativo", FieldType.Single)]
        public bool Ativo { get; set; }

        [DBFieldInfo("numerocontrato_contratoId", FieldType.Single)]
        public object ContratoId { get; set; }

        [DBFieldInfo("numerocontrato_cv", FieldType.Single)]
        public string CV { get; set; }

        [DBFieldInfo("numerocontrato_via", FieldType.Single)]
        public virtual int Via { get; set; }
        /// <summary>
        /// Dígito verificador
        /// </summary>
        [DBFieldInfo("numerocontrato_dv", FieldType.Single)]
        public virtual int DV { get; protected set; }

        public void GerarCV()
        {
            Random r1 = new Random(DateTime.Now.Millisecond);

            CV = string.Concat(r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));

            r1 = null;
        }

        public virtual void SetaDv(int dv)
        {
            this.DV = dv;
        }

        public virtual string GerarNumeroInicial()
        {
            this.Numero = System.Configuration.ConfigurationManager.AppSettings["cartaoNumeroInicial"];

            return this.Numero;
        }

        public int DigitoVerificador()
        {
            long intNumero = Convert.ToInt64(Numero);

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

namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades.Enuns;
    using System.Collections.Generic;

    public class Beneficiario
    {
        public Beneficiario() { Tipo = TipoPessoa.Fisica; }

        public virtual long ID { get; set; }
        public virtual TipoPessoa Tipo { get; set; }
        public virtual int EstadoCivilId { get; set; }
        public virtual string Nome { get; set; }
        public virtual string SexoId { get; set; }
        public virtual string CPF { get; set; }
        public virtual string RG { get; set; }
        public virtual string RgUF { get; set; }
        public virtual string RGOrgaoExp { get; set; }
        public virtual DateTime DataNascimento { get; set; }
        public virtual DateTime? DataCasamento { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string Ramal { get; set; }
        public virtual string Telefone2 { get; set; }
        public virtual string Ramal2 { get; set; }
        public virtual string Celular { get; set; }
        public virtual string CelularOperadora { get; set; }
        public virtual string Email { get; set; }
        public virtual string NomeMae { get; set; }
        public virtual decimal Altura { get; set; }
        public virtual decimal Peso { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string DeclaracaoNascimentoVivo { get; set; }
        public virtual string CNS { get; set; }
    }
}

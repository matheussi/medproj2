namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using MedProj.Entidades.Enuns;

    public class Contrato : EntidadeBase, IEquatable<Contrato>
    {
        public Contrato()
        {
            Vidas = 0;
            Rascunho = false;
            Importado = false;
            Data = DateTime.Now;
            KitSolicitado = false;
            Tipo = TipoPessoa.Fisica;
            CartaoSolicitado = false;
        }

        public override bool  Equals(object obj)
        {
            if (obj == null) return false;
            Contrato objAsPart = obj as Contrato;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return Convert.ToInt32(ID);
        }
        public virtual bool Equals(Contrato other)
        {
            if (other == null) return false;
            return (this.ID.Equals(other.ID));
        }

        public virtual TipoPessoa Tipo { get; set; }
        public virtual int Vidas { get; set; }

        public virtual string Numero { get; set; }
        public virtual bool Cancelado { get; set; }
        public virtual bool Inativo { get; set; }
        public virtual bool Rascunho { get; set; }
        public virtual string Senha { get; set; }
        public virtual bool Importado { get; set; }

        //---//

        public virtual long FilialID { get; set; }
        public virtual long EstipulanteID { get; set; }
        public virtual long OperadoraID { get; set; }
        public virtual long ContratoADMID { get; set; }
        public virtual long PlanoID { get; set; }
        public virtual int TipoContratoID { get; set; }
        /// <summary>
        /// Corretor
        /// </summary>
        public virtual long DonoID { get; set; }

        public virtual string CorretorTerceiroNome { get; set; }
        public virtual string CorretorTerceiroCPF { get; set; }

        public virtual long EnderecoReferenciaID { get; set; }
        public virtual long EnderecoCobrancaID { get; set; }

        public virtual ContratoBeneficiario ContratoBeneficiario { get; set; }

        public virtual string EmailCobranca { get; set; }
        public virtual long NumeroID { get; set; }
        /// <summary>
        /// Usuário que digitou
        /// </summary>
        public virtual long UsuarioID { get; set; }
        public virtual DateTime? DataCancelamento { get; set; }

        public virtual string ResponsavelNome { get; set; }
        public virtual string ResponsavelCPF { get; set; }
        public virtual string ResponsavelRG { get; set; }
        public virtual DateTime? ResponsavelDataNascimento { get; set; }
        public virtual string ResponsavelSexo { get; set; }
        public virtual int TipoAcomodacao { get; set; }

        public virtual DateTime Data { get; set; }
        public virtual DateTime DataAdmissao { get; set; }
        public virtual DateTime DataVigencia { get; set; }
        public virtual DateTime DataValidade { get; set; }

        public virtual string Matricula { get; set; }
        public virtual bool CartaoSolicitado { get; set; }
        public virtual bool KitSolicitado { get; set; }

        public virtual Endereco EnderecoReferencia { get; set; }

        public virtual string Ramo { get; set; }
        public virtual string Produto { get; set; }
        public virtual string NumeroApolice { get; set; }
        public virtual string CaminhoArquivo { get; set; }

        public virtual string GerarSenha()
        {
            Random r1 = new Random(DateTime.Now.Millisecond);

            this.Senha = string.Concat(r1.Next(1, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));

            r1 = null;

            return this.Senha;
        }
    }
}

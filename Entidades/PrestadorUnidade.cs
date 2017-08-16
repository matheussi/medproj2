using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using MedProj.Entidades.Enuns;
using GoogleMaps.LocationServices;

namespace MedProj.Entidades
{
    /// <summary>
    /// Representa um contrato de prestador
    /// </summary>
    public class PrestadorUnidade
    {
        public PrestadorUnidade()
        {
            //Owner = new Prestador();
            //Regiao = new Entidades.Regiao();
            //Tabela = new TabelaProcedimento();
            //TabelaPreco = new Entidades.TabelaPreco();
            //Procedimentos = new List<UnidadeProcedimento>();
            //Especialidades = new List<UnidadeEspecialidade>();
        }

        public virtual long ID { get; set; }

        public virtual Banco Banco { get; set; }
        public virtual Regiao Regiao { get; set; }
        public virtual Prestador Owner { get; set; }
        public virtual TabelaPreco TabelaPreco { get; set; }
        public virtual TabelaProcedimento Tabela { get; set; }

        public virtual IList<UnidadeProcedimento> Procedimentos { get; set; }
        public virtual IList<UnidadeEspecialidade> Especialidades { get; set; }

        public virtual PeriodicidadePagto TipoPagto { get; set; }

        public virtual TipoPessoa Tipo { get; set; }
        public virtual string Documento { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string Celular { get; set; }
        public virtual string Email { get; set; }
        public virtual string Observacoes { get; set; }

        public virtual string CEP { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string UF { get; set; }

        public virtual double Latitude { get; protected set; }
        public virtual double Longitude { get; protected set; }

        public virtual void SetaCoordenadas()
        {
            if (string.IsNullOrEmpty(this.Endereco)) return;

            GoogleLocationService g = new GoogleLocationService();
            AddressData           a = new AddressData();
            
            try
            {
                a.Address       = this.Endereco + ", " + this.Numero; //"Avenida Lins de Vasconcelos, 473";
                a.City          = this.Cidade; //"São Paulo";
                a.Country       = "Brasil"; //"São Paulo"
                a.State         = this.UF; //"SP";
                a.Zip           = this.CEP; //"01537-000";

                var ponto       = g.GetLatLongFromAddress(a);
                this.Latitude   = ponto.Latitude;
                this.Longitude  = ponto.Longitude;
                ponto           = null;
            }
            catch
            {
                //todo: log
            }

            g = null;
            a = null;
        }
    }
}

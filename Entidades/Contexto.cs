namespace MedProj.Entidades
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Data.Entity;
    using System.Configuration;
    using System.Collections.Generic;

    public class Contexto : DbContext
    {
        public virtual DbSet<Segmento> Segmentos { get; set; }
        public virtual DbSet<Beneficiario> Beneficiarios { get; set; }

        public virtual DbSet<Procedimento> Procedimentos { get; set; }
        public virtual DbSet<TabelaProcedimento> TabelasProcedimento { get; set; }

        public virtual DbSet<Especialidade> Especialidades { get; set; }

        public virtual DbSet<TabelaPreco> TabelasPreco { get; set; }
        public virtual DbSet<TabelaPrecoVigencia> TabelasVigencia { get; set; }

        public virtual DbSet<Prestador> Prestadores { get; set; }
        public virtual DbSet<PrestadorUnidade> PrestadorUnidades { get; set; }
        public virtual DbSet<UnidadeProcedimento> UnidadeProcedimentos { get; set; }
        public virtual DbSet<UnidadeEspecialidade> UnidadeEspecialidades { get; set; }
        public virtual DbSet<Banco> Bancos { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<Regiao> Regioes { get; set; }

        //public Contexto()
        //    : base(ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString)
        //{
        //    Database.SetInitializer<Contexto>(null);
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Procedimento>().ToTable("procedimento");
            modelBuilder.Entity<TabelaProcedimento>().ToTable("tabela_procedimento");
            modelBuilder.Entity<Especialidade>().ToTable("especialidade");

            modelBuilder.Entity<TabelaPreco>().ToTable("tabela_preco");


            modelBuilder.Entity<TabelaPrecoVigencia>().ToTable("tabela_preco_vigencia");

            modelBuilder.Entity<Beneficiario>().ToTable("beneficiario");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Altura).HasColumnName("beneficiario_altura");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Celular).HasColumnName("beneficiario_celular");
            modelBuilder.Entity<Beneficiario>().Property(b => b.CelularOperadora).HasColumnName("beneficiario_celularOperadora");
            modelBuilder.Entity<Beneficiario>().Property(b => b.CNS).HasColumnName("beneficiario_cns");
            modelBuilder.Entity<Beneficiario>().Property(b => b.CPF).HasColumnName("beneficiario_cpf");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Data).HasColumnName("beneficiario_data");
            modelBuilder.Entity<Beneficiario>().Property(b => b.DataCasamento).HasColumnName("beneficiario_dataCasamento");
            modelBuilder.Entity<Beneficiario>().Property(b => b.DataNascimento).HasColumnName("beneficiario_dataNascimento");
            modelBuilder.Entity<Beneficiario>().Property(b => b.DeclaracaoNascimentoVivo).HasColumnName("beneficiario_declaracaoNascimentoVivo");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Email).HasColumnName("beneficiario_email");
            modelBuilder.Entity<Beneficiario>().Property(b => b.EstadoCivilId).HasColumnName("beneficiario_estadoCivilId");
            modelBuilder.Entity<Beneficiario>().Property(b => b.ID).HasColumnName("beneficiario_id");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Nome).HasColumnName("beneficiario_nome");
            modelBuilder.Entity<Beneficiario>().Property(b => b.NomeMae).HasColumnName("beneficiario_nomeMae");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Peso).HasColumnName("beneficiario_peso");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Ramal).HasColumnName("beneficiario_ramal");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Ramal2).HasColumnName("beneficiario_ramal2");
            modelBuilder.Entity<Beneficiario>().Property(b => b.RG).HasColumnName("beneficiario_rg");
            modelBuilder.Entity<Beneficiario>().Property(b => b.RGOrgaoExp).HasColumnName("beneficiario_rgOrgaoExp");
            modelBuilder.Entity<Beneficiario>().Property(b => b.RgUF).HasColumnName("beneficiario_rgUF");
            modelBuilder.Entity<Beneficiario>().Property(b => b.SexoId).HasColumnName("beneficiario_sexo");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Telefone).HasColumnName("beneficiario_telefone");
            modelBuilder.Entity<Beneficiario>().Property(b => b.Telefone2).HasColumnName("beneficiario_telefone2");
            modelBuilder.Entity<Beneficiario>().HasKey(b => b.ID);

            modelBuilder.Entity<Prestador>().ToTable("prestador");
            modelBuilder.Entity<PrestadorUnidade>().ToTable("prestador_unidade");
            modelBuilder.Entity<UnidadeProcedimento>().ToTable("unidade_procedimento");
            modelBuilder.Entity<UnidadeEspecialidade>().ToTable("unidade_especialidade");
            modelBuilder.Entity<Banco>().ToTable("banco");

            modelBuilder.Entity<Regiao>().ToTable("regiao");

            modelBuilder.Entity<Segmento>().ToTable("segmento");

            modelBuilder.Entity<Usuario>().ToTable("usuarios");
        }
    }
}

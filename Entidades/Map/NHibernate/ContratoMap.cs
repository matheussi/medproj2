namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ContratoMap : ClassMap<Contrato>
    {
        public ContratoMap()
        {
            base.Table("contrato");
            base.Id(c => c.ID).Column("contrato_id").GeneratedBy.Identity();

            base.Map(c => c.Tipo).Column("contrato_tipoPessoa").CustomType(typeof(int));
            base.Map(c => c.Vidas).Column("contrato_qtdVidas");

            base.Map(c => c.Numero).Column("contrato_numero");
            base.Map(c => c.Senha).Column("contrato_senha");

            base.Map(c => c.Ramo).Column("contrato_ramo").Nullable();
            base.Map(c => c.Produto).Column("contrato_produto").Nullable();
            base.Map(c => c.NumeroApolice).Column("contrato_numeroApolice").Nullable();
            base.Map(c => c.CaminhoArquivo).Column("contrato_caminhoArquivo").Nullable();

            base.Map(c => c.Inativo).Column("contrato_inativo");
            base.Map(c => c.Cancelado).Column("contrato_cancelado");

            base.Map(c => c.FilialID).Column("contrato_filialId");
            base.Map(c => c.EstipulanteID).Column("contrato_estipulanteId");
            base.Map(c => c.OperadoraID).Column("contrato_operadoraId");
            base.Map(c => c.ContratoADMID).Column("contrato_contratoAdmId");
            base.Map(c => c.PlanoID).Column("contrato_planoId");
            base.Map(c => c.TipoContratoID).Column("contrato_tipoContratoId");
            base.Map(c => c.DonoID).Column("contrato_donoId");
            base.Map(c => c.CorretorTerceiroNome).Column("contrato_corretorTerceiroNome");
            base.Map(c => c.CorretorTerceiroCPF).Column("contrato_corretorTerceiroCPF");
            base.Map(c => c.EnderecoReferenciaID).Column("contrato_enderecoReferenciaId");
            base.Map(c => c.EnderecoCobrancaID).Column("contrato_enderecoCobrancaId");
            base.Map(c => c.EmailCobranca).Column("contrato_emailCobranca");
            base.Map(c => c.NumeroID).Column("contrato_numeroId");
            base.Map(c => c.DataVigencia).Column("contrato_vigencia");
            base.Map(c => c.UsuarioID).Column("contrato_usuarioId");
            base.Map(c => c.DataCancelamento).Column("contrato_dataCancelamento");
            base.Map(c => c.ResponsavelNome).Column("contrato_responsavelNome");
            base.Map(c => c.ResponsavelCPF).Column("contrato_responsavelCPF");
            base.Map(c => c.ResponsavelRG).Column("contrato_responsavelRG");
            base.Map(c => c.ResponsavelDataNascimento).Column("contrato_responsavelDataNascimento");
            base.Map(c => c.ResponsavelSexo).Column("contrato_responsavelSexo");
            base.Map(c => c.TipoAcomodacao).Column("contrato_tipoAcomodacao");
            base.Map(c => c.DataAdmissao).Column("contrato_admissao");
            base.Map(c => c.DataValidade).Column("contrato_validade").Not.Nullable();
            base.Map(c => c.Matricula).Column("contrato_numeroMatricula");
            base.Map(c => c.KitSolicitado).Column("contrato_kitSolicitado");
            base.Map(c => c.CartaoSolicitado).Column("contrato_cartaoSolicitado");
            base.Map(c => c.Rascunho).Column("contrato_rascunho");

            base.Map(c => c.Data).Column("contrato_data").Not.Update();

            base.References(c => c.EnderecoReferencia).Column("contrato_enderecoReferenciaId")
                .Not.Insert()
                .Not.Update();

            //base.HasOne(c => c.ContratoBeneficiario);
        }
    }
}
namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaImportacaoMap : ClassMap<AgendaImportacao>
    {
        public AgendaImportacaoMap()
        {
            base.Table("importacao");
            base.Id(i => i.ID).Column("importacao_id").GeneratedBy.Identity();

            base.Map(i => i.Descricao).Column("importacao_descricao");

            base.Map(i => i.DataCriacao).Column("importacao_dataCriacao");
            base.Map(i => i.DataProcessamento).Column("importacao_dataExecucao");
            base.Map(i => i.DataConclusao).Column("importacao_dataConclusao");
            //base.Map(i => i.Processado).Column("importacao_processado");

            //base.Map(i => i.CaminhoArquivoBeneficiarios).Column("importacao_arquivoBeneficiario");
            base.Map(i => i.Arquivo).Column("importacao_arquivo");
            base.Map(i => i.Ativa).Column("importacao_ativa");

            base.Map(i => i.Erro).Column("importacao_erro");

            base.References(i => i.Autor).Column("importacao_usuarioId").Not.Nullable();

            base.References(i => i.Filial).Column("importacao_filialId").Not.Nullable();
            base.References(i => i.AssociadoPj).Column("importacao_associadoPjId").Not.Nullable();
            base.References(i => i.Operadora).Column("importacao_operadoraId").Not.Nullable();
            base.References(i => i.Contrato).Column("importacao_contratoAdmId").Not.Nullable();
            base.References(i => i.Plano).Column("importacao_planoId").Not.Nullable();
            base.References(i => i.ContratoPjId).Column("importacao_contratoPjId").Nullable();

            base.Map(i => i.NaoCriticarCPF).Column("importacao_naocriticarcpf").Not.Nullable();
        }
    }
}
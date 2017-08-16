namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaExportacaoKitMap : ClassMap<AgendaExportacaoKit>
    {
        public AgendaExportacaoKitMap()
        {
            base.Table("exportacaoKit");
            base.Id(i => i.ID).Column("exportacao_id").GeneratedBy.Identity();

            base.Map(i => i.Descricao).Column("exportacao_descricao");

            base.Map(i => i.DataCriacao).Column("exportacao_dataCriacao");
            base.Map(i => i.DataProcessamento).Column("exportacao_dataExecucao");
            base.Map(i => i.DataConclusao).Column("exportacao_dataConclusao");

            base.Map(i => i.Ativa).Column("exportacao_ativa");

            base.References(i => i.Autor).Column("exportacao_usuarioId").Not.Nullable();

            base.References(i => i.Filial).Column("exportacao_filialId").Not.Nullable();
            base.References(i => i.AssociadoPj).Column("exportacao_associadoPjId").Not.Nullable();
            base.References(i => i.Operadora).Column("exportacao_operadoraId").Not.Nullable();
            base.References(i => i.Contrato).Column("exportacao_contratoAdmId").Not.Nullable();
        }
    }
}
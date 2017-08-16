namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaAtribuicaoProcedimentoMap : ClassMap<AgendaAtribuicaoProcedimento>
    {
        public AgendaAtribuicaoProcedimentoMap()
        {
            base.Table("agenda_atribuicao_procedimento");
            base.Id(c => c.ID).Column("aap_id").GeneratedBy.Identity();

            base.Map(c => c.Erro).Column("aap_erro");
            base.Map(c => c.Arquivo).Column("aap_arquivo");
            base.Map(c => c.Ativa).Column("aap_ativa");

            base.Map(c => c.DataConclusao).Column("aap_dataconclusao");
            base.Map(c => c.DataCriacao).Column("aap_datacriacao");
            base.Map(c => c.DataProcessamento).Column("aap_dataprocessamento");
            base.Map(c => c.Descricao).Column("aap_descricao");
            base.Map(c => c.TabelaDePrecoViaPlanilha).Column("aap_tabelaPrecoDaPlanilha");

            //base.Map(c => c.Log).Column("aap_arquivo");

            base.Map(c => c.Processado).Column("aap_processado");
            base.References(c => c.Tabela).Column("aap_tabelaPrecoId");

            base.HasManyToMany<PrestadorUnidade>(a => a.Contratos)
                .Table("agenda_atribuicao_procedimento_contrato")
                .ParentKeyColumn("aapc_agendaId")
                .ChildKeyColumn("aapc_contratoId");
        }
    }

    public class AgendaAtribProcedRESULTADOMap : ClassMap<AgendaAtribProcedRESULTADO>
    {
        public AgendaAtribProcedRESULTADOMap()
        {
            base.Table("agenda_atribuicao_procedimento_log");
            base.Id(c => c.ID).Column("aapl_id").GeneratedBy.Identity();

            base.References(c => c.Agenda).Column("aapl_agendaId");
            base.References(c => c.ContratoDePrestador).Column("aapl_contratoId");
            base.References(c => c.Procedimento).Column("aapl_procedimentoId");

            base.Map(c => c.Mensagem).Column("aapl_mensagem");
            base.Map(c => c.Ok).Column("aapl_ok");
        }
    }
}
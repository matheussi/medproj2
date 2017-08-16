namespace MedProj.Entidades.Enuns
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public enum TipoRemessaCnab : int
    {
        Novo = 0,
        Alteracao
    }

    public enum TipoPessoa : int
    {
        Fisica,
        Juridica
    }

    public enum TipoConta : int
    {
        Corrente,
        Poupanca
    }

    public enum TipoUsuario : int
    {
        Administrador,
        Operador,
        ContratoDePrestador
    }

    public enum TipoConfig : int
    {
        AvisoDePagamento,
        AvisoDeVencimentoProximo,
        AvisoDeVencimentoPassado
    }

    public enum TipoDataValidade : int
    {
        Indefinido,
        DataFixa,
        MesesAPartirDaVigencia
    }

    public enum TipoArquivoBaixa : int
    {
        Itau,
        DepositoIdentificadoItau
    }

    public enum TipoItemBaixa : int
    {
        TituloLiquidado,
        TituloNaoEncontrado,
        TituloCanceladoLiquidado,
        DepositoBaixado,
        DepositoNaoIdentificado,
        DepositoBaixadoContratoCancelado,
        PagamentoDuplicado,
        OcorrenciaSemLiquidacao,
        AlteracaoRejeitada
    }

    public enum TipoMovimentacao : int
    {
        Credito,
        Debito
    }

    public enum AgendaStatus : int
    {
        Indefinido,
        Pendente,
        Concluido
    }

    public enum AgendaImportacaoItemLogStatus : int
    {
        Ok,
        Erro,
        Alerta
    }

    public enum PeriodicidadePagto : int
    {
        Mensal,
        Quinzenal,
        Semanal
    }

    public enum FormaPagtoAtendimento : int
    {
        Cartao,
        Dinheiro,
        Indefinido
    }

    public enum ComissaoInicioConfTipo : int
    {
        Vitalicio,
        Parcela1,
        Parcela2,
        Parcela3,
        Parcela4,
        Parcela5
    }

    public enum TipoFiltroPendenciaCNAB : int
    {
        Hoje,
        Ontem,
        Periodo
    }

    public enum TipoDono : int
    {
        Beneficiario,
        CorretorOuSupervisor,
        Operadora,
        Filial,
        Produtor
    }
}

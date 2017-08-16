namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class RegraComissaoFacade : FacadeBase
    {
        #region singleton 

        static RegraComissaoFacade _instance;
        public static RegraComissaoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new RegraComissaoFacade(); }
                return _instance;
            }
        }

        #endregion

        public long Salvar(RegraComissao obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public RegraComissao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                RegraComissao obj = sessao.Query<RegraComissao>()
                    .Fetch(t => t.Estipulante)
                    .Where(t => t.ID == id).Single();

                return obj;
            }
        }

        public List<RegraComissao> Carregar(string nome)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nome))
                {
                    return sessao.Query<RegraComissao>()
                        .Fetch(es => es.Estipulante)
                        .OrderBy(e => e.Nome).ToList();
                }
                else
                {
                    return sessao.Query<RegraComissao>()
                        .Fetch(es => es.Estipulante)
                        .Where(e => e.Nome.Contains(nome))
                        .OrderBy(e => e.Nome).ToList();
                }
            }
        }

        // Item

        public long SalvarItem(RegraComissaoItem obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public void ExcluirItem(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        RegraComissaoItem obj = sessao.Query<RegraComissaoItem>().Where(p => p.ID == id).Single();
                        sessao.Delete(obj);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<RegraComissaoItem> CarregarItens(long regraId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<RegraComissaoItem>()
                    .Fetch(r => r.Corretor)
                    .Where(e => e.RegraID == regraId)
                    .OrderBy(e => e.Corretor.Nome).OrderBy(r => r.Parcela)
                    .ToList();
            }
        }

        public bool CorretorJaAdicionado(long regraId, long corretorId, int parcela)
        {
            RegraComissaoItem ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegraComissaoItem>()
                        .Where(e => e.RegraID == regraId && e.Corretor.ID == corretorId && e.Parcela == parcela)
                        .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }

        public bool CorretorJaAdicionado(long regraId, long corretorId, int parcela, long itemId)
        {
            RegraComissaoItem ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegraComissaoItem>()
                        .Where(e => e.RegraID == regraId && e.Corretor.ID == corretorId && e.Parcela == parcela && e.ID != itemId)
                        .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }

        public RegraComissaoItem CarregarItem(long id)
        {
            using (var sessao = ObterSessao())
            {
                RegraComissaoItem obj = sessao.Query<RegraComissaoItem>()
                    .Fetch(t => t.Corretor)
                    .Where(t => t.ID == id).Single();

                return obj;
            }
        }

        #region // Corretor - em desuso 

        public long SalvarCorretor(RegracomCorretor obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public void ExcluirCorretor(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        RegracomCorretor obj = sessao.Query<RegracomCorretor>().Where(p => p.ID == id).Single();
                        sessao.Delete(obj);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<RegracomCorretor> CarregarCorretores(long regraId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<RegracomCorretor>()
                    .Fetch(e => e.Corretor)
                    .Fetch(e => e.Regra)
                    .Where(e => e.Regra.ID == regraId)
                    .OrderBy(e => e.Corretor.Nome)
                    .ToList();
            }
        }

        public bool ExisteCorretorParaRegra(long regraId, long corretorId)
        {
            RegracomCorretor ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegracomCorretor>()
                        .Where(e => e.Regra.ID == regraId && e.Corretor.ID == corretorId)
                        .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }

        #endregion

        #region // Contrato

        public long SalvarContrato(RegracomContrato obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public void SalvarContratoLOTE(List<RegracomContrato> lote)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        RegracomContrato ret = null;

                        foreach (var obj in lote)
                        {
                            if (obj.TemId)
                            {
                                continue;
                            }
                            else
                            {
                                ret = sessao.Query<RegracomContrato>()
                                    .Where(e => e.Contrato.ID == obj.Contrato.ID) //e.Regra.ID == regraId && 
                                    .FirstOrDefault();

                                if (ret != null) continue;

                                obj.Regra = sessao.Get<RegraComissao>(obj.Regra.ID);
                                obj.Contrato = sessao.Get<Contrato>(obj.Contrato.ID);
                                sessao.Save(obj);
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ExcluirContrato(long regraid, long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        RegracomContrato obj = sessao.Query<RegracomContrato>().Where(p => p.Regra.ID == regraid && p.Contrato.ID == contratoId).Single();

                        var excecoes = sessao.Query<RegraComissaoItemExcecao>()
                            .Where(e => e.RegraID == regraid && e.ContratoID == contratoId)
                            .ToList();

                        if (excecoes != null)
                        {
                            foreach (var ex in excecoes)
                            {
                                sessao.Delete(ex);
                            }
                        }

                        sessao.Delete(obj);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<RegracomContrato> CarregarContratos(long regraId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<RegracomContrato>()
                    .Fetch(e => e.Contrato)
                    .Fetch(e => e.Regra)
                    .Where(e => e.Regra.ID == regraId)
                    .OrderBy(e => e.Contrato.Numero)
                    .ToList();
            }
        }

        public List<Contrato> CarregarContratosDoAssociadoPj(long associadoPjId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Contrato>()
                    .Where(c => c.EstipulanteID == associadoPjId)
                    .OrderBy(c => c.Numero)
                    .ToList();
            }
        }

        public bool ExisteContratoParaRegra(long regraId, long contratoId)
        {
            RegracomContrato ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegracomContrato>()
                        .Where(e => e.Contrato.ID == contratoId) //e.Regra.ID == regraId && 
                        .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }

        public Contrato CarregarContrato(long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Contrato>()
                    .Where(c => c.ID == contratoId)
                    .Single();
            }
        }

        #endregion

        #region // Excecoes - em desuso 

        public long SalvarExcecao(ComissaoInicioConf obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public void ExcluirExcecao(long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        ComissaoInicioConf obj = sessao.Query<ComissaoInicioConf>().Where(p => p.ContratoId == contratoId).SingleOrDefault();

                        if(obj != null) sessao.Delete(obj);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public ComissaoInicioConf CarregarExcecaoPorContratoId(long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ComissaoInicioConf>()
                    .Where(e => e.ContratoId == contratoId)
                    .SingleOrDefault();
            }
        }

        #endregion 

        // Item Excecao

        public long SalvarItemExcecao(RegraComissaoItemExcecao obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        //obj.Corretor = sessao.Get<Corretor>(obj.Corretor.ID);

                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        //public long SalvarItemExcecao2(RegraComissaoItemExcecao2 obj)
        //{
        //    using (var sessao = ObterSessao())
        //    {
        //        using (ITransaction tran = sessao.BeginTransaction())
        //        {
        //            try
        //            {
        //                if (obj.TemId)
        //                {
        //                    sessao.Update(obj);
        //                }
        //                else
        //                {
        //                    sessao.Save(obj);
        //                }

        //                tran.Commit();
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                throw;
        //            }
        //        }
        //    }

        //    return obj.ID;
        //}

        public void ExcluirItemExcecao(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        RegraComissaoItemExcecao obj = sessao.Query<RegraComissaoItemExcecao>().Where(p => p.ID == id).Single();
                        sessao.Delete(obj);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<RegraComissaoItemExcecao> CarregarItensExcecao(long regraId, long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<RegraComissaoItemExcecao>()
                    .Fetch(e => e.Corretor)
                    .Where(e => e.RegraID == regraId && e.ContratoID == contratoId)
                    .OrderBy(r => r.Corretor.Nome).OrderBy(r => r.Parcela)
                    .ToList();
            }
        }

        public bool ParcelaJaAdicionada(long regraId, long contratoId, long corretorId, int parcela)
        {
            RegraComissaoItemExcecao ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegraComissaoItemExcecao>()
                    .Where(e => e.RegraID == regraId && e.Corretor.ID == corretorId && e.Parcela == parcela && e.ContratoID == contratoId)
                    .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }

        public bool NaoComissionamentoJaAdicionado(long regraId, long contratoId, long corretorId)
        {
            RegraComissaoItemExcecao ret = null;

            using (var sessao = ObterSessao())
            {
                ret = sessao.Query<RegraComissaoItemExcecao>()
                    .Where(e => e.RegraID == regraId && e.Corretor.ID == corretorId && e.NaoComissionado == true && e.ContratoID == contratoId)
                    .FirstOrDefault();
            }

            if (ret != null)
                return true;
            else
                return false;
        }
    }
}

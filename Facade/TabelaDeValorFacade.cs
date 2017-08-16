namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public sealed class TabelaDeValorFacade
    {
        #region Singleton 

        static TabelaDeValorFacade _instance;
        public static TabelaDeValorFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new TabelaDeValorFacade(); }
                return _instance;
            }
        }
        #endregion

        private TabelaDeValorFacade() { }

        public void RecalcularTaxaEmPlanos(Object tabelaValorID, Taxa taxa)
        {
            IList<Plano> planos = Plano.CarregaPlanosDaTabelaDeValor(tabelaValorID);

            if (planos == null) { return; }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                TabelaValor tabela = new TabelaValor(tabelaValorID);
                pm.Load(tabela);

                foreach (Plano plano in planos)
                {
                    IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(tabela.ID, plano.ID, pm);
                    if (itens == null || itens.Count == 0) { continue; }

                    foreach (TabelaValorItem item in itens)
                    {
                        item.AplicaTaxa(taxa, false);
                        pm.Save(item);
                    }
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        [Obsolete("Usar a outra sobrecarga", true)]
        public void Duplicar(Object tabelaValorID, DateTime vigencia, Taxa taxa)
        {
            IList<Plano> planos = Plano.CarregaPlanosDaTabelaDeValor(tabelaValorID);

            PersistenceManager pm = new PersistenceManager();
            //pm.TransactionContext();

            try
            {
                TabelaValor tabela = new TabelaValor(tabelaValorID);
                pm.Load(tabela);

                TabelaValor novaTabela = new TabelaValor();
                novaTabela.ContratoID = tabela.ContratoID;
                //novaTabela.Data = vigencia;
                pm.Save(novaTabela);

                taxa.TabelaValorID = novaTabela.ID;
                pm.Save(taxa);

                if (planos != null) //se tem planos, duplica os itens de tabela de valor para cada plano, aplicando a nova taxa
                {
                    foreach (Plano plano in planos)
                    {
                        IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(tabela.ID, plano.ID);
                        if (itens == null || itens.Count == 0) { continue; }

                        foreach (TabelaValorItem item in itens)
                        {
                            item.ID = null;
                            item.TabelaID = novaTabela.ID;
                            item.AplicaTaxa(taxa, true);
                            pm.Save(item);
                        }
                    }
                }

                //pm.Commit();
            }
            catch (Exception ex)
            {
                //pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        public void Duplicar(Object tabelaValorID, DateTime inicio, DateTime fim, Taxa taxa)
        {
            IList<Plano> planos = Plano.CarregaPlanosDaTabelaDeValor(tabelaValorID);

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                TabelaValor tabela = new TabelaValor(tabelaValorID);
                pm.Load(tabela);

                TabelaValor novaTabela = new TabelaValor();
                novaTabela.ContratoID = tabela.ContratoID;
                novaTabela.Inicio = inicio;
                novaTabela.Fim = fim;
                pm.Save(novaTabela);

                taxa.TabelaValorID = novaTabela.ID;
                pm.Save(taxa);

                if (planos != null) //se tem planos, duplica os itens de tabela de valor para cada plano, aplicando a nova taxa
                {
                    foreach (Plano plano in planos)
                    {
                        IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(tabela.ID, plano.ID, pm);
                        if (itens == null || itens.Count == 0) { continue; }

                        foreach (TabelaValorItem item in itens)
                        {
                            item.ID = null;
                            item.TabelaID = novaTabela.ID;
                            item.AplicaTaxa(taxa, true);
                            pm.Save(item);
                        }
                    }
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }
    }
}

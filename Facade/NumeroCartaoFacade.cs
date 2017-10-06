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

    public class NumeroCartaoFacade : FacadeBase
    {
        NumeroCartaoFacade() { }

        #region singleton  

        static NumeroCartaoFacade _instancia;
        public static NumeroCartaoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new NumeroCartaoFacade (); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(NumeroCartao numeroCartao)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        numeroCartao.Contrato = sessao.Get<Contrato>(numeroCartao.Contrato.ID);
                        sessao.SaveOrUpdate(numeroCartao);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return numeroCartao.ID;
        }

        public NumeroCartao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<NumeroCartao>()
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public bool CancelarNumerosDeCartao(string[] numeros, string mensagem, long usuarioId)
        {
            #region remover isso

            List<string> listNum = new List<string>();

            listNum.Add("6370870002636915");
            listNum.Add("6370870008416216");
            listNum.Add("6370870008416119");
            listNum.Add("6370870035390518");
            listNum.Add("6370870003170111");
            listNum.Add("6370870003170316");
            listNum.Add("6370870003170413");
            listNum.Add("6370870003170510");
            listNum.Add("6370870003170618");
            listNum.Add("6370870003170715");
            listNum.Add("6370870003170812");
            listNum.Add("6370870003170910");
            listNum.Add("6370870003170219");
            listNum.Add("6370870003171010");
            listNum.Add("6370870003171118");
            listNum.Add("6370870003171312");
            listNum.Add("6370870003171819");
            listNum.Add("6370870003171215");
            listNum.Add("6370870003171410");
            listNum.Add("6370870003171517");
            listNum.Add("6370870003171614");
            listNum.Add("6370870003171711");
            listNum.Add("6370870003171916");
            listNum.Add("6370870002656525");
            listNum.Add("6370870009527216");
            listNum.Add("6370870002655715");
            listNum.Add("6370870002656410");
            listNum.Add("6370870009527518");
            listNum.Add("6370870002656010");
            listNum.Add("6370870009528115");
            listNum.Add("6370870002656118");
            listNum.Add("6370870009527615");
            listNum.Add("6370870009527410");
            listNum.Add("6370870002655910");
            listNum.Add("6370870008428117");
            listNum.Add("6370870009527313");
            listNum.Add("6370870002655812");
            listNum.Add("6370870003172211");
            listNum.Add("6370870003172114");
            listNum.Add("6370870010455113");
            listNum.Add("6370870002995419");
            listNum.Add("6370870002995311");
            listNum.Add("6370870002995516");
            listNum.Add("6370870008445615");
            listNum.Add("6370870008445810");
            listNum.Add("6370870008445712");
            listNum.Add("6370870008445410");
            listNum.Add("6370870008445313");
            listNum.Add("6370870008445216");
            listNum.Add("6370870009986911");
            listNum.Add("6370870008417212");
            listNum.Add("6370870008417310");
            listNum.Add("6370870008417115");
            listNum.Add("6370870006288210");
            listNum.Add("6370870006288318");
            listNum.Add("6370870006288415");
            listNum.Add("6370870009986318");
            listNum.Add("6370870006288814");
            listNum.Add("6370870008777618");
            listNum.Add("6370870035186111");
            listNum.Add("6370870006289012");
            listNum.Add("6370870006289217");
            listNum.Add("6370870010194611");
            listNum.Add("6370870006290312");
            listNum.Add("6370870006290410");
            listNum.Add("6370870006290614");
            listNum.Add("6370870010455415");
            listNum.Add("6370870010455512");
            listNum.Add("6370870002995710");
            listNum.Add("6370870010465917");
            listNum.Add("6370870040690014");
            listNum.Add("6370870002995915");
            listNum.Add("6370870035306710");
            listNum.Add("6370870008560210");
            listNum.Add("6370870008560317");
            listNum.Add("6370870008560015");
            listNum.Add("6370870008559815");
            listNum.Add("6370870008224414");
            listNum.Add("6370870008224813");
            listNum.Add("6370870008224317");
            listNum.Add("6370870008224210");
            listNum.Add("6370870008224716");
            listNum.Add("6370870008224511");
            listNum.Add("6370870008224619");
            listNum.Add("6370870002841110");
            listNum.Add("6370870002841810");
            listNum.Add("6370870002841012");
            listNum.Add("6370870002841519");
            listNum.Add("6370870002841217");
            listNum.Add("6370870002840814");
            listNum.Add("6370870002841616");
            listNum.Add("6370870002840911");
            listNum.Add("6370870002842418");
            listNum.Add("6370870002840717");
            listNum.Add("6370870002840610");
            listNum.Add("6370870002842019");
            listNum.Add("6370870002841314");
            listNum.Add("6370870008423611");
            listNum.Add("6370870008423913");
            listNum.Add("6370870010461512");
            listNum.Add("6370870002605211");
            listNum.Add("6370870002605319");
            listNum.Add("6370870002605610");
            listNum.Add("6370870002605718");
            listNum.Add("6370870002605513");
            listNum.Add("6370870002604819");
            listNum.Add("6370870002605017");
            listNum.Add("6370870002605114");
            listNum.Add("6370870002604916");
            listNum.Add("6370870008886417");
            listNum.Add("6370870002608016");
            listNum.Add("6370870002608717");
            listNum.Add("6370870002609217");
            listNum.Add("6370870002608610");
            listNum.Add("6370870002609012");
            listNum.Add("6370870002608318");
            listNum.Add("6370870002607613");
            listNum.Add("6370870002608113");
            listNum.Add("6370870002609411");
            listNum.Add("6370870002608210");
            listNum.Add("6370870002607915");
            listNum.Add("6370870002607818");
            listNum.Add("6370870002608415");
            listNum.Add("6370870002609314");
            listNum.Add("6370870002607710");
            listNum.Add("6370870002609110");
            listNum.Add("6370870002994412");
            listNum.Add("6370870002994510");
            listNum.Add("6370870002994617");
            listNum.Add("6370870002994714");
            listNum.Add("6370870002994811");
            listNum.Add("6370870002994919");
            listNum.Add("6370870002995010");
            listNum.Add("6370870002995117");
            listNum.Add("6370870010193518");
            listNum.Add("6370870002636516");
            listNum.Add("6370870002636214");
            listNum.Add("6370870002636419");
            listNum.Add("6370870002636613");

            numeros = listNum.ToArray();

            #endregion

            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    IDbCommand cmd = sessao.Connection.CreateCommand();
                    tran.Enlist(cmd);

                    try
                    {
                        int ret = 0;
                        object numeroId = null;
                        object contratoId = null;

                        foreach (string num in numeros)
                        {
                            cmd.CommandText = string.Concat("select contrato_id from contrato where contrato_numero='", num, "'");
                            contratoId = cmd.ExecuteScalar();

                            if (contratoId != null)
                            {
                                cmd.CommandText = string.Concat("select contrato_numeroId from contrato where contrato_id=", contratoId);
                                numeroId = cmd.ExecuteScalar();

                                if (numeroId != null)
                                {
                                    cmd.CommandText = string.Concat("update contrato set contrato_cancelado=1 where contrato_id=", contratoId);
                                    ret = cmd.ExecuteNonQuery();

                                    cmd.CommandText = string.Concat("update numero_contrato set numerocontrato_ativo=0 where numerocontrato_id=", numeroId);
                                    ret = cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

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

    public class ProcedimentoFacade : FacadeBase
    {
        ProcedimentoFacade() { }

        #region singleton 

        static ProcedimentoFacade _instancia;
        public static ProcedimentoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new ProcedimentoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(Procedimento procedimento, long segmentoId, bool salvaEspecialidade)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(procedimento);

                        if (salvaEspecialidade)
                        {
                            Especialidade ret = sessao.Query<Especialidade>()
                                .Where(e => e.SegmentoId == segmentoId && e.Nome == procedimento.Especialidade)
                                .FirstOrDefault();

                            if (ret == null)
                            {
                                ret = new Especialidade();
                                ret.Codigo = "";
                                ret.Nome = procedimento.Especialidade;
                                ret.SegmentoId = segmentoId;
                                sessao.Save(ret);
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

            return procedimento.ID;
        }

        public Procedimento Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Procedimento>().Where(p => p.ID == id).Single();
            }
        }

        public List<Procedimento> Carregar(string nomeOuCodigo)
        {
            using (var sessao = ObterSessao())
            {
                if (!string.IsNullOrEmpty(nomeOuCodigo))
                {
                    return sessao.Query<Procedimento>()
                        .Where(p => p.Nome.Contains(nomeOuCodigo) || p.Codigo.ToString().Contains(nomeOuCodigo))
                        .OrderBy(p => p.Nome)
                        .ToList();
                }
                else
                    return null;
            }
        }
        public List<Procedimento> Carregar()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Procedimento>()
                    .OrderBy(p => p.Nome)
                    .ToList();
            }
        }

        public List<Procedimento> CarregarPorEspecialidade(string especialidade)
        {
            using (var sessao = ObterSessao())
            {
                if (!string.IsNullOrEmpty(especialidade))
                {
                    return sessao.Query<Procedimento>()
                        .Where(p => p.Especialidade == retiraAcentos(especialidade).ToUpper() || p.Especialidade == especialidade.ToUpper())
                        .OrderBy(p => p.Nome)
                        .ToList();
                }
                else
                    return null;
            }
        }

        String retiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }

        public List<Procedimento> CarregarPorTabela(long idTabelaProcedimento, string nomeProcedimento)
        {
            using (var sessao = ObterSessao())
            {
                if (string.IsNullOrEmpty(nomeProcedimento))
                {
                    return sessao.Query<Procedimento>()
                        .Where(p => p.Tabela.ID == idTabelaProcedimento)
                        .OrderBy(p => p.Nome)
                        .ToList();
                }
                else
                {
                    return sessao.Query<Procedimento>()
                        .Where(p => p.Tabela.ID == idTabelaProcedimento && (p.Nome.Contains(nomeProcedimento) || p.Codigo.ToString().Contains(nomeProcedimento)))
                        .OrderBy(p => p.Especialidade)
                        .OrderBy(p => p.Nome)
                        .ToList();
                }
            }
        }

        public List<Procedimento> CarregarPorTabela(long idTabelaProcedimento, string especialidade, string categoria)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Procedimento>()
                    .Where(p => p.Tabela.ID == idTabelaProcedimento && p.Especialidade == especialidade && p.Categoria == categoria)
                    .OrderBy(p => p.Nome)
                    .ToList();
            }
        }

        public List<UnidadeProcedimento> CarregarProcedimentosDaUnidade(long unidadeId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<UnidadeProcedimento>()
                    .Fetch(p => p.Procedimento)
                    .Fetch(p => p.Unidade)
                    .Fetch(p => p.TabelaPreco).ThenFetchMany(t => t.Vigencias)
                    .Where(p => p.Unidade.ID == unidadeId)
                    .OrderBy(p => p.Procedimento.Especialidade)
                    .OrderBy(p => p.Procedimento.Nome)
                    .ToList();
            }
        }

        public List<UnidadeProcedimento> CarregarProcedimentosDaUnidade(long unidadeId, string nomeOuCodigo)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<UnidadeProcedimento>()
                    .Fetch(p => p.Procedimento)
                    .Fetch(p => p.Unidade)
                    .Fetch(p => p.TabelaPreco).ThenFetchMany(t => t.Vigencias)
                    .Where(p => p.Unidade.ID == unidadeId && (p.Procedimento.Codigo.ToString().Contains(nomeOuCodigo) || p.Procedimento.Nome.Contains(nomeOuCodigo)))
                    .ToList();
            }
        }

        public UnidadeProcedimento CarregarUnidadeProcedimento(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<UnidadeProcedimento>()
                    .Fetch(p => p.Procedimento)
                    .Fetch(p => p.Unidade)
                    .Fetch(p => p.TabelaPreco).ThenFetchMany(t => t.Vigencias)
                    .Where(p => p.ID == id)
                    .Single();
            }
        }

        public UnidadeProcedimento CarregarUnidadeProcedimento(long procedimentoId, long unidadeId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<UnidadeProcedimento>()
                    .Fetch(p => p.Procedimento)
                    .Fetch(p => p.Unidade)
                    .Fetch(p => p.TabelaPreco).ThenFetchMany(t => t.Vigencias)
                    .Where(p => p.Procedimento.ID == procedimentoId && p.Unidade.ID == unidadeId)
                    .Single();
            }
        }

        public void ExcluirUnidadeProcedimento(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        UnidadeProcedimento obj = sessao.Query<UnidadeProcedimento>().Where(p => p.ID == id).Single();

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

        public void Excluir(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        Procedimento obj = sessao.Query<Procedimento>().Where(p => p.ID == id).Single();

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

        public string[] CarregarEspecialidades(long tabelaId)
        {
            List<string> especialidades = new List<string>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = string.Concat("select distinct(Especialidade) from Procedimento where Tabela_ID=", tabelaId, " order by Especialidade");

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr[0] == null || dr[0] == DBNull.Value) continue;
                        especialidades.Add(dr.GetString(0));
                    }
                }

                cmd.Dispose();
            }

            if (especialidades.Count == 0) return null;

            return especialidades.ToArray();
        }

        public string[] CarregarCategorias(long tabelaId, string especialidade)
        {
            List<string> categorias = new List<string>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = string.Concat("select distinct(Categoria) from Procedimento where  Tabela_ID=", tabelaId, " and Especialidade=@Especialidade order by Categoria");

                IDataParameter param = cmd.CreateParameter();
                param.ParameterName = "@Especialidade";
                param.Value = especialidade;
                cmd.Parameters.Add(param);

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        categorias.Add(dr.GetString(0));
                    }
                }

                cmd.Dispose();
            }

            if (categorias.Count == 0) return null;

            return categorias.ToArray();
        }
    }
}

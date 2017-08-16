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

    public class PrestadorFacade : FacadeBase
    {
        PrestadorFacade() { }

        #region singleton 

        static PrestadorFacade _instancia;
        public static PrestadorFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new PrestadorFacade(); }
                return _instancia;
            }
        }
        #endregion

        public Prestador Salvar(Prestador prestador)
        {
            using (ISession sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    sessao.SaveOrUpdate(prestador);
                    tran.Commit();
                }
            }

            return prestador;
        }

        public Prestador CarregarPorId(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Prestador>()
                    .Fetch(p => p.Segmento)
                    .FetchMany(p => p.Unidades)
                    .Where(p => p.ID == id)
                    .Single();
            }
        }

        public object CarregarPorParametros(long? especialidadeId, string nome, bool deletado)
        {
            using (var sessao = ObterSessao())
            {
                if (!especialidadeId.HasValue)
                {
                    var lista =
                    (
                        from p in  sessao.Query<Prestador>()
                            join s in  sessao.Query<Segmento>() on p.Segmento.ID equals s.ID
                            join pu in sessao.Query<PrestadorUnidade>() on p.ID equals pu.Owner.ID
                            join ue in sessao.Query<UnidadeEspecialidade>() on pu.ID equals ue.Unidade.ID
                            join e in  sessao.Query<Especialidade>() on ue.Especialidade.ID equals e.ID
                        where (p.Nome.Contains(nome) || pu.Nome.Contains(nome)) && p.Deletado == deletado
                        orderby p.Nome, e.Nome

                        select new
                        {
                            ID                = p.ID,
                            UID               = pu.ID,
                            Nome              = p.Nome,
                            EspecialidadeNome = e.Nome,
                            EspecialidadeEnde = string.Concat(pu.Endereco, " ,", pu.Numero, " - ", pu.Bairro, " - ", pu.Cidade, " - ", pu.UF)
                        }
                    )
                    .ToList();

                    return lista;
                }
                else
                {
                    var lista =
                    (
                        from     p  in sessao.Query<Prestador>()
                            join s  in sessao.Query<Segmento>() on p.Segmento.ID equals s.ID
                            join pu in sessao.Query<PrestadorUnidade>() on p.ID equals pu.Owner.ID
                            join ue in sessao.Query<UnidadeEspecialidade>() on pu.ID equals ue.Unidade.ID
                            join e  in sessao.Query<Especialidade>() on ue.Especialidade.ID equals e.ID
                        where (p.Nome.Contains(nome) || pu.Nome.Contains(nome)) && ue.Especialidade.ID == especialidadeId.Value
                        orderby p.Nome,e.Nome

                        select new
                        {
                            ID                = p.ID,
                            UID               = pu.ID,
                            Nome              = p.Nome,
                            EspecialidadeNome = e.Nome,
                            EspecialidadeEnde = string.Concat(pu.Endereco, ", ", pu.Numero, " - ", pu.Bairro, " - ", pu.Cidade, " - ", pu.UF)
                        }
                    )
                    .ToList();

                    return lista;
                }
            }
        }

        public object CarregarPorNome(string nome, bool deletado)
        {
            using (var sessao = ObterSessao())
            {
                var lista =
                (
                    from p in sessao.Query<Prestador>()
                    where (p.Nome.Contains(nome) && p.Deletado == deletado)
                    orderby p.Nome

                    select new
                    {
                        ID = p.ID,
                        UID = 0,
                        Nome = p.Nome,
                        EspecialidadeNome = "",
                        EspecialidadeEnde = ""
                    }
                )
                .ToList();

                return lista;
            }

            //using (var sessao = ObterSessao())
            //{
            //    List<Prestador> lista = sessao.Query<Prestador>()
            //        .Fetch(p => p.Segmento)
            //        .Where(p => p.Nome.Contains(nome))
            //        .OrderBy(p => p.Nome)
            //        .ToList();

            //    return lista;
            //}
        }

        public object CarregarPorNome(string nome, long segmentoId, bool deletado)
        {
            using (var sessao = ObterSessao())
            {
                var lista =
                (
                    from p in sessao.Query<Prestador>()
                    where (p.Nome.Contains(nome) && p.Deletado == deletado && p.Segmento.ID == segmentoId)
                    orderby p.Nome

                    select new
                    {
                        ID = p.ID,
                        UID = 0,
                        Nome = p.Nome,
                        EspecialidadeNome = "",
                        EspecialidadeEnde = ""
                    }
                )
                .ToList();

                return lista;
            }
        }

        public DataTable CarregarPorNome(string nome, long segmentoId)
        {
            List<string> idsAdicionados = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Nome");
            dt.Columns.Add("EspecialidadeNome");
            dt.Columns.Add("EspecialidadeEnde");

            using (var sessao = ObterSessao())
            {
                using (IDbCommand cmd = sessao.Connection.CreateCommand())
                {
                    cmd.CommandText = string.Concat(
                        "select pu.ID as UID, p.ID as ID, p.Nome as Nome ",
                        "   from prestador_unidade pu ",
                        "   inner join prestador p on p.ID = pu.Owner_ID ", 
                        "where ",
                        "   (pu.Nome like @nome or p.Nome like @nome) ", 
                        "   AND (p.Deletado = 0 or p.Deletado is null) ",
                        "   AND (p.segmento_id=", segmentoId, ") ",
                        "order by p.Nome");

                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@nome";
                    parameter.DbType = DbType.String;
                    parameter.Value = "%" + nome + "%";
                    cmd.Parameters.Add(parameter);

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (idsAdicionados.Contains(Convert.ToString(dr["ID"]))) continue;
                            idsAdicionados.Add(Convert.ToString(dr["ID"]));

                            DataRow row = dt.NewRow();

                            row["ID"] = dr["ID"];
                            row["Nome"] = dr["Nome"];

                            dt.Rows.Add(row);
                        }
                    }
                }

                sessao.Close();
            }

            return dt;

            //using (var sessao = ObterSessao())
            //{
            //    var lista =
            //    (
            //        from p in sessao.Query<Prestador>()
            //        where (p.Nome.Contains(nome) && p.Deletado == deletado && p.Segmento.ID == segmentoId)
            //        orderby p.Nome

            //        select new
            //        {
            //            ID = p.ID,
            //            UID = 0,
            //            Nome = p.Nome,
            //            EspecialidadeNome = "",
            //            EspecialidadeEnde = ""
            //        }
            //    )
            //    .ToList();
            //}
        }

        public object CarregarTodos(bool deletados = false)
        {
            using (var sessao = ObterSessao())
            {
                var lista =
                (
                    from p in sessao.Query<Prestador>()
                    where p.Deletado == deletados
                    orderby p.Nome

                    select new
                    {
                        ID = p.ID,
                        Nome = p.Nome
                    }
                )
                .ToList();

                return lista;
            }
        }
    }
}

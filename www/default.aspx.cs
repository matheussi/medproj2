namespace MedProj.www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Data.OleDb;
    using System.Web.Security;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;

    using NHibernate;
    using NHibernate.Dialect;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using LC.Web.PadraoSeguros.Facade;
    using MedProj.Entidades.Map.NHibernate;
    using FluentNHibernate.Conventions.Helpers;

    //using LC.SmartTools.Lead.Entity;
    //using LC.SmartTools.Lead.Facade;
    //using LC.SmartTools.Lead.www.SiteUtil;
    //using LC.SmartTools.Lead.Entity.Enuns;
    //using LC.SmartTools.Lead.www.SiteUtil.Seguranca;

    public partial class _default : System.Web.UI.Page
    {
        private static NHibernate.Cfg.Configuration _config = null;
        static ISessionFactory CriarSessaoNHibernate()
        {
            string connString = ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString;

            var f = Fluently.Configure().Database((MsSqlConfiguration.MsSql2008.Dialect<MsSql2008Dialect>().ConnectionString(connString).ShowSql()))
                    .ExposeConfiguration(p => p.Properties["command_timeout"] = "1000") //timeout
                    .ExposeConfiguration(p => p.Properties["hibernate.cache.use_query_cache"] = "false") //sem cache 
                    .ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>().Conventions.Setup(x => x.Add(AutoImport.Never())));

            _config = f.BuildConfiguration();
            return f.BuildSessionFactory();
        }

        /// <summary>
        /// Gráfico 1
        /// </summary>
        public string G1;
        public string G1_Total;
        public string G1_Periodo;

        public string G2 = "";
        public string G2_Meses = "";
        public string G2_Periodo = "";

        /// <summary>
        /// Gráfico 3
        /// </summary>
        public string G3;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //if (UsuarioAutenticado.ContratanteId == 0 && UsuarioAutenticado.UsuarioTipo != (int)TipoUsuario.Master)
            //    Response.Redirect("~/login.aspx");

            this.ConfigHome();

            if (!IsPostBack)
            {

                //string[][] itens = new string[][] { new string[] { "Clube Azul", "100" } };

                //DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                //DateTime to = from.AddMonths(1).AddDays(-1);
                //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

                //txtDe.Text = from.ToString("dd/MM/yyyy");
                //txtAte.Text = to.ToString("dd/MM/yyyy");

                //this.gerarGraficos();
                //this.carregarDados();

                //ORIENTADOR
                this.carregaEstados();
                this.carregaCidades();
                this.carregaBairros();
                this.carregaSegmentos();
                this.carregaEspecialidades();
                this.carregaProcedimentos();
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
        }

        void carregaSegmentos()
        {
            IList<MedProj.Entidades.Segmento> lista = SegmentoFacade.Instancia.CarregarTodos(string.Empty);

            cboSegmento.Items.Clear();

            if (lista != null)
            {
                var ret = lista.OrderBy(e => e.Nome);

                foreach (var seg in ret)
                {
                    cboSegmento.Items.Add(new ListItem(seg.Nome, seg.ID.ToString()));
                }
            }
        }

        void ConfigHome()
        {
            pnlOrientador.Visible = false;

            if (Util.UsuarioLogado.TipoUsuario == Util.UsuarioLogado.Tipo.ContratoDePrestador)
            {
                litPrestador.Text = Util.UsuarioLogado.Nome;
                litNomeUnidade.Text = Util.UsuarioLogado.NomeUnidade;
                litEnderecoUnidade.Text = Util.UsuarioLogado.EnderecoUnidade;

                if (!string.IsNullOrEmpty(Util.UsuarioLogado.FoneUnidade))
                    litContatoUnidade.Text = string.Concat("<b>Telefone:</b> ", Util.UsuarioLogado.FoneUnidade);

                if (!string.IsNullOrEmpty(Util.UsuarioLogado.EmailUnidade))
                {
                    if (!string.IsNullOrEmpty(litContatoUnidade.Text)) litContatoUnidade.Text += "&nbsp;&nbsp;";

                    litContatoUnidade.Text = string.Concat(litContatoUnidade.Text, "<b>E-mail:</b> ", Util.UsuarioLogado.EmailUnidade);
                }
            }
            else if (Util.UsuarioLogado.TipoUsuario == Util.UsuarioLogado.Tipo.Administrador)
            {
                pnlOrientador.Visible = true;
            }
        }

        void gerarGraficos()
        {
            //List<Proposta> lista = null;
            //DateTime? from = UtilTypeConvert.ToDateTime(txtDe.Text, 0, 0, 0, 2);
            //DateTime? to   = UtilTypeConvert.ToDateTime(txtAte.Text, 23, 59, 59, 995);

            ////se for vendedor, mostra apenas suas propostas
            //long? criadorId = null;
            //if (UsuarioAutenticado.UsuarioTipo == (int)TipoUsuario.Vendedor) criadorId = UsuarioAutenticado.UsuarioId;

            //System.Globalization.CultureInfo cinfoBRA = new System.Globalization.CultureInfo("pt-Br");
            //System.Globalization.CultureInfo cinfoUSA = new System.Globalization.CultureInfo("en-US");

            //#region Gráfico 1 - barras laterais 

            //this.G1 = "";
            //this.G1_Total = "";
            //this.G1_Periodo = string.Concat(from.Value.ToString("dd/MM/yyyy"), " a ", to.Value.ToString("dd/MM/yyyy"));

            //LeadStatus[] status = new LeadStatus[] { LeadStatus.RetornoAgendado, LeadStatus.EmAbandono, LeadStatus.Novo, LeadStatus.Recusado, LeadStatus.NegocioFechado };

            //lista = PropostaFacade.Instancia.Carregar(from, to, criadorId, null, status, UsuarioAutenticado.ContratanteId).Objeto;

            //if (lista != null)
            //{
            //    QtdPropostas auxP = null;
            //    List<QtdPropostas> dadosP = new List<QtdPropostas>();

            //    foreach (Proposta p in lista)
            //    {
            //        auxP = dadosP.Where(d => d.PropostaTipo == p.Status.ToString()).SingleOrDefault();

            //        if (auxP == null)
            //        {
            //            QtdPropostas acrescenta = new QtdPropostas();
            //            acrescenta.PropostaValor = p.ValorFinal;
            //            acrescenta.PropostaTipo = p.Status.ToString();
            //            dadosP.Add(acrescenta);
            //        }
            //        else
            //        {
            //            auxP.PropostaValor += p.ValorFinal;
            //        }
            //    }

            //    //monta dados para o grafico
            //    System.Text.StringBuilder sb    = new System.Text.StringBuilder();

            //    foreach (QtdPropostas obj in dadosP)
            //    {
            //        if (sb.Length > 0) sb.Append(",");

            //        sb.Append("{ ");
            //        sb.Append("name: '");
            //        sb.Append(QtdPropostas.TrataNome(obj.PropostaTipo));
            //        sb.Append("', ");
            //        sb.Append("data: [");
            //        sb.Append(obj.PropostaValor.ToString("N2", cinfoUSA).Replace(",", ""));
            //        sb.Append("]");
            //        sb.Append("}");
            //    }

            //    int max = 0;
            //    if (dadosP != null && dadosP.Count > 0)
            //    {
            //        max = Convert.ToInt32(dadosP.Max(d => d.PropostaValor) + 50);
            //    }
            //    this.G1 = sb.ToString();
            //    this.G1_Total = max.ToString();
            //}

            //#endregion

            //#region Gráfico 2 

            //this.G2_Meses = "";
            //if (lista != null)
            //{
            //    System.Text.StringBuilder sbMeses = new System.Text.StringBuilder();

            //    List<Proposta> ordenadasDoAno = lista
            //        .Where(p => p.DataCadastro.Year == DateTime.Now.Year)
            //        .OrderBy(p => p.DataCadastro)
            //        .ToList();

            //    this.G2_Periodo = string.Concat("Ano de ", DateTime.Now.Year);

            //    if (ordenadasDoAno.Count > 0)
            //    {
            //        List<int> mesesI   = new List<int>();
            //        List<string> meses = new List<string>();

            //        foreach (Proposta p in ordenadasDoAno)
            //        {
            //            if (!meses.Contains(p.DataCadastro.ToString("MMM", cinfoBRA)))
            //            {
            //                meses.Add((p.DataCadastro.ToString("MMM", cinfoBRA)));
            //                mesesI.Add(p.DataCadastro.Month);
            //            }
            //        }

            //        foreach (string mes in meses)
            //        {
            //            if (sbMeses.Length > 0) sbMeses.Append(",");

            //            sbMeses.Append("'");
            //            sbMeses.Append(mes);
            //            sbMeses.Append("'");
            //        }

            //        this.G2_Meses = sbMeses.ToString();

            //        List<Proposta> todas = null, fechadas = null;
            //        List<decimal> valoresTodas = new List<decimal>();
            //        List<decimal> valoresFechadas = new List<decimal>();

            //        foreach (int mes in mesesI)
            //        {
            //            todas = ordenadasDoAno.Where(p => p.DataCadastro.Month == mes).ToList();
            //            fechadas = ordenadasDoAno.Where(p => p.DataCadastro.Month == mes && p.Status == LeadStatus.NegocioFechado).ToList();

            //            valoresTodas.Add(todas.Sum(p => p.ValorFinal));
            //            valoresFechadas.Add(fechadas.Sum(p => p.ValorFinal));
            //        }

            //        System.Text.StringBuilder sbTodas = new System.Text.StringBuilder();
            //        System.Text.StringBuilder sbFechadas = new System.Text.StringBuilder();

            //        for (int i = 0; i < valoresTodas.Count; i++)
            //        {
            //            if (sbTodas.Length > 0) { sbTodas.Append(","); sbFechadas.Append(","); }
            //            else if (i == 0) { sbTodas.Append("["); sbFechadas.Append("["); }

            //            sbTodas.Append(valoresTodas[i].ToString("N2", cinfoUSA).Replace(",", ""));
            //            sbFechadas.Append(valoresFechadas[i].ToString("N2", cinfoUSA).Replace(",", ""));
            //        }

            //        sbTodas.Append("]"); sbFechadas.Append("]");

            //        this.G2 = string.Concat(
            //            "{ name: 'Todas', data: ", sbTodas.ToString(), "}, ",
            //            "{ name: 'Fechadas', data: ", sbFechadas.ToString(), "}");

            //        meses.Clear();
            //        mesesI.Clear();
            //    }
            //}
            //#endregion

            //#region Gráfico 3 - origens

            //this.G3 = "";
            //lista = PropostaFacade.Instancia.CarregarDoMes(DateTime.Now, UsuarioAutenticado.ContratanteId).Objeto;

            //if (lista != null)
            //{
            //    QtdOrigens aux = null;

            //    List<QtdOrigens> dados = new List<QtdOrigens>();

            //    foreach (Proposta p in lista)
            //    {
            //        aux = dados.Where(d => d.Id == p.Origem.ID).SingleOrDefault();

            //        if (aux == null)
            //        {
            //            QtdOrigens acrescenta = new QtdOrigens();
            //            acrescenta.Id = p.Origem.ID;
            //            acrescenta.OrigemQtd = 1;
            //            acrescenta.OrigemNome = p.Origem.Descricao;
            //            dados.Add(acrescenta);
            //        }
            //        else
            //        {
            //            aux.OrigemQtd++;
            //        }
            //    }

            //    //monta dados para o grafico
            //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //    foreach (QtdOrigens obj in dados)
            //    {
            //        if (sb.Length > 0) sb.Append(",");

            //        sb.Append("[\""); sb.Append(obj.OrigemNome); sb.Append("\",");
            //        sb.Append(obj.OrigemQtd); sb.Append("]");
            //    }

            //    this.G3 = sb.ToString();
            //}

            
            //#endregion
        }

        void carregarDados()
        {
            //int aux = 0;
            //DateTime? from = UtilTypeConvert.ToDateTime(txtDe.Text, 0, 0, 0, 2);
            //DateTime? to = UtilTypeConvert.ToDateTime(txtAte.Text, 23, 59, 59,995);

            ////enviadas e nao retornadas
            //List<Proposta> lista = PropostaFacade.Instancia.CarregarRetornosNaoFeitos(UsuarioAutenticado.ContratanteId).Objeto;
            //if (lista == null) h1Prioritarias.Attributes.Add("data-to", "0");
            //else
            //{ h1Prioritarias.Attributes.Add("data-to", lista.Count.ToString()); aBox1.HRef = string.Concat("~/mov/lead/leadLista.aspx?", Keys.naoRetornadosKey, "=1"); }

            ////e-mails nao visualizados
            //lista = PropostaFacade.Instancia.CarregarComEmailsNaoVisualizados(UsuarioAutenticado.ContratanteId).Objeto;
            //if (lista == null) h1Todas.Attributes.Add("data-to", "0");
            //else
            //{ h1Todas.Attributes.Add("data-to", lista.Count.ToString()); aBox2.HRef = string.Concat("~/mov/lead/leadLista.aspx?", Keys.naoVisualizadosKey, "=1"); }

            //if (from.HasValue && to.HasValue)
            //{
            //    //novas
            //    aux = PropostaFacade.Instancia.CarregarQtdNoStatus(new RequisicaoComplex<LeadStatus, DateTime, DateTime>(LeadStatus.Novo, from.Value, to.Value), UsuarioAutenticado.ContratanteId);
            //    h1Novas.Attributes.Add("data-to", aux.ToString());
            //    if (aux > 0) { aBox3.HRef = string.Concat("~/mov/lead/leadLista.aspx?", Keys.novosKey, "=1"); }

            //    //fechadas
            //    aux = PropostaFacade.Instancia.CarregarQtdNoStatus(new RequisicaoComplex<LeadStatus, DateTime, DateTime>(LeadStatus.NegocioFechado, from.Value, to.Value), UsuarioAutenticado.ContratanteId);
            //    h1Fechadas.Attributes.Add("data-to", aux.ToString());
            //    if (aux > 0) { aBox4.HRef = string.Concat("~/mov/lead/leadLista.aspx?", Keys.fechadosKey, "=1"); }
            //}
        }

        void carregarLembretes()
        {
            //if (UsuarioAutenticado.UsuarioTipo == (int)Entity.Enuns.TipoUsuario.Master) return;

            //Nullable<long> usuarioId = null;

            //if (UsuarioAutenticado.UsuarioTipo == (int)Entity.Enuns.TipoUsuario.Vendedor) usuarioId = UsuarioAutenticado.UsuarioId;

            //List<PropostaInteracao> interacoes = PropostaFacade.Instancia.
            //    CarregarLembretes(new RequisicaoComplex<DateTime, long, long?>(DateTime.Now, UsuarioAutenticado.ContratanteId, usuarioId)).Objeto;

            //dlLembrete.DataSource = interacoes;
            //dlLembrete.DataBind();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void lnkAbrir_Click(object sender, EventArgs e)
        {
            //long id = UtilTypeConvert.ConverterTipo<long>(((LinkButton)sender).CommandArgument);

            //SiteUtil.UIState.AddParametro(Keys.IdKey, id);
            //Response.Redirect("~/mov/lead/leadLista.aspx");
        }

        protected void cmdFiltrar_Click(object sender, EventArgs e)
        {
            this.carregarDados();
            this.gerarGraficos();
        }

        /* ORIENTADOR */

        void carregaEspecialidades()
        {
            cboEspecialidade.Items.Clear();

            if (cboSegmento.Items.Count == 0) return;

            IList<MedProj.Entidades.Especialidade> lista = 
                EspecialidadeFacade.Instance.Carregar(string.Empty, Util.CTipos.CTipo<long>(cboSegmento.SelectedValue));

            if (lista != null)
            {
                var ret = lista.OrderBy(e => e.Nome);
                cboEspecialidade.Items.Add(new ListItem("todas", "-1"));

                foreach (var espec in ret)
                {
                    cboEspecialidade.Items.Add(new ListItem(espec.Nome, espec.ID.ToString()));
                }
            }
        }
        void carregaProcedimentos()
        {
            IList<MedProj.Entidades.Procedimento> lista = null;

            if (cboEspecialidade.SelectedValue != "-1")
                lista = ProcedimentoFacade.Instancia.CarregarPorEspecialidade(cboEspecialidade.SelectedItem.Text);
            else
                lista = ProcedimentoFacade.Instancia.Carregar();

            cboProcedimento.Items.Clear();
            cboProcedimento.Items.Add(new ListItem("selecione", "-1"));

            if (lista != null)
            {
                /// FORNCANDO O PROCEDIMENTO Consulta ////////////////////////
                var ret = lista.FirstOrDefault(p => p.Codigo == "10101012");
                if (ret == null && cboProcedimento.SelectedValue != "10015") //não pode adicionar para Patologia Clínica
                {
                    cboProcedimento.Items.Add(new ListItem("(10101012) Consulta em consultorio", "1"));
                }
                //////////////////////////////////////////////////////////////

                foreach (var proc in lista)
                {
                    cboProcedimento.Items.Add(new ListItem(
                        string.Concat("(", proc.Codigo, ") ", trataNomeProcedimento(proc.Nome)),
                        proc.ID.ToString()));
                }
            }
            else
            {
                /// FORNCANDO O PROCEDIMENTO Consulta ////////////////////////
                if (cboProcedimento.SelectedValue != "10015") //não pode adicionar para Patologia Clínica
                { 
                    cboProcedimento.Items.Add(new ListItem("(10101012) Consulta em consultorio", "1")); 
                }
                //////////////////////////////////////////////////////////////
            }
        }

        string trataNomeProcedimento(string nome)
        {
            if (nome.Length > 100)
                return string.Concat(nome.Substring(0, 99), " (...)");
            else
                return nome;
        }
        void carregaEstados()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(UF) as uf from prestador_unidade where UF <> '' and UF is not null order by UF";

                        cboUf.Items.Clear();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboUf.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }
        void carregaCidades()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(Cidade) as cid from prestador_unidade where UF='" + cboUf.SelectedValue + "' and Cidade <> '' and Cidade is not null order by Cidade";

                        cboCidade.Items.Clear();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboCidade.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }
        void carregaBairros()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = "select distinct(Bairro) from prestador_unidade where UF='" + cboUf.SelectedValue + "' and Cidade='" + cboCidade.SelectedValue + "' and Bairro <> '' and Bairro is not null order by Bairro";

                        cboBairro.Items.Clear();
                        cboBairro.Items.Add("todos");

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cboBairro.Items.Add(new ListItem(dr.GetString(0), dr.GetString(0)));
                            }

                            dr.Close();
                        }
                    }
                }
            }
        }

        protected void cboSegmento_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaEspecialidades();

            if (cboEspecialidade.SelectedIndex > 0)
            {
                this.carregaProcedimentos();
                if (cboSegmento.SelectedItem.Text == "Saúde")   pnlDivProcedimentos.Visible = true;
                else                                            pnlDivProcedimentos.Visible = false;
            }
            else
            {
                cboProcedimento.Items.Clear();
                pnlDivProcedimentos.Visible = false;
            }
        }

        protected void cboEspecialidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboEspecialidade.SelectedIndex > 0)
            {
                this.carregaProcedimentos();

                if(cboSegmento.SelectedItem.Text == "Saúde") pnlDivProcedimentos.Visible = true;
                else                                         pnlDivProcedimentos.Visible = false;
            }
            else
            {
                cboProcedimento.Items.Clear();
                pnlDivProcedimentos.Visible = false;
            }
        }
        protected void cboUf_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaCidades();
            this.carregaBairros();
        }
        protected void cboCidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaBairros();
        }
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (cboEspecialidade.SelectedIndex > 0)
            {
                if (cboProcedimento.SelectedIndex <= 0)
                    this.pesquisarSemValor();
                else
                    this.pesquisarComValor();
            }
            else
            {
                this.pesquisarSemValor();
            }
        }

        void pesquisarComValor() 
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Especialidade");
            dt.Columns.Add("Prestador");
            dt.Columns.Add("Telefone");
            dt.Columns.Add("Endereco");
            dt.Columns.Add("Bairro");
            dt.Columns.Add("Cidade");
            dt.Columns.Add("UF");
            dt.Columns.Add("Valor");
            dt.Columns.Add("Fantasia");

            grid2.Columns[7].Visible = true;

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        if (cboProcedimento.SelectedValue != "1")
                        {
                            cmd.CommandText = string.Concat(
                                "select pu.*, up.ValorSobrescrito, e.Nome as Especialidade, pre.Nome as Fantasia from prestador_unidade pu ",
                                "   inner join unidade_especialidade ue on pu.ID = ue.Unidade_ID ",
                                "   inner join especialidade e on e.ID = ue.Especialidade_ID ",
                                "   inner join unidade_procedimento up on up.Unidade_ID = pu.ID ",
                                "   inner join procedimento p on up.Procedimento_ID = p.ID ",
                                "   inner join prestador pre on pre.ID = pu.Owner_ID ",                  //adicionado
                                "where ",
                                "   ue.Especialidade_ID=", cboEspecialidade.SelectedValue,
                                "   AND (pre.Deletado = 0 or pre.Deletado is null) ",                   //adicionado
                                "   AND p.ID = ", cboProcedimento.SelectedValue,
                                "   AND pu.UF = '", cboUf.SelectedValue, "'",
                                "   AND pu.Cidade = '", cboCidade.SelectedValue, "'");
                        }
                        else
                        {
                            cmd.CommandText = string.Concat(
                                "select pu.*, up.ValorSobrescrito, '", cboEspecialidade.SelectedItem.Text, "' as Especialidade, pre.Nome as Fantasia from prestador_unidade pu ",
                                "   inner join unidade_especialidade ue on pu.ID = ue.Unidade_ID ",
                                "   inner join especialidade e on e.ID = ue.Especialidade_ID ",
                                "   inner join unidade_procedimento up on up.Unidade_ID = pu.ID ",
                                "   inner join procedimento p on up.Procedimento_ID = p.ID ",
                                "   inner join prestador pre on pre.ID = pu.Owner_ID ",                  //adicionado
                                "where ",
                                "   ue.Especialidade_ID=", cboEspecialidade.SelectedValue,
                                "   AND (pre.Deletado = 0 or pre.Deletado is null) ",                   //adicionado
                                "   AND p.ID = ", cboProcedimento.SelectedValue,
                                "   AND pu.UF = '", cboUf.SelectedValue, "'",
                                "   AND pu.Cidade = '", cboCidade.SelectedValue, "'");
                        }

                        if (cboBairro.SelectedIndex > 0)
                        {
                            cmd.CommandText += string.Concat(" and pu.Bairro='", cboBairro.SelectedValue, "'");
                        }

                        cmd.CommandText += " order by up.ValorSobrescrito ";

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DataRow row = dt.NewRow();

                                row["Especialidade"] = dr["Especialidade"];
                                row["Prestador"] = dr["Nome"];
                                row["Telefone"] = dr["Telefone"];
                                row["Fantasia"] = dr["Fantasia"];

                                if (dr["Numero"] != null && dr["Numero"] != DBNull.Value && Convert.ToString(dr["Numero"]).Trim() != "")
                                    row["Endereco"] = string.Concat(dr["Endereco"], ", ", dr["Numero"]);
                                else
                                    row["Endereco"] = dr["Endereco"];

                                if (dr["Complemento"] != null && dr["Complemento"] != DBNull.Value && Convert.ToString(dr["Complemento"]).Trim() != "")
                                {
                                    string enderecoTemp = Util.CTipos.CToString(row["Endereco"]);
                                    enderecoTemp += " " + Util.CTipos.CToString(dr["Complemento"]);

                                    row["Endereco"] = enderecoTemp;
                                }

                                row["Bairro"] = dr["Bairro"];
                                row["Cidade"] = dr["Cidade"];
                                row["UF"] = dr["UF"];
                                row["Valor"] = Convert.ToDecimal(dr["ValorSobrescrito"]).ToString("N2");

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                pnlResult.Visible = true;
                pnlNoResult.Visible = false;
            }
            else
            {
                pnlResult.Visible = false;
                pnlNoResult.Visible = true;
            }

            grid2.DataSource = dt;
            grid2.DataBind();
        }
        void pesquisarSemValor()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Especialidade");
            dt.Columns.Add("Prestador");
            dt.Columns.Add("Telefone");
            dt.Columns.Add("Endereco");
            dt.Columns.Add("Bairro");
            dt.Columns.Add("Cidade");
            dt.Columns.Add("UF");
            dt.Columns.Add("Valor");
            dt.Columns.Add("Fantasia");

            grid2.Columns[7].Visible = false;

            string condicaoEspecialiade = string.Concat(" ue.Especialidade_ID=", cboEspecialidade.SelectedValue);

            if (cboEspecialidade.SelectedIndex == 0) condicaoEspecialiade = " ue.Especialidade_ID > 0 ";

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        cmd.CommandText = string.Concat(
                            "select pu.*, e.Nome as Especialidade, p.Nome as Fantasia from prestador_unidade pu ",
                            "   inner join unidade_especialidade ue on pu.ID = ue.Unidade_ID ",
                            "   inner join especialidade e on e.ID = ue.Especialidade_ID ",
                            "   inner join prestador p on p.ID = pu.Owner_ID ",                  //adicionado
                            "where ",
                            condicaoEspecialiade, //"   ue.Especialidade_ID=", cboEspecialidade.SelectedValue,
                            "   AND (p.Deletado = 0 or p.Deletado is null) ",                   //adicionado
                            "   AND pu.UF = '", cboUf.SelectedValue, "'",
                            "   AND pu.Cidade = '", cboCidade.SelectedValue, "' and (p.segmento_id=", cboSegmento.SelectedValue ,")");

                        if (cboBairro.SelectedIndex > 0)
                        {
                            cmd.CommandText += string.Concat(" and pu.Bairro='", cboBairro.SelectedValue, "'");
                        }

                        cmd.CommandText += " order by pu.Nome, e.Nome ";

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DataRow row = dt.NewRow();

                                row["Especialidade"] = dr["Especialidade"];
                                row["Prestador"] = dr["Nome"];
                                row["Telefone"] = dr["Telefone"];
                                row["Fantasia"] = dr["Fantasia"];

                                if (dr["Numero"] != null && dr["Numero"] != DBNull.Value && Convert.ToString(dr["Numero"]).Trim() != "")
                                    row["Endereco"] = string.Concat(dr["Endereco"], ", ", dr["Numero"]);
                                else
                                    row["Endereco"] = dr["Endereco"];

                                row["Bairro"] = dr["Bairro"];
                                row["Cidade"] = dr["Cidade"];
                                row["UF"] = dr["UF"];
                                row["Valor"] = "0,00";

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                pnlResult.Visible = true;
                pnlNoResult.Visible = false;
            }
            else
            {
                pnlResult.Visible = false;
                pnlNoResult.Visible = true;
            }

            grid2.DataSource = dt;
            grid2.DataBind();
        }

        protected void grid2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }
        protected void grid2_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
        protected void grid2_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void cmdBusca_Click(object sender, EventArgs e)
        {
            if (txtBusca.Text.Trim() != "")
            {
                int j = 0;
                if (cboProcedimento.SelectedIndex > 0 && cboProcedimento.SelectedIndex < cboProcedimento.Items.Count) 
                    j = cboProcedimento.SelectedIndex + 1;

                bool localizado = false;

                for (int i = j; i < cboProcedimento.Items.Count; i++)
                {
                    if (cboProcedimento.Items[i].Text.ToLower().IndexOf(txtBusca.Text.ToLower()) > -1)
                    {
                        cboProcedimento.SelectedIndex = i;
                        localizado = true;
                        break;
                    }
                }

                if (!localizado)
                {
                    Util.Geral.Alerta(this, "Nenhum procedimento localizado.");
                    cboProcedimento.SelectedIndex = 0;
                }
            }
        }

        protected void cmdBusca2_Click(object sender, EventArgs e)
        {
            if (txtBuscaAvancada.Text.Trim() != "")
            {
                var lista = ProcedimentoFacade.Instancia.Carregar(txtBuscaAvancada.Text);

                if (lista == null || lista.Count == 0)
                {
                    Util.Geral.Alerta(this, "Nenhum procedimento localizado.");
                    gridBuscaAvancada.DataSource = null;
                    gridBuscaAvancada.DataBind();
                    pnlBuscaAvancada.Visible = false;
                }
                else
                {
                    gridBuscaAvancada.DataSource = lista;
                    gridBuscaAvancada.DataBind();
                    pnlBuscaAvancada.Visible = true;
                }
            }
            else
                pnlBuscaAvancada.Visible = false;
        }

        protected void gridBuscaAvancada_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("sel"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                long id = Convert.ToInt64(gridBuscaAvancada.DataKeys[index].Value);

                var proc = ProcedimentoFacade.Instancia.Carregar(id);

                for (int i = 0; i < cboEspecialidade.Items.Count; i++)
                {
                    if (Util.Geral.RetiraAcentos(cboEspecialidade.Items[i].Text.ToUpper()) == Util.Geral.RetiraAcentos(proc.Especialidade.ToUpper()))
                    {
                        cboEspecialidade.SelectedIndex = i;
                        cboEspecialidade_SelectedIndexChanged(null, null);
                        pnlDivProcedimentos.Visible = true;
                        break;
                    }
                }

                cboProcedimento.SelectedValue = proc.ID.ToString();
                gridBuscaAvancada.DataSource = null;
                gridBuscaAvancada.DataBind();
                pnlBuscaAvancada.Visible = false;


                //for (int i = 0; i < cboProcedimento.Items.Count; i++)
                //{
                //    if (cboProcedimento.Items[i].Text.ToUpper() == proc.Nome.ToUpper())
                //}
            }
        }

        protected void gridBuscaAvancada_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }
    }

    class QtdOrigens
    {
        public long Id { get; set; }
        public string OrigemNome { get; set; }
        public int OrigemQtd { get; set; }
    }

    class QtdPropostas
    {
        public string PropostaTipo { get; set; }
        public decimal PropostaValor { get; set; }

        //public static string TrataNome(string nome)
        //{
        //    if (nome == LeadStatus.EmAbandono.ToString())
        //        return "Em abandono";
        //    else if (nome == LeadStatus.NegocioFechado.ToString())
        //        return "Fechado";
        //    else if (nome == LeadStatus.RetornoAgendado.ToString())
        //        return "Retorno agendado";
        //    else
        //        return nome;
        //}
    }

    internal class SqlStatementInterceptor : EmptyInterceptor
    {
        public static bool LogSQLOn = false;
        public static string LogSQL;

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            if (SqlStatementInterceptor.LogSQLOn)
                SqlStatementInterceptor.LogSQL += "\n" + sql;

            return sql;
        }
    }
}
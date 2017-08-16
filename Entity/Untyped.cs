namespace LC.Web.PadraoSeguros.Entity.Untyped
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity.Filtro;

    public class UntypedProcesses
    {
        private UntypedProcesses() { }

        public static DataTable ManipulacaoGradeComissaoQuery(Object perfilId, Object filialId, Object grupoVendaId, Object superiorId, Object tabelaComissaoAtualId)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT usuario_id AS UsuarioID, usuario_nome AS UsuarioNome, usuario_apelido AS UsuarioApelido, comissaomodelo_id AS ComissaoID, (categoria_descricao + ' - ' + convert(varchar(100), comissaomodelo_data, 103)) AS ComissaoDescricao, comissaousuario_data AS ComissaoVigencia, usuariogvenda_grupovendaId AS GrupoID ");
            sql.AppendLine("    FROM usuario ");
            sql.AppendLine("        INNER JOIN filial ON filial_id=usuario_filialid ");
            sql.AppendLine("        LEFT JOIN usuario_grupoVenda ON usuariogvenda_usuarioId=usuario_id AND usuariogvenda_id=(SELECT TOP 1 usuariogvenda_id FROM usuario_grupoVenda WHERE usuariogvenda_usuarioId=usuario_id ORDER BY usuariogvenda_data DESC) ");
            sql.AppendLine("        LEFT JOIN comissao_usuario ON comissaousuario_id=(SELECT TOP 1 comissaousuario_id FROM comissao_usuario WHERE comissaousuario_usuarioId=usuario_id AND comissaousuario_ativo=1 ORDER BY comissaousuario_data DESC) ");
            sql.AppendLine("        LEFT JOIN comissao_modelo ON comissaomodelo_id=comissaousuario_tabelaComissionamentoId ");
            sql.AppendLine("        LEFT JOIN categoria ON comissaomodelo_categoriaId=categoria_id ");
            sql.AppendLine("   WHERE (usuario_perfilId="); sql.Append(perfilId);

            if (superiorId != null) { sql.Append(" AND usuario_superiorId="); sql.Append(superiorId); }

            sql.Append(")");

            if (filialId != null) { sql.Append(" AND usuario_filialId="); sql.Append(filialId); }
            if (grupoVendaId != null) { sql.Append(" AND usuariogvenda_grupovendaId="); sql.Append(grupoVendaId); }
            if (tabelaComissaoAtualId != null) { sql.Append(" AND comissaomodelo_id="); sql.Append(tabelaComissaoAtualId); }

            sql.AppendLine("    ORDER BY usuario_apelido, usuario_nome ");

            if (superiorId != null)
            {
                //TODO: recursivo para todos no perfil selecionado
            }

            return LocatorHelper.Instance.ExecuteQuery(sql.ToString(), "recordset").Tables[0];
        }

        public static DataTable RelacaoDeTabelasDeExcecaoQuery(Object operadoraId)
        {
            String query = String.Concat("SELECT tabelaexcecao_id as TabelaExecaoID, usuario_id as UsuarioID, tabelaexcecao_vigencia as TabelaExcecaoVigencia, usuario_nome as UsuarioNome, operadora_nome as OperadoraNome, contratoadm_descricao as ContratoAdmDescricao, categoria_descricao + ' - ' + convert(varchar(50), comissaomodelo_data, 103) as TabelaComissaoNome, comissaousuario_data as TabelaComissaoVigencia ",
                "FROM usuario ",
                "   INNER JOIN tabela_excecao ON usuario_id=tabelaexcecao_produtorId ",
                "   INNER JOIN contratoAdm ON contratoadm_id=tabelaexcecao_contratoAdmId ",
                "   INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "   INNER JOIN comissao_modelo ON comissaomodelo_id=tabelaexcecao_tabelaComissaoId ",
                "   INNER JOIN categoria ON categoria_id=comissaomodelo_categoriaId ",
                "   INNER JOIN comissao_usuario ON usuario_id=comissaousuario_usuarioId AND comissaousuario_tabelaComissionamentoId=tabelaexcecao_tabelaComissaoId ",
                "WHERE operadora_id=", operadoraId, " ORDER BY usuario_nome");

            return LocatorHelper.Instance.ExecuteQuery(query, "recordset").Tables[0];
        }

        /// <summary>
        ///  Método para Carregar a lista com os Produtores Rankeados pel Valor de Contratos vendidos 
        ///  em um determinado periodo, por operadora, perfil e tipo de contrato.
        /// </summary>
        /// <param name="From">Data Inicial.</param>
        /// <param name="To">Data Final.</param>
        /// <param name="OperadoraID">Array com ID de Operadoras.</param>
        /// <param name="PerfilID">ID do Perfil.</param>
        /// <param name="TipoContratoID">Array com os Tipos de Contrato.</param>
        /// <param name="Filtros">Filtros com Restrições.</param>
        /// <returns>Retorna um Datatable preenchido com os Produtores, Apelidos e Quantidades.</returns>
        public static DataTable GetProdutoresByValorRank(DateTime From, DateTime To, Object[] OperadoraID, Object PerfilID, Object[] TipoContratoID, IList<ComissaoProducaoFiltro> Filtros)
        {
            StringBuilder sbSQL               = new StringBuilder();
            StringBuilder sbSQLOperadoraRange = new StringBuilder(String.Empty);
            StringBuilder sbSQLTipoContrato   = new StringBuilder(String.Empty);
            StringBuilder sbSQLValContrato    = new StringBuilder(String.Empty);

            Int32 intIntialParamCount = 3;
            Int32 intQtdOperadora     = (OperadoraID != null) ? OperadoraID.Length : 0;
            Int32 intQtdTipoContrato  = (TipoContratoID != null) ? TipoContratoID.Length : 0;
            Int32 intQtdContratos     = (Filtros != null && Filtros.Count > 0) ? (Filtros.Count * 2) : 0;

            String[] strParam = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdContratos];
            String[] strValue = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdContratos];

            strParam[0] = "@dataInicial";
            strParam[1] = "@dataFinal";
            strParam[2] = "@perfilID";

            strValue[0] = String.Concat(From.ToString("yyyy-MM-dd "), "00:00:00");
            strValue[1] = String.Concat(To.ToString("yyyy-MM-dd "), "23:59:59");
            strValue[2] = PerfilID.ToString();

            #region Build Dynamic Params

            // OPERADORA

            if (OperadoraID != null && OperadoraID.Length > 0)
            {
                sbSQLOperadoraRange.Append(" AND (con.contrato_operadoraId IN (");

                String strOperadoraIDAux = "@operadoraID_";

                for (Int32 i = 0; i < intQtdOperadora; i++)
                {
                    if (i > 0)
                        sbSQLOperadoraRange.Append(",");

                    sbSQLOperadoraRange.Append(String.Concat(strOperadoraIDAux, i.ToString()));

                    strParam[intIntialParamCount + i] = String.Concat(strOperadoraIDAux, i.ToString());
                    strValue[intIntialParamCount + i] = OperadoraID[i].ToString();
                }

                sbSQLOperadoraRange.Append(" ))");
            }

            // TIPO DE CONTRATO

            if (TipoContratoID != null && TipoContratoID.Length > 0)
            {
                sbSQLTipoContrato.Append(" AND (contrato_tipoContratoId IN (");

                String strTipoContratoIDAux = "@tipoContratoID_";

                for (Int32 i = 0; i < intQtdTipoContrato; i++)
                {
                    if (i > 0)
                        sbSQLTipoContrato.Append(",");

                    sbSQLTipoContrato.Append(String.Concat(strTipoContratoIDAux, i.ToString()));

                    strParam[intIntialParamCount + intQtdOperadora + i] = String.Concat(strTipoContratoIDAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + i] = TipoContratoID[i].ToString();
                }

                sbSQLTipoContrato.Append(" ))");

                strTipoContratoIDAux = null;
            }

            // VALOR FINAL DE CONTRATO

            if (Filtros != null && Filtros.Count > 0)
            {
                sbSQLValContrato.Append("HAVING");

                String strQtdBeneficiarioRangeFromAux = "@valFrom_";
                String strQtdBeneficiarioRangeToAux   = "@valTo_";

                for (Int32 i = 0; i < Filtros.Count; i++)
                {
                    if (i > 0)
                        sbSQLValContrato.Append(" OR");

                    sbSQLValContrato.Append(" (SUM(conval.contratovalor_valorFinal) BETWEEN ");
                    sbSQLValContrato.Append(strQtdBeneficiarioRangeFromAux);
                    sbSQLValContrato.Append(i.ToString());
                    sbSQLValContrato.Append(" AND ");
                    sbSQLValContrato.Append(strQtdBeneficiarioRangeToAux);
                    sbSQLValContrato.Append(i.ToString());
                    sbSQLValContrato.Append(")");

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = String.Concat(strQtdBeneficiarioRangeFromAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = Filtros[i].IntervaloInicial.ToString();

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = String.Concat(strQtdBeneficiarioRangeToAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = Filtros[i].IntervaloFinal.ToString();
                }

                strQtdBeneficiarioRangeFromAux = strQtdBeneficiarioRangeToAux = null;
            }

            sbSQL.AppendLine("SELECT usu.usuario_id AS UsuarioID, usu.usuario_nome AS UsuarioNome, usu.usuario_apelido AS UsuarioApelido, SUM(conval.contratovalor_valorFinal) AS ValorTotalContratos ");
            sbSQL.AppendLine("FROM contrato con ");
            sbSQL.AppendLine("INNER JOIN contrato_valor conval ON con.contrato_Id = conval.contratovalor_contratoid ");
            sbSQL.AppendLine("INNER JOIN usuario usu on con.contrato_donoId = usu.usuario_id ");
            sbSQL.AppendLine("WHERE (contrato_admissao BETWEEN @dataInicial AND @dataFinal) AND ");
            sbSQL.AppendLine("      ((contrato_rascunho = 0 OR contrato_rascunho IS NULL) AND (contrato_cancelado = 0 OR contrato_cancelado IS NULL) AND (contrato_inativo = 0 OR contrato_inativo IS NULL) AND ");
            sbSQL.Append("       (contrato_cancelado = 0))");
            sbSQL.Append(sbSQLOperadoraRange.ToString());
            sbSQL.Append(" AND (usu.usuario_perfilId = @perfilID)");
            sbSQL.AppendLine(sbSQLTipoContrato.ToString());
            sbSQL.AppendLine("GROUP BY usu.usuario_id, usu.usuario_apelido, usu.usuario_nome ");
            sbSQL.AppendLine(sbSQLValContrato.ToString());
            sbSQL.AppendLine("ORDER BY usu.usuario_nome ASC ");

            DataTable dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue).Tables[0];

            sbSQL               = null;
            sbSQLOperadoraRange = null;
            sbSQLValContrato    = null;
            sbSQLTipoContrato   = null;

            return dtRet;

            #endregion
        }

        /// <summary>
        ///  Método para Carregar a lista com os Produtores Rankeados pela quantidade de Contratos vendidos 
        /// em um determinado periodo, por operadora, perfil e tipo de contrato.
        /// </summary>
        /// <param name="From">Data Inicial.</param>
        /// <param name="To">Data Final.</param>
        /// <param name="OperadoraID">Array com ID de Operadoras.</param>
        /// <param name="PerfilID">ID do Perfil.</param>
        /// <param name="TipoContratoID">Array com os Tipos de Contrato.</param>
        /// <param name="Filtros">Filtros com Restrições.</param>
        /// <returns>Retorna um Datatable preenchido com os Produtores, Apelidos e Quantidades.</returns>
        public static DataTable GetProdutoresByContratoRank(DateTime From, DateTime To, Object[] OperadoraID, Object PerfilID, Object[] TipoContratoID, IList<ComissaoProducaoFiltro> Filtros)
        {
            StringBuilder sbSQL               = new StringBuilder();
            StringBuilder sbSQLOperadoraRange = new StringBuilder(String.Empty);
            StringBuilder sbSQLTipoContrato   = new StringBuilder(String.Empty);
            StringBuilder sbSQLQtdContrato    = new StringBuilder(String.Empty);

            Int32 intIntialParamCount = 3;
            Int32 intQtdOperadora     = (OperadoraID != null) ? OperadoraID.Length : 0;
            Int32 intQtdTipoContrato  = (TipoContratoID != null) ? TipoContratoID.Length : 0;
            Int32 intQtdContratos     = (Filtros != null && Filtros.Count > 0) ? (Filtros.Count * 2) : 0;

            String[] strParam = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdContratos];
            String[] strValue = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdContratos];

            strParam[0] = "@dataInicial";
            strParam[1] = "@dataFinal";
            strParam[2] = "@perfilID";

            strValue[0] = String.Concat(From.ToString("yyyy-MM-dd "), "00:00:00");
            strValue[1] = String.Concat(To.ToString("yyyy-MM-dd "), "23:59:59");
            strValue[2] = PerfilID.ToString();

            #region Build Dynamic Params

            // OPERADORA

            if (OperadoraID != null && OperadoraID.Length > 0)
            {
                sbSQLOperadoraRange.Append(" AND (con.contrato_operadoraId IN (");

                String strOperadoraIDAux = "@operadoraID_";

                for (Int32 i = 0; i < intQtdOperadora; i++)
                {
                    if (i > 0)
                        sbSQLOperadoraRange.Append(",");

                    sbSQLOperadoraRange.Append(String.Concat(strOperadoraIDAux, i.ToString()));

                    strParam[intIntialParamCount + i] = String.Concat(strOperadoraIDAux, i.ToString());
                    strValue[intIntialParamCount + i] = OperadoraID[i].ToString();
                }

                sbSQLOperadoraRange.Append(" ))");
            }

            // TIPO DE CONTRATO

            if (TipoContratoID != null && TipoContratoID.Length > 0)
            {
                sbSQLTipoContrato.Append(" AND (contrato_tipoContratoId IN (");

                String strTipoContratoIDAux = "@tipoContratoID_";

                for (Int32 i = 0; i < intQtdTipoContrato; i++)
                {
                    if (i > 0)
                        sbSQLTipoContrato.Append(",");

                    sbSQLTipoContrato.Append(String.Concat(strTipoContratoIDAux, i.ToString()));

                    strParam[intIntialParamCount + intQtdOperadora + i] = String.Concat(strTipoContratoIDAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + i] = TipoContratoID[i].ToString();
                }

                sbSQLTipoContrato.Append(" ))");

                strTipoContratoIDAux = null;
            }

            // QUANTIDADE DE BENEFICIÁRIO

            if (Filtros != null && Filtros.Count > 0)
            {
                sbSQLQtdContrato.Append("HAVING");

                String strQtdBeneficiarioRangeFromAux = "@valFrom_";
                String strQtdBeneficiarioRangeToAux   = "@valTo_";

                for (Int32 i = 0; i < Filtros.Count; i++)
                {
                    if (i > 0)
                        sbSQLQtdContrato.Append(" OR");

                    sbSQLQtdContrato.Append(" (COUNT(usu.usuario_id) BETWEEN ");
                    sbSQLQtdContrato.Append(strQtdBeneficiarioRangeFromAux);
                    sbSQLQtdContrato.Append(i.ToString());
                    sbSQLQtdContrato.Append(" AND ");
                    sbSQLQtdContrato.Append(strQtdBeneficiarioRangeToAux);
                    sbSQLQtdContrato.Append(i.ToString());
                    sbSQLQtdContrato.Append(")");

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = String.Concat(strQtdBeneficiarioRangeFromAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = Filtros[i].IntervaloInicial.ToString();

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = String.Concat(strQtdBeneficiarioRangeToAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = Filtros[i].IntervaloFinal.ToString();
                }

                strQtdBeneficiarioRangeFromAux = strQtdBeneficiarioRangeToAux = null;
            }

            #endregion

            sbSQL.AppendLine("SELECT usu.usuario_id AS UsuarioID, usu.usuario_nome AS UsuarioNome, usu.usuario_apelido AS UsuarioApelido, COUNT(usu.usuario_id) AS QtdContratos ");
            sbSQL.AppendLine("FROM contrato con ");
            sbSQL.AppendLine("INNER JOIN operadora op ON con.contrato_operadoraId = op.operadora_id ");
            sbSQL.AppendLine("INNER JOIN usuario usu on con.contrato_donoId = usu.usuario_id ");
            sbSQL.AppendLine("WHERE (contrato_admissao BETWEEN @dataInicial AND @dataFinal) AND ");
            sbSQL.AppendLine("      ((contrato_rascunho = 0 OR contrato_rascunho IS NULL) AND (contrato_cancelado = 0 OR contrato_cancelado IS NULL) AND (contrato_inativo = 0 OR contrato_inativo IS NULL) AND ");
            sbSQL.Append("       (contrato_cancelado = 0))");
            sbSQL.Append(sbSQLOperadoraRange.ToString());
            sbSQL.Append(" AND (usu.usuario_perfilId = @perfilID)");
            sbSQL.AppendLine(sbSQLTipoContrato.ToString());
            sbSQL.AppendLine("GROUP BY usu.usuario_id, usu.usuario_apelido, usu.usuario_nome ");
            sbSQL.AppendLine(sbSQLQtdContrato.ToString());
            sbSQL.AppendLine("ORDER BY usu.usuario_nome ASC ");

            DataTable dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue).Tables[0];

            sbSQL                = null;
            sbSQLOperadoraRange  = null;
            sbSQLQtdContrato     = null;
            sbSQLTipoContrato    = null;

            return dtRet;
        }

        /// <summary>
        /// Método para Carregar a lista com os Produtores Rankeados pela quantidade de Beneficiários 
        /// em um determinado periodo, por operadora, perfil e tipo de contrato.
        /// </summary>
        /// <param name="From">Data Inicial.</param>
        /// <param name="To">Data Final.</param>
        /// <param name="OperadoraID">Array com ID de Operadoras.</param>
        /// <param name="PerfilID">ID do Perfil.</param>
        /// <param name="TipoContratoID">Array com os Tipos de Contrato.</param>
        /// <param name="Filtros">Filtros com Restrições.</param>
        /// <returns>Retorna um Datatable preenchido com os Produtores, Apelidos e Quantidades.</returns>
        public static DataTable GetProdutoresByBenRank(DateTime From, DateTime To, Object[] OperadoraID, Object PerfilID, Object[] TipoContratoID, IList<ComissaoProducaoFiltro> Filtros)
        {
            StringBuilder sbSQL                = new StringBuilder();
            StringBuilder sbSQLOperadoraRange  = new StringBuilder(String.Empty);
            StringBuilder sbSQLTipoContrato    = new StringBuilder(String.Empty);
            StringBuilder sbSQLQtdBeneficiario = new StringBuilder(String.Empty);

            Int32 intIntialParamCount = 3;
            Int32 intQtdOperadora     = (OperadoraID != null) ? OperadoraID.Length : 0;
            Int32 intQtdTipoContrato  = (TipoContratoID != null) ? TipoContratoID.Length : 0;
            Int32 intQtdBeneficiario  = (Filtros != null && Filtros.Count > 0) ? (Filtros.Count * 2) : 0;

            String[] strParam = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdBeneficiario];
            String[] strValue = new String[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + intQtdBeneficiario];

            strParam[0] = "@dataInicial";
            strParam[1] = "@dataFinal";
            strParam[2] = "@perfilID";

            strValue[0] = String.Concat(From.ToString("yyyy-MM-dd "),  "00:00:00");
            strValue[1] = String.Concat(To.ToString("yyyy-MM-dd "), "23:59:59");
            strValue[2] = PerfilID.ToString();

            #region Build Dynamic Params

            // OPERADORA

            if (OperadoraID != null && OperadoraID.Length > 0)
            {
                sbSQLOperadoraRange.Append(" AND (con.contrato_operadoraId IN (");

                String strOperadoraIDAux = "@operadoraID_";

                for (Int32 i = 0; i < intQtdOperadora; i++)
                {
                    if (i > 0)
                        sbSQLOperadoraRange.Append(",");

                    sbSQLOperadoraRange.Append(String.Concat(strOperadoraIDAux, i.ToString()));

                    strParam[intIntialParamCount + i] = String.Concat(strOperadoraIDAux, i.ToString());
                    strValue[intIntialParamCount + i] = OperadoraID[i].ToString();
                }

                sbSQLOperadoraRange.Append(" ))");
            }

            // TIPO DE CONTRATO

            if (TipoContratoID != null && TipoContratoID.Length > 0)
            {
                sbSQLTipoContrato.Append(" AND (contrato_tipoContratoId IN (");

                String strTipoContratoIDAux = "@tipoContratoID_";

                for (Int32 i = 0; i < intQtdTipoContrato; i++)
                {
                    if (i > 0)
                        sbSQLTipoContrato.Append(",");

                    sbSQLTipoContrato.Append(String.Concat(strTipoContratoIDAux, i.ToString()));

                    strParam[intIntialParamCount + intQtdOperadora + i] = String.Concat(strTipoContratoIDAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + i] = TipoContratoID[i].ToString();
                }

                sbSQLTipoContrato.Append(" ))");

                strTipoContratoIDAux = null;
            }

            // QUANTIDADE DE BENEFICIÁRIO

            if (Filtros != null && Filtros.Count > 0)
            {
                sbSQLQtdBeneficiario.Append("HAVING");

                String strQtdBeneficiarioRangeFromAux = "@valFrom_";
                String strQtdBeneficiarioRangeToAux   = "@valTo_";

                for (Int32 i = 0; i < Filtros.Count; i++)
                {
                    if (i > 0)
                        sbSQLQtdBeneficiario.Append(" OR");

                    sbSQLQtdBeneficiario.Append(" (SUM(conBen.QtdBeneficiarios) BETWEEN ");
                    sbSQLQtdBeneficiario.Append(strQtdBeneficiarioRangeFromAux);
                    sbSQLQtdBeneficiario.Append(i.ToString());
                    sbSQLQtdBeneficiario.Append(" AND ");
                    sbSQLQtdBeneficiario.Append(strQtdBeneficiarioRangeToAux);
                    sbSQLQtdBeneficiario.Append(i.ToString());
                    sbSQLQtdBeneficiario.Append(")");

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = String.Concat(strQtdBeneficiarioRangeFromAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + (2 * i)] = Filtros[i].IntervaloInicial.ToString();

                    strParam[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = String.Concat(strQtdBeneficiarioRangeToAux, i.ToString());
                    strValue[intIntialParamCount + intQtdOperadora + intQtdTipoContrato + ((2 * i) + 1)] = Filtros[i].IntervaloFinal.ToString();
                }

                strQtdBeneficiarioRangeFromAux = strQtdBeneficiarioRangeToAux = null;
            }

            #endregion

            sbSQL.AppendLine("SELECT usu.usuario_id AS UsuarioID, usu.usuario_nome AS UsuarioNome, usu.usuario_apelido AS UsuarioApelido, SUM(conBen.QtdBeneficiarios) AS QtdBeneficiarios");
            sbSQL.AppendLine("FROM contrato con");
            sbSQL.AppendLine("INNER JOIN (");
            sbSQL.AppendLine("		SELECT");
            sbSQL.AppendLine("			DISTINCT");
            sbSQL.AppendLine("				(");
            sbSQL.AppendLine("				 SELECT COUNT(contratobeneficiario_id)");
            sbSQL.AppendLine("				 FROM contrato_beneficiario");
            sbSQL.AppendLine("				 WHERE contratobeneficiario_contratoId = conAux.contrato_id  AND contratobeneficiario_ativo = 1 AND");
            sbSQL.AppendLine("				       (contratobeneficiario_data BETWEEN @dataInicial AND @dataFinal)");
            sbSQL.AppendLine("				) AS QtdBeneficiarios, conAux.contrato_Id");
            sbSQL.AppendLine("		FROM contrato conAux");
            sbSQL.AppendLine("		LEFT JOIN contrato_beneficiario conBenAux ON conAux.contrato_id = conBenAux.contratobeneficiario_contratoId");
            sbSQL.AppendLine("		WHERE (");
            sbSQL.AppendLine("			 SELECT COUNT(contratobeneficiario_id)");
            sbSQL.AppendLine("			 FROM contrato_beneficiario");
            sbSQL.AppendLine("			 WHERE contratobeneficiario_contratoId = conAux.contrato_id  AND contratobeneficiario_ativo = 1 AND");
            sbSQL.AppendLine("			       (contratobeneficiario_data BETWEEN @dataInicial AND @dataFinal)");
            sbSQL.AppendLine("			) > 0");
            sbSQL.AppendLine("	   ) conBen ON con.contrato_Id = conBen.contrato_Id ");
            sbSQL.AppendLine("INNER JOIN usuario usu ON con.contrato_donoId = usu.usuario_id ");
            sbSQL.AppendLine("WHERE (contrato_admissao BETWEEN @dataInicial AND @dataFinal) AND ");
            sbSQL.AppendLine("      ((contrato_rascunho = 0 OR contrato_rascunho IS NULL) AND (contrato_cancelado = 0 OR contrato_cancelado IS NULL) AND (contrato_inativo = 0 OR contrato_inativo IS NULL) AND");
            sbSQL.Append("       (contrato_cancelado = 0))");
            sbSQL.Append(sbSQLOperadoraRange.ToString());
            sbSQL.Append(" AND (usu.usuario_perfilId = @perfilID)");
            sbSQL.AppendLine(sbSQLTipoContrato.ToString());
            sbSQL.AppendLine("GROUP BY usu.usuario_id, usu.usuario_apelido, usu.usuario_nome ");
            sbSQL.AppendLine(sbSQLQtdBeneficiario.ToString());
            sbSQL.AppendLine("ORDER BY usu.usuario_nome ASC ");

            DataTable dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue).Tables[0];

            sbSQL                = null;
            sbSQLOperadoraRange  = null;
            sbSQLQtdBeneficiario = null;
            sbSQLTipoContrato    = null;

            return dtRet;
        }

        #region GetBeneficiarioInclusao

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes para inclusão.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioInclusao(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioInclusao(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes para inclusão.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioInclusao(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioInclusao(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes para inclusão.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioInclusao(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contradoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.Novo, ContratoID, BeneficiarioID, loteId, PM,vigencia, contradoAdmId);
            }
            catch (Exception) { throw; }
        }

        public static DataTable GetBeneficiarioInclusao(Object OperadoraID, Object[] ContratoAdmIDs, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.Novo, ContratoAdmIDs, BeneficiarioID, loteId, PM, vigencia);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioPendente

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes na operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioPendente(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioPendente(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes na operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioPendente(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão pendentes na operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios pendente para inclusão em uma operadora.</returns>
        public static DataTable GetBeneficiarioPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.PendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioDevolvido
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioDevolvido(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioDevolvido(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.Devolvido, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioAlteracaoCadastroPendente
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendente(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioAlteracaoCadastroPendente(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioAlteracaoCadastroPendente(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que foram devolvido pela operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.AlteracaoCadastroPendente, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioAlteracaoCadastroPendenteOperadora
        /// <summary>
        /// Método para trazer todos os beneficiarios que estão com a alteração de cadastro pendente na Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteOperadora(Object OperadoraID)
        {
            try
            {
                return null; // GetBeneficiarioAlteracaoCadastroPendenteOperadora(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios que estão com a alteração de cadastro pendente na Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioAlteracaoCadastroPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios que estão com a alteração de cadastro pendente na Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioAlteracaoCadastroPendenteDevolvido

        /// <summary>
        /// Método para trazer todos os beneficiarios que a alteração cadastral foi devolvida pela Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteDevolvido(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioAlteracaoCadastroPendenteDevolvido(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios que a alteração cadastral foi devolvida pela Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioAlteracaoCadastroPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios que a alteração cadastral foi devolvida pela Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioAlteracaoCadastroPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null; // GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioSegundaViaCartaoPendente
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendente(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendente(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendente(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.SegundaViaCartaoPendente, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioSegundaViaCartaoPendenteOperadora
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente na OPERDORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteOperadora(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendenteOperadora(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente na OPERDORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão pendente na OPERDORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioSegundaViaCartaoPendenteDevolvido
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteDevolvido(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendenteDevolvido(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioSegundaViaCartaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// segunda via de cartão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioSegundaViaCartaoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioExclusaoPendente
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendente(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendente(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendente(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.ExclusaoPendente, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioExclusaoPendenteOperadora
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteOperadora(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendenteOperadora(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioExclusaoPendenteDevolvido
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteDevolvido(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendenteDevolvido(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioExclusaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// exclusão devolvida pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioExclusaoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.ExclusaoDevolvido, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioMudancaPlanoPendente
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendente(Object OperadoraID)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendente(OperadoraID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendente(OperadoraID, ContratoID, BeneficiarioID, null, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM)
        {
            try
            {
                return null;// GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.MudancaPlanoPendente, ContratoID, BeneficiarioID, loteId, PM);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioMudancaPlanoPendenteOperadora
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente na OPERDAORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteOperadora(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendenteOperadora(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente na OPERDAORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano pendente na OPERDAORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioMudancaPlanoPendenteDevolvido
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano devolvido pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteDevolvido(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendenteDevolvido(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano devolvido pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMudancaPlanoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// mudança de plano devolvido pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioMudancaPlanoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioCancelamentoContratoPendente
        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendente(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendente(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendente(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendente(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.CancelamentoPendente, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region GetBeneficiarioCancelamentoContratoPendenteOperadora

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteOperadora(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendenteOperadora(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente na OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteOperadora(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioCancelamentoContratoPendenteDevolvido

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteDevolvido(Object OperadoraID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendenteDevolvido(OperadoraID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioCancelamentoContratoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora que estão com uma solicitação de
        /// cancelamento de contrato pendente no SISTEMA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <returns>Retorna todos os beneficiarios devolvidos por uma operadora.</returns>
        public static DataTable GetBeneficiarioCancelamentoContratoPendenteDevolvido(Object OperadoraID, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            try
            {
                return GetBeneficiarioMovimentacao(OperadoraID, (int)ContratoBeneficiario.eStatus.CancelamentoDevolvido, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetBeneficiarioMovimentacao

        /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora com critério de status do mesmo no 
        /// contrato.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <returns>Retorna um DataTable com todos os beneficiarios de uma operadora.</returns>
        private static DataTable GetBeneficiarioMovimentacao(Object OperadoraID, Object Status, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia, Object contratoAdmId)
        {
            return GetBeneficiarioMovimentacao(OperadoraID, Status, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
        }

         /// <summary>
        /// Método para trazer todos os beneficiarios de uma operadora com critério de status do mesmo no 
        /// contrato.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <returns>Retorna um DataTable com todos os beneficiarios de uma operadora.</returns>
        private static DataTable GetBeneficiarioMovimentacao(Object OperadoraID, Object Status, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            if (OperadoraID != null && Status != null)
            {
                StringBuilder sbSQL = new StringBuilder();

                Int32 intQtdParam = 5;
                Int32 intQtdParamContratoBeneficiario = 0;
                String strContratoBeneficiarioWhere = String.Empty;

                if ((ContratoID != null && ContratoID.Length > 0) &&
                    (BeneficiarioID != null && BeneficiarioID.Length > 0) &&
                    (ContratoID.Length == BeneficiarioID.Length))
                {
                    intQtdParamContratoBeneficiario = ContratoID.Length * 2;
                }

                String[] strParam = new String[intQtdParam + intQtdParamContratoBeneficiario];
                String[] strValue = new String[intQtdParam + intQtdParamContratoBeneficiario];

                strParam[0] = "@cOperadoraId";
                strParam[1] = "@status";
                strParam[2] = "@contrato_cancelado";
                strParam[3] = "@contratobeneficiario_ativo";
                strParam[4] = "@contrato_inativo";

                strValue[0] = OperadoraID.ToString();
                strValue[1] = Status.ToString();
                strValue[2] = "0";
                strValue[3] = "1";
                strValue[4] = "1"; //diferente de 1

                if (intQtdParamContratoBeneficiario > 0)
                {
                    strContratoBeneficiarioWhere = " AND (";

                    Int16 intLocalContratoBeneficiarioIndex = 0;

                    for (Int32 i = 0; i < ContratoID.Length; i++)
                    {
                        if (i > 0)
                            strContratoBeneficiarioWhere += " OR";

                        intLocalContratoBeneficiarioIndex = 0;

                        strContratoBeneficiarioWhere += String.Concat(" (cBen.contratobeneficiario_contratoId = @contrato_id_", i.ToString());
                        strContratoBeneficiarioWhere += String.Concat(" AND cBen.contratobeneficiario_beneficiarioId = @beneficiario_id_", i.ToString(), ")");

                        strParam[intQtdParam + (i * 2)] = String.Concat("@contrato_id_", i.ToString());
                        strValue[intQtdParam + (i * 2)] = ContratoID[i].ToString();

                        intLocalContratoBeneficiarioIndex++;

                        strParam[intQtdParam + (i * 2) + intLocalContratoBeneficiarioIndex] = String.Concat("@beneficiario_id_", i.ToString());
                        strValue[intQtdParam + (i * 2) + intLocalContratoBeneficiarioIndex] = BeneficiarioID[i].ToString();
                    }

                    strContratoBeneficiarioWhere += ")";
                }

                if(loteId != null)
                {
                    strContratoBeneficiarioWhere = String.Concat(strContratoBeneficiarioWhere, " AND item_lote_id=", loteId);
                }

                if (Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoPendente) ||
                    Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoDevolvido) ||
                    Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora))
                    strValue[2] = "1";
                else if (Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoPendente) ||
                         Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoDevolvido) ||
                         Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora))
                    strValue[3] = "0";

                sbSQL.AppendLine("SELECT");
                sbSQL.AppendLine("	contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo,"); //lote_id, lote_numeracao, lote_data_criacao, 
                sbSQL.AppendLine("	contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,");
                sbSQL.AppendLine("	beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,");
                sbSQL.AppendLine("	beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento, ");
                sbSQL.AppendLine("	endereco_bairro, endereco_cidade, endereco_uf, endereco_cep ");
                sbSQL.AppendLine("FROM contrato_beneficiario cBen ");
                sbSQL.AppendLine("INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id ");
                sbSQL.AppendLine("INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ");
                sbSQL.AppendLine("INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id ");
                sbSQL.AppendLine("INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId ");
                sbSQL.AppendLine("INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId ");
                sbSQL.AppendLine("LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1 ");
                sbSQL.AppendLine("WHERE (contratobeneficiario_status = @status AND contratobeneficiario_ativo = @contratobeneficiario_ativo) AND ");
                sbSQL.AppendLine("      (c.contrato_cancelado = @contrato_cancelado AND c.contrato_rascunho = 0 AND c.contrato_inativo <> @contrato_inativo) AND ");
                sbSQL.AppendLine("      (endereco_donoTipo = 0 AND endereco_tipo = 0 AND c.contrato_operadoraId = @cOperadoraId) "); //and contrato_numero in ('2128128','2130387')

                if (vigencia != DateTime.MinValue)
                {
                    sbSQL.Append(" AND contrato_vigencia='"); sbSQL.Append(vigencia.ToString("yyyy-MM-dd 00:00:00.000"));
                }
                //else
                //    throw new ApplicationException("Data de vigência obrigatória.");

                if (contratoAdmId != null)
                {
                    if (Convert.ToString(contratoAdmId).IndexOf(',') == -1)
                    {
                        sbSQL.Append("' AND contrato_contratoAdmId="); sbSQL.Append(contratoAdmId);
                    }
                    else
                    {
                        sbSQL.Append("' AND contrato_contratoAdmId IN ("); sbSQL.Append(contratoAdmId);
                        sbSQL.Append(") ");
                    }
                }

                sbSQL.AppendLine(strContratoBeneficiarioWhere);
                sbSQL.AppendLine(" ORDER BY item_lote_id DESC, c.contrato_numero, contratobeneficiario_numeroSequencia; "); //lote_id, 

                DataTable dtRet = null;

                try
                {
                    if (PM == null)
                        dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue).Tables[0];
                    else
                        dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue, PM).Tables[0];
                }
                catch (Exception) { throw; }

                sbSQL    = null;
                strParam = null;
                strValue = null;

                return dtRet;
            }
            else
                throw new ArgumentNullException("O ID da Operadora ou Status é nulo.");
        }

        private static DataTable GetBeneficiarioMovimentacao(Object OperadoraID, Object Status, Object[] ContratoAdmIDs, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia)
        {
            if (OperadoraID != null && Status != null)
            {
                StringBuilder sbSQL = new StringBuilder();

                Int32 intQtdParam = 5;
                Int32 intQtdParamContratoBeneficiario = 0;
                String strContratoBeneficiarioWhere = String.Empty;

                if ((ContratoAdmIDs != null && ContratoAdmIDs.Length > 0) &&
                    (BeneficiarioID != null && BeneficiarioID.Length > 0) &&
                    (ContratoAdmIDs.Length == BeneficiarioID.Length))
                {
                    intQtdParamContratoBeneficiario = ContratoAdmIDs.Length * 2;
                }

                String[] array = Array.ConvertAll<Object, String>(ContratoAdmIDs,
                    delegate(object obj)
                    {
                        return (String)obj;
                    });

                String contratoAdmId = String.Join(",", array);

                String[] strParam = new String[intQtdParam + intQtdParamContratoBeneficiario];
                String[] strValue = new String[intQtdParam + intQtdParamContratoBeneficiario];

                strParam[0] = "@cOperadoraId";
                strParam[1] = "@status";
                strParam[2] = "@contrato_cancelado";
                strParam[3] = "@contratobeneficiario_ativo";
                strParam[4] = "@contrato_inativo";

                strValue[0] = OperadoraID.ToString();
                strValue[1] = Status.ToString();
                strValue[2] = "0";
                strValue[3] = "1";
                strValue[4] = "1"; //diferente de 1

                if (intQtdParamContratoBeneficiario > 0)
                {
                    strContratoBeneficiarioWhere = " AND (";

                    Int16 intLocalContratoBeneficiarioIndex = 0;

                    for (Int32 i = 0; i < ContratoAdmIDs.Length; i++)
                    {
                        if (i > 0)
                            strContratoBeneficiarioWhere += " OR";

                        intLocalContratoBeneficiarioIndex = 0;

                        strContratoBeneficiarioWhere += String.Concat(" (cBen.contratobeneficiario_contratoId = @contrato_id_", i.ToString());
                        strContratoBeneficiarioWhere += String.Concat(" AND cBen.contratobeneficiario_beneficiarioId = @beneficiario_id_", i.ToString(), ")");

                        strParam[intQtdParam + (i * 2)] = String.Concat("@contrato_id_", i.ToString());
                        strValue[intQtdParam + (i * 2)] = ContratoAdmIDs[i].ToString();

                        intLocalContratoBeneficiarioIndex++;

                        strParam[intQtdParam + (i * 2) + intLocalContratoBeneficiarioIndex] = String.Concat("@beneficiario_id_", i.ToString());
                        strValue[intQtdParam + (i * 2) + intLocalContratoBeneficiarioIndex] = BeneficiarioID[i].ToString();
                    }

                    strContratoBeneficiarioWhere += ")";
                }

                if (loteId != null)
                {
                    strContratoBeneficiarioWhere = String.Concat(strContratoBeneficiarioWhere, " AND item_lote_id=", loteId);
                }

                if (Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoPendente) ||
                    Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoDevolvido) ||
                    Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora))
                    strValue[2] = "1";
                else if (Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoPendente) ||
                         Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoDevolvido) ||
                         Convert.ToInt32(Status).Equals((int)ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora))
                    strValue[3] = "0";

                sbSQL.AppendLine("SELECT ");
                sbSQL.AppendLine("	contrato_id,contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm.*, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratobeneficiario_carenciaContratoDataDe,contratobeneficiario_carenciaContratoDataAte, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo,"); //lote_id, lote_numeracao, lote_data_criacao, 
                sbSQL.AppendLine("	contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,");
                sbSQL.AppendLine("	beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,");
                sbSQL.AppendLine("	beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento, ");
                sbSQL.AppendLine("	endereco_bairro, endereco_cidade, endereco_uf, endereco_cep, usuario_codigo, ");
                sbSQL.AppendLine("  operadoraorigem_codigo ");
                sbSQL.AppendLine("FROM contrato_beneficiario cBen ");
                sbSQL.AppendLine("INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id ");
                sbSQL.AppendLine("INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ");
                sbSQL.AppendLine("INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id ");
                sbSQL.AppendLine("INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId ");
                sbSQL.AppendLine("INNER JOIN operadora on operadora_id=contrato_operadoraId ");
                sbSQL.AppendLine("INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId ");
                sbSQL.AppendLine("LEFT JOIN usuario ON usuario_id=c.contrato_donoId ");
                sbSQL.AppendLine("LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1 ");
                sbSQL.AppendLine("LEFT JOIN operadoraOrigem ON operadoraorigem_id = contratobeneficiario_carenciaOperadoraOrigemId ");
                //sbSQL.AppendLine(" where contrato_id=216821 ");
                sbSQL.AppendLine("WHERE (contratobeneficiario_status = @status AND contratobeneficiario_ativo = @contratobeneficiario_ativo) AND ");
                sbSQL.AppendLine("      (c.contrato_cancelado = @contrato_cancelado AND c.contrato_rascunho = 0 AND c.contrato_inativo <> @contrato_inativo) AND ");
                sbSQL.AppendLine("      (endereco_donoTipo = 0 AND endereco_tipo = 0 AND c.contrato_operadoraId = @cOperadoraId) "); //and contrato_numero in ('2128128','2130387')

                if (vigencia != DateTime.MinValue)
                {
                    sbSQL.Append(" AND contrato_vigencia='"); sbSQL.Append(vigencia.ToString("yyyy-MM-dd 00:00:00.000"));
                }

                if (contratoAdmId != null)
                {
                    if (Convert.ToString(contratoAdmId).IndexOf(',') == -1)
                    {
                        sbSQL.Append("' AND contrato_contratoAdmId="); sbSQL.Append(contratoAdmId);
                    }
                    else
                    {
                        sbSQL.Append("' AND contrato_contratoAdmId IN ("); sbSQL.Append(contratoAdmId);
                        sbSQL.Append(") ");
                    }
                }

                sbSQL.AppendLine(strContratoBeneficiarioWhere);
                sbSQL.AppendLine(" ORDER BY item_lote_id DESC, c.contrato_numero, c.contrato_operadoraId, contratobeneficiario_numeroSequencia; "); //lote_id, 

                DataTable dtRet = null;

                try
                {
                    if (PM == null)
                        dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue).Tables[0];
                    else
                        dtRet = LocatorHelper.Instance.ExecuteParametrizedQuery(sbSQL.ToString(), strParam, strValue, PM).Tables[0];
                }
                catch (Exception) { throw; }

                sbSQL = null;
                strParam = null;
                strValue = null;

                return dtRet;
            }
            else
                throw new ArgumentNullException("O ID da Operadora ou Status é nulo.");
        }

        public static DataTable GetBeneficiarios(String[] propostaIDs, PersistenceManager PM)
        {
                StringBuilder sbSQL = new StringBuilder();
                String strContratoBeneficiarioWhere = String.Empty;
                String propostas = String.Join(",", propostaIDs);

                sbSQL.AppendLine("SELECT ");
                sbSQL.AppendLine("	contrato_id,contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, operadora_id, contratoadm.*, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratobeneficiario_carenciaContratoDataDe,contratobeneficiario_carenciaContratoDataAte, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo,"); //lote_id, lote_numeracao, lote_data_criacao, 
                sbSQL.AppendLine("	contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,");
                sbSQL.AppendLine("	beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,");
                sbSQL.AppendLine("	beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento, ");
                sbSQL.AppendLine("	endereco_bairro, endereco_cidade, endereco_uf, endereco_cep, usuario_codigo, ");
                sbSQL.AppendLine("  operadoraorigem_codigo ");
                sbSQL.AppendLine("FROM contrato_beneficiario cBen ");
                sbSQL.AppendLine("INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id ");
                sbSQL.AppendLine("INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ");
                sbSQL.AppendLine("INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id ");
                sbSQL.AppendLine("INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId ");
                sbSQL.AppendLine("INNER JOIN operadora on operadora_id=contrato_operadoraId ");
                sbSQL.AppendLine("INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId ");
                sbSQL.AppendLine("LEFT JOIN usuario ON usuario_id=c.contrato_donoId ");
                sbSQL.AppendLine("LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 ");
                sbSQL.AppendLine("LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1 ");
                sbSQL.AppendLine("LEFT JOIN operadoraOrigem ON operadoraorigem_id = contratobeneficiario_carenciaOperadoraOrigemId ");
                sbSQL.AppendLine(" where contrato_id in ( ");
                sbSQL.AppendLine(propostas);
                sbSQL.AppendLine(")");
                sbSQL.AppendLine(" ORDER BY item_lote_id DESC, c.contrato_numero, c.contrato_operadoraId, contratobeneficiario_numeroSequencia; "); //lote_id, 

                DataTable dtRet = null;

                try
                {
                    dtRet = LocatorHelper.Instance.ExecuteQuery(sbSQL.ToString(), "result", PM).Tables[0];
                }
                catch (Exception) { throw; }

                sbSQL = null;
                return dtRet;
        }

        #endregion

        #region RelatorioComissionamento

        /// <summary>
        /// Carrega os dados do cabeçalho do relatório de Comissionamento
        /// </summary>
        /// <param name="perfilIds">Array com os ids dos perfis selecionados</param>
        /// <param name="listagemId">Id da listagem selecionada</param>
        /// <returns>DataTable com os dados do cabeçalho do relatório</returns>
        public static DataTable CarregaCabecalhoRelatorioComissionamento(Object[] perfilIds, Object listagemId)
        {
            Int32 totalPerfis = (perfilIds != null) ? perfilIds.Length : 0;
            String lstPerfis = "";

            for (int i = 0; i < totalPerfis; i++)
            {
                if (lstPerfis.Length > 0)
                    lstPerfis += ",";

                lstPerfis += perfilIds[i];
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT prd.usuario_id AS produtorId, prd.usuario_nome AS produtorNome, perfil_descricao,");
            sql.Append("        listagemrelacaografo_superiorBanco, listagemrelacaografo_superiorAgencia, listagemrelacaografo_superiorConta,");
            sql.Append("        sup.usuario_id AS superiorId, sup.usuario_nome AS superiorNome, ");
            sql.Append("        sub.usuario_id AS imediatoId, sub.usuario_nome AS imediatoNome ");
            sql.Append(" FROM listagem_relacao_grafo ");
            sql.Append(" INNER JOIN usuario prd ON prd.usuario_id = listagemrelacaografo_superiorId ");
            sql.Append(" LEFT  JOIN usuario sup ON sup.usuario_id = prd.usuario_superiorId ");
            sql.Append(" left JOIN usuario sub ON sub.usuario_id = listagemrelacaografo_imediatoId ");
            sql.Append(" INNER JOIN perfil ON listagemrelacaografo_superiorPerfilId = perfil_id ");
            sql.Append(" WHERE listagemrelacaografo_listagemId = " + listagemId + " AND listagemrelacaografo_superiorPerfilId IN (" + lstPerfis + ") ");
            sql.Append(" ORDER BY perfil_ranking, produtorId, imediatoId ");
            
            return LocatorHelper.Instance.ExecuteParametrizedQuery(sql.ToString(), null, null).Tables[0];
        }

        /// <summary>
        /// Carrega as ids dos corretores que estão relacionados ao supervisor do relatório de Comissionamento
        /// </summary>
        /// <param name="superiorId">Id do superior</param>
        /// <param name="listagemId">Id da listagem</param>
        /// <returns>Ids dos imediatos</returns>
        public static void CarregaCorretoresDoImediato(Object superiorId, Object listagemId, ref List<Object> lstCorretores)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT listagemrelacaografo_imediatoId, listagemrelacaografo_imediatoPerfilId ");
            sql.Append(" FROM listagem_relacao_grafo ");
            sql.Append(" WHERE listagemrelacaografo_superiorId = " + superiorId + " AND listagemrelacaografo_listagemId = " + listagemId + " ");

            DataTable retorno = LocatorHelper.Instance.ExecuteParametrizedQuery(sql.ToString(), null, null).Tables[0];
            Boolean encontrou = false;

            if (retorno != null && retorno.Rows != null && retorno.Rows.Count > 0)
            {
                for (int i = 0; i < retorno.Rows.Count; i++)
                {
                    if (Perfil.IsCorretor(retorno.Rows[i]["listagemrelacaografo_imediatoPerfilId"].ToString()))
                    {
                        encontrou = true;
                        lstCorretores.Add(retorno.Rows[i]["listagemrelacaografo_imediatoId"]);
                    }
                }
                
                if (!encontrou)
                    CarregaCorretoresDoImediato(retorno.Rows[0]["listagemrelacaografo_imediatoId"], listagemId, ref lstCorretores);
            }
        }

        /// <summary>
        /// Carrega as Operadoras do Produtor.
        /// </summary>
        /// <param name="corretorId">Id do Produtor</param>
        /// <param name="listagemId">Id da Listagem</param>
        /// <returns>DataTable com as Operadoras do Produtor.</returns>
        public static DataTable CarregaOperadorasDoProdutor(Object ProdutorId, Object ListagemId)
        {
            if (ProdutorId != null && ListagemId != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT listagemrelacao_operadoraId, operadora_nome ");
                sql.Append(" FROM listagem_relacao ");
                sql.Append(" INNER JOIN listagem_relacao_grafo ON listagemrelacao_produtorId = listagemrelacaografo_superiorId ");
                sql.Append(" INNER JOIN operadora ON listagemrelacao_operadoraId = operadora_id ");
                sql.Append(" GROUP BY listagemrelacao_listagemId, listagemrelacao_operadoraId, operadora_nome, listagemrelacaografo_imediatoPerfilId, listagemrelacao_produtorId ");
                sql.Append(" HAVING listagemrelacao_listagemId = ");
                sql.Append(ListagemId);
                sql.Append(" AND listagemrelacao_produtorId = ");
                sql.Append(ProdutorId);
                sql.Append(" ORDER BY operadora_nome ");

                return LocatorHelper.Instance.ExecuteParametrizedQuery(sql.ToString(), null, null).Tables[0];
            }
            else
                throw new ArgumentNullException("ID do Produtor ou ID da Listagem são nulos.");
        }

        /// <summary>
        /// Carrega as parcelas relacionadas ao Produtor/Operadora/Listagem do relatório de Comissionamento.
        /// </summary>
        /// <param name="corretorId">Id do Produtor.</param>
        /// <param name="operadoraId">Id da Operadora.</param>
        /// <param name="listagemId">Id da Listagem.</param>
        /// <returns>Retorna um DataTable com os dados das parcelas.</returns>
        public static DataTable CarregaParcelasDoProdutor(Object ProdutorId, Object OperadoraId, Object ListagemId)
        {
            if (ProdutorId != null && OperadoraId != null && ListagemId != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT listagemrelacao_contratoNumero, listagemrelacao_contratoNomeTitular, listagemrelacao_contratoAdmissao, listagemrelacao_cobrancaParcela, ");
                sql.Append("        listagemrelacao_cobrancaDataPago, listagemrelacao_cobrancaValorPago, listagemrelacao_percentualComissao, listagemrelacao_produtorValor, ");
                sql.Append("        listagemrelacao_cobrancaDataVencto, listagemrelacao_contratoVigencia");
                sql.Append(" FROM listagem_relacao ");
                sql.Append(" WHERE listagemrelacao_listagemId = ");
                sql.Append(ListagemId);
                sql.Append(" AND listagemrelacao_operadoraId = ");
                sql.Append(OperadoraId);
                sql.Append(" AND listagemrelacao_produtorId = ");
                sql.Append(ProdutorId);
                sql.Append(" ORDER BY listagemrelacao_contratoNumero, listagemrelacao_cobrancaParcela ");

                return LocatorHelper.Instance.ExecuteParametrizedQuery(sql.ToString(), null, null).Tables[0];
            }
            else
                throw new ArgumentNullException("ID do Produtor ou ID da Listagem ou ID da Operadora são nulos.");
        }

        #endregion
    }
}

namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("perfil")]
    public class Perfil : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            Produtor,
            UsuarioSistema,
            Telemarketing,
            Indefinido
        }

        public const String AdministradorIDKey                      = "1";
        public const String SupervidorIDKey                         = "2";
        public const String OperadorIDKey                           = "5";
        public const String CorretorIDKey                           = "3";
        public const String OperadorCallCenterIDKey                 = "8";
        public const String ConferenciaIDKey                        = "9";
        public const String CadastroIDKey                           = "10";
        public const String ConsultaPropostaBeneficiarioIDKey       = "13";
        public const String PropostaBeneficiarioIDKey               = "14";
        public const String PropostaBeneficiarioDemaisLeituraIDKey  = "15";
        public const String Atendimento_Liberacao_Vencimento        = "16";
        public const String JuridicoIDKey                           = "17";//public const String RelatorioFinanceiroIDKey                = "17";
        public const String MarcaOticaIDKey                         = "18";
        public const String OperadorLiberBoletoIDKey                = "19";
        public const String ConsulPropBenefProdLiberBoletoIDKey     = "20";
        public const String Financeiro_RecupPendencias              = "21";

        #region fields 

        Object  _id;
        Object  _parentId;
        String  _descricao;
        Boolean _comissionavel;
        Boolean _participanteContrato;
        Int32   _tipo;

        #endregion

        #region propriedades 

        [DBFieldInfo("perfil_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("perfil_parentId", FieldType.Single)]
        public Object ParentID
        {
            get { return _parentId; }
            set { _parentId= value; }
        }

        [DBFieldInfo("perfil_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        /// <summary>
        /// Se true, usuários com esse perfil recebem comissão, mesmo que não tenham 
        /// necessariamente "vendido" o contrato, mas estejam ligado direta ou inderetamente 
        /// a um usuário que tenha "vendido".
        /// </summary>
        [DBFieldInfo("perfil_comissionavel", FieldType.Single)]
        public Boolean Comissionavel
        {
            get { return _comissionavel; }
            set { _comissionavel= value; }
        }

        /// <summary>
        /// Se true, usuários com esse perfil podem "vender" contratos.
        /// </summary>
        [DBFieldInfo("perfil_participanteContrato", FieldType.Single)]
        public Boolean ParticipanteContrato
        {
            get { return _participanteContrato; }
            set { _participanteContrato= value; }
        }

        [DBFieldInfo("perfil_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        public String TipoSTR
        {
            get
            {
                switch (_tipo)
                {
                    case 0:
                    {
                        return "Produtor";
                    }
                    case 1:
                    {
                        return "Usuário do sistema";
                    }
                    case 2:
                    {
                        return "Telemarketing";
                    }
                    default:
                    {
                        return String.Empty;
                    }
                }
            }
        }

        #endregion

        public Perfil() { }
        public Perfil(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        /// <summary>
        /// ID do Corretor.
        /// </summary>
        public static Object CorretorID
        {
            get { return Perfil.CorretorIDKey; }
        }

        public static String[] Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray
        {
            get
            {
                return new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey };
            }
        }

        /// <summary>
        /// Verifica se o ID do Perfil é o de Corretor.
        /// </summary>
        /// <param name="OperadoraID">ID do Perfil.</param>
        /// <returns>True se for Corretor e False se for outro Perfil.</returns>
        public static Boolean IsCorretor(Object PerfilID)
        {
            if (PerfilID != null)
            {
                Int32 intPerfilID = -1;

                if (Int32.TryParse(PerfilID.ToString(), out intPerfilID))
                    return (Convert.ToInt32(CorretorID) == intPerfilID);
                else
                    throw new ArgumentException("ID do Corretor é inválido");
            }
            else
                throw new ArgumentNullException("ID do Corretor não pode ser nulo.");
        }

        public static Boolean IsFinanceiro(Object PerfilID)
        {
            if (PerfilID != null)
            {
                Int32 intPerfilID = -1;

                if (Int32.TryParse(PerfilID.ToString(), out intPerfilID))
                    return (Convert.ToInt32(Perfil.Financeiro_RecupPendencias) == intPerfilID);
                else
                    throw new ArgumentException("ID é inválido");
            }
            else
                throw new ArgumentNullException("ID não pode ser nulo.");
        }

        public static Boolean IsAdmin(Object PerfilID)
        {
            if (PerfilID != null)
            {
                Int32 intPerfilID = -1;

                if (Int32.TryParse(PerfilID.ToString(), out intPerfilID))
                    return (Convert.ToInt32(Perfil.AdministradorIDKey) == intPerfilID);
                else
                    throw new ArgumentException("ID é inválido");
            }
            else
                throw new ArgumentNullException("ID não pode ser nulo.");
        }

        public static IList<Perfil> CarregarTodos(eTipo tipo)
        {
            String query = "";

            if (tipo != eTipo.Indefinido)
            {
                query = "* FROM perfil WHERE perfil_tipo=" + Convert.ToInt32(tipo) + " ORDER BY perfil_descricao";
            }
            else
            {
                query = "* FROM perfil ORDER BY perfil_descricao";
            }

            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        }

        public static IList<Perfil> CarregarTodos(Boolean apenasComissionaveis)
        {
            String query = "";

            if (apenasComissionaveis)
                query = "* FROM perfil WHERE perfil_comissionavel=1 ORDER BY perfil_descricao";
            else
                query = "* FROM perfil ORDER BY perfil_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        }

        public static IList<Perfil> CarregarTodos(eTipo[] tipos)
        {
            return CarregarTodos(tipos, false);
        } 

        public static IList<Perfil> CarregarTodos(eTipo[] tipos, Boolean apenasComissionaveis)
        {
            String query = "";
            query = "* FROM perfil WHERE perfil_tipo IN (";

            for(int i=0; i < tipos.Length; i++)
            {
                if (i > 0) { query += ","; }
                query += Convert.ToInt32(tipos[i]);
            }

            query += ") ";

            if (apenasComissionaveis)
                query += " AND perfil_comissionavel=1";

            query += "ORDER BY perfil_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        } 

        public static IList<Perfil> CarregarComissionaveis()
        {
            String query = "* FROM perfil WHERE perfil_comissionavel=1 ORDER BY perfil_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        }

        public static IList<Perfil> CarregarParticipantesDeContrato()
        {
            String query = "* FROM perfil WHERE perfil_participanteContrato=1 ORDER BY perfil_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        }

        public static Hashtable CarregarTipos()
        {
            Hashtable ht = new Hashtable();

            ht.Add(0, "Produtor");
            ht.Add(1, "Usuário do sistema");
            ht.Add(2, "Telemarketing");

            return ht;
        }

        public static IList<Perfil> CarregarPorListagem(Object listagemId)
        {
            String query = String.Concat("* FROM perfil ",
                                         "  INNER JOIN listagem_perfil ON listagemperfil_perfilId = perfil_id ",
                                         "  WHERE listagemperfil_listagemId = ", listagemId);
            return LocatorHelper.Instance.ExecuteQuery<Perfil>(query, typeof(Perfil));
        }
    }
}

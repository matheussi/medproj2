namespace MedProj.www.Util
{
    using System;
    using System.Web;
    using System.Collections.Generic;

    public class UsuarioLogado
    {
        
        public enum Tipo : int
        {
            Indefinido,
            Administrador,
            Operador,
            ContratoDePrestador
        }

        private UsuarioLogado() { }

        public static string ID
        {
            get
            {
                if (HttpContext.Current.Session["uId"] != null)
                    return (string)HttpContext.Current.Session["uId"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uId"] = value; }
        }

        public static string Nome
        {
            get
            {
                if (HttpContext.Current.Session["uNm"] != null)
                    return (string)HttpContext.Current.Session["uNm"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uNm"] = value; }
        }

        public static string PrimeiroNome
        {
            get
            {
                if (HttpContext.Current.Session["uNm"] != null)
                    return ((string)HttpContext.Current.Session["uNm"]).Split(' ')[0];
                else
                    return "";
            }
        }

        public static Tipo TipoUsuario
        {
            get
            {
                if (HttpContext.Current.Session["uTp"] != null)
                    return (Tipo)HttpContext.Current.Session["uTp"];
                else
                    return Tipo.Indefinido;
            }
            set { HttpContext.Current.Session["uTp"] = value; }
        }

        public static long IDUnidade
        {
            get
            {
                if (HttpContext.Current.Session["uIdU"] != null)
                    return (long)HttpContext.Current.Session["uIdU"];
                else
                    return 0;
            }
            set { HttpContext.Current.Session["uIdU"] = value; }
        }

        public static string NomeUnidade
        {
            get
            {
                if (HttpContext.Current.Session["uNmU"] != null)
                    return (string)HttpContext.Current.Session["uNmU"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uNmU"] = value; }
        }

        public static string EnderecoUnidade
        {
            get
            {
                if (HttpContext.Current.Session["uNmEn"] != null)
                    return (string)HttpContext.Current.Session["uNmEn"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uNmEn"] = value; }
        }

        public static string FoneUnidade
        {
            get
            {
                if (HttpContext.Current.Session["uNmF"] != null)
                    return (string)HttpContext.Current.Session["uNmF"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uNmF"] = value; }
        }

        public static string EmailUnidade
        {
            get
            {
                if (HttpContext.Current.Session["uNmE"] != null)
                    return (string)HttpContext.Current.Session["uNmE"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["uNmE"] = value; }
        }

        public static void Encerrar()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
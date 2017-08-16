namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    
    [Serializable]
    public abstract class EntidadeBase
    {
        public virtual long ID { get; set; }

        public virtual bool TemId
        {
            get { return ID > 0; }
        }

        System.Text.RegularExpressions.Regex regExc = new System.Text.RegularExpressions.Regex("DA|da|DE|de|DO|do|DAS|das|DOS|dos|Dos");

        public virtual String RetiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }

        public virtual string Abreviar(string s, bool nomesDoMeio)
        {
            // Quebro os nomes...
            string[] nomes = s.Split(' ');
            int inicio = 0;
            int fim = nomes.Length - 1;

            // Se eu não quiser abreviar o primeiro e o ultimo nome
            if (nomesDoMeio)
            {
                inicio = 1;
                fim = nomes.Length - 2;
            }

            // Monto o retorno
            string retorno = "";

            for (int i = 0; i < nomes.Length; i++)
            {
                if (!PalavrasExcecoes(nomes[i]) && i >= inicio && i <= fim)
                    retorno += nomes[i][0] + ". ";
                else
                    retorno += nomes[i] + " ";
            }

            return retorno.Trim();
        }

        public virtual string Abreviar2(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            string[] nome = s.Trim().Split(' ');

            if (nome.Length <= 2) return s;

            if (nome.Length == 3)
            {
                if (nome[2].Length <= 3) return s;
                else return string.Concat(nome[0], " ", nome[2]);
            }
            else
                return string.Concat(nome[0], " ", nome[nome.Length - 1]);

        }

        public virtual bool PalavrasExcecoes(string palavra)
        {
            return regExc.Match(palavra).Success;
        }
    }
}

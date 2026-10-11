using System;
using System.Collections.Generic;

namespace projGerenciador
{
    internal class Projetos
    {
        private List<Projeto> itens = new List<Projeto>();
        private int proxProjeto = 1;

        // Numera o projeto sequencialmente; false se for nulo ou o id já existir.
        public bool adicionar(Projeto p)
        {
            if (p == null)
                return false;
            p.Id = proxProjeto;
            if (itens.Contains(p))
                return false;
            proxProjeto++;
            itens.Add(p);
            return true;
        }

        // Regra de negócio: só remove se o projeto não tiver tarefas.
        public bool remover(Projeto p)
        {
            Projeto achado = buscar(p);
            if (achado == null || achado.Tarefas.Count > 0)
                return false;
            return itens.Remove(achado);
        }

        public Projeto buscar(Projeto p)
        {
            int i = itens.IndexOf(p);
            return i >= 0 ? itens[i] : null;
        }

        public List<Projeto> listar()
        {
            return new List<Projeto>(itens);
        }
    }
}

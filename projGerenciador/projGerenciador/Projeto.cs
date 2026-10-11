using System;
using System.Collections.Generic;

namespace projGerenciador
{
    internal class Projeto
    {
        private int id;
        private string nome;
        private List<Tarefa> tarefas = new List<Tarefa>();
        private int proxTarefa = 1;

        public int Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public List<Tarefa> Tarefas { get => tarefas; }

        public Projeto(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
        }

        public Projeto(int id) : this(id, "")
        { }

        public Projeto() : this(0, "")
        { }

        // Numera a tarefa sequencialmente dentro do projeto.
        public void adicionarTarefa(Tarefa t)
        {
            t.Id = proxTarefa;
            proxTarefa++;
            tarefas.Add(t);
        }

        public bool removerTarefa(Tarefa t)
        {
            return tarefas.Remove(t);
        }

        public Tarefa buscarTarefa(Tarefa t)
        {
            int i = tarefas.IndexOf(t);
            return i >= 0 ? tarefas[i] : null;
        }

        public List<Tarefa> tarefasPorStatus(string s)
        {
            return tarefas.FindAll(t => string.Equals(t.Status, s, StringComparison.OrdinalIgnoreCase));
        }

        public List<Tarefa> tarefasPorPrioridade(int p)
        {
            return tarefas.FindAll(t => t.Prioridade == p);
        }

        public int totalAbertas()
        {
            return tarefasPorStatus(Tarefa.ABERTA).Count;
        }

        public int totalFechadas()
        {
            return tarefasPorStatus(Tarefa.FECHADA).Count;
        }

        public int totalCanceladas()
        {
            return tarefasPorStatus(Tarefa.CANCELADA).Count;
        }

        public override string ToString()
        {
            return string.Format("Projeto {0} - {1} ({2} tarefa(s))", id, nome, tarefas.Count);
        }

        public override bool Equals(object obj)
        {
            Projeto outro = obj as Projeto;
            return outro != null && this.id == outro.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}

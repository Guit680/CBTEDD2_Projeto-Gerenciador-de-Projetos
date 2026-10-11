using System;

namespace projGerenciador
{
    internal class Tarefa
    {
        public const string ABERTA = "Aberta";
        public const string FECHADA = "Fechada";
        public const string CANCELADA = "Cancelada";

        private int id;
        private string titulo;
        private string descricao;
        private int prioridade;         // 1=alta, 2=média, 3=baixa
        private string status;
        private DateTime dataCriacao;
        private DateTime dataConclusao; // DateTime.MinValue = sem data

        public int Id { get => id; set => id = value; }
        public string Titulo { get => titulo; set => titulo = value; }
        public string Descricao { get => descricao; set => descricao = value; }
        public int Prioridade
        {
            get => prioridade;
            set
            {
                if (value < 1 || value > 3)
                    throw new ArgumentException("Prioridade deve ser 1 (alta), 2 (média) ou 3 (baixa).");
                prioridade = value;
            }
        }
        public string Status { get => status; }
        public DateTime DataCriacao { get => dataCriacao; }
        public DateTime DataConclusao { get => dataConclusao; }

        public Tarefa(int id, string titulo, string descricao, int prioridade)
        {
            this.id = id;
            this.titulo = titulo;
            this.descricao = descricao;
            this.Prioridade = prioridade;
            this.status = ABERTA;
            this.dataCriacao = DateTime.Now;
            this.dataConclusao = DateTime.MinValue;
        }

        public Tarefa(int id) : this(id, "", "", 2)
        { }

        public Tarefa() : this(0, "", "", 2)
        { }

        // Só tarefas abertas podem ser concluídas / canceladas.
        public void concluir()
        {
            if (status == ABERTA)
            {
                status = FECHADA;
                dataConclusao = DateTime.Now;
            }
        }

        public void cancelar()
        {
            if (status == ABERTA)
            {
                status = CANCELADA;
                dataConclusao = DateTime.Now;
            }
        }

        // Só tarefas fechadas ou canceladas podem ser reabertas.
        public void reabrir()
        {
            if (status != ABERTA)
            {
                status = ABERTA;
                dataConclusao = DateTime.MinValue;
            }
        }

        public string nomePrioridade()
        {
            switch (prioridade)
            {
                case 1: return "Alta";
                case 2: return "Média";
                default: return "Baixa";
            }
        }

        public override string ToString()
        {
            string s = string.Format("[{0}] {1} | Prioridade: {2} | {3} | Criada em {4:dd/MM/yyyy}",
                id, titulo, nomePrioridade(), status, dataCriacao);
            if (dataConclusao != DateTime.MinValue)
                s += string.Format(" | Encerrada em {0:dd/MM/yyyy}", dataConclusao);
            if (!string.IsNullOrWhiteSpace(descricao))
                s += "\n      " + descricao;
            return s;
        }

        public override bool Equals(object obj)
        {
            Tarefa outra = obj as Tarefa;
            return outra != null && this.id == outra.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;

namespace projGerenciador
{
    internal class Program
    {
        static Projetos projetos = new Projetos();

        static void Main(string[] args)
        {
            int op;
            do
            {
                Console.WriteLine();
                Console.WriteLine("===== GERENCIADOR DE PROJETOS =====");
                Console.WriteLine(" 0. Sair");
                Console.WriteLine(" 1. Adicionar projeto");
                Console.WriteLine(" 2. Pesquisar projeto");
                Console.WriteLine(" 3. Remover projeto (apenas se sem tarefas)");
                Console.WriteLine(" 4. Adicionar tarefa em projeto");
                Console.WriteLine(" 5. Concluir tarefa");
                Console.WriteLine(" 6. Cancelar tarefa");
                Console.WriteLine(" 7. Reabrir tarefa");
                Console.WriteLine(" 8. Listar tarefas de um projeto");
                Console.WriteLine(" 9. Filtrar tarefas (status/prioridade) em um projeto");
                Console.WriteLine("10. Filtrar tarefas (status/prioridade) em todos os projetos");
                Console.WriteLine("11. Resumo geral");
                op = lerInt("Opção: ");

                switch (op)
                {
                    case 0: Console.WriteLine("Encerrando..."); break;
                    case 1: adicionarProjeto(); break;
                    case 2: pesquisarProjeto(); break;
                    case 3: removerProjeto(); break;
                    case 4: adicionarTarefa(); break;
                    case 5: mudarStatus(5); break;
                    case 6: mudarStatus(6); break;
                    case 7: mudarStatus(7); break;
                    case 8: listarTarefas(); break;
                    case 9: filtrarEmProjeto(); break;
                    case 10: filtrarEmTodos(); break;
                    case 11: resumoGeral(); break;
                    default: Console.WriteLine("Opção inválida!"); break;
                }
            } while (op != 0);
        }

        static void adicionarProjeto()
        {
            Console.Write("Nome do projeto: ");
            string nome = Console.ReadLine();
            Projeto p = new Projeto(0, nome);
            if (projetos.adicionar(p))
                Console.WriteLine("Projeto {0} criado.", p.Id);
            else
                Console.WriteLine("Não foi possível adicionar o projeto.");
        }

        static void pesquisarProjeto()
        {
            Projeto p = pedirProjeto();
            if (p == null) return;

            Console.WriteLine(p);
            string[] statuses = { Tarefa.ABERTA, Tarefa.FECHADA, Tarefa.CANCELADA };
            foreach (string s in statuses)
            {
                List<Tarefa> lista = p.tarefasPorStatus(s);
                Console.WriteLine("--- {0}s: {1} ---", s, lista.Count);
                foreach (Tarefa t in lista)
                    Console.WriteLine("  " + t);
            }
            Console.WriteLine("Totais -> Abertas: {0} | Fechadas: {1} | Canceladas: {2}",
                p.totalAbertas(), p.totalFechadas(), p.totalCanceladas());
        }

        static void removerProjeto()
        {
            Projeto p = pedirProjeto();
            if (p == null) return;
            if (p.Tarefas.Count > 0)
            {
                Console.WriteLine("Não é possível remover: o projeto possui {0} tarefa(s).", p.Tarefas.Count);
                return;
            }
            Console.WriteLine(projetos.remover(p) ? "Projeto removido." : "Não foi possível remover.");
        }

        static void adicionarTarefa()
        {
            Projeto p = pedirProjeto();
            if (p == null) return;

            Console.Write("Título: ");
            string titulo = Console.ReadLine();
            Console.Write("Descrição: ");
            string descricao = Console.ReadLine();
            int prio = lerPrioridade();

            Tarefa t = new Tarefa(0, titulo, descricao, prio);
            p.adicionarTarefa(t);
            Console.WriteLine("Tarefa {0} adicionada ao projeto {1}.", t.Id, p.Id);
        }

        // 5 = concluir, 6 = cancelar, 7 = reabrir
        static void mudarStatus(int acao)
        {
            Projeto p = pedirProjeto();
            if (p == null) return;
            Tarefa t = pedirTarefa(p);
            if (t == null) return;

            switch (acao)
            {
                case 5:
                    if (t.Status != Tarefa.ABERTA) { Console.WriteLine("Só tarefas abertas podem ser concluídas (está {0}).", t.Status); return; }
                    t.concluir();
                    break;
                case 6:
                    if (t.Status != Tarefa.ABERTA) { Console.WriteLine("Só tarefas abertas podem ser canceladas (está {0}).", t.Status); return; }
                    t.cancelar();
                    break;
                case 7:
                    if (t.Status == Tarefa.ABERTA) { Console.WriteLine("A tarefa já está aberta."); return; }
                    t.reabrir();
                    break;
            }
            Console.WriteLine("Tarefa atualizada: " + t);
        }

        static void listarTarefas()
        {
            Projeto p = pedirProjeto();
            if (p == null) return;
            Console.WriteLine(p);
            mostraTarefas(p.Tarefas, null);
        }

        static void filtrarEmProjeto()
        {
            Projeto p = pedirProjeto();
            if (p == null) return;

            List<Tarefa> lista = filtrar(p.Tarefas);
            if (lista == null) return;
            mostraTarefas(lista, null);
        }

        static void filtrarEmTodos()
        {
            List<Projeto> todos = projetos.listar();
            if (todos.Count == 0)
            {
                Console.WriteLine("Nenhum projeto cadastrado.");
                return;
            }

            // Pergunta o critério uma vez e aplica em cada projeto
            Console.WriteLine("Filtrar por: 1-Status  2-Prioridade");
            int criterio = lerInt("Critério: ");
            string status = null;
            int prio = 0;
            if (criterio == 1)
            {
                status = lerStatus();
                if (status == null) return;
            }
            else if (criterio == 2)
            {
                prio = lerPrioridade();
            }
            else
            {
                Console.WriteLine("Critério inválido!");
                return;
            }

            int total = 0;
            foreach (Projeto p in todos)
            {
                List<Tarefa> lista = criterio == 1 ? p.tarefasPorStatus(status) : p.tarefasPorPrioridade(prio);
                if (lista.Count == 0) continue;
                Console.WriteLine("--- {0} ---", p);
                foreach (Tarefa t in lista)
                    Console.WriteLine("  " + t);
                total += lista.Count;
            }
            Console.WriteLine("Total de tarefas encontradas: {0}", total);
        }

        static void resumoGeral()
        {
            List<Projeto> todos = projetos.listar();
            int abertas = 0, fechadas = 0, canceladas = 0;
            foreach (Projeto p in todos)
            {
                abertas += p.totalAbertas();
                fechadas += p.totalFechadas();
                canceladas += p.totalCanceladas();
            }
            Console.WriteLine("=== Resumo geral ===");
            Console.WriteLine("Projetos: {0}", todos.Count);
            Console.WriteLine("Tarefas abertas: {0} | fechadas: {1} | canceladas: {2}", abertas, fechadas, canceladas);

            // Concluídas em relação às abertas: fechadas / (abertas + fechadas)
            int base_ = abertas + fechadas;
            double perc = base_ == 0 ? 0 : 100.0 * fechadas / base_;
            Console.WriteLine("Concluídas em relação às abertas: {0:F1}%", perc);
        }

        // ---------- auxiliares ----------
        static List<Tarefa> filtrar(List<Tarefa> origem)
        {
            Console.WriteLine("Filtrar por: 1-Status  2-Prioridade");
            int criterio = lerInt("Critério: ");
            if (criterio == 1)
            {
                string s = lerStatus();
                if (s == null) return null;
                return origem.FindAll(t => t.Status == s);
            }
            if (criterio == 2)
            {
                int p = lerPrioridade();
                return origem.FindAll(t => t.Prioridade == p);
            }
            Console.WriteLine("Critério inválido!");
            return null;
        }

        static void mostraTarefas(List<Tarefa> lista, string titulo)
        {
            if (titulo != null) Console.WriteLine(titulo);
            Console.WriteLine("=== Tarefas: {0} ===", lista.Count);
            foreach (Tarefa t in lista)
                Console.WriteLine("  " + t);
        }

        static Projeto pedirProjeto()
        {
            int id = lerInt("Nº do projeto: ");
            Projeto p = projetos.buscar(new Projeto(id));
            if (p == null)
                Console.WriteLine("Projeto não encontrado!");
            return p;
        }

        static Tarefa pedirTarefa(Projeto p)
        {
            int id = lerInt("Nº da tarefa: ");
            Tarefa t = p.buscarTarefa(new Tarefa(id));
            if (t == null)
                Console.WriteLine("Tarefa não encontrada neste projeto!");
            return t;
        }

        static int lerPrioridade()
        {
            int v = lerInt("Prioridade (1=alta, 2=média, 3=baixa): ");
            while (v < 1 || v > 3)
                v = lerInt("Inválida. Prioridade (1=alta, 2=média, 3=baixa): ");
            return v;
        }

        static string lerStatus()
        {
            int v = lerInt("Status (1=Aberta, 2=Fechada, 3=Cancelada): ");
            switch (v)
            {
                case 1: return Tarefa.ABERTA;
                case 2: return Tarefa.FECHADA;
                case 3: return Tarefa.CANCELADA;
                default:
                    Console.WriteLine("Status inválido!");
                    return null;
            }
        }

        static int lerInt(string msg)
        {
            int v;
            Console.Write(msg);
            while (!int.TryParse(Console.ReadLine(), out v))
            {
                Console.Write("Valor inválido. " + msg);
            }
            return v;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

class Veiculo
{
    public int Id { get; private set; }
    public int Capacidade { get; private set; }
    public int PassageirosTransportados { get; set; }
    public int Viagens { get; set; }

    public Veiculo(int id, int capacidade)
    {
        Id = id;
        Capacidade = capacidade;
        PassageirosTransportados = 0;
        Viagens = 0;
    }
}

class Garagem
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    private Stack<Veiculo> PilhaVeiculos { get; set; }

    public Garagem(int id, string nome)
    {
        Id = id;
        Nome = nome;
        PilhaVeiculos = new Stack<Veiculo>();
    }

    public void Estacionar(Veiculo veiculo)
    {
        PilhaVeiculos.Push(veiculo);
    }

    public Veiculo Remover()
    {
        return PilhaVeiculos.Count > 0 ? PilhaVeiculos.Pop() : null;
    }

    public List<Veiculo> ListarVeiculos()
    {
        return PilhaVeiculos.ToList();
    }

    public int QuantidadeVeiculos()
    {
        return PilhaVeiculos.Count;
    }
}

class ControleFrota
{
    private List<Veiculo> veiculos;
    private List<Garagem> garagens;
    private List<(int OrigemId, int DestinoId, int VeiculoId, int Passageiros)> viagens;
    private bool jornadaAtiva;

    public ControleFrota()
    {
        veiculos = new List<Veiculo>();
        garagens = new List<Garagem>();
        viagens = new List<(int, int, int, int)>();
        jornadaAtiva = false;
    }

    public void CadastrarVeiculo(int capacidade)
    {
        if (jornadaAtiva)
        {
            Console.WriteLine("Não é possível cadastrar veículos durante a jornada.");
            return;
        }

        int id = veiculos.Count + 1;
        veiculos.Add(new Veiculo(id, capacidade));
        Console.WriteLine($"Veículo {id} cadastrado com capacidade {capacidade}.");
    }

    public void CadastrarGaragem(string nome)
    {
        if (jornadaAtiva)
        {
            Console.WriteLine("Não é possível cadastrar garagens durante a jornada.");
            return;
        }

        int id = garagens.Count + 1;
        garagens.Add(new Garagem(id, nome));
        Console.WriteLine($"Garagem '{nome}' cadastrada com ID {id}.");
    }

    public void IniciarJornada()
    {
        if (jornadaAtiva)
        {
            Console.WriteLine("A jornada já foi iniciada.");
            return;
        }

        if (garagens.Count < 2)
        {
            Console.WriteLine("É necessário pelo menos duas garagens.");
            return;
        }

        for (int i = 0; i < veiculos.Count; i++)
        {
            garagens[i % garagens.Count].Estacionar(veiculos[i]);
        }

        jornadaAtiva = true;
        Console.WriteLine("Jornada iniciada.");
    }

    public void EncerrarJornada()
    {
        if (!jornadaAtiva)
        {
            Console.WriteLine("A jornada não foi iniciada.");
            return;
        }

        foreach (var veiculo in veiculos)
        {
            Console.WriteLine($"Veículo {veiculo.Id}: Passageiros transportados: {veiculo.PassageirosTransportados}");
            veiculo.PassageirosTransportados = 0;
            veiculo.Viagens = 0;
        }

        viagens.Clear();
        jornadaAtiva = false;
        Console.WriteLine("Jornada encerrada.");
    }

    public void LiberarViagem(int origemId, int destinoId, int passageiros)
    {
        if (!jornadaAtiva)
        {
            Console.WriteLine("A jornada não foi iniciada.");
            return;
        }

        var garagemOrigem = garagens.FirstOrDefault(g => g.Id == origemId);
        var garagemDestino = garagens.FirstOrDefault(g => g.Id == destinoId);

        if (garagemOrigem == null || garagemDestino == null)
        {
            Console.WriteLine("Garagem de origem ou destino inválida.");
            return;
        }

        var veiculo = garagemOrigem.Remover();

        if (veiculo == null)
        {
            Console.WriteLine("Nenhum veículo disponível na garagem de origem.");
            return;
        }

        if (passageiros > veiculo.Capacidade)
        {
            Console.WriteLine($"Número de passageiros excede a capacidade do veículo {veiculo.Capacidade}.");
            return;
        }

        veiculo.PassageirosTransportados += passageiros;
        veiculo.Viagens++;

        viagens.Add((origemId, destinoId, veiculo.Id, passageiros));
        garagemDestino.Estacionar(veiculo);

        Console.WriteLine($"Viagem liberada de '{garagemOrigem.Nome}' para '{garagemDestino.Nome}' com veículo {veiculo.Id} transportando {passageiros} passageiros.");
    }

    public void ListarVeiculosGaragem(int garagemId)
    {
        var garagem = garagens.FirstOrDefault(g => g.Id == garagemId);

        if (garagem == null)
        {
            Console.WriteLine("Garagem não encontrada.");
            return;
        }

        Console.WriteLine($"Garagem '{garagem.Nome}' - Veículos:");

        foreach (var veiculo in garagem.ListarVeiculos())
        {
            Console.WriteLine($"- Veículo {veiculo.Id}, Capacidade: {veiculo.Capacidade}");
        }
    }

    public void InformarViagens(int origemId, int destinoId)
    {
        int quantidade = viagens.Count(v => v.OrigemId == origemId && v.DestinoId == destinoId);
        Console.WriteLine($"Quantidade de viagens de garagem ID {origemId} para garagem ID {destinoId}: {quantidade}");
    }

    public void ListarViagens(int origemId, int destinoId)
    {
        var viagensFiltradas = viagens.Where(v => v.OrigemId == origemId && v.DestinoId == destinoId).ToList();

        Console.WriteLine($"Viagens de garagem ID {origemId} para garagem ID {destinoId}:");

        foreach (var viagem in viagensFiltradas)
        {
            Console.WriteLine($"- Veículo {viagem.VeiculoId}, Passageiros: {viagem.Passageiros}");
        }
    }

    public void InformarPassageiros(int origemId, int destinoId)
    {
        int totalPassageiros = viagens
            .Where(v => v.OrigemId == origemId && v.DestinoId == destinoId)
            .Sum(v => v.Passageiros);

        Console.WriteLine($"Passageiros transportados de garagem ID {origemId} para garagem ID {destinoId}: {totalPassageiros}");
    }
}

// Programa principal
class Program
{
    static void Main(string[] args)
    {
        var controle = new ControleFrota();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1. Cadastrar Garagem");
            Console.WriteLine("2. Cadastrar Veículo");
            Console.WriteLine("3. Iniciar Jornada");
            Console.WriteLine("4. Encerrar Jornada");
            Console.WriteLine("5. Liberar Viagem");
            Console.WriteLine("6. Listar Veículos em Garagem");
            Console.WriteLine("7. Informar Viagens");
            Console.WriteLine("8. Listar Viagens");
            Console.WriteLine("9. Informar Passageiros");
            Console.WriteLine("0. Sair");
            Console.Write("Opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Nome da garagem: ");
                    string nomeGaragem = Console.ReadLine();
                    controle.CadastrarGaragem(nomeGaragem);
                    break;
                case "2":
                    Console.Write("Capacidade do veículo: ");
                    int capacidadeVeiculo = int.Parse(Console.ReadLine());
                    controle.CadastrarVeiculo(capacidadeVeiculo);
                    break;
                case "3":
                    controle.IniciarJornada();
                    break;
                case "4":
                    controle.EncerrarJornada();
                    break;
                case "5":
                    Console.Write("ID da garagem de origem: ");
                    int origemId = int.Parse(Console.ReadLine());
                    Console.Write("ID da garagem de destino: ");
                    int destinoId = int.Parse(Console.ReadLine());
                    Console.Write("Número de passageiros: ");
                    int passageiros = int.Parse(Console.ReadLine());
                    controle.LiberarViagem(origemId, destinoId, passageiros);
                    break;
                case "6":
                    Console.Write("ID da garagem: ");
                    int garagemId = int.Parse(Console.ReadLine());
                    controle.ListarVeiculosGaragem(garagemId);
                    break;
                case "7":
                    Console.Write("ID da garagem de origem: ");
                    origemId = int.Parse(Console.ReadLine());
                    Console.Write("ID da garagem de destino: ");
                    destinoId = int.Parse(Console.ReadLine());
                    controle.InformarViagens(origemId, destinoId);
                    break;
                case "8":
                    Console.Write("ID da garagem de origem: ");
                    origemId = int.Parse(Console.ReadLine());
                    Console.Write("ID da garagem de destino: ");
                    destinoId = int.Parse(Console.ReadLine());
                    controle.ListarViagens(origemId, destinoId);
                    break;
                case "9":
                    Console.Write("ID da garagem de origem: ");
                    origemId = int.Parse(Console.ReadLine());
                    Console.Write("ID da garagem de destino: ");
                    destinoId = int.Parse(Console.ReadLine());
                    controle.InformarPassageiros(origemId, destinoId);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
}


class Pessoa
{
    public string Nome { get; set; }
    public int Idade { get; set; }
    public double Altura { get; set; }

    private static Random random = new Random();

    public static Pessoa[] GerarArray(int quantidade)
    {
        Pessoa[] array = new Pessoa[quantidade];
        for (int i = 0; i < quantidade; i++)
        {
            array[i] = new Pessoa
            {
                Nome = "Pessoa" + i,
                Idade = random.Next(1, 100),
                Altura = random.NextDouble() * 2
            };
        }
        return array;
    }
}
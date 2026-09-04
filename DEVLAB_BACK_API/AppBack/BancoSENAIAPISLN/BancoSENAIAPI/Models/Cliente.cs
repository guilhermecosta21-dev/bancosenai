namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string CPF { get; set; }
        public int NumeroAgencia { get; set; } = 10;
        public decimal SaldoTotal { get; set; } = 0.0m;
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}
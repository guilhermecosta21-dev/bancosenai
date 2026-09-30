using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        [Key]
        public int CodigoCliente { get; set; }
        [Required]
        public string NomeCliente { get; set; }
        [Required]
        public string CPF { get; set; }
        [Required]
        public int NumeroAgencia { get; set; } = 10;
        [Required]
        public decimal SaldoTotal { get; set; } = 0.0m;
        [Required]
        public string Sexo { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string Cidade { get; set; }
        [Required]
        public string Estado { get; set; }
    }
}
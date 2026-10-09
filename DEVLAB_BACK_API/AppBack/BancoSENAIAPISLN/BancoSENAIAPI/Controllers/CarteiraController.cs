using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteira.ToListAsync();
            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {
            if (novaCarteira == null)
                return BadRequest(new { message = "Dados inválidos." });

            if (string.IsNullOrWhiteSpace(novaCarteira.NomeCarteira))
                return BadRequest(new { message = "Nome da carteira é obrigatório." });

            if (await _context.Carteira.AnyAsync(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de carteira já existe." });

            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            _context.Carteira.Add(novaCarteira);
            await _context.SaveChangesAsync();
            return Created("", novaCarteira);
        }

        [HttpPut("{numero}")]
        public async Task<IActionResult> Atualizar(int numero, [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == numero);
            if (carteiraExistente == null)
                return NotFound(new { message = "Carteira não encontrada." });

            if (carteiraAtualizada.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            carteiraExistente.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteiraExistente.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{numero}")]
        public async Task<IActionResult> Apagar(int numero)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == numero);
            if (carteira == null)
                return NotFound(new { message = "Carteira não encontrada." });

            _context.Carteira.Remove(carteira);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Carteira excluída com sucesso." });
        }
    }
}
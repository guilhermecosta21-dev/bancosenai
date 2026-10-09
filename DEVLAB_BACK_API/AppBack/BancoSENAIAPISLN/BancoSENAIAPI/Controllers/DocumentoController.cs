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
    public class DocumentoController : Controller
    {

        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
            );

        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if(arquivo.Length > 2097152)
            {
                return BadRequest(new { mensagem = "Erro: O arquivo excede o limite de 2MB." });
            }

            string extensaoDeArquivo = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };

            if (!extensoesPermitidas.Contains(extensaoDeArquivo))
            {
                return BadRequest(new { mensagem = "Deu erro, o formato de arquivo está inválido. Apenas arquivos .pdf, .png e .jpg são permitidos" });
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string extensao = Path.GetExtension(arquivo.FileName);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadado = new DocumentoMetadado
            {
                Id = _nextId++,
                Nome = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _context.DocumentoMetadado.Add(documentoMetadado);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarPorCliente(int codigoCliente)
        {
            var documentos = await _context.DocumentoMetadado.Where(d => d.CodigoCliente == codigoCliente).ToListAsync();

            if (!documentos.Any())
            {
                return NotFound(new { mensagem = "Nenhum documento encontrado." });
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArquivo(int id)
        {
            var arquivo = await _context.DocumentoMetadado.FirstOrDefaultAsync(d => d.Id == id);

            if (arquivo == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (!System.IO.File.Exists(arquivo.Caminho))
            {
                return NotFound(new { mensagem = "Arquivo físico não encontrado no servidor." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(arquivo.Caminho);

            string nomeArquivo = $"{arquivo.Nome}{arquivo.Extensao}";

            return File(fileBytes, "application/octet-stream", nomeArquivo);
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id) 
        {
            var arquivo = await _context.DocumentoMetadado.FirstOrDefaultAsync(d => d.Id == id);

            if(arquivo == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (System.IO.File.Exists(arquivo.Caminho))
            {
                System.IO.File.Delete(arquivo.Caminho);
            }

            _context.DocumentoMetadado.Remove(arquivo);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "O documento e arquivo foram excluídos com sucesso." });
        }
    }
}

using DatabaseAccess.Data;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvogadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdvogadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Advogados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advogado>>> GetAdvogados()
        {
            return await _context.Advogados.ToListAsync();
        }

        // GET: api/Advogados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Advogado>> GetAdvogado(int id)
        {
            var advogado = await _context.Advogados.FindAsync(id);

            if (advogado == null)
            {
                return NotFound();
            }

            return advogado;
        }

        // GET: api/Advogados/Departamento/Civil
        [HttpGet("Departamento/{departamento}")]
        public async Task<ActionResult<IEnumerable<Advogado>>> GetAdvogadosByDepartamento(string departamento)
        {
            return await _context.Advogados
                .Where(a => a.Departamento == departamento)
                .ToListAsync();
        }
    }
} 
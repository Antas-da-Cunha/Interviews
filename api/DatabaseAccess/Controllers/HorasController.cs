using DatabaseAccess.Data;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HorasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Horas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hora>>> GetHoras()
        {
            return await _context.Horas.ToListAsync();
        }

        // GET: api/Horas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hora>> GetHora(int id)
        {
            var hora = await _context.Horas.FirstOrDefaultAsync(h => h.ID == id);

            if (hora == null)
            {
                return NotFound();
            }

            return hora;
        }

        // GET: api/Horas/Advogado/5
        [HttpGet("Advogado/{id}")]
        public async Task<ActionResult<IEnumerable<Hora>>> GetHorasByAdvogado(int id)
        {
            return await _context.Horas
                .Where(h => h.ID_Advogado == id)
                .ToListAsync();
        }

        // GET: api/Horas/Departamento/Civil
        [HttpGet("Departamento/{departamento}")]
        public async Task<ActionResult<IEnumerable<Hora>>> GetHorasByDepartamento(string departamento)
        {
            return await _context.Horas
                .Where(h => h.Departamento == departamento)
                .ToListAsync();
        }
    }
} 
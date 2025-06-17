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

        // GET: api/Advogados/total-hours
        [HttpGet("total-hours")]
        public async Task<ActionResult<IEnumerable<object>>> GetTotalHoursPerAdvogado()
        {
            var result = await _context.Advogados
                .Select(a => new {
                    AdvogadoId = a.ID,
                    Nome = a.Nome + " " + a.Sobrenome,
                    TotalHoras = _context.Horas
                        .Where(h => h.ID_Advogado == a.ID && h.Minutos_Registados != null)
                        .Sum(h => (int?)h.Minutos_Registados) / 60.0 // Convert minutes to hours
                })
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Advogados/total-hours/all
        [HttpGet("total-hours/all")]
        public async Task<ActionResult<double>> GetTotalHoursAllAdvogados()
        {
            // Sum all registered minutes in Horas and convert to hours
            var totalMinutos = await _context.Horas.SumAsync(h => (int?)h.Minutos_Registados);
            double totalHoras = (totalMinutos ?? 0) / 60.0;
            return Ok(totalHoras);
        }

        // GET: api/Advogados/Departamento/Civil
        [HttpGet("Departamento/{departamento}")]
        public async Task<ActionResult<IEnumerable<Advogado>>> GetAdvogadosByDepartamento(string departamento)
        {
            return await _context.Advogados
                .Where(a => a.Departamento == departamento)
                .ToListAsync();
        }

        // GET: api/Advogados/{id}/monthly-evolution
        [HttpGet("{id:int}/monthly-evolution")]
        public async Task<ActionResult<IEnumerable<object>>> GetMonthlyEvolution(int id)
        {
            // Group Horas by year and month for a specific advogado, sum hours per month
            var monthly = await _context.Horas
                .Where(h => h.ID_Advogado == id && h.Data != null && h.Minutos_Registados != null)
                .GroupBy(h => new { h.Data.Value.Year, h.Data.Value.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalHoras = g.Sum(h => (int?)h.Minutos_Registados) / 60.0 // Convert minutes to hours
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            return Ok(monthly);
        }

        // GET: api/Advogados/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Advogado>> GetAdvogado(int id)
        {
            var advogado = await _context.Advogados.FindAsync(id);

            if (advogado == null)
            {
                return NotFound();
            }

            return advogado;
        }
    }
}
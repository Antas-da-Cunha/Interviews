using DatabaseAccess.Data;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard/TopAdvogados
        [HttpGet("TopAdvogados")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetTopAdvogados()
        {
            var topAdvogados = await _context.Horas
                .GroupBy(h => h.ID_Advogado)
                .Select(g => new
                {
                    AdvogadoID = g.Key,
                    TotalMinutos = g.Sum(h => h.Minutos_Registados)
                })
                .OrderByDescending(a => a.TotalMinutos)
                .Take(5)
                .ToListAsync();

            var result = new List<dynamic>();
            foreach (var adv in topAdvogados)
            {
                var advogado = await _context.Advogados.FirstOrDefaultAsync(a => a.ID == adv.AdvogadoID);
                if (advogado != null)
                {
                    result.Add(new
                    {
                        ID = advogado.ID,
                        Nome = advogado.Nome,
                        Sobrenome = advogado.Sobrenome,
                        Departamento = advogado.Departamento,
                        TotalMinutos = adv.TotalMinutos
                    });
                }
            }

            return result;
        }

        // GET: api/Dashboard/HorasPorDepartamento
        [HttpGet("HorasPorDepartamento")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetHorasPorDepartamento()
        {
            var horasPorDepartamento = await _context.Horas
                .GroupBy(h => h.Departamento)
                .Select(g => new
                {
                    Departamento = g.Key,
                    TotalMinutos = g.Sum(h => h.Minutos_Registados)
                })
                .OrderByDescending(d => d.TotalMinutos)
                .ToListAsync();

            return horasPorDepartamento;
        }

        // GET: api/Dashboard/AdvogadosPorDepartamento
        [HttpGet("AdvogadosPorDepartamento")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAdvogadosPorDepartamento()
        {
            var advogadosPorDepartamento = await _context.Advogados
                .GroupBy(a => a.Departamento)
                .Select(g => new
                {
                    Departamento = g.Key,
                    TotalAdvogados = g.Count()
                })
                .OrderByDescending(d => d.TotalAdvogados)
                .ToListAsync();

            return advogadosPorDepartamento;
        }
    }
} 
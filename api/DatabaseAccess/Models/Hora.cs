using System;

namespace DatabaseAccess.Models
{
    public class Hora
    {
        public int? ID { get; set; }
        public string? Caso { get; set; }
        public int? ID_Advogado { get; set; }
        public int? Minutos_Registados { get; set; }
        public string? Departamento { get; set; }
        public DateTime? Data { get; set; }
    }
} 
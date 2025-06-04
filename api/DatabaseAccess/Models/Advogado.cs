using System;

namespace DatabaseAccess.Models
{
    public class Advogado
    {
        public int ID { get; set; }
        public required string Nome { get; set; }
        public required string Sobrenome { get; set; }
        public required string Email { get; set; }
        public required string Departamento { get; set; }
        public required string Escalao { get; set; }
        public required string Cidade { get; set; }
        public DateTime Aniversario { get; set; }
    }
} 
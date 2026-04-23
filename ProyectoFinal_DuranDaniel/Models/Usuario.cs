namespace ProyectoFinal_DuranDaniel.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string CorreoPersonal { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
        public ICollection<Curso> CursosQueImparte { get; set; } = new List<Curso>();
    }
}
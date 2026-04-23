namespace ProyectoFinal_DuranDaniel.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CarreraId { get; set; }
        public int DocenteId { get; set; }
        public Carrera Carrera { get; set; } = null!;
        public Usuario Docente { get; set; } = null!;
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
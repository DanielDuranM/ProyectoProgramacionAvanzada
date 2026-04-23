namespace ProyectoFinal_DuranDaniel.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
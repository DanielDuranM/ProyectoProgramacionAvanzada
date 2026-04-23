namespace ProyectoFinal_DuranDaniel.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int CursoId { get; set; }
        public DateTime FechaMatricula { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Matriculado";
        public Usuario Estudiante { get; set; } = null!;
        public Curso Curso { get; set; } = null!;
    }
}
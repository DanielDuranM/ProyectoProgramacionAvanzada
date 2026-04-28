namespace ProyectoFinal_DuranDaniel.Models
{
    /// <summary>
    /// Representa un archivo/material subido a Firebase Storage
    /// y asociado a un Curso.
    /// </summary>
    public class MaterialCurso
    {
        public int     Id          { get; set; }
        public int     CursoId     { get; set; }
        public int     SubidoPorId { get; set; }          // UsuarioId del docente

        public string  NombreArchivo { get; set; } = string.Empty;   // nombre original
        public string  UrlFirebase   { get; set; } = string.Empty;   // URL pública en Storage
        public string  TipoArchivo   { get; set; } = string.Empty;   // ContentType
        public long    TamanioBytes  { get; set; }

        public DateTime FechaSubida  { get; set; } = DateTime.Now;

        // Navegación
        public Curso   Curso      { get; set; } = null!;
        public Usuario SubidoPor  { get; set; } = null!;
    }
}

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace ProyectoFinal_DuranDaniel.Services
{
    /// <summary>
    /// Servicio para interactuar con Firebase Storage (Google Cloud Storage).
    /// Permite subir, obtener URL pública y eliminar archivos.
    /// </summary>
    public class FirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration config)
        {
            var credPath = config["Firebase:CredentialsPath"]!;
            _bucketName  = config["Firebase:StorageBucket"]!;

            var credential = GoogleCredential.FromFile(credPath);
            _storageClient = StorageClient.Create(credential);
        }

        // ── Subir archivo ────────────────────────────────────────────────────
        /// <summary>
        /// Sube un IFormFile a Firebase Storage y devuelve la URL pública.
        /// </summary>
        /// <param name="archivo">Archivo proveniente del formulario</param>
        /// <param name="carpeta">Carpeta virtual dentro del bucket (ej: "materiales")</param>
        public async Task<string> SubirArchivoAsync(IFormFile archivo, string carpeta = "materiales")
        {
            // Nombre único para evitar colisiones
            var nombreObjeto = $"{carpeta}/{Guid.NewGuid()}_{Path.GetFileName(archivo.FileName)}";

            using var stream = archivo.OpenReadStream();
            var obj = await _storageClient.UploadObjectAsync(
                bucket      : _bucketName,
                objectName  : nombreObjeto,
                contentType : archivo.ContentType,
                source      : stream
            );

            // Hacer el objeto público
            await _storageClient.UpdateObjectAsync(obj, new UpdateObjectOptions());

            return $"https://storage.googleapis.com/{_bucketName}/{nombreObjeto}";
        }

        // ── Eliminar archivo ─────────────────────────────────────────────────
        /// <summary>
        /// Elimina un archivo de Firebase Storage a partir de su URL pública.
        /// </summary>
        public async Task EliminarArchivoAsync(string urlArchivo)
        {
            if (string.IsNullOrEmpty(urlArchivo)) return;

            var prefijo       = $"https://storage.googleapis.com/{_bucketName}/";
            var nombreObjeto  = urlArchivo.Replace(prefijo, string.Empty);

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, nombreObjeto);
            }
            catch
            {
                // Si el objeto no existe, ignorar el error
            }
        }

        // ── Listar archivos de una carpeta ───────────────────────────────────
        /// <summary>
        /// Lista todos los objetos dentro de una carpeta virtual del bucket.
        /// </summary>
        public async Task<List<string>> ListarArchivosAsync(string carpeta)
        {
            var urls  = new List<string>();
            var items = _storageClient.ListObjectsAsync(_bucketName, prefix: carpeta + "/");

            await foreach (var item in items)
            {
                if (!item.Name.EndsWith("/")) // Ignorar entradas de "carpeta"
                    urls.Add($"https://storage.googleapis.com/{_bucketName}/{item.Name}");
            }

            return urls;
        }
    }
}

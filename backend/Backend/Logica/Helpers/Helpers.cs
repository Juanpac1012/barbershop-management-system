using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class Helpers
    {
        private const int SaltSize = 16; // 16 bytes = 128 bits
        private const int HashSize = 32; // 32 bytes = 256 bits
        private const int Iterations = 10000; // Aumentar iteraciones mejora seguridad
                                            
        public static string HashearContraseña(string contraseña, out string salt)
        {
            if (string.IsNullOrWhiteSpace(contraseña))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            // Generar un salt aleatorio
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes); // Guardamos el salt en formato string

            // Hashear con PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(contraseña, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(hash); // Guardamos el hash en formato string
            }
        }

        public static bool VerificarContraseña(string contraseñaIngresada, string hashGuardado, string saltGuardado)
        {
            byte[] saltBytes = Convert.FromBase64String(saltGuardado);

            using (var pbkdf2 = new Rfc2898DeriveBytes(contraseñaIngresada, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashIngresado = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(hashIngresado) == hashGuardado;
            }
        }

        //ver cuales metodos tienen que ser static
        public bool EsCorreoValido(string correo)
        {
            // Verifica que el correo no sea nulo o vacío.
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            // Patrón simple para validar correo electrónico.
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(correo, patron);
        }

        public bool EsPasswordSeguro(string password)
        {
            // Verifica que el password no sea nulo o vacío.
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Patrón que valida el password según los criterios mencionados.
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            return Regex.IsMatch(password, patron);
        }

        //revisar y ver si lo cambio por un token
        public string GenerarPin(int longitud)
        {
            // Crea una instancia de Random.
            Random rnd = new Random();
            // Utiliza StringBuilder para construir el PIN.
            StringBuilder pin = new StringBuilder();

            // Itera la cantidad de veces según la longitud deseada.
            for (int i = 0; i < longitud; i++)
            {
                // Genera un dígito entre 0 y 9 y lo agrega al PIN.
                pin.Append(rnd.Next(0, 10));
            }

            return pin.ToString();
        }

        //revisar
        public bool SesionActiva(Sesion sesion)
        {
            return sesion != null && sesion.estado == EnumEstadoSesion.abierto;
        }

        //revisar
        public byte[] ConvertirImagenBinario(string rutaImagen)
        {
            // Verificar que la ruta de la imagen no esté vacía o nula
            if (string.IsNullOrWhiteSpace(rutaImagen))
            {
                throw new ArgumentException("La ruta de la imagen no puede estar vacía.");
            }

            // Verificar si el archivo existe en la ruta proporcionada
            if (!File.Exists(rutaImagen))
            {
                throw new FileNotFoundException("La imagen no se encontró en la ruta especificada.");
            }

            // Leer el archivo de imagen y convertirlo a binario (arreglo de bytes)
            byte[] imagenBinaria = File.ReadAllBytes(rutaImagen);

            return imagenBinaria;
        }
        //revisar
        public static Error CrearError(EnumErrores codigo, string mensaje)
        {
            return new Error
            {
                ErrorCode = codigo,
                Message = mensaje
            };
        }
        //revisar
        public static string ObtenerErrores(List<Error> errores)
        {
            if (errores == null || errores.Count == 0)
                return "No se encontraron errores.";

            return string.Join("\n", errores.Select(e => $"{e.ErrorCode}: {e.Message}"));
        }

    }
}
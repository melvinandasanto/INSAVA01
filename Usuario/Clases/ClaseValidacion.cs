using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Usuario
{
    public static class ClaseValidacion
    {
        /// <summary>
        /// Permite solo dígitos numéricos (y teclas de control como borrar, flechas, etc.).
        /// </summary>
        public static void ValidarCampoNumerico(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Permite solo letras (y teclas de control como borrar, flechas, etc.).
        /// </summary>
        public static void ValidarCampoLetras(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Permite solo números decimales en el evento KeyPress.
        /// </summary>
        public static void ValidarCampoDecimal(KeyPressEventArgs e, TextBox txt)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (char.IsDigit(e.KeyChar))
                return;

            if (e.KeyChar == '.')
            {
                if (txt.Text.Contains("."))
                    e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Valida que un número decimal no sea negativo.
        /// </summary>
        public static bool EsNegativo(decimal valor)
        {
            return valor < 0;
        }

        /// <summary>
        /// Valida si una cadena es un correo electrónico válido.
        /// </summary>
        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida si una cadena es un número de teléfono válido (solo dígitos, longitud configurable).
        /// </summary>
        public static bool EsTelefonoValido(string telefono, int longitudMin = 7, int longitudMax = 15)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;
            if (!telefono.All(char.IsDigit)) return false;
            return telefono.Length >= longitudMin && telefono.Length <= longitudMax;
        }

        /// <summary>
        /// Valida si una cadena cumple con una longitud mínima y máxima.
        /// </summary>
        public static bool EsLongitudValida(string texto, int min, int max)
        {
            if (texto == null) return false;
            return texto.Length >= min && texto.Length <= max;
        }

        /// <summary>
        /// Valida si una cadena está vacía o es nula.
        /// </summary>
        public static bool EstaVacioONulo(string texto)
        {
            return string.IsNullOrWhiteSpace(texto);
        }

        /// <summary>
        /// Valida si una contraseña es segura (mínimo 8 caracteres, mayúscula, minúscula, número y símbolo).
        /// </summary>
        public static bool EsPasswordSegura(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;
            bool tieneMayus = password.Any(char.IsUpper);
            bool tieneMinus = password.Any(char.IsLower);
            bool tieneNumero = password.Any(char.IsDigit);
            bool tieneSimbolo = password.Any(ch => !char.IsLetterOrDigit(ch));
            return tieneMayus && tieneMinus && tieneNumero && tieneSimbolo;
        }

        /// <summary>
        /// Valida si una cadena es alfanumérica.
        /// </summary>
        public static bool EsAlfanumerico(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            return texto.All(char.IsLetterOrDigit);
        }

        /// <summary>
        /// Valida si un número está dentro de un rango.
        /// </summary>
        public static bool EstaEnRango(decimal valor, decimal min, decimal max)
        {
            return valor >= min && valor <= max;
        }

        /// <summary>
        /// Valida si una fecha está en un rango.
        /// </summary>
        public static bool FechaEnRango(DateTime fecha, DateTime min, DateTime max)
        {
            return fecha >= min && fecha <= max;
        }

        /// <summary>
        /// Valida si un objeto es nulo.
        /// </summary>
        public static bool EsNulo(object obj)
        {
            return obj == null;
        }

        // ==========================================================
        // ✅ NUEVA VALIDACIÓN: Validar datos del proveedor
        // ==========================================================
        /// <summary>
        /// Valida los campos requeridos de un proveedor.
        /// </summary>
        /// <param name="nombre">Nombre del proveedor</param>
        /// <param name="telefono">Teléfono del proveedor</param>
        /// <param name="mensaje">Mensaje de error si hay algo incorrecto</param>
        /// <returns>True si los datos son válidos, False si hay errores</returns>
        public static bool ValidarProveedor(string nombre, string telefono, out string mensaje)
        {
            mensaje = "";

            // Nombre vacío o nulo
            if (EstaVacioONulo(nombre))
            {
                mensaje = "Por favor, ingrese el nombre del proveedor.";
                return false;
            }

            // Longitud del nombre
            if (!EsLongitudValida(nombre, 3, 50))
            {
                mensaje = "El nombre del proveedor debe tener entre 3 y 50 caracteres.";
                return false;
            }

            // Teléfono vacío o nulo
            if (EstaVacioONulo(telefono))
            {
                mensaje = "Por favor, ingrese el número de teléfono del proveedor.";
                return false;
            }

            // Teléfono numérico y longitud correcta
            if (!EsTelefonoValido(telefono, 8, 8))
            {
                mensaje = "El número de teléfono debe contener exactamente 8 dígitos numéricos.";
                return false;
            }

            return true; // ✅ Todo correcto
        }
    }
}

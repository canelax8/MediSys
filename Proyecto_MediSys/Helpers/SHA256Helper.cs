using System.Security.Cryptography;
using System.Text;

namespace Proyecto_MediSys.Helpers
{
    public static class SHA256Helper
    {
        public static string Encriptar(string texto)
        {
            using SHA256 sha = SHA256.Create();

            byte[] bytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(texto));

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
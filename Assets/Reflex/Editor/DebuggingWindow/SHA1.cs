using System.Security.Cryptography;
using System.Text;

namespace Reflex.Editor.DebuggingWindow
{
    internal static class SHA1
    {
        public static string Hash(object obj)
        {
            var input = obj.ToString();
            
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        
        public static string ShortHash(object obj)
        {
            var hash = Hash(obj);
            return hash.Substring(0, 7);
        }
    }
}
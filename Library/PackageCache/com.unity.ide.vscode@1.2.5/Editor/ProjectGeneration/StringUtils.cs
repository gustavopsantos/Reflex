using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.VSCode.EditorTests")]

namespace VSCodeEditor
{
    internal static class StringUtils
    {
        private const char WinSeparator = '\\';
        private const char UnixSeparator = '/';

        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (Path.DirectorySeparatorChar == WinSeparator)
                path = path.Replace(UnixSeparator, WinSeparator);
            if (Path.DirectorySeparatorChar == UnixSeparator)
                path = path.Replace(WinSeparator, UnixSeparator);

            return path.Replace(string.Concat(WinSeparator, WinSeparator), WinSeparator.ToString());
        }
    }
}
using System;
using System.IO;
using System.Security;
using System.Text;

namespace VSCodeEditor
{
    public interface IFileIO
    {
        bool Exists(string fileName);

        string ReadAllText(string fileName);
        void WriteAllText(string fileName, string content);

        void CreateDirectory(string pathName);
        string EscapedRelativePathFor(string file, string projectDirectory);
    }

    class FileIOProvider : IFileIO
    {
        public bool Exists(string fileName)
        {
            return File.Exists(fileName);
        }

        public string ReadAllText(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public void WriteAllText(string fileName, string content)
        {
            File.WriteAllText(fileName, content, Encoding.UTF8);
        }

        public void CreateDirectory(string pathName)
        {
            Directory.CreateDirectory(pathName);
        }

        public string EscapedRelativePathFor(string file, string projectDirectory)
        {
            var projectDir = Path.GetFullPath(projectDirectory);

            // We have to normalize the path, because the PackageManagerRemapper assumes
            // dir seperators will be os specific.
            var absolutePath = Path.GetFullPath(file.NormalizePath());
            var path = SkipPathPrefix(absolutePath, projectDir);

            return SecurityElement.Escape(path);
        }

        private static string SkipPathPrefix(string path, string prefix)
        {
            return path.StartsWith($@"{prefix}{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                ? path.Substring(prefix.Length + 1)
                : path;
        }
    }
}

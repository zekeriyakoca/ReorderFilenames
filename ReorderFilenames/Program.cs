using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReorderFilenames
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            if (Debugger.IsAttached)
            {
                currentDirectory = @"C:\TestDirectoryForReorderFilenames";
                Directory.CreateDirectory(currentDirectory);
            }
            foreach (var dir in Directory.EnumerateDirectories(currentDirectory))
            {
                ReorderFiles(dir);
            }

        }

        private static void ReorderFiles(string directory)
        {
            var fileIndex = 1;
            var files = Directory.EnumerateFiles(directory).ToList()
               .Select(f => new { Name = Path.GetFileNameWithoutExtension(f), FullPath = f })
               .Where(f => int.TryParse(f.Name, out _))
               .Select(f => new { Name = int.Parse(f.Name), FullPath = f.FullPath })
               .OrderBy(f => f.Name)
               .Select(f => f.FullPath)
               .ToList();

            foreach (var file in files)
            {
                var newPath = GetNewPath(file, fileIndex++);
                if (file != newPath)
                    File.Move(file, newPath);
            }

            foreach (var dir in Directory.EnumerateDirectories(directory))
            {
                ReorderFiles(dir);
            }
        }

        static string GetNewPath(string oldPath, int fileIndex)
            => $"{Path.GetDirectoryName(oldPath)}{Path.DirectorySeparatorChar}{fileIndex}{Path.GetExtension(oldPath)}";
    }
}

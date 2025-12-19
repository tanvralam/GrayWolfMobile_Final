using GrayWolf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IFileSystem = GrayWolf.Interfaces.IFileSystem;
namespace GrayWolf.Services
{
    public class FileSystem : IFileSystem
    {
        public virtual string GetAppDocumentsFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
        }

        public virtual Task<string> GetOrCreateFolderByNameAsync(string folderName)
        {
            var folderPath = Path.Combine(GetAppDocumentsFolderPath(), folderName);
            Directory.CreateDirectory(folderPath);
            var isExists = Directory.Exists(folderPath);
            return Task.FromResult(folderPath);
        }

        public virtual Task<string> GetOrCreateFolderByPathAsync(string folderPath)
        {
            var isExists = Directory.Exists(folderPath);
            if (!isExists)
            {
                Directory.CreateDirectory(folderPath);
            }
            return Task.FromResult(folderPath);
        }

        public virtual Task WriteAllTextAsync(string path, string text)
        {
            CreateDirectoryForFile(path);
            File.WriteAllText(path, text);
            return Task.CompletedTask;
        }

        public virtual Task AppendAllTextAsync(string path, string text)
        {
            CreateDirectoryForFile(path);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, text);
            }
            else
            {
                File.AppendAllText(path, text);
            }
            return Task.CompletedTask;
        }

        public virtual Task WriteAllLinesAsync(string path, IEnumerable<string> lines)
        {
            CreateDirectoryForFile(path);
            File.WriteAllLines(path, lines);
            return Task.CompletedTask;
        }

        private void CreateDirectoryForFile(string path)
        {
            var directory = path.Replace(Path.GetFileName(path), "");
            var exists = Directory.Exists(directory);
            if (!exists)
            {
                Directory.CreateDirectory(directory);
            }
        }

        public virtual Task<string> ReadAllTextAsync(string path)
        {
            var text = File.ReadAllText(path);
            return Task.FromResult(text);
        }

        public virtual Task<byte[]> ReadAllBytesAsync(string path)
        {
            return Task.FromResult(File.ReadAllBytes(path));
        }

        public virtual Task<IEnumerable<string>> ReadAllLinesAsync(string path)
        {
            var result = File.ReadAllLines(path).AsEnumerable();
            return Task.FromResult(result);
        }

        public virtual Task<bool> IsFileExistAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public virtual Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            CreateDirectoryForFile(path);
            File.WriteAllBytes(path, bytes);
            return Task.CompletedTask;
        }

        public virtual Task DeleteDirectoryAsync(string path, bool recursive) 
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
            return Task.CompletedTask;
        }

        public virtual Task<bool> IsDirectoryExistAsync(string path)
        {
            return Task.FromResult(Directory.Exists(path));
        }

        public virtual Task DeleteAsync(string filePath)
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        public virtual Task MoveAsync(string from, string to)
        {
            File.Move(from, to);
            return Task.CompletedTask;
        }
    }
}

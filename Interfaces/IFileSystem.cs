using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IFileSystem
    {
        Task AppendAllTextAsync(string path, string text);
        Task<string> GetOrCreateFolderByNameAsync(string folderName);
        Task<string> GetOrCreateFolderByPathAsync(string folderPath);
        Task<bool> IsFileExistAsync(string path);
        Task<byte[]> ReadAllBytesAsync(string path);
        Task<string> ReadAllTextAsync(string path);
        Task<IEnumerable<string>> ReadAllLinesAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        Task WriteAllBytesAsync(string path, byte[] bytes);
        Task DeleteAsync(string filePath);
        Task<bool> IsDirectoryExistAsync(string folderPath);
        Task DeleteDirectoryAsync(string folderPath, bool recursive);
        Task MoveAsync(string from, string to);
        Task WriteAllLinesAsync(string path, IEnumerable<string> lines);
        string GetAppDocumentsFolderPath();
    }
}

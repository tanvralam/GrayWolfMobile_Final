using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IThumbnailService
    {
        Task<Stream> GetImageStreamAsync(string filePath);
    }
}

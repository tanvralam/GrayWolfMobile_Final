using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface ISaveFile
    {
       Task SaveFileAsync(string localFilePath, string type, IEnumerable<string> attachments);
    }
}

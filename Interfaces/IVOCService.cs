using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IVOCService
    {
        Task<List<VOCItem>> GetVOCItemsAsync();
    }
}

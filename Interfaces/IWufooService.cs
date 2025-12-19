using GrayWolf.Models.Domain;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IWufooService
    {
        Task SubmitDemoFormAsync(DemoModeForm form);
    }
}

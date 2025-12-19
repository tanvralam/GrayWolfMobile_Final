using GrayWolf.Models.DTO;
using System.Collections.Generic;

namespace GrayWolf.Interfaces
{
    public interface IGetLocalDeviceData
    {
        GetDeviceDataReturn GetLocalDeviceData();
        string GetExternalPath();

        IEnumerable<string> GetDirectories(string dir);

    }
}

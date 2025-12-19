using System;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
	public interface IPermissionsService
	{
        Task<bool> RequestBlePermissions();
    }
}


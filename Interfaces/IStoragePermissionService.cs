using System;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
	public interface IStoragePermissionService
	{
        Task<bool> RequestStoragePermissions();
    }
}


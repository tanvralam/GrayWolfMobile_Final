using System;
using System.Threading.Tasks;
using GrayWolf.Interfaces;

namespace GrayWolf.iOS.Dependencies
{
    public class PermissionsService : IPermissionsService
    {
        public Task<bool> RequestBlePermissions()
        {
            return Task.FromResult(true);
        }
    }
}


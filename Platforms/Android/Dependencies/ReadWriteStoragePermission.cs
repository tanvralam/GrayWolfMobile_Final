using System;
using System.Collections.Generic;
using Android.OS;
using GrayWolf.Interfaces;
using System.Threading.Tasks;
using GrayWolf.Droid.Dependencies;

namespace GrayWolf.Droid.Dependencies
{
    public class StoragePermissionsService : IStoragePermissionService
    {
        public async Task<bool> RequestStoragePermissions()
        {
            // Working files use app-specific storage and exports use Android's
            // document picker, so modern Android does not need legacy storage access.
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                return true;
            }

            var status = await Permissions.CheckStatusAsync<ReadWriteStoragePermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<ReadWriteStoragePermission>();
            }

            return status == PermissionStatus.Granted;
        }
    }

    public class ReadWriteStoragePermission : Permissions.BasePlatformPermission
    {
       public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.ReadExternalStorage, true),
            (Android.Manifest.Permission.WriteExternalStorage, true)
        }.ToArray();
    }
}


using GrayWolf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace GrayWolf.Services
{
    public class StoragePermissionsService : IStoragePermissionService
    {
        public async Task<bool> RequestStoragePermissions()
        {
            PermissionStatus status;
            // New SDK 31 API for storage permissions

            if (Buiildv.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                status = await Permissions.CheckStatusAsync<ReadWriteStoragePermission>();
                bool showAlert = Permissions.ShouldShowRationale<ReadWriteStoragePermission>();
                if (showAlert || status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    // show an alert explaining why we need the permissions
                }
                status = await Permissions.RequestAsync<ReadWriteStoragePermission>();

                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            else
            {
                // < 31 API, use location permission to search and connect to devices
                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                bool showAlert = Permissions.ShouldShowRationale<ReadWriteStoragePermission>();
                if (showAlert || status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    // show an alert explaining why we need the permissions
                }
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return true;// status == PermissionStatus.Granted;
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

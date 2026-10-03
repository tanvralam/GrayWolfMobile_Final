using Android.OS;
using GrayWolf.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace GrayWolf.Droid.Dependencies
{
    public class PermissionsService : IPermissionsService
    {
        public async Task<bool> RequestBlePermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                // Android presents SCAN and CONNECT together as the Nearby devices
                // permission group. Request them together for both upgrades and
                // clean installations.
                var bluetoothStatus = await Permissions.CheckStatusAsync<BluetoothPermission>();
                if (bluetoothStatus != PermissionStatus.Granted)
                {
                    bluetoothStatus = await Permissions.RequestAsync<BluetoothPermission>();
                }

                return bluetoothStatus == PermissionStatus.Granted;
            }

            // Android 11 and earlier use location permission for BLE scanning.
            var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (locationStatus != PermissionStatus.Granted)
            {
                locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return locationStatus == PermissionStatus.Granted;
        }
    }

    public class BluetoothPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[]
            {
                (Android.Manifest.Permission.BluetoothScan, true),
                (Android.Manifest.Permission.BluetoothConnect, true)
            };
    }
}

using System;
using Android.OS;
using System.Threading.Tasks;
using System.Collections.Generic;
using GrayWolf.Interfaces;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace GrayWolf.Droid.Dependencies
{
    public class PermissionsService : IPermissionsService
    {
        public async Task<bool> RequestBlePermissions1()
        {
            PermissionStatus status;
            // New SDK 31 API for bluetooth permissions
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                status = await Permissions.CheckStatusAsync<BluetoothPermission>();
                bool showAlert = Permissions.ShouldShowRationale<BluetoothPermission>();
                if (showAlert || status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    // show an alert explaining why we need the permissions
                }
                status = await Permissions.RequestAsync<BluetoothPermission>();
                ///////////////////////////
                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                //bool showAlert = Permissions.ShouldShowRationale<BluetoothPermission>();
                if (showAlert || status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    // show an alert explaining why we need the permissions
                }
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            else
            {
                // < 31 API, use location permission to search and connect to devices
                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                bool showAlert = Permissions.ShouldShowRationale<BluetoothPermission>();
                if (showAlert || status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    // show an alert explaining why we need the permissions
                }
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return true;
        }

        public async Task<bool> RequestBlePermissions()
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.S) // Android 12+
            {
                // Request BLUETOOTH_SCAN
                var scanStatus = await Permissions.CheckStatusAsync<BluetoothScanPermission>();
                if (scanStatus != PermissionStatus.Granted)
                {
                    var showScanRationale = Permissions.ShouldShowRationale<BluetoothScanPermission>();
                    if (showScanRationale || scanStatus == PermissionStatus.Unknown || scanStatus == PermissionStatus.Denied)
                    {
                        // Optionally show rationale dialog here
                    }

                    scanStatus = await Permissions.RequestAsync<BluetoothScanPermission>();
                }

                // Request BLUETOOTH_CONNECT
                var connectStatus = await Permissions.CheckStatusAsync<BluetoothConnectPermission>();
                if (connectStatus != PermissionStatus.Granted)
                {
                    var showConnectRationale = Permissions.ShouldShowRationale<BluetoothConnectPermission>();
                    if (showConnectRationale || connectStatus == PermissionStatus.Unknown || connectStatus == PermissionStatus.Denied)
                    {
                        // Optionally show rationale dialog here
                    }

                    connectStatus = await Permissions.RequestAsync<BluetoothConnectPermission>();
                }

                // Optional: request location if needed
                var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (locationStatus != PermissionStatus.Granted)
                {
                    locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            else
            {
                // Older Android versions still need location for BLE
                var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (locationStatus != PermissionStatus.Granted)
                {
                    locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }

            return true;
        }

    }

    public class BluetoothPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            ("android.permission.BLUETOOTH_SCAN", true),
            ("android.permission.BLUETOOTH_CONNECT", true)
        }.ToArray();
    }




    public class BluetoothScanPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[] { (Android.Manifest.Permission.BluetoothScan, true) };
    }

    public class BluetoothConnectPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[] { (Android.Manifest.Permission.BluetoothConnect, true) };
    }


}


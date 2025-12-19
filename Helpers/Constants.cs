using System;

namespace GrayWolf.Helpers
{
    public static class Constants
    {
        //To maintain Battery Statuses...

        public static string BastteryStatusOK = "OK";
        public static string BastteryStatusStable = "Stabilizing";
        public static string BastteryStatusCalError = "CalError";
        public static string BastteryStatusLowBattery = "LowBattery";

        public const string DEVICE_SOURCE_CLOUD = "Cloud";
        public const string DEVICE_SOURCE_BLUETOOTH = "BT";
        public const string DEVICE_SOURCE_USB = "USB";
        public const string DEVICE_SOURCE_ANY = "Any";

        //BLE devices factories
        public const string BLE_FACTORY_REAL = "BleFactoryReal";
        public const string BLE_FACTORY_MOCK = "BleFactoryMock";

        public const string BLE_VISIBLE_DEVICES_UPDATED = "BleVisibleDevicesUpdated";
        public const string BLE_CONNECTED_DEVICES_UPDATED = "BleConnectedDevicesUpdated";

        public const int MAX_FILE_LOG_NAME_LENGTH = 30;

        //To Save Images Path...
        public static string ImgFilePath = String.Empty;

        //To save Log File Path...

        public static string TextFilePath = String.Empty;

        public const int INTERVAL_MS_CLOUD = 30000;  //CLOUD is only polled every 30-60 seconds
        public const int INTERVAL_MS_BLE = 1000; //Other data sources are polled at 1-5 second intervals.
        public const int INTERVAL_MS_USB = 1000;

        public const int INTERVAL_MIN_TO_CONSIDER_OFFLINE = 30;

        public const int INTERVAL_LOG_MS_DEFAULT = 30000;

        public const string SYNC_FUSION_LICENSE = "NDkzNjkxQDMxMzkyZTMyMmUzMG8vOXVMU0xqZi91T3FMek9CMWhPWmJHTVpxcXU4UFJiZktXQktpN2pNcjQ9";
        // "MzE1ODUwQDMxMzgyZTMyMmUzMFpmbzdzU3J6bFdrTUVQTmV4OU1ESDg2SGFPaXJ4aDF1UzQzbDdlMit0T2c9";

        public const string REBEX_LICENSE = "@31382e332e30X19jnzx1N8bNosvFzU/eYu2gZDn6X4yKZ+e6oE6bmFk=";// "==AdVbXrEa5KJ0+1QDYY1KbOTAWsgewsMFt5VJXc1CoKIk==";

        //Default zip file password
        public const string DEFAULT_LCZ_PASSWORD = "1997GWSS&%";

        public const string HTTP_CLIENT_KEY = "HttpClient";
        public const string WUFOO_HTTP_CLIENT_KEY = "WufooHttpClient";
        public const string SENSOR_TIPS_HTTP_CLIENT_KEY = "SensorTipsHttpClient";

        public const string WUFOO_API_SERVICE_KEY = "WufooApiService";
        public const string SENSOR_TIPS_API_SERVICE_KEY = "SensorTipsApiService";
        public const string GRAYWOLF_API_SERVICE_KEY = "GrayWolfApiService";

        public const string WUFOO_USERNAME = "OABY-7OKO-4VJZ-SQHP";
        public const string WUFOO_SECRET = "footastic"; //it can be any value, this value is from docs sample

        public const string WUFOO_BASE_URL = @"https://graywolf.wufoo.com";
        public const string SENSOR_TIPS_BASE_URL = @"https://sensortips.azurewebsites.net";

        public const long BLE_RESTORE_CONNECTION_TIMEOUT_MS = 1000;//1000 * 60 * 5;

        public const string CUSTOM_ZIP_PASSWORD_KEY = "CustomZipPassword";
        public const string CUSTOM_PASSWORD_REGEX = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";

        public const string DEMO_LOGIN = "demo@graywolfsensing.com";
        public const string DEMO_PASS = "vocprobe";

        public const int SNAPSHOT_BUTTON_DURATION = 5; // can't press faster than once every 5 seconds....

        public const double CO2_CUTOFF = 300.0;

    }
}

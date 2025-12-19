namespace GrayWolf.Enums
{
    public enum ProbeStatus
    {
        UNKNOWN = 1,
        READY = 2,
        LOGGING = 3,
        SNAPSHOT = 4,
        SHUTTINGDOWN = 5,
        GENERALERROR = 6,
        STABILIZING = 7,
        CHECKCALIBRATION = 8,
        OK = 9,
        HCHO_HI_CO=15,
        HCHO_NO_CO=16,
        PUMP_ERROR=17,
        LASER_ERROR=18,
        HCHO_LO_RH=19,
        ERROR_BATTERYCRITICAL = 102,
        ERROR_NORTHTEMP = 103,
        ERROR_BADRTC = 104,
        ERROR_NOSENSORS = 105,
        ERROR_BADSENSORVOLTAGE = 106,
        ERROR_FANFAULT = 107,
        ERROR_INTERNALTEMP = 108,
        ERROR_WIFIDISCONNECT = 109,
        ERROR_LOWMEMORY = 110
    }

    public class ProbeStatusToTextConverter
    {
        public static string ToFriendlyText(ProbeStatus status)
        {
            string message = status.ToString();
            switch (status)
            {
                // additional translations
                case Enums.ProbeStatus.GENERALERROR: message = "General Error"; break;
                case Enums.ProbeStatus.LASER_ERROR: message = "Laser Error"; break;
                case Enums.ProbeStatus.PUMP_ERROR: message = "Pump Error"; break;
                case Enums.ProbeStatus.ERROR_BATTERYCRITICAL: message = "Battery Critical"; break;
                case Enums.ProbeStatus.ERROR_BADRTC: message = "Clock Error"; break;
                case Enums.ProbeStatus.ERROR_NORTHTEMP: message = "Missing RH/Temp Sensor"; break;
                case Enums.ProbeStatus.ERROR_BADSENSORVOLTAGE:
                case Enums.ProbeStatus.ERROR_NOSENSORS: message = "Sensor Error"; break;
                case Enums.ProbeStatus.HCHO_HI_CO:
                case Enums.ProbeStatus.HCHO_LO_RH:
                case Enums.ProbeStatus.HCHO_NO_CO: message = "HCHO Error"; break;
                case Enums.ProbeStatus.ERROR_FANFAULT: message = "Fan Fault"; break;
                case Enums.ProbeStatus.ERROR_WIFIDISCONNECT: message = "WIFI Config Error"; break;
                case Enums.ProbeStatus.ERROR_LOWMEMORY: message = "Memory Low"; break;
            }

            return message;
        }
    }

}

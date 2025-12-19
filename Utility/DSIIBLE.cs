using GrayWolf.Models.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using GrayWolf.Services;
using GrayWolf.Enums;

namespace GrayWolf.Utility
{
    public class BLEJsonSensor
    {
        public int code = -1;
        public int unitCode = -1;
        public float value = 0.0f;
        public string timeStamp = "";
        public long id = -1;
        public int channel = 0;
    }

    public class BLEJson
    {
        public float version = 1.0f;
        public string generator = "DirectSenseII";
        public string deviceSerialNum = "";
        public string comID = "-1";
        public string stationID = "";
        public string deviceName = "";
        public string token = "";
        public BatteryStatus batteryStatusEnum = BatteryStatus.UNKNOWN;
        public ProbeStatus statusEnum = ProbeStatus.UNKNOWN;
        public SensorStatus sensorStatusEnum = SensorStatus.OK;
        public string uptime = "";
        public string timeStamp = "";
        public string source = "";
        public string deviceID = "";
        public List<BLEJsonSensor> data = new List<BLEJsonSensor>();
    }

    public class DSII_BLEGenerator
    {

        private BLEJson Device { get; set; }

        public const int TIMEOUTMINS = 3;
        public const int NUMPROBESTOLOOKFOR = 3;

        public DSII_BLEGenerator()
        {
            TheJSON = "";
        }

        public string TheJSON
        {
            get;
            set;
        }

        private float _simtemp = 20.0f;
        private float _simrh = 18.0f;
        private float _simco = 0.1f;
        private float _simco2 = 653.0f;
        private float _simtvoc = 140.0f;

        private float _simno2 = 0.01f;
        private float _simo3 = 0.21f;
        private float _simh2s = 0.35f;
        private float _simnh3 = 0.4f;
        private float _simso2 = 0.3f;
        private float _simhcn = 0.0f;
        private float _simdew = -5.0f;
        private float _simcl2 = 0.0f;

        private float _readingdirection = 1.0f; // climb to start

        private Random _random = new Random();

        private DateTime _simstartedat = DateTime.MinValue;

        public string GenerateSimulatedJsonDevice(DemoProbeType type)
        {
            _simstartedat = DateTime.UtcNow;
            Device = GenerateDevice(type);
            return UpdateSimulatedDevice(type);
        }

        private double Es(double T)
        {
            //Returns saturated vapour pressure in Pa
            double A, B, es;
            if (T > 0) { A = 17.269; B = 237.3; }
            else { A = 21.875; B = 265.5; }
            es = 610.78 * Math.Exp(A * T / (T + B));
            return es;
        }

        private double invEs(double e, double T)
        {
            //Returns T 0C from saturated vapour pressure (Pa)
            double A, B, f;
            if (T > 0) { A = 17.269; B = 237.3; }
            else { A = 21.875; B = 265.5; }
            f = Math.Log(e / 610.78) / A;
            return (B * f / (1 - f));
        }

        private double DewPoint(double rh, double T)
        {
            //%rh T 0C

            // added to avoid 1.$ reading
            if (rh <= 0.00) return 0.00;

            //First calculate the saturation vap. pres. es (Pa) at temperature T(0C):
            //es = 610.78 * exp {A T / (T + B) }  
            //where es in Pa, A = 17.2694 and B = 237.3 for T>0 otherwise 265.5.
            double es, e;
            es = Es(T);

            //Then calculate the actual vapour pressure e (Pa) using e = rH / 100 * es
            e = rh * es / 100.0;

            //Finally invert the equation for es since
            //e = es(Td). The dewpoint temperature Td (0C) is then obtained from
            //Td = B  f / { 1 - f  }where f = ln ( e / 610.78 ) / A	return counts;
            return (invEs(e, T));
        }



        public void UpdateValues()
        {

            _simtemp += (float)(_random.NextDouble() / 30.0) * _readingdirection;
            _simrh += (float)(_random.NextDouble() / 30.0) * _readingdirection;

            _simco += (float)(_random.NextDouble() / 80.0) * _readingdirection;
            _simco2 += (float)Math.Floor((_random.NextDouble() / 2.00) * _readingdirection);

            if (_random.Next(20) > 10)
            {
                _simtvoc += (float)Math.Floor((_random.NextDouble() / 3.50) * _readingdirection);
            }

            _simno2 += (float)(_random.NextDouble() / 100.0) * _readingdirection;
            _simo3 += (float)(_random.NextDouble() / 100.0) * _readingdirection;
            _simh2s += (float)(_random.NextDouble() / 100.0) * _readingdirection;
            _simnh3 += (float)(_random.NextDouble() / 100.0) * _readingdirection;
            _simso2 += (float)(_random.NextDouble() / 100.0) * _readingdirection;
            _simhcn += (float)(_random.NextDouble() / 100.0) * _readingdirection;

            if (_random.Next(20) > 18) _readingdirection *= -1.0f; // change direction

            if (_simtemp < 10.0f) _simtemp = 10.0f;
            if (_simtemp > 40.0f) _simtemp = 40.0f;

            if (_simrh<11.0f) _simrh = 11.0f;
            if (_simrh > 80.0f) _simrh = 80.0f;

            if (_simco < 0.0f) _simco = 0.0f;
            if (_simco > 10.0f) _simco = 10.0f;

            if (_simno2 < 0.0f) _simno2 = 0.00f;
            if (_simo3 < 0.0f) _simo3 = 0.00f;
            if (_simno2 > 0.038f) _simno2 = 0.038f;
            if (_simo3 > 0.041f) _simo3 = 0.041f;

            if (_simco2 < 340.0f) _simco2 = 340.0f;
            if (_simco2 > 2000.0f) _simco2 = 2000.0f;

            if (_simtvoc < 10.0f) _simtvoc = 10.0f;
            if (_simtvoc > 800.0f) _simtvoc = 800.0f;

            if (_simno2 <  0.0f) _simno2  = 0.0f;
            if (_simo3 < 0.0f) _simo3 = 0.0f;
            if (_simh2s < 0.0f) _simh2s = 0.0f;
            if (_simnh3 < 0.0f) _simnh3 = 0.0f;
            if (_simhcn < 0.0f) _simhcn = 0.0f;
            if (_simso2 < 0.0f) _simso2 = 0.0f;

            _simdew = (float) DewPoint((double)_simrh, (double)_simtemp);
        }

        public string UpdateSimulatedDevice(DemoProbeType mode)
        {
            TimeSpan sp = DateTime.UtcNow - _simstartedat;
            var device = Device;
            var timeStamp = DateTime.UtcNow.ToString("o");
            device.timeStamp = timeStamp;
            device.deviceID = $"{9999 + (int)mode}";
            device.uptime = $"{sp.Seconds}";

            UpdateValues();

            for (int ch = 0; ch < device.data.Count; ch++)
            {
                if (device.data[ch].code == SensorsService.TEMP) device.data[ch].value = _simtemp;
                if (device.data[ch].code == SensorsService.DEW) device.data[ch].value = _simdew;

                if (device.data[ch].code == SensorsService.RH) device.data[ch].value = _simrh;
                if (device.data[ch].code == SensorsService.CO) device.data[ch].value = _simco;
                if (device.data[ch].code == SensorsService.CO2) device.data[ch].value = _simco2;

                if (device.data[ch].code == SensorsService.O3) device.data[ch].value = _simo3;
                if (device.data[ch].code == SensorsService.SO2) device.data[ch].value = _simso2;
                if (device.data[ch].code == SensorsService.NO2) device.data[ch].value = _simno2;
                if (device.data[ch].code == SensorsService.SO2) device.data[ch].value = _simhcn;

                if (device.data[ch].code == SensorsService.H2S) device.data[ch].value = _simh2s;
                if (device.data[ch].code == SensorsService.NH3) device.data[ch].value = _simnh3;
                if (device.data[ch].code == SensorsService.CL2) device.data[ch].value = _simcl2;

                if (device.data[ch].code == SensorsService.TVOC_Baseline_PPB) device.data[ch].value = _simtvoc;
                if (device.data[ch].code == SensorsService.TVOC_PPM_200) device.data[ch].value = _simtvoc / 1000.0f;
                device.data[ch].timeStamp = timeStamp;
            }
            return JsonConvert.SerializeObject(device);
        }

        private List<BLEJsonSensor> GenerateSensorsForDevice_IAQ(int sensorIdInset)
        {
            SensorsService sensors = new SensorsService();
            GWL_Units units = new GWL_Units(sensors);

            var list = new List<BLEJsonSensor>
            {
                new BLEJsonSensor
                {
                    code = SensorsService.TVOC_Baseline_PPB,
                    unitCode = GWL_Units.PPB,
                    id = sensorIdInset + 1,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=0
                },
                new BLEJsonSensor
                {
                    code = SensorsService.CO,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 2,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=1
                },
                new BLEJsonSensor
                {
                    code = SensorsService.CO2,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 3,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=2
                },
                new BLEJsonSensor
                {
                    code = SensorsService.O3,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 4,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=3
                },
                new BLEJsonSensor
                {
                    code = SensorsService.NO2,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 5,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=4
                },
                new BLEJsonSensor
                {
                    code = SensorsService.CL2,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 6,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=5
                },
                new BLEJsonSensor
                {
                    code = SensorsService.TEMP,
                    unitCode = GWL_Units.C,
                    id = sensorIdInset + 7,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=6
                },
                new BLEJsonSensor
                {
                    code = SensorsService.RH,
                    unitCode = GWL_Units.RH,
                    id = sensorIdInset + 7,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=7
                },
                new BLEJsonSensor
                {
                    code = SensorsService.DEW,
                    unitCode = GWL_Units.C,
                    id = sensorIdInset + 8,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel=8
                }
            };
            return list;
        }
        private List<BLEJsonSensor> GenerateSensorsForDevice_TOX(int sensorIdInset)
        {
            SensorsService sensors = new SensorsService();
            GWL_Units units = new GWL_Units(sensors);

            var list = new List<BLEJsonSensor>
            {
                new BLEJsonSensor
                {
                    code = SensorsService.TVOC_PPM_200,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 1,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 0
                },
                new BLEJsonSensor
                {
                    code = SensorsService.NH3,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 2,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 1
                },
                new BLEJsonSensor
                {
                    code = SensorsService.SO2,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 3,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 2
                },
                new BLEJsonSensor
                {
                    code = SensorsService.NO,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 4,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 3
                },
                new BLEJsonSensor
                {
                    code = SensorsService.H2S,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 5,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 4
                },
                new BLEJsonSensor
                {
                    code = SensorsService.HCN,
                    unitCode = GWL_Units.PPM,
                    id = sensorIdInset + 6,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 5
                },
                new BLEJsonSensor
                {
                    code = SensorsService.TEMP,
                    unitCode = GWL_Units.C,
                    id = sensorIdInset + 7,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 6
                },
                new BLEJsonSensor
                {
                    code = SensorsService.RH,
                    unitCode = GWL_Units.RH,
                    id = sensorIdInset + 7,
                    timeStamp = DateTime.UtcNow.ToString("o"),
                    channel = 7
                },
            };
            return list;
        }

        private BLEJson GenerateDevice(DemoProbeType type)
        {
            string name = "DSII-IAQ";
            if (type == DemoProbeType.TOX) name = "DSII-TOX";
            if (type == DemoProbeType.Particulate) name = "PC-3016";

            var deviceNumber = (int)type;
            BLEJson device = new BLEJson
            {
                batteryStatusEnum = BatteryStatus.OK,
                comID = "-1",
                deviceName = $"SIMULATED {name}",
                generator = "DSII",
                deviceSerialNum = $"09-190{deviceNumber}",
                stationID = $"Test{deviceNumber}",
                statusEnum = ProbeStatus.OK,
                sensorStatusEnum = SensorStatus.OK,
                version = 1.0f,
                token = "",
                source = "Ble",
                timeStamp = DateTime.UtcNow.ToString("o"),
            };
            if (type== DemoProbeType.IAQ) device.data = GenerateSensorsForDevice_IAQ(19000 + (deviceNumber - 1) * 2);
            if (type == DemoProbeType.TOX) device.data = GenerateSensorsForDevice_TOX(19000 + (deviceNumber - 1) * 2);
            //if (type == DemoProbeType.Particulate) device.data = GenerateSensorsForDevice_Particulate(19000 + (deviceNumber - 1) * 2);

            return device;
        }
    }
}

using CommunityToolkit.Mvvm.Messaging;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace GrayWolf.Utility
{
    public class LoggerJSON
    {
        public const int FLUSHCOUNTER = 100;

        private LJH_Holder _holder;

        private GrayWolf.Interfaces.IFileSystem FileSystem { get; }
        private ISensorsService SensorsService { get; }

        public LoggerJSON(
            GrayWolf.Interfaces.IFileSystem fileSystem,
            ISensorsService sensorsService)
        {
            FileSystem = fileSystem;
            SensorsService = sensorsService;
        }

        public const string LOGGERZIPFILEEXTENSION = ".lcz";
        public const string DEFAULT_LCZ_PASSWORD = "1997GWSS&%";
        public const string LOGGERJSONFILEEXTENSION = ".lhj";
        public const int CURRENT_ENCODING_VERSION = 1;

        public async Task InitializeHolderAsync(string ljhHolderPath, string lcvFilePath)
        {
            try
            {
                if(lcvFilePath.IsNullOrWhiteSpace() || ljhHolderPath.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException();
                }
                var isLcvExistis = await FileSystem.IsFileExistAsync(lcvFilePath);
                if (!isLcvExistis)
                {
                    await FileSystem.WriteAllTextAsync(lcvFilePath, "");
                }
                var text = await FileSystem.ReadAllTextAsync(ljhHolderPath);
                _holder = JsonConvert.DeserializeObject<LJH_Holder>(text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize holder: {ex.Message}");
                _holder = new LJH_Holder()
                {
                    Version = 1
                };
            }
        }

        private bool ConvertToLHJ(LogFileRowDTO row, out LJH_Set set, out string csv, ParameterNameDisplayOption parameterNameDisplayOption)
        {
            // Takes "input" string, parses into Json
            // Creates a set representing the columns
            // creates a TAG to uniquely identify the set
            // if tag does not match, creates a new setID
            // returns CSV time and values

            set = new LJH_Set();
            var tag = "";
            csv = "";
            try
            {
                var utcrow = row.Timestamp;

                List<LJH_Column> ColumnArray = new List<LJH_Column>();
                List<LJH_Device> DeviceArray = new List<LJH_Device>();

                foreach (var source in row.Sources)
                {
                    if (!source.Devices.Any())
                    {
                        continue;
                    }

                    string src = $"{source.DataSource}";
                    tag += src + ":";

                    foreach (var logDevice in source.Devices)
                    {
                        var device = logDevice.ToLjhDevice();

                        set.Devices.Add(device);

                        tag += device.SerialNumber + ":";

                        foreach (var data in logDevice.Data)
                        {
                            LJH_Column column = data.ToLjhColumn(device.SerialNumber, parameterNameDisplayOption);

                            tag += $"{column.Code}:{column.UnitCode}:{column.Id}:";

                            double value = data.Value;

                            if (csv.Length == 0)
                            {
                                csv = utcrow.ToString("o");
                            }

                            if (csv.Length != 0) csv += ", ";
                            csv += value.ToString(CultureInfo.InvariantCulture);

                            set.Columns.Add(column);

                        } // for devices



                    } // for sources


                }

                if (_holder.Sets.FirstOrDefault(x => x.TAG == tag) is LJH_Set existingSet)
                {
                    set = existingSet;
                }
                else
                {
                    set.TAG = tag;
                }

                _holder.IsSimulated = _holder.IsSimulated || set.Devices.Any(x => x.IsSimulated);

                return set.Columns.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoggerJson: Failed to convert to LHJ: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> AppendRowAsync(LogFileRowDTO row, string ljhPath, string lcvPath, ParameterNameDisplayOption parameterNameDisplayOption)
        {
            try
            {
                ConvertDevicePositionsIntoSensors(row);

                // Take the Json coming in and extract to a header version of the Json and retrieve the CSV time and value data
                if (!ConvertToLHJ(row, out LJH_Set set, out string csv, parameterNameDisplayOption)) 
                    return false;

                // if the tag has changed, that means the configuration has changed and we need to treat it as nother dataset
                var tasks = new List<Task>();
                if(!(_holder.Sets.Any(x => x.TAG == set.TAG)))
                {
                    set.SetID = _holder.Sets.Count;
                    _holder.Sets.Add(set);
                    _holder.Version += 1;
                    tasks.Add(WriteLjhAsync(_holder, ljhPath));
                }

                // save the time, value data and prefix it with an ID identifying which set the data goes with
                string outline = set.SetID.ToString() + "," + csv + "\r\n";
                tasks.Add(FileSystem.AppendAllTextAsync(lcvPath, outline));
                await Task.WhenAll(tasks);
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Messenger.Default.Send(new LogRowAddedMessage(_holder, outline));
                });
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoggerJSON: Failed to append row: {ex.Message}");
                return false;
            }
        }

        private Task WriteLjhAsync(LJH_Holder holder, string path)
        {
            var json = JsonConvert.SerializeObject(holder);
            return FileSystem.WriteAllTextAsync(path, json);
        }

        private void ConvertDevicePositionsIntoSensors(LogFileRowDTO row)
        {
            foreach (var source in row.Sources)
            {
                var devices = source.Devices;
                if (devices == null)
                {
                    continue;
                }
                foreach (var device in devices)
                {
                    UpdateCoordinateSensor(device, SensorType.PRBSEN_LATITUDE, device.Position.Latitude);
                    UpdateCoordinateSensor(device, SensorType.PRBSEN_LONGITUDE, device.Position.Longitude);
                }
            }
        }

        private void UpdateCoordinateSensor(LogDeviceDTO device, SensorType type, double sensorValue)
        {
            var sensor = device.Data.FirstOrDefault(x => x.Sensor == $"{type}") ?? new LogDatumDTO
            {
                //Unit = SensorsService.GetSensorUnitName(65),
                Unit = $"{SensorUnit.PRBUNT_DEG}",
                Code = type,
                Id = 0,
                UnitCode = SensorUnit.PRBUNT_DEG,
                Sensor = SensorsService.GetSensorName((int)type)
            };
            sensor.Value = sensorValue;

            if (!device.Data.Contains(sensor))
            {
                device.Data.Add(sensor);
            }
        }

        public LJH_Holder Finalize()
        {
            return _holder;
        }

    }
}

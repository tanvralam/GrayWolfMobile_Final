using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GrayWolf.Models.Domain
{
    public abstract class RealBleDevice : BleDevice, IDisposable
    {
        private DateTime _lastButtonClickDateTime = DateTime.MinValue;

        protected enum ChannelReadingPriority { Top, High, Normal, Low};

        #region guid
        protected Guid ProbeGuid => new Guid("5adcfa03-1ae1-4b75-9667-d56bbad13008");

        protected Guid ProbeCharacteristicGuid => new Guid("e0803ded-165e-44e9-9848-9b2c385ba66c");

        protected Guid SensorGuid => new Guid("24cfd5b2-4663-435f-9634-52b08834c61b");

        protected Guid SensorChannelGuid => new Guid("ce16df8f-90cf-405d-b276-2941f5a036aa");

        protected Guid SensorDataGuid => new Guid("bcf8341b-ae61-4f9c-a6aa-7107a0694be6");

        protected Guid SensorValuesGuid => new Guid("70659e6a-e8a4-4ddd-b173-a6bf8781de1d");

        protected Guid SensorSTELsGuid => new Guid("6bf1c0ad-588e-45d2-bf24-18b60b955427");
        #endregion

        public IDevice NativeDevice { get; protected set; }

        public bool IsFetchFailed { get; set; }

        protected int Channels { get; set; }

        public bool IsDisconnectedByUser { get; set; }

        protected bool IsReconnecting { get; set; }

        protected bool WasConnected { get; set; }

        protected int Channel { get; set; } = 0;

        protected Dictionary<int, Reading> Sensors { get; } = new Dictionary<int, Reading>();

        protected ISensorsService SensorsService { get; }

        public ILogService LogService => Ioc.Default.GetService<ILogService>();

        private IAlertSoundService AlertSound => Ioc.Default.GetService<IAlertSoundService>();

        public RealBleDevice(
            IBleService bleService,
            IDeviceService deviceService) : base(
                bleService,
                deviceService)
        {
            SensorsService = Ioc.Default.GetService<ISensorsService>();
        }

        public abstract Task ConnectAsync();

        protected Reading GetOrCreateReading(int channel)
        {
            if (!Sensors.TryGetValue(channel, out Reading datum))
            {
                datum = new Reading
                {
                    Channel = channel,
                    OriginalUnit = new SensorUnit(),
                    DeviceId = GrayWolfDevice.DeviceID
                };
                Sensors[channel] = datum;
            }
            datum.TimeStamp = DateTime.UtcNow;
            return datum;
        }

        protected bool UpdateProbe(byte[] bytes)
        {
            if (!bytes.Any())
            {
                return false;
            }
            Debug.WriteLine("OnProbeResult");
            GrayWolfDevice.DeviceType = GetDeviceModel(bytes[0]);
            if (GrayWolfDevice.DeviceName.Contains("DSII"))
            {
                switch (GrayWolfDevice.DeviceType)
                {
                    case DeviceModelsEnum.Particulate_Meter:
                        GrayWolfDevice.DeviceName = GrayWolfDevice.DeviceName.Replace("DSII", "PM");
                        break;
                    case DeviceModelsEnum.Particle_Counter:
                        GrayWolfDevice.DeviceName = GrayWolfDevice.DeviceName.Replace("DSII", "PC");
                        break;
                    case DeviceModelsEnum.WolfRadio:
                        GrayWolfDevice.DeviceName = GrayWolfDevice.DeviceName.Replace("DSII", "WR");
                        break;
                    case DeviceModelsEnum.ZephyrXM:
                        GrayWolfDevice.DeviceName = GrayWolfDevice.DeviceName.Replace("DSII", "ZP");
                        break;
                }
            }
            GrayWolfDevice.Title = GrayWolfDevice.DeviceName;
            GrayWolfDevice.BatteryStatus = $"{GetBatteryStatus(bytes[4])}";
            GrayWolfDevice.BatteryStatusEnum = GetBatteryStatus(bytes[4]);
            GrayWolfDevice.Status = $"{GetProbeStatus(bytes[3])}";
            GrayWolfDevice.StatusEnum = GetProbeStatus(bytes[3]);


            /////////////////////////////////////////////////////////
            // Bill VW - June 24, 2022
            //
            // The goal here is to set the OVERALL status of the GrayWolfDevice's SensorStatus
            // Probe Status is the whole probe's condition
            // Sensor Status is set if there is one or more sensors exhibiting errors
            // so we walk thru the readings and see if any sensor is showing a problem
            //
            SensorStatus overallSensorStatus = SensorStatus.OK;
            if (GrayWolfDevice.Data != null)
            {
                foreach (Reading R in GrayWolfDevice.Data)
                {
                    SensorStatus sensorStatus;
                    if (!Enum.TryParse(R.Status, out sensorStatus)) sensorStatus = SensorStatus.OK;
                    if (!GrayWolf.Converters.DeviceConverters.IsSensorOK(sensorStatus))
                    {
                        if (sensorStatus != SensorStatus.NOTPRESENT) overallSensorStatus = sensorStatus;
                    }
                } // foreach
            }

            GrayWolfDevice.SensorStatus = overallSensorStatus.ToString();
            GrayWolfDevice.SensorStatusEnum = overallSensorStatus;
            // -- End Set Sensor Status for Device 
            ////////////////////////////////////////////////////////////

            GrayWolfDevice.DeviceSerialNum = GetDeviceSerialNum(bytes[0], new[] { bytes[1], bytes[2] });
            GrayWolfDevice.Uptime = GetUptime(new[] { bytes[9], bytes[8], bytes[7], bytes[6] });
            return true;
        }

        protected void UpdateReading(Reading reading, byte[] bytes)
        {
            var serialBytes = new[] { bytes[8], bytes[7], bytes[6], bytes[5] };
            var serial = BitConverter.ToInt32(serialBytes, 0);
            var sensorTypeCode = (bytes[1] << 8) + bytes[2];
            var sensorUnitCode = Convert.ToInt32(bytes[3]);

            //Sensor status
            reading.SensorId = serial;
            reading.SensorCode = (SensorType)sensorTypeCode;
            reading.Status = $"{GetSensorStatus(bytes[4])}";
            reading.DeviceSerialNumber = GrayWolfDevice.DeviceSerialNum;
            reading.Id = ReadingConverters.GetReadingId(GrayWolfDevice.DeviceID, reading.SensorId, (int)reading.SensorCode, reading.OriginalUnit.Code);
            reading.OriginalUnit.Name = SensorsService.GetSensorUnitName(sensorUnitCode);
            reading.OriginalUnit.Code = sensorUnitCode;
            reading.Name = SensorsService.GetSensorName(sensorTypeCode);
        }

        protected void UpdateReadingValue(Reading reading, byte[] bytes)
        {
            var value = BitConverter.ToSingle(new[] { bytes[1], bytes[2], bytes[3], bytes[4] }, 0);
            reading.OriginalUnit.Value = value.ToString(CultureInfo.InvariantCulture);
        }

        protected SensorStatus GetSensorStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(SensorStatus), status) ? (SensorStatus)status : SensorStatus.ERROR;
        }

        protected ProbeStatus GetProbeStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(ProbeStatus), status) ? (ProbeStatus)status : ProbeStatus.UNKNOWN;
        }

        protected BatteryStatus GetBatteryStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(BatteryStatus), status) ? (BatteryStatus)status : BatteryStatus.UNKNOWN;
        }

        protected string GetDeviceSerialNum(byte preByte, byte[] postBytes)
        {
            var preString = Convert.ToInt32(preByte & 0x1F);
            var post = (postBytes[0] << 8) + postBytes[1];
            var serialNum = $"{preString:00}-{post}";

            return serialNum;
        }

        private DeviceModelsEnum GetDeviceModel(byte preByte)
        {
            return DeviceConverters.ToDeviceModelFromBits(preByte);
        }

        protected long GetUptime(byte[] bytes)
        {
            var seconds = (uint)BitConverter.ToInt32(bytes, 0);
            return seconds;
        }

        public async void SendLogButtonMessageIfClicked(byte channelByte)
        {
            var wasClicked = WasLogButtonClicked(channelByte);
            if (wasClicked)
            {
                _lastButtonClickDateTime = DateTime.Now;
                LogFile logFile = LogService.CurrentFile;
                if (logFile != null)
                {
                    bool isCreated = await LogService.CreateSnapshotAsync(logFile, true);
                    var message = "";
                    if (isCreated)
                    {
                        message = string.Format(Localization.Localization.Log_SnapshotCreated_Format, logFile.Name);
                        AlertSound.PlaySystemSound(false);
                    }
                    else
                    {
                        message = Localization.Localization.Log_SnapshotNotCreated;
                        AlertSound.PlaySystemSound(true);
                    }
                    var tst = new ToastConfig(Localization.Localization.RecordSound_AttachmentAdded)
                    {
                        Duration = new TimeSpan(0, 0, 4),
                        Message = message,
                        MessageTextColor =System.Drawing.Color.White,
                        Position = ToastPosition.Bottom,
                    };
                    Ioc.Default.GetService<IUserDialogs>().Toast(tst);
                    Messenger.Default.Send(new LogButtonMessage(GrayWolfDevice));
                }
                else
                {
                    var toast = new ToastConfig("")
                    {
                        Duration = new TimeSpan(0, 0, 5),
                        Message = "File not created",
                        MessageTextColor =System.Drawing.Color.White,
                        Position = ToastPosition.Bottom
                    };
                    Ioc.Default.GetService<IUserDialogs>().Toast(toast);
                }
            }
        }

        protected bool WasLogButtonClicked(byte channelByte)
        {
            bool thebuttonpushed = (channelByte & 0x80) > 0; // mask of top 2 Bits
            int sec = DateTime.Now.Subtract(_lastButtonClickDateTime).Seconds;
            return thebuttonpushed && sec > Constants.SNAPSHOT_BUTTON_DURATION;
        }

        public virtual void Dispose()
        {
        }


        protected Reading FindReadingForChannel(int targetChannel)
        {
            foreach (Reading R in GrayWolfDevice.Data)
            {
                if (R.Channel == targetChannel) return R;
            }
            return null;
        }

        protected ChannelReadingPriority FindPriorityForChannel(int targetChannel)
        {
            Reading R = FindReadingForChannel(targetChannel);
            if (R == null) return ChannelReadingPriority.Normal; // can't find it, that's a NORMAL
            int sensor = (int)R.SensorCode;

            if (SensorsService.IsDerived(sensor)) return ChannelReadingPriority.Low;
            if (SensorsService.IsCO2(sensor)) return ChannelReadingPriority.Top;
            if (SensorsService.IsHumidity(sensor)) return ChannelReadingPriority.Normal;
            if (SensorsService.IsTemperature(sensor)) return ChannelReadingPriority.Normal;
            if (SensorsService.IsParticleCount(sensor)) return ChannelReadingPriority.Low;
            if (SensorsService.IsParticleMass(sensor)) return ChannelReadingPriority.Low;
            if (SensorsService.IsDifferentialPressure(sensor)) return ChannelReadingPriority.Top;
            if (SensorsService.IsPID(sensor)) return ChannelReadingPriority.High;
            if (SensorsService.IsHCHO(sensor)) return ChannelReadingPriority.Low;
            if (SensorsService.IsEC(sensor)) return ChannelReadingPriority.High;

            return ChannelReadingPriority.High;
        }

        private int _channelPriorityCounter = 0;
        private int _lastChannel_TOP = 0;
        private int _lastChannel_HIGH = 0;
        private int _lastChannel_NORMAL = 0;
        private int _lastChannel_LOW = 0;

        private int _stabilizationLoadCounter = 20;

        public int NextChannel(out bool isComplete)
        {
            int newChannel = 0;
            isComplete = false;

            if (_stabilizationLoadCounter > 0)
            {
                // for the first little while, just slowly update every channel
                _stabilizationLoadCounter--;
                newChannel = _lastChannel_LOW + 1;
                if (newChannel > Channels) newChannel = Channels;
                if (newChannel == Channels) { isComplete = true; };
                _lastChannel_LOW = newChannel;
                Debug.WriteLine("================== STABILIZING ===================== BLE Channel " + newChannel.ToString());

                return newChannel;
            }
            
            
            
            // This figures out what channel we need to switch to get the next reading - giving priority to certain readings
            
            if (++_channelPriorityCounter > 11) _channelPriorityCounter = 0;

            ChannelReadingPriority targetPriority = ChannelReadingPriority.Normal;
            switch (_channelPriorityCounter)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                    targetPriority = ChannelReadingPriority.Top; break;
                case 3:
                case 7:
                case 11:
                    targetPriority = ChannelReadingPriority.High; break;
                case 1:
                case 5:
                    targetPriority = ChannelReadingPriority.Normal; break;
                case 9:
                    targetPriority = ChannelReadingPriority.Low; break;
            }

            // Distribution 
            // 0 - TOP
            // 1 - NORMAL
            // 2 - TOP
            // 3 - HIGH
            // 4 - TOP
            // 5 - NORMAL
            // 6 - TOP
            // 7 - HIGH
            // 8 - TOP
            // 9 - LOW
            // 10 - TOP
            // 11 - HIGH 

            // TOP = 6x
            // HIGH = 3x
            // NORMAL = 2x
            // LOW = 1x


            // Now that we determined our target, find a reading with that priority            

            newChannel = 0;
            if (targetPriority == ChannelReadingPriority.Top) newChannel = _lastChannel_TOP;
            if (targetPriority == ChannelReadingPriority.High) newChannel = _lastChannel_HIGH;
            if (targetPriority == ChannelReadingPriority.Normal) newChannel = _lastChannel_NORMAL;

            if (targetPriority == ChannelReadingPriority.Low) newChannel = _lastChannel_LOW;
            if (newChannel >= Channels) newChannel = 0;

            ChannelReadingPriority foundPriority;
            do // do everything except for SLOWS
            {
                newChannel++;
                foundPriority = FindPriorityForChannel(newChannel);
            } while ((newChannel < Channels) && (foundPriority != targetPriority));


            if (newChannel > Channels) newChannel = Channels;
            if (newChannel == Channels) { isComplete = true; };

            Debug.WriteLine("=============================================== BLE Channel (" + targetPriority.ToString() + ") " + newChannel.ToString());

            if (targetPriority == ChannelReadingPriority.Top) _lastChannel_TOP=newChannel;
            if (targetPriority == ChannelReadingPriority.High) _lastChannel_HIGH = newChannel;
            if (targetPriority == ChannelReadingPriority.Normal) _lastChannel_NORMAL=newChannel;
            if (targetPriority == ChannelReadingPriority.Low) _lastChannel_LOW=newChannel;

            return newChannel;
        }

    }
}
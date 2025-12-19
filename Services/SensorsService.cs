using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using System;
using System.Collections;
using System.Collections.Generic;


namespace GrayWolf.Services
{
    public class SensorsService : ISensorsService
    {
        // SYNC_SENSORS181!!! - keep this in sync with SensorType enum in SensorType.cs

        #region Sensor Consts from CPP program
        public const int TEMP = 0;
        public const int RH = 1;
        public const int DEW = 2;
        public const int CO = 3;
        public const int CO_A4 = 189;

        // CO2 Sensors
        public const int CO2 = 4; // SGX - 
        public const int CO2A1 = 130; // AlphaSense IRC A1
        public const int CO2C2 = 131; // Clairair Ltd Cirius 2
        public const int CO2_THERMISTOR = 70; // Internal CO2 Thermistor Temp
        public const int CO2_RATIO = 181;
        public const int CO2_SGX = 174;
        public const int CO2_DYNAMIC = 175;
        public const int CO2_IR21EM = 176;
        public const int CO2_AUTORANGE = 177;
        public const int CO2_AUTO_20K = 185;
        public const int CO2_RATIO_20K = 186;
        // CO2 Sensors

        public const int HOTWIRE = 5;
        public const int PARTICULATE = 6;
        public const int VOLFLOW = 7;
        public const int COUNTS = 8;
        public const int BAT = 9;
        public const int ABSHUMIDITY = 10;
        public const int WETBULB = 11;
        public const int HUMIDITYRATIO = 12;
        public const int SO2 = 13;
        public const int NO2 = 14;
        public const int H2S = 15;
        public const int NO = 16;
        public const int SPECHUMIDITY = 17;
        public const int O2 = 18;
        public const int CL2 = 19;
        public const int H2 = 20;
        public const int HCN = 21;
        public const int NH3 = 22;
        public const int ETO = 23;
        public const int LINTEMP = 24;	//Linear temperature sensor used in TG501



        public const int CONC_CUM_0_3UM = 41;	// ARTI 0.3
        public const int CONC_CUM_0_5UM = 42;	// ARTI 0.5
        public const int CONC_CUM_0_7UM = 43;	// ARTI 0.7
        public const int CONC_CUM_1_0UM = 44;	// ARTI 1.0
        public const int CONC_CUM_2_0UM = 45;	// ARTI 2.0
        public const int CONC_CUM_2_5UM = 46;	// ARTI	2.5
        public const int CONC_CUM_5_0UM = 47;	// ARTI 5.0
        public const int CONC_CUM_10_0UM = 48;	// ARTI 10.0
        public const int CONC_CUM_25_0UM = 49;	// ARTI 25.0 
        public const int CONC_CUM_3_0UM = 103;	// ARTI 3.0
        public const int CONC_CUM_0_2UM = 104;	// ARTI 0.2



        public const int O3 = 72;
        public const int O3_A1 = 184;
        public const int HCL = 73;
        public const int HCL_SEM = 178;
        public const int HCL_MEM = 188;


        public const int PH3 = 86;  // Phosphine
        public const int PITOT_AIRSPEED = 89;
        public const int HF = 90;
        public const int HF_SEM = 179;
        public const int HF_SENSORIX = 194;

        public const int CONC_DIFF_0_3UM = 91;	// Differential Mode ARTI!!!!
        public const int CONC_DIFF_0_5UM = 92;
        public const int CONC_DIFF_0_7UM = 93;
        public const int CONC_DIFF_1_0UM = 94;
        public const int CONC_DIFF_2_0UM = 95;
        public const int CONC_DIFF_2_5UM = 96;
        public const int CONC_DIFF_5_0UM = 97;
        public const int CONC_DIFF_10_0UM = 98;
        public const int CONC_DIFF_25_0UM = 99;
        public const int CONC_DIFF_3_0UM = 102;
        public const int CONC_DIFF_0_2UM = 105;	// ARTI 0.2
        public const int TOTAL_PM = 115; //Total Particulate mass

        public const int ASH3 = 100;
        public const int SIH4 = 101;
        public const int CLO2 = 106;
        public const int CLO2_SEM = 180;
        public const int F2 = 107;
        public const int B2H6 = 108; //Diborane
        public const int COSH_H2S = 109;
        public const int NH3H = 110; //Nitrogen High
        public const int COCL2 = 111;	//Phosgene
        public const int THT = 112;
        public const int PEL = 113;

        public const int TVOC_Generic_PPM = 87;//generic sensor id for Alphasense PID-A1 and DSI Baseline 200,2000,10000
        public const int TVOC_PPM_200 = 26;//DSII Baseline 200 ppm
        public const int TVOC_PPM_2000 = 27;//DSII Baseline 2000 ppm
        public const int TVOC_PPM_10000 = 28;//DSII Baseline 10000 ppm
        public const int TVOC_Baseline_PPB = 88;//Baseline silver and yellow ppb for DSI
        public const int TVOC_Alphasense_PPB = 114;//Alphasense ppb
        public const int TVOC_PID_PPBY = 25;//DSII Baseline yellow ppb
        public const int TVOC_Generic = 29; //generic sensor id for ANY PID, ppb or ppm


        public const int PARTICLE_0_2UM = 116;
        public const int PARTICLE_0_3UM = 117;
        public const int PARTICLE_0_5UM = 118;
        public const int PARTICLE_0_7UM = 119;
        public const int PARTICLE_1_0UM = 120;
        public const int PARTICLE_2_0UM = 121;
        public const int PARTICLE_2_5UM = 122;
        public const int PARTICLE_3_0UM = 123;
        public const int PARTICLE_5_0UM = 124;
        public const int PARTICLE_10_0UM = 125;
        public const int PARTICLE_25_0UM = 126;
        public const int PARTICLE_FLOWRATE = 127;
        public const int PARTICLE_SAMPLETIME = 128;
        public const int PARTICLE_DENSITY = 129;

        public const int DIFF_PRESSURE = 135;
        public const int BAROMETRIC_PRESSURE = 136;


        public const int DIFF_PRESS_020 = 137;
        public const int DIFF_PRESS_100 = 138;
        public const int BAR_PRESS_600 = 139;
        public const int BAR_PRESS_800 = 140;
        public const int DIFF_TEMP = 141;
        public const int DIFF_TEMP_1 = 142;
        public const int DIFF_TEMP_2 = 143;
        public const int REF_TEMP = 144;
        public const int DIFF_PRESS_002_5 = 145;

        public const int LATITUDE = 151;
        public const int LONGITUDE = 152;

        public const int PM_0_2UM = 160;
        public const int PM_0_3UM = 161;
        public const int PM_0_5UM = 162;
        public const int PM_0_7UM = 163;
        public const int PM_1_0UM = 164;
        public const int PM_2_0UM = 165;
        public const int PM_2_5UM = 166;
        public const int PM_3_0UM = 167;
        public const int PM_5_0UM = 168;
        public const int PM_10_0UM = 169;
        public const int PM_25_0UM = 170;
        public const int HCHO = 171;

        public const int VOLFLOW_ACTUAL = 172;
        public const int ILLUMINANCE = 173;

        public const int HCHO_EC = 187;
        public const int HCHO_ECSENSE = 191;

        public const int B2H6_SX = 203;
        public const int ASH3_SX = 196; 
        public const int CLO2_SX = 205;
        public const int H2O2 = 192;
        public const int NO2_4E = 193;


        #endregion

        private SensorUnitLookupTable _sensors = new SensorUnitLookupTable(); // maps Sensor Name to Sensor Code ******************* ie _sensor["temperature"] is 1
        private SensorUnitLookupTable _sensors_short = new SensorUnitLookupTable(); // maps Sensor Name to Sensor Code ******************* ie _sensor["temperature"] is 1

        public const double NUMBER_NONNUMBER_CONSTANT = -666666.66;
        public const double NUMBER_NONNUMBER_CUTOFF = -666660.00; // ie, anything less than this is not a number!
        public const double NUMBER_OVERRANGE_CUTOFF = -327600.00;
        public const double NUMBER_OVERRANGE_POSITIVE_CUTOFF = 60000000.00;

        public const double FM801_SATURATED_SENSOR = 99888.0;
        public const double FM801_LOD_PPB = 10.0;
        public const double FM801_LOD_UGM3 = 12.0;
        public const double FM801_BELOW_LOD = 9.999;

        public const int PRBSEN_TVOCSTART = 1000;

        public const int PROBEID_MULTIPLER = 2500;

        public AvailableUnits AvailableUnits { get; }

        public GroupEnvironmentalValues GroupEnvironmentalValues { get; }

        public GWL_Units GWL_Units { get; }

        public IDatabase Database { get; }
        public IAnalyticsService AnalyticsService { get; }

        public SensorsService()
        {
            InitStandardList();
            GroupEnvironmentalValues = new GroupEnvironmentalValues();
            Database = Ioc.Default.GetService<IDatabase>();
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
            AvailableUnits = new AvailableUnits(this);
            GWL_Units = new GWL_Units(this);
        }

        #region implementation
        public int LookupSensorCode(string name)
        {
            try
            {
                SensorUnitLookupObject o = _sensors.FindByName(name, false);
                if (o == null) return -1;

                return o.Code;

            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return -1;

        }

        public int GetDPsForSensor(int rawsensorcode, int units)
        {
            // units -1 means we don't know or don't care



            // SYNCBLOCK DPS1551AAA1
            int senscode = NormalizeSensorCode(rawsensorcode);
            int def = 1;
            if (IsPID(senscode))
            {
                def = 0;
                if ((senscode == TVOC_PPM_10000) || (senscode == TVOC_PPM_200) || (senscode == TVOC_PPM_2000) || (senscode == TVOC_Generic_PPM)) def = 2;
            }
            if (IsCO2(rawsensorcode)) def = 0;
            if ((senscode == HUMIDITYRATIO) || (senscode == SPECHUMIDITY)) def = 4;
            if (IsBarometricPressure(rawsensorcode)) def = 0;
            if (IsParticleCount(rawsensorcode)) def = 0;
            

            if ((senscode == ASH3) ||
                (senscode == ASH3_SX) ||
                (senscode == B2H6) ||
                (senscode == B2H6_SX) ||
                (senscode == CL2) ||
                (senscode == CLO2) ||
                (senscode == CLO2_SEM) ||
                (senscode == CLO2_SX) ||
                (senscode == COCL2) ||
                (senscode == H2S) ||
                (senscode == F2) ||
                (senscode == NO2) ||
                (senscode == NO2_4E) ||
                (senscode == O3) ||
                (senscode == H2O2) ||
                (senscode == O3_A1) ||
                (senscode == PH3) ||
                (senscode == SO2)) def = 2;

            if (IsParticleMass(senscode)) def = 2;

            if (IsHCHO(senscode)) def = 0;

            if (units == (int)GrayWolf.Enums.SensorUnit.PRBUNT_PPB) def = 0; // BVW all ppbs are 0 resolution

            if (senscode == CO2_RATIO || senscode == CO2_RATIO_20K) def = 5;

            return def;
        }



        public string GetSensorUnitName(int code)
        {
            return GWL_Units.LookupUnitName(code);
        }

        public List<SensorUnit> GetSensorUnits(int code)
        {
            return AvailableUnits.GetSensorUnits(code);
        }

        public async Task SetUnitConversion(Reading reading, SensorUnit sensorUnit, string conversionId, int targetCode)
        {
            await Database.UpsertAsync(new UnitConversionDBO
            {
                Id = conversionId,
                TargetUnitCode = targetCode
            });
        }


        public void RemoveUnitConversion(int sensorId)
        {
            Database.DeleteItemAsync<UnitConversionDBO>(sensorId);
        }

        public string ConvertValue(double val, int unitCode, int targetUnitCode, int sensor)
        {
            return $"{GWL_Units.AdjustUnit(val, unitCode, targetUnitCode, sensor, GroupEnvironmentalValues)}";
        }

        public bool IsSameParameterAndUnit(int code1, int unit1, int code2, int unit2)
        {
            if (unit1 != unit2) return false;
            if (code1 == code2) return true;


            if ((IsCO2(code1)) && (IsCO2(code2))) return true;
            if ((IsTVOC(code1)) && (IsTVOC(code2))) return true;
            if ((IsHCHO(code1)) && (IsHCHO(code2))) return true;
            if ((IsHumidity(code1)) && (IsHumidity(code2))) return true;
            if ((IsTemperature(code1)) && (IsTemperature(code2))) return true;
            if ((IsOzone(code1)) && (IsOzone(code2))) return true;
            if ((IsAirVelocity(code1)) && (IsAirVelocity(code2))) return true;
            if ((IsBarometricPressure(code1)) && (IsBarometricPressure(code2))) return true;
            if ((IsDifferentialPressure(code1)) && (IsDifferentialPressure(code2))) return true;

            return false;
        }

        public void DefaultScale(int sensorcode, int unit, out double min, out double max)
        {
            min = -1;
            max = -1;

            try
            {

                int code = NormalizeSensorCode(sensorcode);

                if ((IsCO2(code)) && (unit == GWL_Units.PPM))
                {
                    min = 350.0;
                    max = 2000.0;
                }
                if ((IsTemperature(code)) && (unit == GWL_Units.F))
                {
                    min = 65.0;
                    max = 85.0;
                }
                if ((IsTemperature(code)) && (unit == GWL_Units.C))
                {
                    min = 15.0;
                    max = 30.0;
                }
                if ((IsCarbonMonoxide(code)) && (unit == GWL_Units.PPM))
                {
                    min = 0.0;
                    max = 10.0;
                }
                if ((IsHumidity(code)) && (unit == GWL_Units.RH))
                {
                    min = 15.0;
                    max = 90.0;
                }
                if ((IsHCHO(code)) && (unit == GWL_Units.PPB))
                {
                    min = 0.0;
                    max = 200.0;
                }
                if ((IsHCHO(code)) && (unit == GWL_Units.UG_M3))
                {
                    min = 0.0;
                    max = 800.0;
                }
                if ((IsOzone(code)) && (unit == GWL_Units.PPM))
                {
                    min = 0.0;
                    max = 2.0;
                }
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                min = -1;
                max = -1;
            }
        }

        public Color GetSensorColor(int sensorCode, int index)
        {
            try
            {

                List<Color> colors = new List<Color>();


                int code = NormalizeSensorCode(sensorCode);

                if (code >= 0)
                {
                    if (IsTVOC(code))
                    {
                        colors.Add(Color.FromRgb(0, 176, 80));
                        colors.Add(Color.FromRgb(0, 129, 80));
                        colors.Add(Color.FromRgb(0, 80, 80));

                    }

                    if (IsParticleMass(code))
                    {
                        colors.Add(Color.FromRgb(255, 153, 51));
                        if (code == PM_2_5UM) colors.Add(Color.FromRgb(153, 102, 0));
                        if (code == PM_10_0UM) colors.Add(Color.FromRgb(102, 51, 0));
                        if (code == TOTAL_PM) colors.Add(Color.FromRgb(204, 153, 0));
                    }

                    if (IsParticleCount(code)) colors.Add(Color.FromRgb(153, 51, 255));
                    if (IsBarometricPressure(code)) colors.Add(Color.FromRgb(102, 255, 51));
                    if (IsDifferentialPressure(code)) colors.Add(Color.FromRgb(192, 192, 192));
                    if ((IsAirVelocity(code)) || (IsAirVolumeFlow(code))) colors.Add(Color.FromRgb(118, 122, 122));

                    if (IsEC(code))
                    {
                        int r = code % 3;
                        if (index > 0)
                        {
                            switch (r)
                            {
                                case 0: colors.Add(Color.FromRgb(51, 171, 21)); break;
                                case 1: colors.Add(Color.FromRgb(106, 40, 191)); break;
                                case 2: colors.Add(Color.FromRgb(147, 212, 8)); break;
                            }
                        }
                        else
                        {
                            switch (r)
                            {
                                case 0: colors.Add(Color.FromRgb(184, 204, 228)); break;
                                case 1: colors.Add(Color.FromRgb(177, 160, 199)); break;
                                case 2: colors.Add(Color.FromRgb(196, 215, 155)); break;
                            }

                        }
                    }

                    if (IsHumidity(code))
                    {
                        colors.Add(Color.FromRgb(204, 204, 27));
                        colors.Add(Color.FromRgb(255, 255, 0));
                    }

                    if (IsTemperature(code)) colors.Add(Color.FromRgb(214, 42, 42));

                    if (IsHCHO(code)) colors.Add(Color.FromRgb(255, 204, 0));

                    if (IsCO2(code)) colors.Add(Color.FromRgb(0, 0, 204));

                    if (IsCarbonMonoxide(code))
                    {
                        colors.Add(Color.FromRgb(0, 176, 240));
                    }

                    colors.Add(Colors.BlueViolet);
                    colors.Add(Colors.CadetBlue);
                    colors.Add(Colors.Firebrick);
                    colors.Add(Colors.GreenYellow);
                    colors.Add(Colors.SpringGreen);
                    colors.Add(Colors.YellowGreen);
                    colors.Add(Colors.Chocolate);
                    colors.Add(Colors.Chartreuse);
                    colors.Add(Colors.DarkKhaki);
                    colors.Add(Colors.DodgerBlue);
                    colors.Add(Colors.Plum);
                    colors.Add(Colors.LemonChiffon);
                    colors.Add(Colors.PaleVioletRed);

                    return colors[index];
                }
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }

            return Colors.Blue;
        }

        #region internal
        public int NormalizeSensorCode(int senscode)
        {
            int ret = senscode;
            if (senscode >= PROBEID_MULTIPLER)
            {
                ret = senscode % PROBEID_MULTIPLER;
            }
            return ret;
        }


        private void InitStandardList()
        {
            _sensors.Clear();
            _sensors.Add(TEMP, "Temperature");
            _sensors.Add(RH, "Relative Humidity");
            _sensors.Add(DEW, "Dew Point");
            _sensors.Add(CO, "Carbon Monoxide");
            _sensors.Add(CO_A4, "Carbon Monoxide");
            _sensors.Add(CO2, "Carbon Dioxide");
            _sensors.Add(CO2A1, "Carbon Dioxide");
            _sensors.Add(CO2C2, "Carbon Dioxide");
            _sensors.Add(CO2_SGX, "Carbon Dioxide");
            _sensors.Add(CO2_IR21EM, "Carbon Dioxide");
            _sensors.Add(CO2_DYNAMIC, "Carbon Dioxide");
            _sensors.Add(CO2_AUTORANGE, "Carbon Dioxide");
            _sensors.Add(CO2_AUTO_20K, "Carbon Dioxide");
            _sensors.Add(HOTWIRE, "Air Speed");
            _sensors.Add(PARTICULATE, "Particulate");
            _sensors.Add(VOLFLOW, "Volume Flow");
            _sensors.Add(VOLFLOW_ACTUAL, "Vol Flow (Actual)");
            _sensors.Add(COUNTS, "Counts");
            _sensors.Add(ABSHUMIDITY, "Absolute Humidity");
            _sensors.Add(WETBULB, "Wet Bulb");
            _sensors.Add(HUMIDITYRATIO, "Humidity Ratio");
            _sensors.Add(SPECHUMIDITY, "Specific Humidity");
            _sensors.Add(SO2, "Sulfur Dioxide");
            _sensors.Add(NO2, "Nitrogen Dioxide");
            _sensors.Add(H2S, "Hydrogen Sulfide");
            _sensors.Add(NO, "Nitric Oxide");
            _sensors.Add(O2, "Oxygen");
            _sensors.Add(CL2, "Chlorine");
            _sensors.Add(H2, "Hydrogen");
            _sensors.Add(HCN, "Hydrogen Cyanide");
            _sensors.Add(NH3, "Ammonia");
            _sensors.Add(NH3H, "Ammonia");

            _sensors.Add(ETO, "Ethylene Oxide");

            _sensors.Add(O3, "Ozone");
            _sensors.Add(O3_A1, "Ozone");
            _sensors.Add(HCL, "Hydrogen Chloride");
            _sensors.Add(HCL_SEM, "Hydrogen Chloride");
            _sensors.Add(HCL_MEM, "Hydrogen Chloride");
            _sensors.Add(PH3, "Phosphine");
           
            _sensors.Add(TVOC_Generic_PPM, "TVOC");
            _sensors.Add(TVOC_PPM_200, "TVOC");
            _sensors.Add(TVOC_PPM_2000, "TVOC");
            _sensors.Add(TVOC_PPM_10000, "TVOC");
            _sensors.Add(TVOC_Baseline_PPB, "TVOC");
            _sensors.Add(TVOC_PID_PPBY, "TVOC");
            _sensors.Add(TVOC_Generic, "TVOC");
            _sensors.Add(PITOT_AIRSPEED, "Air Speed");
            _sensors.Add(HF, "Hydrogen Fluoride");
            _sensors.Add(HF_SEM, "Hydrogen Fluoride");
            _sensors.Add(HF_SENSORIX, "Hydrogen Fluoride");
            _sensors.Add(ASH3, "Arsine");
            _sensors.Add(ASH3_SX, "Arsine");
            _sensors.Add(SIH4, "Silane");
            _sensors.Add(CLO2, "Chlorine Dioxide");
            _sensors.Add(CLO2_SEM, "Chlorine Dioxide");
            _sensors.Add(CLO2_SX, "Chlorine Dioxide");
            _sensors.Add(F2, "Fluorine");
            _sensors.Add(B2H6, "Diborane");
            _sensors.Add(B2H6_SX, "Diborane");
            _sensors.Add(COSH_H2S, "H2S");
            _sensors.Add(COCL2, "Phosgene");
            _sensors.Add(THT, "THT");
            _sensors.Add(PEL, "Pellistor");
            _sensors.Add(TVOC_Alphasense_PPB, "TVOC");

            _sensors.Add(DIFF_PRESSURE, "\x394P");
            _sensors.Add(BAROMETRIC_PRESSURE, "Barometric"); // NEXTSTEP: Add to ARG Sensors!

            _sensors.Add(LATITUDE, "Latitude");
            _sensors.Add(LONGITUDE, "Longitude");


            _sensors.Add(DIFF_PRESS_002_5, "\x394P");
            _sensors.Add(DIFF_PRESS_020, "\x394P");
            _sensors.Add(DIFF_PRESS_100, "\x394P");
            _sensors.Add(BAR_PRESS_600, "Barometric");
            _sensors.Add(BAR_PRESS_800, "Barometric");
            _sensors.Add(DIFF_TEMP, "\x394T");
            _sensors.Add(DIFF_TEMP_1, "Temp1");
            _sensors.Add(DIFF_TEMP_2, "Temp2");
            _sensors.Add(REF_TEMP, "RefTemp");

            //_sensors.Add(ILLUMINANCE, "Illuminance");
            _sensors.Add(CO2_THERMISTOR, "CO2_INT_TEMP");
            _sensors.Add(CO2_RATIO, "CO2_RATIO");
            _sensors.Add(CO2_RATIO_20K, "CO2_RATIO_20K");

            _sensors.Add(HCHO, "Formaldehyde");
            _sensors.Add(HCHO_EC, "Formaldehyde");
            _sensors.Add(HCHO_ECSENSE, "Formaldehyde");

            _sensors.Add(H2O2, "Hydrogen Peroxide");
            _sensors.Add(NO2_4E, "Nitrogen Dioxide");

            _sensors.Add(PM_0_2UM, "PM 0.2");
            _sensors.Add(PM_0_3UM, "PM 0.3");
            _sensors.Add(PM_0_5UM, "PM 0.5");
            _sensors.Add(PM_0_7UM, "PM 0.7");
            _sensors.Add(PM_1_0UM, "PM 1.0");
            _sensors.Add(PM_2_0UM, "PM 2.0");
            _sensors.Add(PM_2_5UM, "PM 2.5");
            _sensors.Add(PM_3_0UM, "PM 3.0");
            _sensors.Add(PM_5_0UM, "PM 5.0");
            _sensors.Add(PM_10_0UM, "PM 10.0");
            _sensors.Add(PM_25_0UM, "PM 25.0");
            _sensors.Add(TOTAL_PM, "TPM");

            ////////////////////////////////////////////// SHORT NAMES
            ///

            _sensors_short.Clear();
            _sensors_short.Add(TEMP, "Temp");
            _sensors_short.Add(RH, "RH");
            _sensors_short.Add(DEW, "Dew");
            _sensors_short.Add(CO, "CO");
            _sensors_short.Add(CO_A4, "CO");
            _sensors_short.Add(CO2, "CO2");
            _sensors_short.Add(CO2A1, "CO2");
            _sensors_short.Add(CO2C2, "CO2");
            _sensors_short.Add(CO2_SGX, "CO2");
            _sensors_short.Add(CO2_IR21EM, "CO2");
            _sensors_short.Add(CO2_DYNAMIC, "CO2");
            _sensors_short.Add(CO2_AUTORANGE, "CO2");
            _sensors_short.Add(CO2_AUTO_20K, "CO2");

            _sensors_short.Add(HOTWIRE, "Air Speed");
            _sensors_short.Add(PARTICULATE, "Particulate");
            _sensors_short.Add(VOLFLOW, "Vol Flow");
            _sensors_short.Add(VOLFLOW_ACTUAL, "Vol Flow");
            _sensors_short.Add(COUNTS, "Counts");
            _sensors_short.Add(ABSHUMIDITY, "AbsHum");
            _sensors_short.Add(WETBULB, "WetBulb");
            _sensors_short.Add(HUMIDITYRATIO, "Hum Ratio");
            _sensors_short.Add(SPECHUMIDITY, "Spec Hum");
            _sensors_short.Add(SO2, "SO2");
            _sensors_short.Add(NO2, "NO2");
            _sensors_short.Add(NO2_4E, "NO2");
            _sensors_short.Add(H2S, "H2S");
            _sensors_short.Add(NO, "NO");
            _sensors_short.Add(O2, "O2");
            _sensors_short.Add(CL2, "Cl2");
            _sensors_short.Add(H2, "H2");
            _sensors_short.Add(HCN, "HCN");
            _sensors_short.Add(NH3, "NH3");
            _sensors_short.Add(NH3H, "NH3");
            _sensors_short.Add(H2O2, "H2O2");


            _sensors_short.Add(ETO, "EtO");

            _sensors_short.Add(O3, "O3");
            _sensors_short.Add(O3_A1, "O3");
            _sensors_short.Add(HCL, "HCl");
            _sensors_short.Add(HCL_SEM, "HCl");
            _sensors_short.Add(HCL_MEM, "HCl");
            _sensors_short.Add(PH3, "PH3");
            _sensors_short.Add(LATITUDE, "Lat");
            _sensors_short.Add(LONGITUDE, "Long");
            _sensors_short.Add(HCHO, "HCHO");
            _sensors_short.Add(HCHO_EC, "HCHO");
            _sensors_short.Add(HCHO_ECSENSE, "HCHO");

            _sensors_short.Add(TVOC_Generic_PPM, "TVOC");
            _sensors_short.Add(TVOC_PPM_200, "TVOC");
            _sensors_short.Add(TVOC_PPM_2000, "TVOC");
            _sensors_short.Add(TVOC_PPM_10000, "TVOC");
            _sensors_short.Add(TVOC_Baseline_PPB, "TVOC");
            _sensors_short.Add(TVOC_PID_PPBY, "TVOC");
            _sensors_short.Add(TVOC_Generic, "TVOC");

            _sensors_short.Add(PITOT_AIRSPEED, "Air Speed");
            _sensors_short.Add(HF, "HF");
            _sensors_short.Add(HF_SEM, "HF");
            _sensors_short.Add(HF_SENSORIX, "HF");

            _sensors_short.Add(ASH3, "AsH3");
            _sensors_short.Add(ASH3_SX, "AsH3");

            _sensors_short.Add(SIH4, "SiH4");
            _sensors_short.Add(CLO2, "ClO2");
            _sensors_short.Add(CLO2_SEM, "ClO2");
            _sensors_short.Add(CLO2_SX, "ClO2");

            _sensors_short.Add(F2, "F2");
            _sensors_short.Add(B2H6, "B2H6");
            _sensors_short.Add(B2H6_SX, "B2H6");

            _sensors_short.Add(COSH_H2S, "H2S");
            _sensors_short.Add(COCL2, "COCl2");
            _sensors_short.Add(THT, "THT");
            _sensors_short.Add(PEL, "Pell");
            _sensors_short.Add(TVOC_Alphasense_PPB, "TVOC");

            _sensors_short.Add(DIFF_PRESSURE, "\x394P");
            _sensors_short.Add(BAROMETRIC_PRESSURE, "Baro"); // NEXTSTEP: Add to ARG Sensors!

            _sensors_short.Add(DIFF_PRESS_002_5, "\x394P");
            _sensors_short.Add(DIFF_PRESS_020, "\x394P");
            _sensors_short.Add(DIFF_PRESS_100, "\x394P");
            _sensors_short.Add(BAR_PRESS_600, "Baro");
            _sensors_short.Add(BAR_PRESS_800, "Baro");
            _sensors_short.Add(DIFF_TEMP, "\x394T");
            _sensors_short.Add(DIFF_TEMP_1, "Temp1");
            _sensors_short.Add(DIFF_TEMP_2, "Temp2");
            _sensors_short.Add(REF_TEMP, "RefTemp");


            _sensors_short.Add(PM_0_2UM, "PM 0.2");
            _sensors_short.Add(PM_0_3UM, "PM 0.3");
            _sensors_short.Add(PM_0_5UM, "PM 0.5");
            _sensors_short.Add(PM_0_7UM, "PM 0.7");
            _sensors_short.Add(PM_1_0UM, "PM 1.0");
            _sensors_short.Add(PM_2_0UM, "PM 2.0");
            _sensors_short.Add(PM_2_5UM, "PM 2.5");
            _sensors_short.Add(PM_3_0UM, "PM 3.0");
            _sensors_short.Add(PM_5_0UM, "PM 5.0");
            _sensors_short.Add(PM_10_0UM, "PM 10.0");
            _sensors_short.Add(PM_25_0UM, "PM 25.0");
            _sensors_short.Add(TOTAL_PM, "TPM");
        }


        public string[] SensorNames
        {
            get
            {
                return _sensors.Names;
            }
        }


        public int[] SensorCodes
        {
            get
            {
                return _sensors.Codes;
            }
        }


        public string GetSensorName(int code)
        {
            try
            {
                int c = NormalizeSensorCode(code);
                SensorUnitLookupObject o = _sensors.FindByCode(code);
                if (o != null) return o.Name;
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return "";
        }


        public string LookupShortSensorName(int code)
        {
            try
            {
                int c = NormalizeSensorCode(code);
                SensorUnitLookupObject o = _sensors_short.FindByCode(code);
                if (o != null) return o.Name;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return "";
        }

        public int LookupSensorCodeNoCase(string name)
        {
            try
            {
                SensorUnitLookupObject o = _sensors.FindByName(name, true);
                if (o == null) return -1;

                return o.Code;

            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return -1;

        }

        public int BruteForceLookupSensorCodeFromName(string name)
        {
            // need a min of 3 letters to match but keep stripping 
            try
            {
                int max = name.Length;
                string searchpart = name;
                do
                {
                    SensorUnitLookupObject o = _sensors.FindByNameTruncated(searchpart);
                    if (o != null) return o.Code;
                    max--;
                    searchpart = name.Substring(0, max);
                }
                while (max > 3);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return -1;

        }


        public bool IsTVOC(int code)
        {
            int c = NormalizeSensorCode(code);
            if (c >= PRBSEN_TVOCSTART) return true;
            return false;
        }

        public bool IsTVOCSensor(int code)
        {
            int t = NormalizeSensorCode(code);
            bool isTVOC = false;
            switch (t)
            {
                case TVOC_Generic:
                case TVOC_Generic_PPM:
                case TVOC_PPM_200:
                case TVOC_PPM_2000:
                case TVOC_PPM_10000:
                case TVOC_Baseline_PPB:
                case TVOC_Alphasense_PPB:
                case TVOC_PID_PPBY:
                    isTVOC = true;
                    break;
            }
            return isTVOC;
        }

        public bool IsCO2(int code)
        {
            int c = NormalizeSensorCode(code);
            if ((c == CO2A1) || (c == CO2) || (c == CO2C2) || (c == CO2_SGX) || (c == CO2_IR21EM) || (c == CO2_DYNAMIC) || (c == CO2_AUTORANGE) || (c == CO2_AUTO_20K)) return true;
            return false;
        }

        public bool IsOzone(int code)
        {
            int c = NormalizeSensorCode(code);
            if ((c == O3) || (c == O3_A1)) return true;
            return false;
        }

        public bool CO2HasThermistor(int code)
        {
            int c = NormalizeSensorCode(code);
            if ((c == CO2A1) || (c == CO2_DYNAMIC) || (c == CO2_AUTORANGE) || (c == CO2_AUTO_20K)) return true;
            return false;
        }


        public bool IsPID(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == TVOC_Generic_PPM) || (c == TVOC_Baseline_PPB) || (c == TVOC_PID_PPBY) || (c == TVOC_Alphasense_PPB)
                || (c == TVOC_PPM_200) || (c == TVOC_PPM_2000) || (c == TVOC_PPM_10000)) return true;
            return false;
        }

        // BVW added for convienience... keep in sync with above 
        public static bool IsPID_static(int c)
        {
            if ((c == TVOC_Generic_PPM) || (c == TVOC_Baseline_PPB) || (c == TVOC_PID_PPBY) || (c == TVOC_Alphasense_PPB)
                || (c == TVOC_PPM_200) || (c == TVOC_PPM_2000) || (c == TVOC_PPM_10000)) return true;
            return false;
        }

        public bool IsDifferentialPressure(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == DIFF_PRESSURE) || (c == DIFF_PRESS_020) || (c == DIFF_PRESS_100) || (c == DIFF_PRESS_002_5)) return true;

            return false;
        }

        public bool IsHumidity(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == RH) || (c == ABSHUMIDITY) || (c == SPECHUMIDITY) || (c == DEW) || (c == WETBULB) || (c == HUMIDITYRATIO)) return true;

            return false;
        }

        public bool IsTemperature(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == TEMP) || (c == DIFF_TEMP) || (c == DIFF_TEMP_1) || (c == DIFF_TEMP_2) || (c == LINTEMP)) return true;

            return false;
        }


        public bool IsAirVolumeFlow(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == VOLFLOW) || (c == VOLFLOW_ACTUAL)) return true;

            return false;
        }

        public bool IsAirVelocity(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == PITOT_AIRSPEED) || (c == HOTWIRE)) return true;

            return false;
        }

        public  bool IsDerived(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == ABSHUMIDITY) || (c == SPECHUMIDITY) || (c == DEW) || (c == WETBULB) || (c == HUMIDITYRATIO) ||
                (c == PITOT_AIRSPEED) || (c == VOLFLOW) || (c == VOLFLOW_ACTUAL)) return true;

            return false;
        }



        public bool IsDifferentialTemperature(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == DIFF_TEMP) || (c == DIFF_TEMP_1) || (c == DIFF_TEMP_2)) return true;

            return false;
        }


        public bool IsBarometricPressure(int code)
        {
            int c = NormalizeSensorCode(code);

            if ((c == BAROMETRIC_PRESSURE) || (c == BAR_PRESS_600) || (c == BAR_PRESS_800)) return true;

            return false;
        }
        public bool IsReferenceTemperature(int code)
        {
            int c = NormalizeSensorCode(code);

            if (c == REF_TEMP) return true;

            return false;
        }




        public bool IsGas(int code)
        {
            // if its a gas as opposed to RH, Temp, Pressure, Particle, Barometric

            if (IsEC(code)) return true;
            if (IsPID(code)) return true;

            int c = NormalizeSensorCode(code);
            if (IsCO2(code)) return true;

            return false;
        }

        public bool IsEC(int code)
        {
            int c = NormalizeSensorCode(code);
            bool ec = false;
            switch (c)
            {
                case CO:
                case SO2:
                case NO2:
                case NO2_4E:
                case H2S:
                case NO:
                case O2:
                case CL2:
                case H2:
                case HCN:
                case NH3:
                case ETO:
                case O3:
                case O3_A1:
                case HCL:
                case HCL_SEM:
                case PH3:
                case HF:
                case HF_SEM:
                case HF_SENSORIX:
                case ASH3:
                case ASH3_SX:
                case SIH4:
                case CLO2:
                case CLO2_SEM:
                case CLO2_SX:
                case F2:
                case B2H6:
                case B2H6_SX:
                case COSH_H2S:
                case NH3H:
                case COCL2:
                case THT:
                case PEL:
                case H2O2:
                case HCHO_EC:
                case HCHO_ECSENSE:
                    ec = true;
                    break;

            }
            return ec;
        }

        public bool IsECppb(int code)
        {
            int c = NormalizeSensorCode(code);
            bool ec = false;
            switch (c)
            {
                case NO2:
                case NO2_4E:
                case H2S:
                case COCL2:
                case ASH3:
                case ASH3_SX:
                case CLO2:
                case CLO2_SEM:
                case CLO2_SX:
                case CL2:
                case B2H6:
                case B2H6_SX:
                case F2:
                case HCHO_EC:
                case HCHO_ECSENSE:
                case O3:
                case O3_A1:
                case HCN:
                    ec = true;
                    break;
            }
            return ec;
        }


        public bool IsHCHO(int code)
        {
            int c = NormalizeSensorCode(code);
            if (c == HCHO_EC) return true;
            if (c == HCHO) return true;
            if (c == HCHO_ECSENSE) return true;
            return false;
        }

        // BVW added for convienience... keep in sync with above 
        public static bool IsHCHO_static(int code)
        {
            if (code == HCHO_EC) return true;
            if (code == HCHO) return true;
            if (code == HCHO_ECSENSE) return true;
            return false;
        }

        public bool IsCarbonMonoxide(int code)
        {
            int c = NormalizeSensorCode(code);
            if (c == CO) return true;
            if (c == CO_A4) return true;
            return false;
        }

        public bool IsParticleCount(int code)
        {
            int c = NormalizeSensorCode(code);
            switch (c)
            {
                case CONC_CUM_0_3UM:
                case CONC_CUM_0_5UM:
                case CONC_CUM_0_7UM:
                case CONC_CUM_1_0UM:
                case CONC_CUM_2_0UM:
                case CONC_CUM_2_5UM:
                case CONC_CUM_5_0UM:
                case CONC_CUM_10_0UM:
                case CONC_CUM_25_0UM:
                case CONC_DIFF_0_3UM:
                case CONC_DIFF_0_5UM:
                case CONC_DIFF_0_7UM:
                case CONC_DIFF_1_0UM:
                case CONC_DIFF_2_0UM:
                case CONC_DIFF_2_5UM:
                case CONC_DIFF_5_0UM:
                case CONC_DIFF_10_0UM:
                case CONC_DIFF_25_0UM:
                case PARTICLE_0_2UM:
                case PARTICLE_0_3UM:
                case PARTICLE_0_5UM:
                case PARTICLE_0_7UM:
                case PARTICLE_1_0UM:
                case PARTICLE_2_0UM:
                case PARTICLE_2_5UM:
                case PARTICLE_3_0UM:
                case PARTICLE_5_0UM:
                case PARTICLE_10_0UM:
                case PARTICLE_25_0UM:
                    return true;
            }

            return false;
        }


        public bool IsParticleMass(int code)
        {
            int c = NormalizeSensorCode(code);
            switch (c)
            {
                case PM_0_2UM:
                case PM_0_3UM:
                case PM_0_5UM:
                case PM_0_7UM:
                case PM_1_0UM:
                case PM_2_0UM:
                case PM_2_5UM:
                case PM_3_0UM:
                case PM_5_0UM:
                case PM_10_0UM:
                case PM_25_0UM:
                case TOTAL_PM:
                    return true;
            }

            return false;
        }

        public string FormatValBasedOnSensor(double d, int rawsensorcode, int unit, int extradps)
        {
            if (d <= NUMBER_NONNUMBER_CUTOFF) return "--";
            int dp = GetDPsForSensor(rawsensorcode, unit)+extradps;
            string def = "N" + dp.ToString();
            return d.ToString(def);
        }


        public string FormatValBasedOnSensor(double d, int rawsensorcode, int extradps)
        {
            if (d <= NUMBER_NONNUMBER_CUTOFF) return "--";
            int dp = GetDPsForSensor(rawsensorcode,-1) + extradps;
            string def = "N" + dp.ToString();
            return d.ToString(def);
        }

        public string FormatValBasedOnSensor(double d, int rawsensorcode, bool allowcomma)
        {
            if (d <= NUMBER_NONNUMBER_CUTOFF) return "--";
            int dp = GetDPsForSensor(rawsensorcode,-1);
            string def = "F" + dp.ToString();
            if (allowcomma) def = "N" + dp.ToString();
            return d.ToString(def);
        }
        #endregion
    }

    #region utils
    public class GWL_Units
    {
        #region Sensor Consts from CPP program
        public const int C = 0;
        public const int F = 1;
        public const int K = 2;
        public const int RH = 3;
        public const int PPM = 4;
        public const int M_S = 5;
        public const int FT_M = 6;
        public const int MG_M3 = 7;
        public const int CFM = 8;
        public const int M3_H = 9;
        public const int COUNTS = 10;
        public const int VOLTS = 11;
        public const int L_S = 12;
        public const int PPMW = 13;
        public const int G_M3 = 14;
        public const int LB_FT3 = 15;
        public const int GR_FT3 = 16;
        public const int PPERCENT = 17;

        public const int LB_LB = 18;
        public const int GR_LB = 19;
        public const int KG_KG = 20;
        public const int G_KG = 21;
        public const int CPM = 22;
        public const int UG_M3 = 23;
        public const int LPM = 24;
        public const int INCHES = 25;
        public const int BLANK = 26;
        public const int CNTS_CF = 27;
        public const int CNTS_L = 28;

        public const int MMHG = 45;
        public const int PSI = 46;
        public const int FT_S = 47;
        public const int PPB = 51;
        public const int CM = 52; // cm
        public const int MM = 53; // mm
        public const int BAR = 54;
        public const int MBAR = 55;


        public const int INHG = 58;

        public const int PASCALS = 59;

        public const int TORR = 60;
        public const int KPA = 61;
        public const int M3_S = 62;

        public const int IN_H2O = 63;
        public const int PERCENT_LEL = 67;
        public const int PERCENT_MET = 68;

        public const int PM_M3 = 69; //Lighthouse Particle Mass
        public const int PM_FT3 = 70; //
        public const int PM_UG_M3 = 71; // LEGACY!!!! Don't USE
        public const int PM_UG_FT3 = 72;
        public const int TPM_M3 = 73;
        public const int TPM_FT3 = 74;

        public const int C_CF_DELTA = 76;
        public const int C_CF_EPSILON = 77;
        public const int C_L_DELTA = 78;
        public const int C_L_EPSILON = 79;
        public const int CNTS_DELTA = 80;
        public const int CNTS_EPSILON = 81;

        public const int PMBAR = 82; // OBSOLETE !!!! Use 55 instead
        public const int ATM = 83;
        public const int HPA = 84;
        public const int ACFM = 85;

        public const int CNTS_M3 = 86; //added meters cube to lighthouse transfer
        public const int LUX = 87;
        public const int FOOTCANDLE = 88;
        public const int PCI_L = 89; // PIOCURIES_LITRE
        public const int BQ_M3 = 90; // BECQUERELS_CM 91
        #endregion

        private ISensorsService Sensors { get; }

        private Hashtable _units = new Hashtable();

        public GWL_Units(ISensorsService sensorsService)
        {
            Sensors = sensorsService;

            _units.Clear();
            _units.Add(C, "°C");
            _units.Add(F, "°F");
            _units.Add(K, "K");
            _units.Add(RH, "%RH");
            _units.Add(PPM, "ppm");
            _units.Add(M_S, "m/s");
            _units.Add(FT_M, "ft/m");
            _units.Add(MG_M3, "mg/m3");
            _units.Add(CFM, "CFM");
            _units.Add(M3_H, "m3/h");
            _units.Add(COUNTS, "counts");
            _units.Add(VOLTS, "Volts");
            _units.Add(L_S, "l/s");
            _units.Add(PPMW, "ppmw");
            _units.Add(G_M3, "g/m^3");
            _units.Add(LB_FT3, "lb/cu ft");
            _units.Add(GR_FT3, "grains/cu ft");
            _units.Add(17, "%");
            _units.Add(LB_LB, "lb/lb");
            _units.Add(GR_LB, "grains/lb");
            _units.Add(KG_KG, "kg/kg");
            _units.Add(G_KG, "g/kg");
            _units.Add(CPM, "cpm");
            _units.Add(UG_M3, "µg/m3");
            _units.Add(LPM, "LPM");
            _units.Add(INCHES, "inches");
            _units.Add(BLANK, " ");
            _units.Add(CNTS_CF, " cnts/CF");
            _units.Add(CNTS_L, " cnts/L");

            _units.Add(MMHG, "mmHg");
            _units.Add(PSI, "psi");
            _units.Add(FT_S, "ft/s");

            _units.Add(PPB, "ppb");
            _units.Add(CM, "cm");
            _units.Add(MM, "mm");

            _units.Add(BAR, "bar");
            _units.Add(MBAR, "mbar");
            _units.Add(PMBAR, "mbar");// Obsolete, kept in for legacy display

            _units.Add(PASCALS, "pa");
            _units.Add(IN_H2O, "inH20");
            _units.Add(ATM, "atm");
            _units.Add(INHG, "inHg");
            _units.Add(TORR, "Torr");
            _units.Add(KPA, "kPa");
            _units.Add(M3_S, "m3/s");



            _units.Add(PERCENT_LEL, "% LEL");
            _units.Add(PERCENT_MET, "% Methane");

            _units.Add(PM_UG_M3, " µg/m3");
            _units.Add(PM_UG_FT3, " µg/ft3");
            _units.Add(PM_M3, " PM m3");
            _units.Add(PM_FT3, " PM ft3");
            _units.Add(TPM_M3, " TPM m3");
            _units.Add(TPM_FT3, " TPM ft3");

            _units.Add(C_CF_DELTA, "c/cf \x394");
            _units.Add(C_CF_EPSILON, "c/cf \x3A3");
            _units.Add(C_L_DELTA, "c/l \x394");
            _units.Add(C_L_EPSILON, "c/l \x3A3");
            _units.Add(CNTS_DELTA, "cnts \x394");
            _units.Add(CNTS_EPSILON, "cnts \x3A3");

            _units.Add(HPA, "HPa");
            _units.Add(ACFM, "ACFM");

            _units.Add(CNTS_M3, " cnts/M3");
            _units.Add(LUX, "lux");
            _units.Add(FOOTCANDLE, "ft-candle");
            _units.Add(PCI_L, "pCi/L");
            _units.Add(BQ_M3, "Bq/m3");
        }

        public string[] UnitNames
        {
            get
            {
                string[] a = new string[_units.Keys.Count];
                _units.Values.CopyTo(a, 0);
                return a;
            }
        }

        public string LookupUnitName(int code)
        {
            try
            {
                object o = _units[code];
                if (o != null)
                    return o.ToString();
            }
            catch
            {
            }
            return "";
        }


        public int LookupUnitCode(string name)
        {
            try
            {
                IDictionaryEnumerator en = _units.GetEnumerator();
                while (en.MoveNext())
                {
                    if (name.ToString().ToUpper().CompareTo(en.Value.ToString().ToUpper()) == 0)
                    {
                        return (int)en.Key;
                    }
                }

            }
            catch
            {
            }
            return -1;

        }

        public static string FlattenUnits(string name)
        {
            try
            {
                string n = name;
                n = n.Replace("µg", "ug");
                n = n.Replace("°C", "C");
                n = n.Replace("°F", "F");
                n = n.Replace("\x394", "delta");
                n = n.Replace("\x3A3", "sigma");
                return n;
            }
            catch
            {

            }
            return "";
        }


        public double ConvertToMGM3(int sensor, double TLVppm, double tempC, double pressMBAR)
        {
            if (sensor== SensorsService.HCHO) 
            {
                // This is a specific test for a FM801 and is not needed in WolfSense Mobile
                double test = SensorsService.FM801_SATURATED_SENSOR / 1000.0;
                if (test == TLVppm) return SensorsService.FM801_SATURATED_SENSOR;
            }

            double output = 0.00;

            double P = pressMBAR * (0.750062); //  in mm Hg
            if (P < 0)
            {
                P = 760.0;// 
            }

            double MW = -1.00; // grams

            if (sensor >= SensorsService.PROBEID_MULTIPLER)
            {
                sensor = sensor % SensorsService.PROBEID_MULTIPLER; // BUG FIX DEFECT #75 & #72
            }

            if (Sensors.IsPID(sensor))
            {
                MW = 56.108;
            }
            else if (Sensors.IsCO2(sensor))
            {
                MW = 44.009;
            }
            else
            {
                switch (sensor)
                {
                    case SensorsService.H2O2:
                        MW = 34.0147;
                        break;
                    case SensorsService.SO2:
                        MW = 64.062;
                        break;
                    case SensorsService.NO2:
                    case SensorsService.NO2_4E:
                        MW = 46.005;
                        break;
                    case SensorsService.H2S:
                    case SensorsService.COSH_H2S:
                        MW = 34.08;
                        break;
                    case SensorsService.NO:
                        MW = 30.0;
                        break;
                    case SensorsService.CL2:
                        MW = 70.905;
                        break;
                    case SensorsService.H2:
                        MW = 2.016;
                        break;
                    case SensorsService.HCN:
                        MW = 27.023;
                        break;
                    case SensorsService.NH3:
                    case SensorsService.NH3H:
                        MW = 17.03;
                        break;
                    case SensorsService.ETO:
                        MW = 44.05; ;
                        break;
                    case SensorsService.O3:
                        MW = 47.998;
                        break;
                    case SensorsService.HCL:
                    case SensorsService.HCL_SEM:
                    case SensorsService.HCL_MEM:
                        MW = 36.46;
                        break;
                    case SensorsService.CO:
                    case SensorsService.CO_A4:
                        MW = 28.01;
                        break;
                    case SensorsService.O2:
                        MW = 31.999;
                        break;
                    case SensorsService.HF:
                        MW = 20.006;
                        break;
                    case SensorsService.PH3:
                        MW = 33.997;
                        break;
                    case SensorsService.ASH3:
                    case SensorsService.ASH3_SX:
                        MW = 77.945;
                        break;
                    case SensorsService.SIH4:
                        MW = 32.117;
                        break;
                    case SensorsService.HCHO:
                    case SensorsService.HCHO_ECSENSE:
                    case SensorsService.HCHO_EC:
                        MW = 30.03;
                        break;
                }
            }

            if (MW < 0)
            {
                return 0.00;
            }

            if (tempC + 273.2 == 0.00) return 0.00;

            output = (P * MW * TLVppm) / (62.4 * (273.2 + tempC));

            return output;
        }

        public double AdjustUnit(double value, int baseunit, int destination, int sensor, GroupEnvironmentalValues enviro)
        {
            // Adjust Units, takes the calibrated input value, 
            // which is given in base units
            // and converts it to the destination units
            // If a conversion is not possible, return the input value
            //
            double output = value; // failsafe, no conversion

            if (baseunit != destination)
            { // units differ so we need to convert


                double currenttemperature = 22;
                double currentpress = 1013.0;

                if (((baseunit == GWL_Units.PPM) || (baseunit == GWL_Units.PPB)) &&
                    ((destination == GWL_Units.MG_M3) || (destination == GWL_Units.UG_M3)))
                {
                    // this is just trying to be efficient by only grabbing the temp/press when we may use it
                    if (enviro != null)
                    {
                        bool valid;
                        currenttemperature = enviro.GetValue(SensorsService.TEMP, GWL_Units.C, 22.5, out valid);
                        currentpress = enviro.GetValue(SensorsService.BAROMETRIC_PRESSURE, GWL_Units.MBAR, 1013.0, out valid);
                    }
                }

                //TRACE1("CONVERT: %7.2f ",value);
                //TRACE2("B: %i -- D: %i == ",base,destination); 

                switch (baseunit)
                {
                    case GWL_Units.C:
                        if (destination == GWL_Units.K) output = value + 273.15;
                        if (destination == GWL_Units.F) output = value * 1.8 + 32;
                        if (sensor == SensorsService.DIFF_PRESSURE)
                        {
                            if (destination == GWL_Units.K) output = value;
                            if (destination == GWL_Units.F) output = value * 1.8;
                        }
                        break;

                    case GWL_Units.F:
                        if (destination == GWL_Units.C) output = (5.0 / 9.0) * (value - 32.0);
                        break;

                    case GWL_Units.M_S:
                        if (destination == GWL_Units.FT_M) output = (value * 3.28084) * 60;
                        if (destination == GWL_Units.FT_S) output = (value * 3.28084);
                        break;
                    case GWL_Units.G_M3:
                        if (destination == GWL_Units.LB_FT3) output = (value / 16018.46);
                        if (destination == GWL_Units.GR_FT3) output = (value / 2.28835);
                        break;


                    case GWL_Units.MG_M3:
                        if (destination == GWL_Units.UG_M3) output = value * 1000.0; // REVERSED: BUG FIX 146
                        break;

                    case GWL_Units.UG_M3:
                        if (destination == GWL_Units.MG_M3) output = value / 1000.0;
                        break;


                    case GWL_Units.PPM:
                        if (destination == GWL_Units.PPERCENT) output = (output / 10000.0);
                        if (destination == GWL_Units.MG_M3) output = ConvertToMGM3(sensor, value, currenttemperature, currentpress);
                        if (destination == GWL_Units.UG_M3) output = ConvertToMGM3(sensor, value, currenttemperature, currentpress) * 1000.0;
                        if (destination == GWL_Units.PERCENT_LEL) output = ((output * 2) / 1000.0);
                        if (destination == GWL_Units.PERCENT_MET) output = (output / 10000.0);
                        if (destination == GWL_Units.PPB)
                        {
                            output = System.Math.Floor(output * 100.00) * 10.00; // Floor it to get resolution correct.
                                                                                 // going from PPM to PPB we don't really have trillionth resolution
                        }
                        break;
                    case GWL_Units.PPB:
                        if (destination == GWL_Units.MG_M3) output = ConvertToMGM3(sensor, (output / 1000.0/*conv to ppm*/), currenttemperature, currentpress);
                        if (destination == GWL_Units.UG_M3) output = ConvertToMGM3(sensor, (output / 1000.0/*conv to ppm*/), currenttemperature, currentpress) * 1000.0;
                        if (destination == GWL_Units.PPM) output = (output / 1000.0);




                        break;
                    case GWL_Units.PPMW:
                        if (destination == GWL_Units.LB_LB) output = (output * 0.000001);
                        if (destination == GWL_Units.GR_FT3) output = (output * 0.007);
                        if (destination == GWL_Units.KG_KG) output = (output * 0.000001);
                        if (destination == GWL_Units.G_KG) output = (output * 0.001);
                        break;
                    case GWL_Units.PASCALS:
                        // 1 Pascal = 0.010 mbar			
                        if (destination == GWL_Units.MBAR) output = output * 0.010;
                        // 1 Pascal = 7.501x10-3 mmHg
                        if (destination == GWL_Units.MMHG) output = output * 0.07501;
                        // 1 Pascal = 0.004 inH20
                        if (destination == GWL_Units.IN_H2O) output = output * 0.004;
                        break;

                    case GWL_Units.MBAR:
                        if (destination == GWL_Units.PASCALS) output = output * 100.0;
                        if ((destination == GWL_Units.MMHG) || (destination == GWL_Units.TORR)) output = output * 0.750061683;
                        if (destination == GWL_Units.IN_H2O) output = output * 0.4018;
                        if (destination == GWL_Units.INHG) output = output * 0.02961; // assumes 60 F
                        if (destination == GWL_Units.PSI) output = output * 0.0145038;
                        if (destination == GWL_Units.KPA) output = output * 0.1;
                        if (destination == GWL_Units.ATM) output = output * 0.000987;

                        if (destination == GWL_Units.HPA) output = output * 1.0; // same


                        break;

                    case GWL_Units.MMHG:
                        if (destination == GWL_Units.MBAR) output = output * 1.333;
                        if (destination == GWL_Units.PASCALS) output = output * 133.322;
                        if (destination == GWL_Units.IN_H2O) output = output * 0.535240;
                        if (destination == GWL_Units.ATM) output = output * 0.0013157895;
                        if (destination == GWL_Units.PSI) output = output * 0.019337;
                        break;

                    case GWL_Units.LUX:
                        if (destination == GWL_Units.FOOTCANDLE) output = output * 10.764;
                        break;


                }; // switch statement
            }; // end conversion is necessary

            //TRACE1("OUT: %7.2f \n",out);


            return output;
        }



    }

    public class SensorUnitListSort : IComparer
    {
        public SensorUnitListSort()
        {
        }

        int IComparer.Compare(object x, object y)
        {
            // < 0 means x < y
            // =0  means x = 0
            // >0  means x > y

            SensorUnitLookupObject xo = (SensorUnitLookupObject)x;
            SensorUnitLookupObject yo = (SensorUnitLookupObject)y;

            if (xo.Code < yo.Code) return -1;
            if (xo.Code > yo.Code) return 1;

            return 0;
        }
    }

    public class SensorUnitLookupTable
    {
        private ArrayList _table = new ArrayList();

        private bool _tablesorted = false;

        protected void Sort()
        {
            _table.Sort(new SensorUnitListSort());
            _tablesorted = true;
        }

        public SensorUnitLookupTable()
        {
        }

        public void Clear()
        {
            _table.Clear();
            _tablesorted = false;
        }

        public void Add(SensorUnitLookupObject o)
        {
            _tablesorted = false;
            _table.Add(o);
        }

        public string[] Names
        {
            get
            {
                string[] n = new string[this.Count];
                for (int i = 0; i < _table.Count; i++)
                {
                    n[i] = SafeGetAt(i).Name;
                }
                return n;
            }
        }

        public int[] Codes
        {
            get
            {
                int[] n = new int[this.Count];
                for (int i = 0; i < _table.Count; i++)
                {
                    n[i] = SafeGetAt(i).Code;
                }
                return n;
            }
        }

        public void Add(int code, string name)
        {
            Add(new SensorUnitLookupObject(name, code));
        }

        public void AddAsStrings(string codestring, string name)
        {
            int code;
            try
            {
                code = int.Parse(codestring);

            }
            catch
            {
                code = -1;
            }

            Add(new SensorUnitLookupObject(name, code));
        }


        public int Count
        {
            get { return _table.Count; }
        }

        public SensorUnitLookupObject GetAt(int i)
        {
            if ((i >= 0) && (i < _table.Count))
            {
                return (SensorUnitLookupObject)_table[i];
            }
            return null;
        }


        protected SensorUnitLookupObject SafeGetAt(int i)
        {
            // safe version returns a blank object instead of null
            if ((i >= 0) && (i < _table.Count))
            {
                return (SensorUnitLookupObject)_table[i];
            }

            SensorUnitLookupObject o = new SensorUnitLookupObject("", -1);
            return o;
        }

        public SensorUnitLookupObject FindByName(string name, bool ignorecase)
        {
            string lcname = name.ToLower();
            for (int i = 0; i < _table.Count; i++)
            {
                SensorUnitLookupObject o = SafeGetAt(i);

                if (ignorecase)
                {
                    if (lcname.CompareTo(GetAt(i).Name.ToLower()) == 0) return o;
                }
                else
                {
                    if (name.CompareTo(GetAt(i).Name) == 0) return o;
                }

            }
            return null;
        }


        public SensorUnitLookupObject FindByNameTruncated(string name)
        {
            string lcname = name.ToLower();

            int maxwidth = lcname.Length;
            for (int i = 0; i < _table.Count; i++)
            {
                SensorUnitLookupObject o = SafeGetAt(i);
                string comp = o.Name.ToLower();
                if (comp.IndexOf(lcname) == 0) return o;
            }
            return null;
        }

        public SensorUnitLookupObject FindByCode(int code)
        {
            // 
            if (!_tablesorted) Sort();

            int a = 0;
            int b = _table.Count;
            int found = -1;
            int dist = (b - a);

            while ((found < 0) && (dist > 1) && (b > a))
            {
                dist = (b - a);
                int mid = a + (dist / 2);
                int test = SafeGetAt(mid).Code;

                if (test == code)
                {
                    found = mid;
                }
                else
                {
                    if (code < test)
                    {
                        b = mid;

                    }
                    else
                    {
                        a = mid;
                    }

                }
            }

            if (found < 0)
            {
                // do a brute force lookup
                for (int i = 0; i < _table.Count; i++)
                {
                    if (SafeGetAt(i).Code == code)
                    {
                        found = i;
                        break;
                    }
                }
            }

            if (found < 0) return null;

            return GetAt(found);




        }

    }

    public class SensorUnitLookupObject
    {
        private string name = "";
        private int code = -1;
        public SensorUnitLookupObject() { }
        public SensorUnitLookupObject(string n, int c) { name = n; code = c; }
        public string Name { get { return name; } }
        public int Code { get { return code; } }
    }

    public class AvailableUnits
    {
        // SYNC GAV61661
        private ISensorsService SensorsService { get; }

        public AvailableUnits(ISensorsService sensorsService)
        {
            SensorsService = sensorsService;
        }

        public int[] AllowedUnitsForSensorCode(int sensor)
        {
            ArrayList u = new ArrayList();
            int normalizedsensor = SensorsService.NormalizeSensorCode(sensor);

            bool ECppbOK = false;
            if (SensorsService.IsECppb(sensor)) ECppbOK = true;

            try
            {
                // first try to see if there's any groups

                if (SensorsService.IsCO2(sensor))
                {
                    u.Add(GWL_Units.PPM);
                    u.Add(GWL_Units.MG_M3);
                    u.Add(GWL_Units.UG_M3);
                };

                if (SensorsService.IsTemperature(sensor))
                {
                    u.Add(GWL_Units.F);
                    u.Add(GWL_Units.C);
                    u.Add(GWL_Units.K);
                }

                // no groups, so do brute force lookup

                if (u.Count == 0)
                {
                    switch (normalizedsensor)
                    {

                        case Services.SensorsService.HOTWIRE:
                            u.Add(GWL_Units.M_S);
                            u.Add(GWL_Units.FT_M);
                            u.Add(GWL_Units.FT_S);
                            break;

                        case Services.SensorsService.COUNTS:
                            u.Add(GWL_Units.COUNTS);
                            break;
                        case Services.SensorsService.BAT:
                            u.Add(GWL_Units.VOLTS);
                            break;
                        case Services.SensorsService.DIFF_TEMP:
                        case Services.SensorsService.DIFF_TEMP_1:
                        case Services.SensorsService.DIFF_TEMP_2:
                        case Services.SensorsService.REF_TEMP:
                        case Services.SensorsService.DEW:
                        case Services.SensorsService.TEMP:
                        case Services.SensorsService.WETBULB:
                            u.Add(GWL_Units.F);
                            u.Add(GWL_Units.C);
                            u.Add(GWL_Units.K);
                            break;
                        case Services.SensorsService.RH:
                            u.Add(GWL_Units.RH);
                            break;
                        case Services.SensorsService.ABSHUMIDITY:
                            u.Add(GWL_Units.G_M3);
                            u.Add(GWL_Units.GR_FT3);
                            break;
                        case Services.SensorsService.HUMIDITYRATIO:
                            u.Add(GWL_Units.PPMW);
                            u.Add(GWL_Units.LB_LB);
                            u.Add(GWL_Units.GR_LB);
                            u.Add(GWL_Units.KG_KG);
                            u.Add(GWL_Units.G_KG);
                            break;
                        case Services.SensorsService.SPECHUMIDITY:
                            u.Add(GWL_Units.PPMW);
                            u.Add(GWL_Units.LB_LB);
                            u.Add(GWL_Units.GR_LB);
                            u.Add(GWL_Units.KG_KG);
                            u.Add(GWL_Units.G_KG);
                            break;
                        case Services.SensorsService.O2:
                            u.Add(GWL_Units.PPERCENT);
                            break;
                        case Services.SensorsService.TVOC_Alphasense_PPB:
                        case Services.SensorsService.TVOC_Baseline_PPB:
                        case Services.SensorsService.TVOC_PID_PPBY:
                            u.Add(GWL_Units.PPB);
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.MG_M3);
                            u.Add(GWL_Units.UG_M3);
                            break;
                        case Services.SensorsService.TVOC_Generic:
                            u.Add(GWL_Units.PPB);
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.UG_M3);
                            break;
                        case Services.SensorsService.TVOC_PPM_10000:
                        case Services.SensorsService.TVOC_PPM_200:
                        case Services.SensorsService.TVOC_PPM_2000:
                        case Services.SensorsService.TVOC_Generic_PPM:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPB);
                            u.Add(GWL_Units.PPERCENT);
                            u.Add(GWL_Units.MG_M3);
                            u.Add(GWL_Units.UG_M3);
                            break;
                        case Services.SensorsService.DIFF_PRESS_002_5:
                        case Services.SensorsService.DIFF_PRESS_020:
                        case Services.SensorsService.DIFF_PRESS_100:
                        case Services.SensorsService.DIFF_PRESSURE:
                            u.Add(GWL_Units.PASCALS);
                            u.Add(GWL_Units.MBAR);
                            u.Add(GWL_Units.MMHG);
                            u.Add(GWL_Units.IN_H2O);
                            break;
                        case Services.SensorsService.BAR_PRESS_800:
                        case Services.SensorsService.BAROMETRIC_PRESSURE:
                            u.Add(GWL_Units.MBAR);
                            u.Add(GWL_Units.INHG);
                            u.Add(GWL_Units.MMHG);
                            u.Add(GWL_Units.PSI);
                            u.Add(GWL_Units.KPA);
                            u.Add(GWL_Units.ATM);
                            u.Add(GWL_Units.TORR);
                            break;

                        case Services.SensorsService.PARTICULATE:
                            u.Add(GWL_Units.UG_M3);
                            u.Add(GWL_Units.MG_M3);

                            break;

                        case Services.SensorsService.PITOT_AIRSPEED:
                            u.Add(GWL_Units.M_S);
                            u.Add(GWL_Units.FT_M);
                            u.Add(GWL_Units.FT_S);
                            break;

                        case Services.SensorsService.SO2:
                        case Services.SensorsService.NO2:
                        case Services.SensorsService.NO2_4E:
                        case Services.SensorsService.H2S:
                        case Services.SensorsService.COSH_H2S:
                        case Services.SensorsService.NO:
                        case Services.SensorsService.CL2:
                        case Services.SensorsService.H2:
                        case Services.SensorsService.HCN:
                        case Services.SensorsService.NH3:
                        case Services.SensorsService.NH3H:
                        case Services.SensorsService.ETO:
                        case Services.SensorsService.O3:
                        case Services.SensorsService.HCL:
                        case Services.SensorsService.HCL_SEM:
                        case Services.SensorsService.HCL_MEM:
                        case Services.SensorsService.HF:
                        case Services.SensorsService.HF_SEM:
                        case Services.SensorsService.HF_SENSORIX:
                        case Services.SensorsService.PH3:
                        case Services.SensorsService.ASH3:
                        case Services.SensorsService.ASH3_SX:
                        case Services.SensorsService.SIH4:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPERCENT);
                            u.Add(GWL_Units.MG_M3);
                            u.Add(GWL_Units.UG_M3);
                            if (ECppbOK) // new feaure, all electrochems with > 0.01 res get PPB
                            {
                                u.Add(GWL_Units.PPB);
                            }
                            break;

                        case Services.SensorsService.CO:
                        case Services.SensorsService.CO_A4:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPERCENT);
                            u.Add(GWL_Units.MG_M3);
                            u.Add(GWL_Units.UG_M3);
                            u.Add(GWL_Units.PPB); // Added 2017 for Well BLDG
                            break;


                        case Services.SensorsService.PEL:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PERCENT_LEL);
                            u.Add(GWL_Units.PERCENT_MET);
                            break;
                        case Services.SensorsService.CLO2:
                        case Services.SensorsService.CLO2_SEM:
                        case Services.SensorsService.CLO2_SX:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPERCENT);
                            break;

                        case Services.SensorsService.HCHO:
                        case Services.SensorsService.HCHO_ECSENSE:
                            u.Add(GWL_Units.PPB);
                            u.Add(GWL_Units.UG_M3);
                            break;

                        case Services.SensorsService.B2H6:
                        case Services.SensorsService.B2H6_SX:
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPB);
                            break;

                        case Services.SensorsService.PARTICLE_0_2UM:
                        case Services.SensorsService.PARTICLE_0_3UM:
                        case Services.SensorsService.PARTICLE_0_5UM:
                        case Services.SensorsService.PARTICLE_0_7UM:
                        case Services.SensorsService.PARTICLE_1_0UM:
                        case Services.SensorsService.PARTICLE_2_0UM:
                        case Services.SensorsService.PARTICLE_2_5UM:
                        case Services.SensorsService.PARTICLE_3_0UM:
                        case Services.SensorsService.PARTICLE_5_0UM:
                        case Services.SensorsService.PARTICLE_10_0UM:
                        case Services.SensorsService.PARTICLE_25_0UM:
                            u.Add(GWL_Units.C_CF_DELTA);
                            u.Add(GWL_Units.C_CF_EPSILON);
                            u.Add(GWL_Units.C_L_DELTA);
                            u.Add(GWL_Units.C_L_EPSILON);
                            u.Add(GWL_Units.CNTS_DELTA);
                            u.Add(GWL_Units.CNTS_EPSILON);
                            break;
                        case Services.SensorsService.ILLUMINANCE:
                            u.Add(GWL_Units.LUX);
                            u.Add(GWL_Units.FOOTCANDLE);
                            break;
                    }

                    if (u.Count == 0) // Nothing was matched, so try some more Groups
                    {

                        if (SensorsService.IsAirVelocity(sensor))
                        {
                            u.Add(GWL_Units.M_S);
                            u.Add(GWL_Units.FT_M);
                            u.Add(GWL_Units.FT_S);
                        }

                        if (SensorsService.IsEC(sensor))
                        {
                            u.Add(GWL_Units.PPM);
                            u.Add(GWL_Units.PPERCENT);
                            u.Add(GWL_Units.MG_M3);
                            u.Add(GWL_Units.UG_M3);
                        }

                        if (SensorsService.IsBarometricPressure(sensor))
                        {
                            u.Add(GWL_Units.MBAR);
                            u.Add(GWL_Units.INHG);
                            u.Add(GWL_Units.MMHG);
                            u.Add(GWL_Units.PSI);
                            u.Add(GWL_Units.KPA);
                            u.Add(GWL_Units.ATM);
                            u.Add(GWL_Units.TORR);
                        }

                        if (SensorsService.IsDifferentialPressure(sensor))
                        {
                            u.Add(GWL_Units.PASCALS);
                            u.Add(GWL_Units.MBAR);
                            u.Add(GWL_Units.MMHG);
                            u.Add(GWL_Units.IN_H2O);
                        }

                        if (SensorsService.IsParticleCount(sensor))
                        {
                            u.Add(GWL_Units.C_CF_DELTA);
                            u.Add(GWL_Units.C_CF_EPSILON);
                            u.Add(GWL_Units.C_L_DELTA);
                            u.Add(GWL_Units.C_L_EPSILON);
                            u.Add(GWL_Units.CNTS_DELTA);
                            u.Add(GWL_Units.CNTS_EPSILON);
                        }

                        if (SensorsService.IsParticleMass(sensor))
                        {
                            u.Add(GWL_Units.UG_M3);
                            u.Add(GWL_Units.MG_M3);
                        }

                        if (SensorsService.IsHCHO(sensor))
                        {
                            u.Add(GWL_Units.PPB);
                            u.Add(GWL_Units.UG_M3);
                        }


                    }


                }

                if (u.Count == 0) return null;
                int[] codes = new int[u.Count];
                for (int i = 0; i < u.Count; i++) codes[i] = (int)u[i];
                return codes;
            }
            catch
            {

            }
            return null;

        }

        public List<SensorUnit> GetSensorUnits(int sensor)
        {
            try
            {
                int[] unitcodes = AllowedUnitsForSensorCode(sensor);

                if (unitcodes == null) return null;
                if (unitcodes.Length == 0) return null;

                GWL_Units u = new GWL_Units(SensorsService);
                var units = new List<SensorUnit>();
                for (int i = 0; i < unitcodes.Length; i++)
                {
                    units.Add(new SensorUnit
                    {
                        Name = u.LookupUnitName(unitcodes[i]),
                        Code = unitcodes[i]
                    });
                }

                return units;

            }
            catch
            {

            }
            return null;

        }


    }

    public class GroupEnvironmentalValues
    {
        private Hashtable _envirovalues = new Hashtable();

        private string HashKey(int sensor, int unit)
        {
            return sensor.ToString() + ":" + unit.ToString();
        }


        public GroupEnvironmentalValues()
        {
            UpdateValue(SensorsService.TEMP, GWL_Units.C, 20.0);
        }

        public void UpdateValue(int sensor, int unit, double value)
        {
            string hash = HashKey(sensor, unit);
            if (_envirovalues.Contains(hash))
            {
                _envirovalues[hash] = value;
            }
            else
            {
                _envirovalues.Add(hash, value);
            }
        }

        public double GetValue(int sensor, int unit, double seedvalue, out bool valid)
        {
            valid = false;
            double value = 0.00;
            string hash = HashKey(sensor, unit);
            if (_envirovalues.Contains(hash))
            {
                value = (double)_envirovalues[hash];
                valid = true;
                return value;
            }

            return seedvalue;
        }

    }

    #endregion
}
#endregion
namespace GrayWolf.Enums
{
    public enum SensorType
    {
        // IMPORTANT: This must be kept in sync with SensorService.cs
        // Search for the code: SYNC_SENSORS181!!! for all places this must be Synced

        PRBSEN_UNDEF = -1,
        PRBSEN_TEMP = 0,
        PRBSEN_RH = 1,
        PRBSEN_DEW = 2,
        PRBSEN_CO = 3,
        PRBSEN_CO_A4 = 189,

        // CO2 Sensors
        PRBSEN_CO2 = 4, // SGX - 
        PRBSEN_CO2A1 = 130, // AlphaSense IRC A1
        PRBSEN_CO2C2 = 131, // Clairair Ltd Cirius 2
        PRBSEN_CO2_THERMISTOR = 70,// Internal CO2 Thermistor Temp
        PRBSEN_CO2_RATIO = 181,
        PRBSEN_CO2_SGX = 174,
        PRBSEN_CO2_DYNAMIC = 175,
        PRBSEN_CO2_IR21EM = 176,
        PRBSEN_CO2_AUTORANGE = 177,
        PRBSEN_CO2_AUTO_20K = 185,
        PRBSEN_CO2_RATIO_20K = 186,
        // CO2 Sensors

        PRBSEN_HOTWIRE = 5,
        PRBSEN_PARTICULATE = 6,
        PRBSEN_VOLFLOW = 7,
        PRBSEN_COUNTS = 8,
        PRBSEN_BAT = 9,
        PRBSEN_ABSHUMIDITY = 10,
        PRBSEN_WETBULB = 11,
        PRBSEN_HUMIDITYRATIO = 12,
        PRBSEN_SO2 = 13,
        PRBSEN_NO2 = 14,
        PRBSEN_H2S = 15,
        PRBSEN_NO = 16,
        PRBSEN_SPECHUMIDITY = 17,
        PRBSEN_O2 = 18,
        PRBSEN_CL2 = 19,
        PRBSEN_H2 = 20,
        PRBSEN_HCN = 21,
        PRBSEN_NH3 = 22,
        PRBSEN_ETO = 23,
        PRBSEN_LINTEMP = 24,//Linear temperature sensor used in TG501
        PRBSEN_O3 = 72,
        PRBSEN_O3_A1 = 184,
        PRBSEN_HCL = 73,
        PRBSEN_HCL_SEM = 178,
        PRBSEN_HCL_MEM = 188,
        PRBSEN_PH3 = 86,  // Phosphine
        PRBSEN_PITOT_AIRSPEED = 89,
        PRBSEN_HF = 90,
        PRBSEN_HF_SEM = 179,
        PRBSEN_HF_SENSORIX = 194,
        PRBSEN_TOTAL_PM = 115, //Total Particulate mass
        PRBSEN_ASH3 = 100,
        PRBSEN_SIH4 = 101,
        PRBSEN_CLO2 = 106,
        PRBSEN_CLO2_SEM = 180,
        PRBSEN_F2 = 107,
        PRBSEN_B2H6 = 108, //Diborane
        PRBSEN_COSH_H2S = 109,
        PRBSEN_NH3H = 110, //Nitrogen High
        PRBSEN_COCL2 = 111,   //Phosgene
        PRBSEN_THT = 112,
        PRBSEN_PEL = 113,

        PRBSEN_TVOC_Generic_PPM = 87,//generic sensor id for Alphasense PID-A1 and DSI Baseline 200,2000,10000
        PRBSEN_TVOC_PPM_200 = 26,//DSII Baseline 200 ppm
        PRBSEN_TVOC_PPM_2000 = 27,//DSII Baseline 2000 ppm
        PRBSEN_TVOC_PPM_10000 = 28,//DSII Baseline 10000 ppm
        PRBSEN_TVOC_Baseline_PPB = 88,//Baseline silver and yellow ppb for DSI
        PRBSEN_TVOC_Alphasense_PPB = 114,//Alphasense ppb
        PRBSEN_TVOC_PID_PPBY = 25,//DSII Baseline yellow ppb
        PRBSEN_TVOC_Generic = 29, //generic sensor id for ANY PID, ppb or ppm


        PRBSEN_PARTICLE_0_2UM = 116,
        PRBSEN_PARTICLE_0_3UM = 117,
        PRBSEN_PARTICLE_0_5UM = 118,
        PRBSEN_PARTICLE_0_7UM = 119,
        PRBSEN_PARTICLE_1_0UM = 120,
        PRBSEN_PARTICLE_2_0UM = 121,
        PRBSEN_PARTICLE_2_5UM = 122,
        PRBSEN_PARTICLE_3_0UM = 123,
        PRBSEN_PARTICLE_5_0UM = 124,
        PRBSEN_PARTICLE_10_0UM = 125,
        PRBSEN_PARTICLE_25_0UM = 126,
        PRBSEN_PARTICLE_FLOWRATE = 127,
        PRBSEN_PARTICLE_SAMPLETIME = 128,
        PRBSEN_PARTICLE_DENSITY = 129,

        PRBSEN_DIFF_PRESSURE = 135,
        PRBSEN_BAROMETRIC_PRESSURE = 136,


        PRBSEN_DIFF_PRESS_020 = 137,
        PRBSEN_DIFF_PRESS_100 = 138,
        PRBSEN_BAR_PRESS_600 = 139,
        PRBSEN_BAR_PRESS_800 = 140,
        PRBSEN_DIFF_TEMP = 141,
        PRBSEN_DIFF_TEMP_1 = 142,
        PRBSEN_DIFF_TEMP_2 = 143,
        PRBSEN_REF_TEMP = 144,
        PRBSEN_DIFF_PRESS_002_5 = 145,

        PRBSEN_LATITUDE = 151,
        PRBSEN_LONGITUDE = 152,

        PRBSEN_PM_0_2UM = 160,
        PRBSEN_PM_0_3UM = 161,
        PRBSEN_PM_0_5UM = 162,
        PRBSEN_PM_0_7UM = 163,
        PRBSEN_PM_1_0UM = 164,
        PRBSEN_PM_2_0UM = 165,
        PRBSEN_PM_2_5UM = 166,
        PRBSEN_PM_3_0UM = 167,
        PRBSEN_PM_5_0UM = 168,
        PRBSEN_PM_10_0UM = 169,
        PRBSEN_PM_25_0UM = 170,
        PRBSEN_HCHO = 171,

        PRBSEN_VOLFLOW_ACTUAL = 172,
        PRBSEN_ILLUMINANCE = 173,

        PRBSEN_HCHO_EC = 187,

 
        PRBSEN_B2H6_SX = 203,
        PRBSEN_ASH3_SX = 196,
        PRBSEN_CLO2_SX = 205,
        PRBSEN_H2O2	=192,
        PRBSEN_NO2_4E	=193



    }
}
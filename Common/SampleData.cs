using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GrayWolf.Common
{
    class SampleData
    {
        public Random _random;

        private double tvoc = 150.1;
        private double co2_1 = 450;
        private double co2_2 = 532;
        private double RH_1 = 30;
        private double TEMP_1 = 22.3;
        private double RH_2 = 30;
        private double TEMP_2 = 22.3;
        private double O3 = 0.00;
        private double SO2 = 0.00;
        private double NO2 = 0.00;

        private int _stabilizingcount=30;
        private int _batterylow = 200;

        public SampleData()
        {
            _random = new Random();
        }

        public string SampleJSON()
        {

            tvoc = tvoc + ((_random.NextDouble()-0.4) * 10); // trend upward
            if (tvoc < 10) tvoc = 10;
            co2_1= co2_1 + ((_random.NextDouble() - 0.4) * 10); // trend upward
            if (co2_1 < 350) co2_1 = 350;
            co2_2 = co2_2 + ((_random.NextDouble() - 0.4) * 10); // trend upward
            if (co2_2 < 350) co2_1 = 350;
            TEMP_1 = TEMP_1 + _random.NextDouble() - 0.4;
            TEMP_2 = TEMP_2 + _random.NextDouble() - 0.4;
            RH_1 = RH_1 + _random.NextDouble() - 0.4;
            if (RH_1 > 90) RH_1 = 90;
            if (RH_1 < 10) RH_1 = 10;
            RH_2 = RH_2 + _random.NextDouble() - 0.4;
            if (RH_2 > 90) RH_2 = 90;
            if (RH_2 < 10) RH_2 = 10;

            SO2 = SO2 + ((_random.NextDouble()-0.4) / 10.0);
            if (SO2 < 0.00) SO2 = 0.00;
            NO2 = NO2 + ((_random.NextDouble() - 0.4) / 10.0);
            if (NO2 < 0.00) NO2 = 0.00;
            O3 = O3 + ((_random.NextDouble() - 0.4) / 10.0);
            if (O3< 0.00) O3= 0.00;


            string status = "OK";
            if (_stabilizingcount>0)
            {
                status = "Stabilizing";
                _stabilizingcount--;
            }

            string battery = "LowBattery";
            if (_batterylow>0)
            {
                battery = "OK";
                _batterylow--;
            }


            string l1 = String.Format(  "{{\"sensor\":\"CO2\",\"unit\":\"ppm\",\"value\":{0},\"decimals\":0,\"status\":\"{1}\"}}," +
                                        "{{\"sensor\":\"Temp\",\"unit\":\"°C\",\"value\":{2},\"decimals\":1,\"status\":\"{3}\"}}," +
                                        "{{\"sensor\":\"RH\",\"unit\":\"%RH\",\"value\":{4},\"decimals\":1,\"status\": \"{5}\"}}," +
                                        "{{\"sensor\":\"TVOC\",\"unit\":\"ppb\",\"value\":{6},\"decimals\":0,\"status\":\"{7}\"}}",
                                           co2_1.ToString(),status,TEMP_1.ToString(),status,RH_1.ToString(),status,tvoc.ToString(),status);

            string l2 = String.Format(  "{{\"sensor\":\"CO2\",\"unit\":\"ppm\",\"value\":{0},\"decimals\":0,\"status\":\"{1}\"}}," +
                                        "{{\"sensor\":\"Temp\",\"unit\":\"°C\",\"value\":{2},\"decimals\":1,\"status\":\"{3}\"}}," +
                                        "{{\"sensor\":\"RH\",\"unit\":\"%RH\",\"value\":{4},\"decimals\":1,\"status\":\"{5}\"}}," +
                                        "{{\"sensor\":\"O3\",\"unit\":\"ppm\",\"value\":{6},\"decimals\":2,\"status\":\"{7}\"}}," +
                                        "{{\"sensor\":\"SO2\",\"unit\":\"ppm\",\"value\":{8},\"decimals\":1,\"status\":\"{9}\"}}," +
                                        "{{\"sensor\":\"NO2\",\"unit\":\"ppm\",\"value\":{10},\"decimals\":1,\"status\":\"{11}\"}}",
                        co2_2.ToString(), status, 
                        TEMP_2.ToString(), status, 
                        RH_2.ToString(), status, 
                        O3.ToString(), status, 
                        SO2.ToString(), status, 
                        NO2.ToString(), status );


            string jsonformat = "{{ \"dataSource\": \"BLE\"," + 
                "\"devices\": "+
                              "["+
                               "{{ \"generator\":\"DSII\",\"version\":1.00, \"model\":\"DSII-8\",\"serialNumber\":\"09-1026\",\"timeStamp\":\"{0}\",\"battery\":\"{1}\",\"status\":\"OK\",\"data\":[{2}] }},"+
                               "{{ \"model\":\"DSII-8\",\"serialNumber\":\"09-1126\",\"timeStamp\":\"{3}\",\"battery\":\"OK\",\"status\":\"OK\",\"data\":[{4}] }}"+
                              "]"+
                            "}}";
            string utc  = DateTime.UtcNow.ToString("o");

            string probeoutput = String.Format(jsonformat, utc, battery, l1, utc, l2);

            return probeoutput;
        }

    }
}

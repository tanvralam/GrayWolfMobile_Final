using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;


namespace GrayWolf.Interfaces
{
    public interface ISensorsService
    {
        int GetDPsForSensor(int rawsensorcode, int unit);
        string GetSensorName(int code);
        string GetSensorUnitName(int code);
        int NormalizeSensorCode(int sensor);
        bool IsHCHO(int sensor);
        bool IsParticleMass(int sensor);
        bool IsParticleCount(int sensor);
        bool IsDifferentialPressure(int sensor);
        bool IsBarometricPressure(int sensor);
        bool IsEC(int sensor);
        bool IsAirVelocity(int sensor);
        bool IsTemperature(int sensor);
        bool IsCO2(int sensor);
        bool IsECppb(int sensor);
        bool IsDerived(int sensor);
        bool IsPID(int sensor);
        List<SensorUnit> GetSensorUnits(int sensor);
        Task SetUnitConversion(Reading reading, SensorUnit sensorUnit, string id, int targetCode);
        void RemoveUnitConversion(int conversionId);
        string ConvertValue(double val, int unitCode, int targetUnitCode, int sensor);
        bool IsSameParameterAndUnit(int code1, int unit1, int code2, int unit2);
        void DefaultScale(int sensorcode, int unit, out double min, out double max);
        Color GetSensorColor(int sensorCode, int index);
        bool IsHumidity(int code);
        string LookupShortSensorName(int code);
    }
}

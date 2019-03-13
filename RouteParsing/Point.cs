using System;

namespace RouteParsing
{
    public class State
    {
        public DateTime GpsTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Altitude { get; set; }
        public int GpsAccuracy { get; set; }
        public int Heading { get; set; }
        public float SpeedGps { get; set; }
        public float SpeedObd { get; set; }
        public bool IsEngineStarted { get; set; }
        public bool IsIgnitionActivated { get; set; }
        public long Rpm { get; set; }
    }
}
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WeatherApp
{
    public class WeatherDataInfo
    {
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("generationtime_ms")]
        public float GenerationTime { get; set; }
        [JsonProperty("utc_offset_seconds")]
        public int UtcOffset { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }
        [JsonProperty("elevation")]
        public float Elevation { get; set; }
        [JsonProperty("hourly_units")]
        public Hourly_Units HourlyUnits { get; set; }
        [JsonProperty("hourly")]
        public Hourly HourlyProps { get; set; }


        public class Hourly_Units
        {
            [JsonProperty("time")]
            public string TimeUnit { get; set; }
            [JsonProperty("temperature_2m")]
            public string TemperatureUnit { get; set; }
        }

        public class Hourly
        {
            [JsonProperty("time")]
            public IList<string> Times { get; set; } = new List<string>();
            [JsonProperty("temperature_2m")]
            public IList<float> Temperatures { get; set; } = new List<float>();
        }

    }
}

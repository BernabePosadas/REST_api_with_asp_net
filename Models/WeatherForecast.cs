using System;

namespace TodoApi.Models{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => this.getFarenheight(TemperatureC);

        public string Summary { get; set; }
        
        private int getFarenheight(int TemperatureC){
            return (32 + (int)(TemperatureC / 0.5556));
        }
    }
}

using System;

namespace WpfApp1
{
    public class Flight : IEquatable<Flight>
    {
        public string FlightName { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime TravelTime { get; set; }
        public double Price { get; set; }
        public string Airplane { get; set; }
        public Flight() { }
        public Flight(string FlightName,
                      string DepartureCity,
                      string ArrivalCity,
                      DateTime DepartureDate,
                      DateTime TravelTime,
                      double Price,
                      string Airplane)
        {
            this.FlightName = FlightName;
            this.DepartureCity = DepartureCity;
            this.ArrivalCity = ArrivalCity;
            this.DepartureDate = DepartureDate;
            this.TravelTime = TravelTime;
            this.Price = Price;
            this.Airplane = Airplane;
        }

        public bool Equals(Flight other)
        {
            return FlightName == other.FlightName &&
                   DepartureCity == other.DepartureCity &&
                   ArrivalCity == other.ArrivalCity &&
                   DepartureDate.ToString(Formatter.DateFormat) == other.DepartureDate.ToString(Formatter.DateFormat) &&
                   TravelTime.ToString(Formatter.TimeFormat) == other.TravelTime.ToString(Formatter.TimeFormat) &&
                   Price == other.Price &&
                   Airplane == other.Airplane;
        }
    }
}

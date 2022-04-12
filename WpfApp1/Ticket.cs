using System;
namespace WpfApp1
{
    public class Ticket
    {
        public uint ticketId;
        public uint flightId;
        public uint seatNumber;
        public DateTime departureDate;
        public DateTime arrivalDate;
        public string flightName;
        public DateTime travelTime;
        public uint price;
        public string departureCity;
        public string arrivalCity;
        public string name;
        public string surname;
        public string sex;

        public Ticket(uint TicketId,
                      uint FlightId,
                      uint SeatNumber,
                      DateTime DepartureDate,
                      DateTime ArrivalDate,
                      string FlightName,
                      DateTime TravelTime,
                      uint Price,
                      string DepartureCity,
                      string ArrivalCity,
                      string Name,
                      string Surname,
                      string Sex)
        {
            ticketId = TicketId;
            flightId = FlightId;
            seatNumber = SeatNumber;
            departureDate = DepartureDate;
            arrivalDate = ArrivalDate;
            flightName = FlightName;
            travelTime = TravelTime;
            price = Price;
            departureCity = DepartureCity;
            arrivalCity = ArrivalCity;
            name = Name;
            surname = Surname;
            sex = Sex;
        }

        public override string ToString()
        {
            return $"Номер билета : {ticketId}\nНомер рейса : {flightId}\nРейс : {flightName}\nМесто : {seatNumber}\nГород вылета : {departureCity}\nГород прилёта : {arrivalCity}\nФамилия : {surname}\nИмя : {name}\nПол : {sex}\nВремя отправления : {departureDate.ToString("dd.MM.yyyy HH:mm")}\nВремя прилёта : {arrivalDate.ToString("dd.MM.yyyy HH:mm")}\nВремя в пути : {travelTime.ToString("HH:mm")}\nЦена : {price}";
        }
    }
}

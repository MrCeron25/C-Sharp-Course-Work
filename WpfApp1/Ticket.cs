using System;
using System.IO;

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

        public void SaveTicket(string pathToDir)
        {
            string fileName = $@"{ticketId} {name} {surname} {flightName}.txt";
            string fullPath = $"{pathToDir}\\{fileName}";
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(CreateTicket());
                }
            }
        }

        public string CreateTicket()
        {
            const char sym = '='; // символ для линии
            const uint symCount = 40; // количество знаков для линии
            string res = $"{Tools.GetSym(symCount, sym)}\nНомер билета : {ticketId}\nНомер рейса : {flightId}\nРейс : {flightName}\nМесто : {seatNumber}\nГород вылета : {departureCity}\nГород прилёта : {arrivalCity}\nФамилия : {surname}\nИмя : {name}\nПол : {sex}\nВремя отправления : {departureDate.ToString("dd.MM.yyyy HH:mm")}\nВремя прилёта : {arrivalDate.ToString("dd.MM.yyyy HH:mm")}\nВремя в пути : {travelTime.ToString("HH:mm")}\nЦена : {price}\n{Tools.GetSym(symCount, sym)}";
            return res;
        }
    }
}

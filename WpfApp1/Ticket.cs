﻿using System;
using System.IO;
using System.Text;

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
            string fileName = $@"\{ticketId} {name} {surname} {flightName}.txt";
            string fullPath = pathToDir + fileName;
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(CreateTicket());
                }
            }
        }

        public string CreateTicket()
        {
            const uint symCount = 100;
            string res = "";
            res += Tools.GetSym(symCount, '=');
            res += '\n';

            res += $"Номер билета : {ticketId}\nНомер рейса : {flightId}\nРейс : {flightName}\nМесто : {seatNumber}\nГород вылета : {departureCity}\nГород прилёта : {arrivalCity}\nФамилия : {surname}\nИмя : {name}\nПол : {sex}\nВремя отправления : {departureDate.ToString("dd.MM.yyyy HH:mm")}\nВремя прилёта : {arrivalDate.ToString("dd.MM.yyyy HH:mm")}\nВремя в пути : {travelTime.ToString("HH:mm")}\nЦена : {price}\n";

            res += Tools.GetSym(symCount, '=');
            return res;
        }

        //public override string ToString()
        //{
        //    return $@"{name} {surname} {flightNumber} {from} {to} {dateTime} {seatNumber}";
        //}
    }
}

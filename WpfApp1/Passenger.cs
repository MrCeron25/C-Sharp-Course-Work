using System;

namespace WpfApp1
{
    public class Passenger
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public uint PassportId { get; set; }
        public uint PassportSeries { get; set; }
        public Passenger(string Name,
                         string Surname,
                         string Sex,
                         DateTime DateOfBirth,
                         uint PassportId,
                         uint PassportSeries)
        {
            this.Name = Name;
            this.Surname = Surname;
            this.DateOfBirth = DateOfBirth;
            this.PassportId = PassportId;
            this.PassportSeries = PassportSeries;
            this.Sex = Sex;
        }

        public override string ToString()
        {
            return $"Имя : {Name}\nФамилия : {Surname}\nДата рождения : {DateOfBirth.ToShortDateString()}\nНомер паспорта : {PassportId}\nСерия паспорта : {PassportSeries}\n\n";
        }
    }
}

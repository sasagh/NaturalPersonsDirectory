using System;
using System.ComponentModel.DataAnnotations;

namespace NaturalPersonsDirectory.Models
{
    public class NaturalPerson
    {
        public int NaturalPersonId { get; set; }
        public string FirstNameGE { get; set; }
        public string FirstNameEn { get; set; }
        public string LastNameGe { get; set; }
        public string LastNameEn { get; set; }
        public string PassportNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string ContactInformations { get; set; }
        public string ImagePath { get; set; }
    }
}

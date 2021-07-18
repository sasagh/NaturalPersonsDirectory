using System;
using System.ComponentModel.DataAnnotations;

namespace NaturalPersonsDirectory.Models
{
    public class NaturalPerson
    {
        public int Id { get; set; }
        
        public string FirstNameGe { get; set; }
        
        public string FirstNameEn { get; set; }
        
        public string LastNameGe { get; set; }
        
        public string LastNameEn { get; set; }
        
        public string PassportNumber { get; set; }
        
        public DateTime Birthday { get; set; }
        
        public string Address { get; set; }
        
        public string ContactInformation { get; set; }
        
        public string ImagePath { get; set; }
    }
}

using System;

namespace NaturalPersonsDirectory.Models
{
    public static class PreparedNaturalPersons
    {
        public static NaturalPerson BidzinaTabagari { get; } = new NaturalPerson()
        {
            FirstNameEn = "Bidzina",
            FirstNameGe = "ბიძინა",
            LastNameEn = "Tabagari",
            LastNameGe = "თაბაგარი",
            Address = "Tbilisi",
            PassportNumber = "98765432111",
            ContactInformation = "+995-555-111-222,bidzina.tabagari@gmail.com",
            Birthday = new DateTime(1950, 1, 1)
        };

        public static NaturalPerson GuramJinoria { get; } = new NaturalPerson()
        {
            FirstNameEn = "Guram",
            FirstNameGe = "გურამ",
            LastNameEn = "Jinoria",
            LastNameGe = "ჯინორია",
            Address = "Moscow",
            PassportNumber = "12345678901",
            ContactInformation = "guram.jinoria@mail.ru",
            Birthday = new DateTime(1951, 1, 1)
        };
        
        public static NaturalPerson UchaZeragia { get; } = new NaturalPerson()
        {
            FirstNameEn = "Ucha",
            FirstNameGe = "უჩა",
            LastNameEn = "Zeragia",
            LastNameGe = "ზერაგია",
            Address = "Unknown",
            PassportNumber = "32112332123",
            ContactInformation = "Unknown",
            Birthday = new DateTime(1955, 1, 1)
        };
    }
}
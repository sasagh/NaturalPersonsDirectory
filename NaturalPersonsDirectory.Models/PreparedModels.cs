using System;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.Models
{
    public static class PreparedModels
    {
        public static NaturalPerson GetBidzinaTabagari() => new NaturalPerson()
        {
            FirstNameEn = "Bidzina",
            FirstNameGe = "ბიძინა",
            LastNameEn = "Tabagari",
            LastNameGe = "თაბაგარი",
            Address = "Tbilisi",
            PassportNumber = "98765432111",
            ContactInformation = "+995-555-111-222,bidzina.tabagari@gmail.com",
            Birthday = new DateTime(1950, 1, 1),
            ImageFileName = "BIDZINA_TABAGARI_IMAGE.jpg"
        };

        public static NaturalPerson GetGuramJinoria() => new NaturalPerson()
        {
            FirstNameEn = "Guram",
            FirstNameGe = "გურამ",
            LastNameEn = "Jinoria",
            LastNameGe = "ჯინორია",
            Address = "Moscow",
            PassportNumber = "12345678901",
            ContactInformation = "guram.jinoria@mail.ru",
            Birthday = new DateTime(1951, 1, 1),
            ImageFileName = "GURAM_JINORIA_IMAGE.jpg"
        };

        public static NaturalPerson GetUchaZeragia() => new NaturalPerson()
        {
            FirstNameEn = "Ucha",
            FirstNameGe = "უჩა",
            LastNameEn = "Zeragia",
            LastNameGe = "ზერაგია",
            Address = "Unknown",
            PassportNumber = "32112332123",
            ContactInformation = "Unknown",
            Birthday = new DateTime(1955, 1, 1),
            ImageFileName = "UCHA_ZERAGIA_IMAGE.jpg"
        };

        public static Relation GetRelation() => new Relation()
        {
            FromId = 1,
            ToId = 2,
            RelationType = Enum.GetName(typeof(RelationType), RelationType.Other)
        };
    }
}
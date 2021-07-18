using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using System;
using System.Linq;

namespace NaturalPersonsDirectory.API
{
    public static class Seeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.Migrate();

                var person1 = new NaturalPerson()
                {
                    FirstNameEn = "Bidzina",
                    FirstNameGe = "ბიძინა",
                    LastNameEn = "Tabagari",
                    LastNameGe = "თაბაგარი",
                    Address = "Tbilisi",
                    PassportNumber = "98765432111",
                    ContactInformation = "+995-555-111-222,bidzina.tabagari@gmail.com",
                    Birthday = DateTime.Now.Date.AddYears(-70)
                };

                var person2 = new NaturalPerson()
                {
                    FirstNameEn = "Guram",
                    FirstNameGe = "გურამ",
                    LastNameEn = "Jinoria",
                    LastNameGe = "ჯინორია",
                    Address = "Moscow",
                    PassportNumber = "12345678901",
                    ContactInformation = "guram.jinoria@mail.ru",
                    Birthday = DateTime.Now.Date.AddYears(-65)
                };

                var person3 = new NaturalPerson()
                {
                    FirstNameEn = "Ucha",
                    FirstNameGe = "უჩა",
                    LastNameEn = "Zeragia",
                    LastNameGe = "ზერაგია",
                    Address = "Unknown",
                    PassportNumber = "32112332123",
                    ContactInformation = "Unknown",
                    Birthday = DateTime.Now.Date.AddYears(-67)
                };

                if (!context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person1.PassportNumber))
                {
                    context.NaturalPersons.Add(person1);
                }

                if (!context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person2.PassportNumber))
                {
                    context.NaturalPersons.Add(person2);
                }

                if (!context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person3.PassportNumber))
                {
                    context.NaturalPersons.Add(person3);
                }

                context.SaveChanges();

                var naturalPersons = context.NaturalPersons.ToList();

                if (!context.Relations.Any() && naturalPersons.Count >= 3)
                {
                    var relation1 = new Relation()
                    {
                        FromId = naturalPersons[0].Id,
                        ToId = naturalPersons[1].Id,
                        RelationType = RelationType.Acquaintance
                    };

                    var relation2 = new Relation()
                    {
                        FromId = naturalPersons[1].Id,
                        ToId = naturalPersons[2].Id,
                        RelationType = RelationType.Employee
                    };

                    context.Relations.Add(relation1);
                    context.Relations.Add(relation2);

                    context.SaveChanges();
                }
            }
        }
    }
}

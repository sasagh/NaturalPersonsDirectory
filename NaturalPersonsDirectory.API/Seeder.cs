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
                var _context = serviceScope.ServiceProvider.GetService<NaturalPersonsDirectoryDbContext>();

                _context.Database.Migrate();

                var person1 = new NaturalPerson()
                {
                    FirstNameEn = "Bidzina",
                    FirstNameGE = "ბიძინა",
                    LastNameEn = "Tabagari",
                    LastNameGe = "თაბაგარი",
                    Address = "Tbilisi",
                    PassportNumber = "98765432111",
                    ContactInformations = "+995-555-111-222,bidzina.tabagari@gmail.com",
                    Birthday = DateTime.Now.Date.AddYears(-70)
                };

                var person2 = new NaturalPerson()
                {
                    FirstNameEn = "Guram",
                    FirstNameGE = "გურამ",
                    LastNameEn = "Jinoria",
                    LastNameGe = "ჯინორია",
                    Address = "Moscow",
                    PassportNumber = "12345678901",
                    ContactInformations = "guram.jinoria@mail.ru",
                    Birthday = DateTime.Now.Date.AddYears(-65)
                };

                var person3 = new NaturalPerson()
                {
                    FirstNameEn = "Ucha",
                    FirstNameGE = "უჩა",
                    LastNameEn = "Zeragia",
                    LastNameGe = "ზერაგია",
                    Address = "Unknown",
                    PassportNumber = "32112332123",
                    ContactInformations = "Unknown",
                    Birthday = DateTime.Now.Date.AddYears(-67)
                };

                if (!_context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person1.PassportNumber))
                {
                    _context.NaturalPersons.Add(person1);
                }

                if (!_context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person2.PassportNumber))
                {
                    _context.NaturalPersons.Add(person2);
                }

                if (!_context.NaturalPersons.Any(naturalPerson => naturalPerson.PassportNumber == person3.PassportNumber))
                {
                    _context.NaturalPersons.Add(person3);
                }

                _context.SaveChanges();

                if (!_context.RelationIds.Any())
                {
                    _context.RelationIds.Add(new RelationId());
                    _context.RelationIds.Add(new RelationId());
                    _context.SaveChanges();
                }

                var naturalPersons = _context.NaturalPersons.ToList();

                if (!_context.Relations.Any() && naturalPersons.Count >= 3)
                {
                    var relation1 = new Relation()
                    {
                        RelationId = _context.RelationIds.FirstOrDefault().Id,
                        FromId = naturalPersons[0].NaturalPersonId,
                        ToId = naturalPersons[1].NaturalPersonId,
                        RelationType = RelationType.Acquaintance.ToString()
                    };

                    var relation2 = new Relation()
                    {
                        RelationId = relation1.RelationId+1,
                        FromId = naturalPersons[1].NaturalPersonId,
                        ToId = naturalPersons[2].NaturalPersonId,
                        RelationType = RelationType.Employee.ToString()
                    };

                    _context.Relations.Add(relation1);
                    _context.Relations.Add(relation2);

                    _context.SaveChanges();
                }
            }
        }
    }
}

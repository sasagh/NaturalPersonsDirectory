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

                var person1 = PreparedNaturalPersons.GetBidzinaTabagari();
                var person2 = PreparedNaturalPersons.GetGuramJinoria();
                var person3 = PreparedNaturalPersons.GetUchaZeragia();

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
                        RelationType = Enum.GetName(typeof(RelationType), RelationType.Employee)
                    };

                    var relation2 = new Relation()
                    {
                        FromId = naturalPersons[1].Id,
                        ToId = naturalPersons[2].Id,
                        RelationType = Enum.GetName(typeof(RelationType), RelationType.Acquaintance)
                    };

                    context.Relations.Add(relation1);
                    context.Relations.Add(relation2);

                    context.SaveChanges();
                }
            }
        }
    }
}

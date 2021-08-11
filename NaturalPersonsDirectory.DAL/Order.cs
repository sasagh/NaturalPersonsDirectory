using System.Linq;
using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.DAL
{
    public static class Order
    {
        public static IOrderedQueryable<NaturalPerson> OrderBy(
            this DbSet<NaturalPerson> naturalPersons,
            bool orderByDescending,
            string orderBy = null)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return orderByDescending
                    ? naturalPersons.OrderByDescending(np => np.Id)
                    : naturalPersons.OrderBy(np => np.Id);
            }
            
            return orderBy.ToLower() switch
            {
                GlobalVariables.FirstNameEn =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.FirstNameEn)
                        : naturalPersons.OrderBy(np => np.FirstNameEn),
                GlobalVariables.FirstNameGe =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.FirstNameGe)
                        : naturalPersons.OrderBy(np => np.FirstNameGe),
                GlobalVariables.LastNameEn =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.LastNameEn)
                        : naturalPersons.OrderBy(np => np.LastNameEn),
                GlobalVariables.LastNameGe =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.LastNameGe)
                        : naturalPersons.OrderBy(np => np.LastNameGe),
                GlobalVariables.PassportNumber =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.PassportNumber)
                        : naturalPersons.OrderBy(np => np.PassportNumber),
                GlobalVariables.Birthday =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.Birthday)
                        : naturalPersons.OrderBy(np => np.Birthday),
                _ =>
                    orderByDescending
                        ? naturalPersons.OrderByDescending(np => np.Id)
                        : naturalPersons.OrderBy(np => np.Id),
            };
        }
        
        public static IOrderedQueryable<Relation> OrderBy(
            this DbSet<Relation> relations,
            bool orderByDescending)
        {
            return orderByDescending
                ? relations.OrderByDescending(relation => relation.Id)
                : relations.OrderBy(relation => relation.Id);
        }
    }
}
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.Models
{
    public class RelatedPerson : NaturalPerson
    {
        public RelationType RelationType { get; set; }
    }
}
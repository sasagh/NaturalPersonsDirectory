using System.Text.Json.Serialization;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.Models
{
    public class Relation
    {
        public int Id { get; set; }
        
        public int FromId { get; set; }
        
        public virtual NaturalPerson From { get; set; }
        
        public int ToId { get; set; }
        
        public virtual NaturalPerson To { get; set; }
        
        public RelationType RelationType { get; set; }
    }
}

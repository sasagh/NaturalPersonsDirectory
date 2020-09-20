using System.Text.Json.Serialization;

namespace NaturalPersonsDirectory.Models
{
    public class Relation
    {
        public int RelationId { get; set; }
        [JsonIgnore]
        public int FromId { get; set; }
        [JsonIgnore]
        public int ToId { get; set; }
        public virtual NaturalPerson RelationFrom { get; set; }
        public virtual NaturalPerson RelationTo { get; set; }
        [JsonIgnore]
        public RelationId RelationIdRef { get; set; }
        public string RelationType { get; set; }
    }
}

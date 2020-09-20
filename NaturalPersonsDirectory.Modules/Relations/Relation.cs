using FluentValidation;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Linq;

namespace NaturalPersonsDirectory.Modules
{
    public class RelationRequest
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public string RelationType { get; set; }
    }

    public class RelationResponse
    {
        private int? _count = default;
        public int Count
        {
            get
            {
                if (_count == null)
                {
                    return Relations == null ? 0 : Relations.Count();
                }
                else
                {
                    return _count.Value;
                }
            }
            set { _count = value; }
        }
        public IEnumerable<Relation> Relations { get; set; }
    }

    public class RelationRequestValidator : AbstractValidator<RelationRequest>
    {
        public RelationRequestValidator()
        {
            RuleFor(request => request.FromId).NotEmpty().WithMessage("From id is required");
            RuleFor(request => request.ToId).NotEmpty().WithMessage("To id is required");
            RuleFor(request => request.RelationType).NotEmpty().IsEnumName(typeof(RelationType)).WithMessage("Relation type is incorrect");
        }
    }
}

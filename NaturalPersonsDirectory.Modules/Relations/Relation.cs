using FluentValidation;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NaturalPersonsDirectory.Modules
{
    public class RelationRequest
    {
        public int FromId { get; set; }

        public int ToId { get; set; }

        public RelationType? RelationType { get; set; }
    }

    public class RelationResponse
    {
        public int Count => Relations.Count;

        public ICollection<Relation> Relations { get; }

        public RelationResponse(ICollection<Relation> relations = null)
        {
            Relations = relations ?? new Collection<Relation>();
        }

        public RelationResponse(Relation relation)
        {
            Relations = new Collection<Relation> { relation };
        }
    }

    public class RelationRequestValidator : AbstractValidator<RelationRequest>
    {
        public RelationRequestValidator()
        {
            RuleFor(request => request.FromId).NotEmpty().WithMessage("From id is required");
            RuleFor(request => request.ToId).NotEmpty().WithMessage("To id is required");
            RuleFor(request => request.RelationType).IsInEnum().WithMessage("Relation type is incorrect");
        }
    }
}

using FluentValidation;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NaturalPersonsDirectory.Modules
{
    public class NaturalPersonRequest
    {
        public string FirstNameGe { get; set; }

        public string FirstNameEn { get; set; }

        public string LastNameGe { get; set; }

        public string LastNameEn { get; set; }

        public string PassportNumber { get; set; }

        public string Birthday { get; set; }

        public string Address { get; set; }

        public string ContactInformation { get; set; }
    }

    public class NaturalPersonResponse
    {
        public int Count => NaturalPersons.Count;

        public ICollection<NaturalPerson> NaturalPersons { get; }

        public NaturalPersonResponse(ICollection<NaturalPerson> naturalPersons = null)
        {
            NaturalPersons = naturalPersons ?? new Collection<NaturalPerson>();
        }

        public NaturalPersonResponse(NaturalPerson naturalPerson)
        {
            NaturalPersons = new Collection<NaturalPerson> { naturalPerson };
        }
    }

    public class RelatedPersonsResponse
    {
        public int Count => RelatedPersons.Count;

        public ICollection<RelatedPerson> RelatedPersons { get; }

        public RelatedPersonsResponse(ICollection<RelatedPerson> relatedPersons = null)
        {
            RelatedPersons = relatedPersons ?? new Collection<RelatedPerson>();
        }

        public RelatedPersonsResponse(RelatedPerson relatedPerson)
        {
            RelatedPersons = new Collection<RelatedPerson> { relatedPerson };
        }
    }

    public class NaturalPersonRequestValidator : AbstractValidator<NaturalPersonRequest>
    {
        public NaturalPersonRequestValidator()
        {
            RuleFor(request => request.FirstNameEn).NotEmpty().WithMessage("FirstNameEn should not be empty.");
            RuleFor(request => request.FirstNameGe).NotEmpty().WithMessage("FirstNameGe should not be empty.");
            RuleFor(request => request.LastNameEn).NotEmpty().WithMessage("LastNameEn should not be empty.");
            RuleFor(request => request.LastNameGe).NotEmpty().WithMessage("LastNameGe should not be empty.");
            RuleFor(request => request.Address).NotEmpty().WithMessage("Address should not be empty.");
            RuleFor(request => request.PassportNumber).NotEmpty().Matches("^[0-9]{11}").WithMessage("Passport number should be 11 characters long and contain only digits");
            RuleFor(request => request.Birthday).NotEmpty().Must(Validator.IsValidDate).WithMessage("Date should be in valid form");
            RuleFor(request => request.ContactInformation)
                .NotEmpty()
                .Must(Validator.IsValidContactInformation)
                .WithMessage(
                "Contact information format is incorrect. " +
                "Contact information should be phone number (pattern +995-5XX-XXX-XXX) or email address. " +
                "Multiple items should be separated by comma (,).");
        }
    }
}

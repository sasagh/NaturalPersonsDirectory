using FluentValidation;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.Modules
{
    public abstract class PaginationParameters
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;
        
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
        public bool OrderByDescending { get; set; }
    }
    
    public class RelationPaginationParameters : PaginationParameters{}

    public class NaturalPersonPaginationParameters : PaginationParameters
    {
        public string OrderBy { get; set; }
    }
    
    public class NaturalPersonPaginationParametersValidator : AbstractValidator<NaturalPersonPaginationParameters>
    {
        public NaturalPersonPaginationParametersValidator()
        {
            RuleFor(param => param.OrderBy)
                .Must(param => param == null || Validator.IsValidOrder(param));
        }
    }
}

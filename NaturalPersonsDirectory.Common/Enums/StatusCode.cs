namespace NaturalPersonsDirectory.Common
{
    public enum StatusCode
    {
        Success,
        Create,
        Delete,
        Update,
        NotFound,
        InternalServerError,
        ImageAdded,
        ImageUpdated,
        ImageDeleted,
        PassportNumberExists,
        IdNotExists,
        ChangingPassportNumberNotAllowed,
        NoImage,
        IncorrectIds,
        UnsupportedFileFormat,
        AlreadyHaveImage,
        RelationNotExists,
        RelationBetweenGivenIdsExists
    }
}

namespace NaturalPersonsDirectory.Common
{
    public enum StatusCode
    {
        Success,
        Delete,
        Update,
        NotFound,
        Error,
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
        DoesNotHaveImage,
    }
}

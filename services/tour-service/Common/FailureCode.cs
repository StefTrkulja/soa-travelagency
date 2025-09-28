namespace TourService.Common;

public static class FailureCode
{
    public const string NotFound = "NOT_FOUND";
    public const string InvalidArgument = "INVALID_ARGUMENT";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string DatabaseError = "DATABASE_ERROR";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string DuplicateResource = "DUPLICATE_RESOURCE";
}
namespace UserManagement.Api.Exceptions;

public sealed class DuplicateUserException : Exception
{
    public DuplicateUserException(string message) : base(message) { }
}

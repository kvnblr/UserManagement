namespace UserManagement.Api.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid credentials")
    {

    }
}

namespace UserManagement.Api.Exceptions;

public sealed class UserNotFoundException(Guid id) : Exception($"User '{id}' was not found") { }

using System;

namespace ChipsMMO.Models.Exceptions
{
    public class AccountAlreadyExists : Exception { }
    public class InvalidLoginCredentials : Exception { }
    public class AccountNotFound : Exception { }
    public class RefreshTokenInvalid : Exception { }
    public class RefreshTokenDoesNotMatch : Exception { }
}

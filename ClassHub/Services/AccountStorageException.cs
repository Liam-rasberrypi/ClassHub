using System;

namespace ClassHub.Services
{
    public sealed class AccountStorageException : Exception
    {
        public AccountStorageException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}

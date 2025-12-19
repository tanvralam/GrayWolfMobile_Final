using System;

namespace GrayWolf.Messages
{
    public class AuthStatusMessage
    {
        public bool IsLoggedIn { get; }

        public string Login { get; }

        public AuthStatusMessage(bool isLoggedIn, string login = "")
        {
            if (isLoggedIn && string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException();
            }

            IsLoggedIn = true;
            Login = login;
        }
    }
}

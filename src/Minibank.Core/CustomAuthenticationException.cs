namespace Minibank.Core
{
    public class CustomAuthenticationException : System.Exception
    {
        
        public CustomAuthenticationException(string message) : base(message) { }
        
    }
}
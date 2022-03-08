namespace Minibank.Core
{
    public class UserFriendlyException : System.Exception
    {
        private readonly string _exceptionText;

        public UserFriendlyException(string exceptionText)
        {
            _exceptionText = exceptionText; 
        }

        public string What()
        {
            return _exceptionText;
        }
        
    }
}
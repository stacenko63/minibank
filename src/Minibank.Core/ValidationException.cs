using System.Net;
using System.Net.Http;

namespace Minibank.Core
{
    
    public class ValidationException  : System.Exception
    {
        public ValidationException(string message) : base(message) { }

    }
}
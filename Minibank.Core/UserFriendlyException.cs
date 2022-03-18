using System.Net;
using System.Net.Http;

namespace Minibank.Core
{
    
    public class UserFriendlyException  : System.Exception
    {
        public UserFriendlyException(string message) : base(message) { }

    }
}
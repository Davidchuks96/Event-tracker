using Microsoft.AspNetCore.Authorization;

namespace E_Tracker.Authorization
{
    public class AccountAuthorizationRequirement : IAuthorizationRequirement
    {
        public AccountAuthorizationRequirement(string operation)
        {
            Operation = operation;
        }
        public string Operation { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDA.Events
{
    public class AccountCreatedEvent : IEvent
    {
        public string AccountId { get; set; }
        public string Owner { get; set; }
        public AccountCreatedEvent(string accountId, string owner)
        {  
            AccountId = accountId; 
            Owner = owner;
        }
    }
}

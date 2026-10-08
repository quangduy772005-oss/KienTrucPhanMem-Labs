using System;

namespace EDA.Events
{
    public record MoneyWithdrawnEvent(string AccountId, double Amount) : IEvent
    {
        // Thuộc tính phụ trợ tương thích ngược
        public string accountId => AccountId;
        public double amount => Amount;
    }
}

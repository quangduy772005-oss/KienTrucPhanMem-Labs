using System;

namespace EDA.Events
{
    public record TransferMoneyEvent(string FromAccountId, string ToAccountId, double Amount) : IEvent
    {
        public string fromAccountId => FromAccountId;
        public string toAccountId => ToAccountId;
        public double amount => Amount;
    }

    // Alias hỗ trợ cả tên theo đề bài có lỗi chính tả 'TranferMoneyEvent'
    public record TranferMoneyEvent(string FromAccountId, string ToAccountId, double Amount) 
        : TransferMoneyEvent(FromAccountId, ToAccountId, Amount);
}

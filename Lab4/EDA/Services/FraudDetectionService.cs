using System;
using EDA.Events;

namespace EDA.Services
{
    public class FraudDetectionService
    {
        public const double FraudThreshold = 10000;

        public FraudDetectionService(IEventBus bus)
        {
            // Subscribe MoneyWithdrawnEvent theo đúng yêu cầu đề bài
            bus.Subscribe<MoneyWithdrawnEvent>(HandleWithdrawn);
            bus.Subscribe<TransferMoneyEvent>(HandleTransfer);
        }

        private void HandleWithdrawn(MoneyWithdrawnEvent @event)
        {
            if (@event.Amount > FraudThreshold)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now} [FraudDetectionService] ⚠️ CẢNH BÁO BẤT THƯỜNG: Tài khoản '{@event.AccountId}' rút số tiền lớn {@event.Amount} (vượt ngưỡng {FraudThreshold})!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} [FraudDetectionService] Rút tiền bình thường: Tài khoản '{@event.AccountId}', số tiền {@event.Amount} (dưới ngưỡng {FraudThreshold}).");
            }
        }

        private void HandleTransfer(TransferMoneyEvent @event)
        {
            if (@event.Amount > FraudThreshold)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{DateTime.Now} [FraudDetectionService] ⚠️ CẢNH BÁO: Chuyển tiền số tiền lớn {@event.Amount} từ '{@event.FromAccountId}' sang '{@event.ToAccountId}' (vượt ngưỡng {FraudThreshold})!");
                Console.ResetColor();
            }
        }
    }

    // Alias hỗ trợ cả tên theo đề bài có lỗi chính tả 'FaudDetectionService'
    public class FaudDetectionService : FraudDetectionService
    {
        public FaudDetectionService(IEventBus bus) : base(bus) { }
    }
}

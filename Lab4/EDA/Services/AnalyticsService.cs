using System;
using EDA.Events;

namespace EDA.Services
{
    public class AnalyticsService
    {
        private double _totalDeposited = 0;
        private double _totalWithdrawn = 0;
        private double _totalTransferred = 0;

        public double TotalDeposited => _totalDeposited;
        public double TotalWithdrawn => _totalWithdrawn;
        public double TotalTransferred => _totalTransferred;

        public AnalyticsService(IEventBus _bus)
        {
            _bus.Subscribe<MoneyDepositedEvent>(HandleDeposit);
            // Sửa lỗi: Đăng ký đúng MoneyWithdrawnEvent thay vì MoneyDepositedEvent
            _bus.Subscribe<MoneyWithdrawnEvent>(HandleWithdrawn);
            _bus.Subscribe<TransferMoneyEvent>(HandleTransfer);
        }

        private void HandleWithdrawn(MoneyWithdrawnEvent @event)
        {
            _totalWithdrawn += @event.Amount;
            Console.WriteLine($"{DateTime.Now} [AnalyticsService]  Total Withdrawn: {_totalWithdrawn}");
        }

        private void HandleDeposit(MoneyDepositedEvent @event)
        {
            _totalDeposited += @event.Amount;
            Console.WriteLine($"{DateTime.Now} [AnalyticsService] Total Deposit: {_totalDeposited}");
        }

        private void HandleTransfer(TransferMoneyEvent @event)
        {
            _totalTransferred += @event.Amount;
            Console.WriteLine($"{DateTime.Now} [AnalyticsService] Total Transferred: {_totalTransferred}");
        }
    }
}

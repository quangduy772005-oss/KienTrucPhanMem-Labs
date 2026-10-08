using System;
using System.Collections.Generic;
using EDA.Events;

namespace EDA.Services
{
    public class AccountService
    {
        private readonly IEventBus _bus;
        private readonly Dictionary<string, double> _accounts = new();

        public AccountService(IEventBus eventBus)
        {
            _bus = eventBus;
        }

        public void CreateAccount(string accountId, string owner)
        {
            _accounts[accountId] = 0;
            Console.WriteLine($"{DateTime.Now} [AccountService] Created account {accountId} for {owner}");
            _bus.Publish(new AccountCreatedEvent(accountId, owner));
        }

        public void Deposit(string accountId, double money)
        {
            if (!_accounts.ContainsKey(accountId))
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản {accountId} không tồn tại!");
                return;
            }

            _accounts[accountId] += money;
            Console.WriteLine($"{DateTime.Now} [AccountService] Account deposited {money} to {accountId}. Số dư hiện tại: {_accounts[accountId]}");
            _bus.Publish(new MoneyDepositedEvent(accountId, money));
        }

        public void Withdrawn(string accountId, double money)
        {
            Withdraw(accountId, money);
        }

        public void Withdraw(string accountId, double money)
        {
            if (!_accounts.ContainsKey(accountId))
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản {accountId} không tồn tại!");
                return;
            }

            if (_accounts[accountId] >= money)
            {
                _accounts[accountId] -= money;
                Console.WriteLine($"{DateTime.Now} [AccountService] Account withdrawn {money} from {accountId}. Số dư còn lại: {_accounts[accountId]}");
                _bus.Publish(new MoneyWithdrawnEvent(accountId, money));
            }
            else
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản {accountId} số dư không đủ (Hiện có: {_accounts[accountId]}, yêu cầu: {money})");
            }
        }

        public void Transfer(string fromAccountId, string toAccountId, double money)
        {
            if (!_accounts.ContainsKey(fromAccountId))
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản chuyển {fromAccountId} không tồn tại!");
                return;
            }

            if (!_accounts.ContainsKey(toAccountId))
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản nhận {toAccountId} không tồn tại!");
                return;
            }

            if (_accounts[fromAccountId] >= money)
            {
                _accounts[fromAccountId] -= money;
                _accounts[toAccountId] += money;

                Console.WriteLine($"{DateTime.Now} [AccountService] Transferred {money} from {fromAccountId} to {toAccountId}. (Số dư {fromAccountId}: {_accounts[fromAccountId]}, Số dư {toAccountId}: {_accounts[toAccountId]})");
                _bus.Publish(new TransferMoneyEvent(fromAccountId, toAccountId, money));
            }
            else
            {
                Console.WriteLine($"[AccountService] Error: Tài khoản {fromAccountId} không đủ số dư để chuyển {money} (Hiện có: {_accounts[fromAccountId]})");
            }
        }

        public double GetBalance(string accountId)
        {
            return _accounts.TryGetValue(accountId, out var balance) ? balance : 0;
        }

        public bool HasAccount(string accountId)
        {
            return _accounts.ContainsKey(accountId);
        }

        public IReadOnlyDictionary<string, double> GetAllAccounts()
        {
            return _accounts;
        }
    }
}

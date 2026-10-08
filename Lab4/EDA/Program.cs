using System;
using System.Threading;
using EDA;
using EDA.Services;

internal class Program
{
    private static IEventBus _bus = null!;
    private static AccountService _accountService = null!;
    private static AnalyticsService _analyticsService = null!;
    private static FraudDetectionService _fraudDetectionService = null!;
    private static bool _useRabbitMQ = false;

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        if (args.Length > 0 && args[0].Equals("--rabbitmq", StringComparison.OrdinalIgnoreCase))
        {
            _useRabbitMQ = true;
        }

        InitSystem(_useRabbitMQ);

        bool running = true;
        while (running)
        {
            PrintMenu();
            Console.Write("👉 Vui lòng chọn chức năng (0-7): ");
            var choice = Console.ReadLine()?.Trim();

            Console.WriteLine();
            switch (choice)
            {
                case "1":
                    HandleCreateAccount();
                    break;
                case "2":
                    HandleDeposit();
                    break;
                case "3":
                    HandleWithdraw();
                    break;
                case "4":
                    HandleTransfer();
                    break;
                case "5":
                    HandleViewAccountsAndAnalytics();
                    break;
                case "6":
                    RunAutomatedDemo();
                    break;
                case "7":
                    ToggleEventBus();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("Đang dừng hệ thống...");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Lựa chọn không hợp lệ, vui lòng chọn từ 0 đến 7.");
                    Console.ResetColor();
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
        }

        _bus.Dispose();
        Console.WriteLine("Chương trình đã kết thúc. Cảm ơn bạn!");
    }

    private static void PrintMenu()
    {
        Console.WriteLine("===============================================================");
        Console.WriteLine("     HỆ THỐNG NGÂN HÀNG KIẾN TRÚC HƯỚNG SỰ KIỆN (EDA)          ");
        Console.WriteLine("===============================================================");
        string busName = _useRabbitMQ ? "RabbitMQ (Broker phân tán)" : "In-Memory EventBus";
        Console.WriteLine($"Kênh sự kiện hiện tại: [{busName}]\n");
        Console.WriteLine("1. Tạo tài khoản mới (AccountCreatedEvent)");
        Console.WriteLine("2. Nạp tiền vào tài khoản (MoneyDepositedEvent)");
        Console.WriteLine("3. Rút tiền (MoneyWithdrawnEvent) - [Cảnh báo nếu > 10,000]");
        Console.WriteLine("4. Chuyển tiền từ A sang B (TransferMoneyEvent)");
        Console.WriteLine("5. Xem danh sách tài khoản & thống kê (Analytics)");
        Console.WriteLine("6. Chạy kịch bản demo mẫu tự động");
        Console.WriteLine($"7. Chuyển đổi kênh sự kiện (Hiện tại: {(_useRabbitMQ ? "RabbitMQ" : "InMemory")})");
        Console.WriteLine("0. Thoát");
        Console.WriteLine("===============================================================");
    }

    private static void InitSystem(bool useRabbitMQ)
    {
        _bus?.Dispose();
        _useRabbitMQ = useRabbitMQ;

        if (_useRabbitMQ)
        {
            try
            {
                Console.WriteLine(">> Đang kết nối tới RabbitMQ Broker (localhost:5672)...");
                _bus = new RabbitMQEventBus(hostName: "localhost", exchangeName: "eda.events");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[CẢNH BÁO] Không thể kết nối RabbitMQ: {ex.Message}");
                Console.WriteLine(">> Tự động fallback sang In-Memory EventBus để chạy tiếp...\n");
                Console.ResetColor();
                _useRabbitMQ = false;
                _bus = new InMemoryEventBus();
            }
        }
        else
        {
            _bus = new InMemoryEventBus();
        }

        _accountService = new AccountService(_bus);
        _analyticsService = new AnalyticsService(_bus);
        _fraudDetectionService = new FraudDetectionService(_bus);
    }

    private static void HandleCreateAccount()
    {
        Console.WriteLine("--- [TẠO TÀI KHOẢN MỚI] ---");
        Console.Write("Nhập mã số tài khoản (ví dụ: ACC01, TVD): ");
        string accountId = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(accountId))
        {
            Console.WriteLine("Lỗi: Mã tài khoản không được để trống!");
            return;
        }

        Console.Write("Nhập tên chủ tài khoản (ví dụ: David, Nguyen Van A): ");
        string owner = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(owner))
        {
            Console.WriteLine("Lỗi: Tên chủ tài khoản không được để trống!");
            return;
        }

        _accountService.CreateAccount(accountId, owner);
        WaitIfRabbitMQ();
    }

    private static void HandleDeposit()
    {
        Console.WriteLine("--- [NẠP TIỀN VÀO TÀI KHOẢN] ---");
        Console.Write("Nhập mã tài khoản cần nạp: ");
        string accountId = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Nhập số tiền cần nạp: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Lỗi: Số tiền nạp phải là số dương hợp lệ!");
            return;
        }

        _accountService.Deposit(accountId, amount);
        WaitIfRabbitMQ();
    }

    private static void HandleWithdraw()
    {
        Console.WriteLine("--- [RÚT TIỀN] ---");
        Console.WriteLine("(Ghi chú: Nếu rút > 10,000, FraudDetectionService sẽ phát hiện và cảnh báo)");
        Console.Write("Nhập mã tài khoản cần rút: ");
        string accountId = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Nhập số tiền cần rút: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Lỗi: Số tiền rút phải là số dương hợp lệ!");
            return;
        }

        _accountService.Withdraw(accountId, amount);
        WaitIfRabbitMQ();
    }

    private static void HandleTransfer()
    {
        Console.WriteLine("--- [CHUYỂN TIỀN TỪ A SANG B] ---");
        Console.Write("Nhập mã tài khoản nguồn (A): ");
        string fromId = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Nhập mã tài khoản đích (B): ");
        string toId = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Nhập số tiền chuyển: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Lỗi: Số tiền chuyển phải là số dương hợp lệ!");
            return;
        }

        _accountService.Transfer(fromId, toId, amount);
        WaitIfRabbitMQ();
    }

    private static void HandleViewAccountsAndAnalytics()
    {
        Console.WriteLine("--- [DANH SÁCH TÀI KHOẢN HIỆN CÓ] ---");
        var accounts = _accountService.GetAllAccounts();
        if (accounts.Count == 0)
        {
            Console.WriteLine("(Chưa có tài khoản nào được tạo trong phiên này)");
        }
        else
        {
            foreach (var acc in accounts)
            {
                Console.WriteLine($"• Mã tài khoản: {acc.Key,-10} | Số dư: {acc.Value:N0} VNĐ");
            }
        }

        Console.WriteLine("\n--- [THỐNG KÊ TOÀN HỆ THỐNG (ANALYTICS SERVICE)] ---");
        Console.WriteLine($"• Tổng số tiền đã nạp  (Total Deposited)  : {_analyticsService.TotalDeposited:N0} VNĐ");
        Console.WriteLine($"• Tổng số tiền đã rút  (Total Withdrawn)  : {_analyticsService.TotalWithdrawn:N0} VNĐ");
        Console.WriteLine($"• Tổng tiền đã chuyển (Total Transferred): {_analyticsService.TotalTransferred:N0} VNĐ");
    }

    private static void RunAutomatedDemo()
    {
        Console.WriteLine(">>> ĐANG CHẠY KỊCH BẢN DEMO TỰ ĐỘNG <<<\n");

        Console.WriteLine("1. Tạo 2 tài khoản: 'TVD' và 'BOB'");
        _accountService.CreateAccount("TVD", "David");
        _accountService.CreateAccount("BOB", "Bob Smith");
        WaitIfRabbitMQ();

        Console.WriteLine("\n2. Nạp tiền:");
        _accountService.Deposit("TVD", 100000);
        _accountService.Deposit("BOB", 20000);
        WaitIfRabbitMQ();

        Console.WriteLine("\n3. Rút tiền bình thường (5,000 <= 10,000):");
        _accountService.Withdraw("TVD", 5000);
        WaitIfRabbitMQ();

        Console.WriteLine("\n4. Rút tiền bất thường (30,000 > 10,000) -> Kích hoạt cảnh báo FraudDetectionService:");
        _accountService.Withdraw("TVD", 30000);
        WaitIfRabbitMQ();

        Console.WriteLine("\n5. Chuyển tiền từ TVD sang BOB (25,000) -> Kích hoạt TransferMoneyEvent:");
        _accountService.Transfer("TVD", "BOB", 25000);
        WaitIfRabbitMQ();

        Console.WriteLine("\n>>> KỊCH BẢN DEMO TỰ ĐỘNG HOÀN TẤT <<<");
    }

    private static void ToggleEventBus()
    {
        bool newMode = !_useRabbitMQ;
        Console.WriteLine($"\n>> Đang chuyển đổi sang {(newMode ? "RabbitMQ" : "InMemory")}...");
        InitSystem(newMode);
        Console.WriteLine($">> Đã chuyển thành công sang: {(_useRabbitMQ ? "RabbitMQ Broker" : "In-Memory EventBus")}");
    }

    private static void WaitIfRabbitMQ()
    {
        if (_useRabbitMQ)
        {
            Thread.Sleep(300); // Đảm bảo consumer in log kịp thời
        }
    }
}
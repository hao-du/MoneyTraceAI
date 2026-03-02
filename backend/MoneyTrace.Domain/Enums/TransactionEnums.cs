namespace MoneyTrace.Domain.Enums;

public enum InterestPeriod
{
    PerMonth = 1,
    PerYear = 2
}

public enum TransferType
{
    BorrowFrom = 1,
    LendTo = 2,
    GiveTo = 3,
    ReceiveFrom = 4
}

public enum TransferStatus
{
    Pending = 1,
    Failed = 2,
    Close = 3
}

public enum TransactionType
{
    Cashflow = 1,
    Bank = 2,
    Exchange = 3,
    Transfer = 4
}

using Game.Application.Shared.Cqrs;

namespace Game.Application.Game.Commands;

public class SpendMoneyCommand : ICommand<SpendMoneyResult>
{
    public int Amount { get; }
    public string Reason { get; }

    public SpendMoneyCommand(int amount, string reason = null)
    {
        Amount = amount;
        Reason = reason ?? "Unknown";
    }
}

public class SpendMoneyResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public int RemainingMoney { get; }

    public SpendMoneyResult(bool success, int remainingMoney, string errorMessage = null)
    {
        Success = success;
        RemainingMoney = remainingMoney;
        ErrorMessage = errorMessage;
    }

    public static SpendMoneyResult Successful(int remainingMoney)
    {
        return new SpendMoneyResult(true, remainingMoney);
    }

    public static SpendMoneyResult Failed(string errorMessage, int currentMoney)
    {
        return new SpendMoneyResult(false, currentMoney, errorMessage);
    }
}

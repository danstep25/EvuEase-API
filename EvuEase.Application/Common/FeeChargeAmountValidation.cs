namespace EvuEase.Application.Common;

public static class FeeChargeAmountValidation
{
    public static void EnsureNonNegative(decimal amount, string fieldLabel)
    {
        if (amount < 0)
        {
            throw new InvalidOperationException($"{fieldLabel} cannot be negative.");
        }
    }

    public static void EnsureFeeAmounts(decimal cash, decimal lowMonthlyPayment)
    {
        EnsureNonNegative(cash, "Cash amount");
        EnsureNonNegative(lowMonthlyPayment, "Low monthly payment");
    }

    public static void EnsureDownpaymentPercent(decimal downpaymentPercent)
    {
        EnsureNonNegative(downpaymentPercent, "Downpayment percent");

        if (downpaymentPercent > 100)
        {
            throw new InvalidOperationException("Downpayment percent cannot exceed 100.");
        }
    }
}

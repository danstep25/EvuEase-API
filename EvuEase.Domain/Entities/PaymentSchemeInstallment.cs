namespace EvuEase.Domain.Entities;

public class PaymentSchemeInstallment : BaseEntity
{
    public long id { get; private set; }
    public long payment_scheme_id { get; private set; }
    public int installment_order { get; private set; }
    public string? payment_name { get; private set; }
    public DateTime due_date { get; private set; }

    private PaymentSchemeInstallment() { }

    public static PaymentSchemeInstallment Create(
        long paymentSchemeId,
        int installmentOrder,
        string? paymentName,
        DateTime dueDate)
    {
        var installment = new PaymentSchemeInstallment();
        var type = typeof(PaymentSchemeInstallment);

        type.GetProperty(nameof(payment_scheme_id))?.SetValue(installment, paymentSchemeId);
        type.GetProperty(nameof(installment_order))?.SetValue(installment, installmentOrder);
        type.GetProperty(nameof(payment_name))?.SetValue(installment, paymentName);
        type.GetProperty(nameof(due_date))?.SetValue(installment, dueDate.Date);
        type.GetProperty(nameof(status))?.SetValue(installment, true);
        type.GetProperty(nameof(created_at))?.SetValue(installment, DateTime.Now);

        return installment;
    }

    public void Update(int installmentOrder, string? paymentName, DateTime dueDate)
    {
        var type = typeof(PaymentSchemeInstallment);

        type.GetProperty(nameof(installment_order))?.SetValue(this, installmentOrder);
        type.GetProperty(nameof(payment_name))?.SetValue(this, paymentName);
        type.GetProperty(nameof(due_date))?.SetValue(this, dueDate.Date);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

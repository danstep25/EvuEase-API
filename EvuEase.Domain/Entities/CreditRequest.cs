namespace EvuEase.Domain.Entities;

public class CreditRequest : BaseEntity
{
    public long id { get; private set; }
    public string credit_request_no { get; private set; } = string.Empty;
    public long? student_id { get; private set; }
    public string student_number { get; private set; } = string.Empty;
    public string first_name { get; private set; } = string.Empty;
    public string? middle_name { get; private set; }
    public string last_name { get; private set; } = string.Empty;
    public long program_id { get; private set; }
    public long sy_id { get; private set; }
    public string request_status { get; private set; } = "Pending";
    public string? signed_pdf_file_name { get; private set; }
    public string? signed_pdf_storage_key { get; private set; }

    private CreditRequest() { }

    public static CreditRequest Create(
        string studentNumber,
        string firstName,
        string? middleName,
        string lastName,
        long? studentId,
        long programId,
        long syId)
    {
        var entity = new CreditRequest();
        var type = typeof(CreditRequest);

        type.GetProperty(nameof(student_number))?.SetValue(entity, studentNumber);
        type.GetProperty(nameof(first_name))?.SetValue(entity, firstName);
        type.GetProperty(nameof(middle_name))?.SetValue(entity, middleName);
        type.GetProperty(nameof(last_name))?.SetValue(entity, lastName);
        type.GetProperty(nameof(student_id))?.SetValue(entity, studentId);
        type.GetProperty(nameof(program_id))?.SetValue(entity, programId);
        type.GetProperty(nameof(sy_id))?.SetValue(entity, syId);
        type.GetProperty(nameof(request_status))?.SetValue(entity, "Pending");
        type.GetProperty(nameof(credit_request_no))?.SetValue(entity, "PENDING");
        type.GetProperty(nameof(status))?.SetValue(entity, true);
        type.GetProperty(nameof(created_at))?.SetValue(entity, DateTime.Now);

        return entity;
    }

    public void SetCreditRequestNo(string creditRequestNo)
    {
        typeof(CreditRequest).GetProperty(nameof(credit_request_no))?.SetValue(this, creditRequestNo);
        typeof(CreditRequest).GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }

    public void SetRequestStatus(string requestStatus)
    {
        typeof(CreditRequest).GetProperty(nameof(request_status))?.SetValue(this, requestStatus);
        typeof(CreditRequest).GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }

    public void SetSignedPdf(string? fileName, string? storageKey)
    {
        typeof(CreditRequest).GetProperty(nameof(signed_pdf_file_name))?.SetValue(this, fileName);
        typeof(CreditRequest).GetProperty(nameof(signed_pdf_storage_key))?.SetValue(this, storageKey);
        typeof(CreditRequest).GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }

    public void SetStudentId(long? studentId)
    {
        typeof(CreditRequest).GetProperty(nameof(student_id))?.SetValue(this, studentId);
        typeof(CreditRequest).GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

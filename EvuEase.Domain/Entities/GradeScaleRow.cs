namespace EvuEase.Domain.Entities;

public class GradeScaleRow : BaseEntity
{
    public long id { get; private set; }

    public string academic_term_key { get; private set; } = string.Empty;

    public decimal mark { get; private set; }

    public decimal grade { get; private set; }

    public int sort_order { get; private set; }

    private GradeScaleRow() { }

    public static GradeScaleRow Create(string academicTermKey, decimal mark, decimal grade, int sortOrder)
    {
        var row = new GradeScaleRow();
        var t = typeof(GradeScaleRow);
        t.GetProperty(nameof(academic_term_key))?.SetValue(row, academicTermKey);
        t.GetProperty(nameof(mark))?.SetValue(row, mark);
        t.GetProperty(nameof(grade))?.SetValue(row, grade);
        t.GetProperty(nameof(sort_order))?.SetValue(row, sortOrder);
        t.GetProperty(nameof(status))?.SetValue(row, true);
        t.GetProperty(nameof(created_at))?.SetValue(row, DateTime.Now);
        return row;
    }
}

using EvuEase.Domain.Entities;

namespace EvuEase.Application.ClassRoster;





public static class GradeScaleMarkResolver
{
    public static decimal? ResolveOfficialGrade(IReadOnlyList<GradeScaleRow> rows, decimal studentMark)
    {
        if (rows == null || rows.Count == 0)
        {
            return null;
        }

        foreach (var r in rows.OrderByDescending(x => x.mark).ThenByDescending(x => x.grade))
        {
            if (studentMark >= r.mark)
            {
                return r.grade;
            }
        }

        return null;
    }
}

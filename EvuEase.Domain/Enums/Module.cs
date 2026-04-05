using System.ComponentModel;

namespace EvuEase.Domain.Enums
{
    public enum Module
    {
        [Description("User Management")]
        User,

        [Description("Faculty Center")]
        GradingSchemeBasis,

        [Description("Faculty Center")]
        GradeScaleRow,

        [Description("Faculty Center")]
        FacultyClass
    }
}

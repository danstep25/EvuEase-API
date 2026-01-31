using System.ComponentModel;

namespace EvuEase.Domain.Enums
{
    public enum Role
    {

        [Description("Admin")]
        Admin,

        [Description("Evaluator")]
        Evaluator,

        [Description("Registrar")]
        Registrar
    }
}
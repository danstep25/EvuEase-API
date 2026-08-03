using EvuEase.Domain.Enums;

namespace EvuEase.Infrastructure.Common;

public static class ModuleHelper
{
    public static string? GetMatchingModuleDescription(string? moduleString)
    {
        if (string.IsNullOrWhiteSpace(moduleString))
        {
            return null;
        }

        var moduleDescriptions = Enum.GetValues(typeof(Module))
            .Cast<Module>()
            .Select(m => m.GetDescription())
            .ToList();

        return moduleDescriptions
            .FirstOrDefault(desc => desc.Equals(moduleString, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsValidModuleDescription(string? moduleString)
    {
        return GetMatchingModuleDescription(moduleString) != null;
    }

    public static List<string> GetAllModuleDescriptions()
    {
        return Enum.GetValues(typeof(Module))
            .Cast<Module>()
            .Select(m => m.GetDescription())
            .ToList();
    }
}


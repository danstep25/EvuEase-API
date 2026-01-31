using EvuEase.Domain.Enums;

namespace EvuEase.Infrastructure.Common;

public static class ModuleHelper
{
    /// <summary>
    /// Gets the Module enum description that matches the provided module string.
    /// Returns null if no match is found.
    /// </summary>
    /// <param name="moduleString">The module string to match against enum descriptions</param>
    /// <returns>The matching module description, or null if no match found</returns>
    public static string? GetMatchingModuleDescription(string? moduleString)
    {
        if (string.IsNullOrWhiteSpace(moduleString))
        {
            return null;
        }

        // Get all Module enum descriptions
        var moduleDescriptions = Enum.GetValues(typeof(Module))
            .Cast<Module>()
            .Select(m => m.GetDescription())
            .ToList();

        // Check if the provided module matches any enum description (case-insensitive)
        return moduleDescriptions
            .FirstOrDefault(desc => desc.Equals(moduleString, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if the provided module string matches any Module enum description.
    /// </summary>
    /// <param name="moduleString">The module string to check</param>
    /// <returns>True if a match is found, false otherwise</returns>
    public static bool IsValidModuleDescription(string? moduleString)
    {
        return GetMatchingModuleDescription(moduleString) != null;
    }

    /// <summary>
    /// Gets all Module enum descriptions.
    /// </summary>
    /// <returns>List of all module descriptions</returns>
    public static List<string> GetAllModuleDescriptions()
    {
        return Enum.GetValues(typeof(Module))
            .Cast<Module>()
            .Select(m => m.GetDescription())
            .ToList();
    }
}


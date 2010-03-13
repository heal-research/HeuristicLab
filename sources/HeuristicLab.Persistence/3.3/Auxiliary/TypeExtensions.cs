using System;
using System.Text;

namespace HeuristicLab.Persistence.Auxiliary {

  /// <summary>
  /// Extension methods for the <see cref="Type"/> class.
  /// </summary>
  public static class TypeExtensions {

    /// <summary>
    /// Get an assembly qualified name withough version information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A full type name without version information.</returns>
    public static string VersionInvariantName(this Type type) {
      return TypeNameParser.Parse(type.AssemblyQualifiedName).ToString(false);
    }

  }

}
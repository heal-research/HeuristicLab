using System;
using System.Text;

namespace HeuristicLab.Persistence.Auxiliary {

  public static class TypeExtensions {

    public static string VersionInvariantName(this Type type) {
      return TypeNameParser.Parse(type.AssemblyQualifiedName).ToString(false);
    }

  }

}
using System;
using System.Text;

namespace HeuristicLab.Persistence.Auxiliary {

  public static class TypeExtensions {

    public static string VersionInvariantName(this Type type) {
      StringBuilder sb = new StringBuilder();
      TypeStringBuilder.BuildVersionInvariantName(type, sb);
      return sb.ToString();
    }

  }

}
using System;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  public static class TypeExtensions {

    public static string VersionInvariantName(this Type type) {
      StringBuilder sb = new StringBuilder();
      TypeStringBuilder.BuildVersionInvariantName(type, sb);
      return sb.ToString();
    }

  }

}
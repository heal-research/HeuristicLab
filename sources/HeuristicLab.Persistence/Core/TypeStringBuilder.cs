using System;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  public static class TypeStringBuilder {

    private static void BuildVersionInvariantName(Type type, StringBuilder sb) {    
      sb.Append(type.Namespace);
      sb.Append('.');
      sb.Append(type.Name);
      if (type.IsGenericType) {
        sb.Append("[");
        Type[] args = type.GetGenericArguments();
        for ( int i = 0; i<args.Length; i++ ) {
          sb.Append("[");
          BuildVersionInvariantName(args[i], sb);
          sb.Append("]");
          sb.Append(",");
        }
        if (args.Length > 0)
          sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
      }            
      sb.Append(", ");
      sb.Append(type.Assembly.GetName().Name);      
    }

    public static string VersionInvariantName(this Type type) {
      StringBuilder sb = new StringBuilder();
      BuildVersionInvariantName(type, sb);
      return sb.ToString();
    }
  }  
  
}
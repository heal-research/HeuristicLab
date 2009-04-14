using System;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  internal static class TypeStringBuilder {

    internal static void BuildDeclaringTypeChain(Type type, StringBuilder sb) {
      if ( type.DeclaringType != null ) {
        BuildDeclaringTypeChain(type.DeclaringType, sb);
        sb.Append(type.DeclaringType.Name).Append('+');
      }
    }

    internal static void BuildVersionInvariantName(Type type, StringBuilder sb) {
      sb.Append(type.Namespace).Append('.');
      BuildDeclaringTypeChain(type, sb);
      sb.Append(type.Name);
      if (type.IsGenericType) {
        sb.Append("[");
        Type[] args = type.GetGenericArguments();
        for ( int i = 0; i<args.Length; i++ ) {
          sb.Append("[");
          BuildVersionInvariantName(args[i], sb);
          sb.Append("],");
        }
        if (args.Length > 0)
          sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
      }            
      sb.Append(", ").Append(type.Assembly.GetName().Name);
    }

  }
  
}
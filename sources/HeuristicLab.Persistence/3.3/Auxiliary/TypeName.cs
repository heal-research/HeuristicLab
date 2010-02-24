using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Auxiliary {
  
  public class TypeName {
    public string Namespace { get; private set; }
    public string ClassName { get; private set; }
    public List<TypeName> GenericArgs { get; internal set; }
    public bool IsGeneric { get { return GenericArgs.Count > 0; } }
    public string MemoryMagic { get; internal set; }
    public string AssemblyName { get; internal set; }
    public Dictionary<string, string> AssemblyAttribues { get; internal set; }
    public bool IsReference { get; internal set; }

    internal TypeName(string nameSpace, string className) {
      Namespace = nameSpace;
      ClassName = className;
      GenericArgs = new List<TypeName>();
      MemoryMagic = "";
      AssemblyAttribues = new Dictionary<string, string>();
    }

    public string ToString(bool full) {
      return ToString(full, true);
    }

    public string ToString(bool full, bool includeAssembly) {
      StringBuilder sb = new StringBuilder();
      if (!string.IsNullOrEmpty(Namespace))
        sb.Append(Namespace).Append('.');
      sb.Append(ClassName);
      if (IsGeneric) {
        sb.Append('`').Append(GenericArgs.Count).Append('[');
        bool first = true;
        foreach (TypeName t in GenericArgs) {
          if (first)
            first = false;
          else
            sb.Append(',');
          sb.Append('[').Append(t.ToString(full)).Append(']');
        }
        sb.Append(']');
      }
      sb.Append(MemoryMagic);
      if (includeAssembly && AssemblyName != null) {
        sb.Append(", ").Append(AssemblyName);
        if (full)
          foreach (var property in AssemblyAttribues)
            sb.Append(", ").Append(property.Key).Append('=').Append(property.Value);
      }
      return sb.ToString();
    }

    public override string ToString() {
      return ToString(true);
    }

    public bool IsNewerThan(TypeName t) {
      try {
        if (this.ClassName != t.ClassName ||
          this.Namespace != t.Namespace ||
          this.AssemblyName != t.AssemblyName)
          throw new Exception("Cannot compare versions of different types");
        if (CompareVersions(
          this.AssemblyAttribues["Version"],
          t.AssemblyAttribues["Version"]) > 0)
          return true;
        IEnumerator<TypeName> thisIt = this.GenericArgs.GetEnumerator();
        IEnumerator<TypeName> tIt = t.GenericArgs.GetEnumerator();
        while (thisIt.MoveNext()) {
          tIt.MoveNext();
          if (thisIt.Current.IsNewerThan(tIt.Current))
            return true;
        }
        return false;
      } catch (KeyNotFoundException) {
        throw new Exception("Could not extract version information from type string");
      }
    }

    public bool IsCompatible(TypeName t) {
      try {
        if (this.ClassName != t.ClassName ||
          this.Namespace != t.Namespace ||
          this.AssemblyName != t.AssemblyName)
          throw new Exception("Cannot compare versions of different types");
        Version thisVersion = new Version(this.AssemblyAttribues["Version"]);
        Version tVersion = new Version(t.AssemblyAttribues["Version"]);
        if (thisVersion.Major != tVersion.Major ||
          thisVersion.Minor != tVersion.Minor)
          return false;
        IEnumerator<TypeName> thisIt = this.GenericArgs.GetEnumerator();
        IEnumerator<TypeName> tIt = t.GenericArgs.GetEnumerator();
        while (thisIt.MoveNext()) {
          tIt.MoveNext();
          if (!thisIt.Current.IsCompatible(tIt.Current))
            return false;
        }
        return true;
      } catch (KeyNotFoundException) {
        throw new Exception("Could not extract version infomration from type string");
      }
    }

    private static int CompareVersions(string v1string, string v2string) {
      return new Version(v1string).CompareTo(new Version(v2string));
    }
  }
}
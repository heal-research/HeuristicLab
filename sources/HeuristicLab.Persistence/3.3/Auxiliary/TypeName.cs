using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Auxiliary {
  
  /// <summary>
  /// Contains a more modular representation of type names that can
  /// be used to compare versions and ignore extended assembly
  /// attributes.
  /// </summary>
  [StorableClass(StorableClassType.MarkedOnly)]
  public class TypeName {

    [Storable]
    public string Namespace { get; private set; }

    [Storable]
    public string ClassName { get; private set; }

    [Storable]
    public List<TypeName> GenericArgs { get; internal set; }
    public bool IsGeneric { get { return GenericArgs.Count > 0; } }

    [Storable]
    public string MemoryMagic { get; internal set; }

    [Storable]
    public string AssemblyName { get; internal set; }

    [Storable]
    public Dictionary<string, string> AssemblyAttribues { get; internal set; }

    [Storable]
    public bool IsReference { get; internal set; }

    internal TypeName(string nameSpace, string className) {
      Namespace = nameSpace;
      ClassName = className;
      GenericArgs = new List<TypeName>();
      MemoryMagic = "";
      AssemblyAttribues = new Dictionary<string, string>();
    }

    /// <param name="full">include assembly properties and generic parameters</param>    
    public string ToString(bool full) {
      return ToString(full, true);
    }


    /// <param name="full">include assembly properties and generic parameters</param>    
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


    /// <summary>
    /// Lexicographically compare version information and make sure type and assembly
    /// names are identical. This function recursively checks generic type arguments.
    /// </summary>    
    public bool IsNewerThan(TypeName typeName) {
      try {
        if (this.ClassName != typeName.ClassName ||
          this.Namespace != typeName.Namespace ||
          this.AssemblyName != typeName.AssemblyName)
          throw new Exception("Cannot compare versions of different types");
        if (CompareVersions(
          this.AssemblyAttribues["Version"],
          typeName.AssemblyAttribues["Version"]) > 0)
          return true;
        IEnumerator<TypeName> thisIt = this.GenericArgs.GetEnumerator();
        IEnumerator<TypeName> tIt = typeName.GenericArgs.GetEnumerator();
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


    /// <summary>
    /// Make sure major and minor version number are identical. This function
    /// recursively checks generic type arguments.
    /// </summary>
    public bool IsCompatible(TypeName typeName) {
      try {
        if (this.ClassName != typeName.ClassName ||
          this.Namespace != typeName.Namespace ||
          this.AssemblyName != typeName.AssemblyName)
          throw new Exception("Cannot compare versions of different types");
        Version thisVersion = new Version(this.AssemblyAttribues["Version"]);
        Version tVersion = new Version(typeName.AssemblyAttribues["Version"]);
        if (thisVersion.Major != tVersion.Major ||
          thisVersion.Minor != tVersion.Minor)
          return false;
        IEnumerator<TypeName> thisIt = this.GenericArgs.GetEnumerator();
        IEnumerator<TypeName> tIt = typeName.GenericArgs.GetEnumerator();
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
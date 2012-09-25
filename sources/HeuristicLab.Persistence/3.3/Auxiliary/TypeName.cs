#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Auxiliary {

  /// <summary>
  /// Contains a more modular representation of type names that can
  /// be used to compare versions and ignore extended assembly
  /// attributes.
  /// </summary>
  [StorableClass]
  public class TypeName {

    /// <summary>
    /// Gets or sets the namespace.
    /// </summary>
    /// <value>The namespace.</value>
    [Storable]
    public string Namespace { get; private set; }

    /// <summary>
    /// Gets or sets the name of the class.
    /// </summary>
    /// <value>The name of the class.</value>
    [Storable]
    public string ClassName { get; private set; }

    /// <summary>
    /// Gets or sets the generic args.
    /// </summary>
    /// <value>The generic args.</value>
    [Storable]
    public List<TypeName> GenericArgs { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this instance is generic.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is generic; otherwise, <c>false</c>.
    /// </value>
    public bool IsGeneric { get { return GenericArgs.Count > 0; } }

    /// <summary>
    /// Gets or sets the memory magic (point or array declaration).
    /// </summary>
    /// <value>The memory magic.</value>
    [Storable]
    public string MemoryMagic { get; internal set; }

    /// <summary>
    /// Gets or sets the name of the assembly.
    /// </summary>
    /// <value>The name of the assembly.</value>
    [Storable]
    public string AssemblyName { get; internal set; }

    /// <summary>
    /// Gets or sets the assembly attribues.
    /// </summary>
    /// <value>The assembly attribues.</value>
    [Storable]
    public Dictionary<string, string> AssemblyAttribues { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is reference.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is reference; otherwise, <c>false</c>.
    /// </value>
    [Storable]
    public bool IsReference { get; internal set; }


    [StorableConstructor]
    protected TypeName(bool deserializing) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeName"/> class.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="className">Name of the class.</param>
    internal TypeName(string nameSpace, string className) {
      Namespace = nameSpace;
      ClassName = className;
      GenericArgs = new List<TypeName>();
      MemoryMagic = "";
      AssemblyAttribues = new Dictionary<string, string>();
    }

    internal TypeName(TypeName typeName, string className = null, string nameSpace = null) {
      Namespace = typeName.Namespace;
      ClassName = typeName.ClassName;
      GenericArgs = new List<TypeName>(typeName.GenericArgs);
      AssemblyAttribues = new Dictionary<string, string>(typeName.AssemblyAttribues);
      MemoryMagic = typeName.MemoryMagic;
      AssemblyName = typeName.AssemblyName;
      IsReference = typeName.IsReference;
      if (nameSpace != null)
        Namespace = nameSpace;
      if (className != null)
        ClassName = className;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <param name="full">if set to <c>true</c> includes full information
    /// about generic parameters and assembly properties.</param>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public string ToString(bool full) {
      return ToString(full, true);
    }


    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <param name="full">if set to <c>true</c> includes full information
    /// about generic parameters and assembly properties.</param>
    /// <param name="includeAssembly">if set to <c>true</c> include assembly properties and generic parameters.</param>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
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

    public string GetTypeNameInCode(HashSet<string> omitNamespaces) {
      StringBuilder sb = new StringBuilder();
      if (!string.IsNullOrEmpty(Namespace) && omitNamespaces == null || !omitNamespaces.Contains(Namespace))
        sb.Append(Namespace).Append('.');
      sb.Append(ClassName);
      if (IsGeneric) {
        sb.Append("<");
        sb.Append(
          string.Join(", ",
            GenericArgs
              .Select(a => a.GetTypeNameInCode(omitNamespaces))
              .ToArray()));
        sb.Append(">");
      }
      sb.Append(MemoryMagic);
      return sb.ToString();
    }

    public string GetTypeNameInCode(bool includeAllNamespaces) {
      StringBuilder sb = new StringBuilder();
      if (includeAllNamespaces)
        sb.Append(Namespace).Append('.');
      sb.Append(ClassName);
      if (IsGeneric) {
        sb.Append("<");
        sb.Append(
          string.Join(", ",
            GenericArgs
              .Select(a => a.GetTypeNameInCode(includeAllNamespaces))
              .ToArray()));
        sb.Append(">");
      }
      sb.Append(MemoryMagic);
      return sb.ToString();
    }


    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      return ToString(true);
    }


    /// <summary>
    /// Lexicographically compare version information and make sure type and assembly
    /// names are identical. This function recursively checks generic type arguments.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>
    /// 	<c>true</c> if is newer than the specified type name; otherwise, <c>false</c>.
    /// </returns>
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
      }
      catch (KeyNotFoundException) {
        throw new Exception("Could not extract version information from type string");
      }
    }


    /// <summary>
    /// Make sure major and minor version number are identical. This function
    /// recursively checks generic type arguments.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>
    /// 	<c>true</c> if the specified type names are compatible; otherwise, <c>false</c>.
    /// </returns>
    public bool IsCompatible(TypeName typeName) {
      try {
        if (this.ClassName != typeName.ClassName ||
          this.Namespace != typeName.Namespace ||
          this.AssemblyName != typeName.AssemblyName)
          throw new Exception("Cannot compare versions of different types");
        Version thisVersion = new Version(this.AssemblyAttribues["Version"]);
        Version tVersion = new Version(typeName.AssemblyAttribues["Version"]);
        if (this.AssemblyName == "mscorlib" &&
          (thisVersion.Major == 2 || thisVersion.Major == 4) &&
          (tVersion.Major == 2 || tVersion.Major == 4)) {
          // skip version check
        } else if (thisVersion.Major != tVersion.Major ||
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
      }
      catch (KeyNotFoundException) {
        throw new Exception("Could not extract version infomration from type string");
      }
    }

    private static int CompareVersions(string v1string, string v2string) {
      return new Version(v1string).CompareTo(new Version(v2string));
    }
  }
}
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Core {
  /// <summary>
  /// Static class for serializing and deserializing objects.
  /// </summary>
  public static class PersistenceManager {
    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified file through creating an 
    /// <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="filename">The name of the file where the <paramref name="object"/> should be saved.</param>
    public static void Save(IItem instance, string filename) {
      XmlGenerator.Serialize(instance, filename, 0);
    }

    public static void SaveCompressed(IItem instance, string filename) {
      XmlGenerator.Serialize(instance, filename, 9);
    }

    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified <paramref name="stream"/> 
    /// through creating an <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="stream">The (file) stream where the object should be saved.</param>
    public static void Save(IItem instance, Stream stream) {
      XmlGenerator.Serialize(instance, stream, ConfigurationService.Instance.GetConfiguration(new XmlFormat()));      
    }
    /// <summary>
    /// Loads an object from a file with the specified <paramref name="filename"/>.
    /// </summary>
    /// <remarks>The object must be saved as an <see cref="XmlDocument"/>. <br/>
    /// Calls <see cref="Restore"/>.</remarks>
    /// <param name="filename">The filename of the file where the data is saved.</param>
    /// <returns>The loaded object.</returns>
    public static IItem Load(string filename) {
      return (IItem)XmlParser.Deserialize(filename);
    }
    /// <summary>
    /// Loads an object from the specified <paramref name="stream"/>.
    /// </summary>
    /// <remarks>The object must be saved as an <see cref="XmlDocument"/>. <br/>
    /// Calls <see cref="Restore"/>.</remarks>
    /// <param name="stream">The stream from where to load the data.</param>
    /// <returns>The loaded object.</returns>
    public static IItem Load(Stream stream) {
      return (IItem)XmlParser.Deserialize(stream);
    }

    /// <summary>
    /// Builds a meaningful string for the given <paramref name="type"/> with the namespace information, 
    /// all its arguments, the assembly name...
    /// </summary>
    /// <param name="type">The type for which a string should be created.</param>
    /// <returns>A string value of this type containing different additional information.</returns>
    public static string BuildTypeString(Type type) {
      string assembly = type.Assembly.FullName;
      assembly = assembly.Substring(0, assembly.IndexOf(", "));

      StringBuilder builder = new StringBuilder();
      builder.Append(type.Namespace);
      builder.Append(".");
      builder.Append(type.Name);
      Type[] args = type.GetGenericArguments();
      if (args.Length > 0) {
        builder.Append("[[");
        builder.Append(BuildTypeString(args[0]));
        builder.Append("]");
        for (int i = 1; i < args.Length; i++) {
          builder.Append(",[");
          builder.Append(BuildTypeString(args[i]));
          builder.Append("]");
        }
        builder.Append("]");
      }
      builder.Append(", ");
      builder.Append(assembly);
      return builder.ToString();
    }
  }
}

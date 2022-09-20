#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure {
  public static class AssemblyHelpers {
    //Based on the code from http://stackoverflow.com/a/7156425
    public static string GetCustomAttributeValue<T>(Assembly assembly, string propertyName)
       where T : Attribute {
      if (assembly == null || string.IsNullOrEmpty(propertyName)) {
        throw new ArgumentException("Arguments are not allowed to be null.");
      }

      object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
      if (attributes.Length == 0) {
        throw new InvalidOperationException(string.Format("No attributes of type {0} found in assembly {1}", typeof(T).Name,
          assembly.FullName));
      }

      var attribute = (T)attributes[0];
      var propertyInfo = attribute.GetType().GetProperty(propertyName);
      if (propertyInfo == null) {
        throw new InvalidOperationException(string.Format("No property {0} found in attribute {1}, assembly: {2}",
          propertyName, typeof(T).Name, assembly.FullName));
      }

      var value = propertyInfo.GetValue(attribute, null);
      return value.ToString();
    }

    public static string GetFileVersion(Assembly assembly) {
      return GetCustomAttributeValue<AssemblyFileVersionAttribute>(assembly, "Version");
    }
  }
}

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

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// ContactInformationAttribute can be used to declare contact information (name, e-mail) for a plugin.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class ContactInformationAttribute : System.Attribute {
    private string name;

    /// <summary>
    /// Gets the name of the contact person.
    /// </summary>
    public string Name {
      get { return name; }
    }

    private string email;
    /// <summary>
    /// Gets the e-mail address of the contact person.
    /// </summary>
    public string EMail {
      get { return email; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ContactInformationAttribute"/>.
    /// <param name="name">Name of the contact person</param>
    /// <param name="email">E-mail address of the person</param>
    /// </summary>
    public ContactInformationAttribute(string name, string email) {
      if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email)) throw new ArgumentException("Empty name or e-mail address.");
      this.name = name;
      this.email = email;
    }
  }
}

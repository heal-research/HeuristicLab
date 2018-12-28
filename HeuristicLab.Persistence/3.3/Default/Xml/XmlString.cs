#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Text;
using HEAL.Fossil;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// XML friendly encapsulation of string data.
  /// </summary>
  [StorableClass]
  public class XmlString : ISerialData {

    /// <summary>
    /// Gets the XML string data. Essentially marks the string as
    /// XML compatible string.
    /// </summary>
    /// <value>The XML string data.</value>
    [Storable]
    public string Data { get; private set; }

    [StorableConstructor]
    protected XmlString(StorableConstructorFlag _) { }
    private XmlString() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlString"/> class.
    /// </summary>
    /// <param name="data">The xml data.</param>
    public XmlString(string data) {
      Data = data;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("XmlString(").Append(Data).Append(')');
      return sb.ToString();
    }
  }
}
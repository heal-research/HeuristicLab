#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// A simple XML format, that can be used to either stream
  /// or save to a file.
  /// </summary>
  [StorableClass]
  public class XmlFormat : FormatBase<XmlString> {
    /// <summary>
    /// Gets the format's name.
    /// </summary>
    /// <value>The format's name.</value>
    public override string Name { get { return "XML"; } }

    [StorableConstructor]
    protected XmlFormat(bool deserializing) : base(deserializing) { }
    public XmlFormat() { }
  }

}
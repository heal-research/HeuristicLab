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

using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.Xml;
using HEAL.Attic;
using System;

namespace HeuristicLab.Core {
  public class PersistenceContentManager : ContentManager {
    public PersistenceContentManager() : base() { }

    protected override IStorableContent LoadContent(string filename) {
      // first try to load using the new persistence format
      var ser = new ProtoBufSerializer();
      try {
        return (IStorableContent)ser.Deserialize(filename, out SerializationInfo info);
      } catch (Exception e) {
        try {
          // try old format if new format fails
          return XmlParser.Deserialize<IStorableContent>(filename);
        } catch (Exception e2) {
          throw new AggregateException($"Cannot open file {filename}", e, e2);
        }
      }
    }

    protected override void SaveContent(IStorableContent content, string filename, bool compressed, CancellationToken cancellationToken) {
      var ser = new ProtoBufSerializer();
      ser.Serialize(content, filename, cancellationToken);
    }
  }
}

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
using System.Diagnostics;

namespace HeuristicLab.Core {
  public class PersistenceContentManager : ContentManager {
    public PersistenceContentManager() : base() { }

    protected override IStorableContent LoadContent(string filename, out Info info) {
      bool useOldPersistence = XmlParser.CanOpen(filename);
      IStorableContent content = null;
      if (useOldPersistence) {
        var sw = new Stopwatch();
        sw.Start();
        content = XmlParser.Deserialize<IStorableContent>(filename);
        sw.Stop();
        info = new Info(filename, sw.Elapsed);
      } else {
        var ser = new ProtoBufSerializer();
        content = (IStorableContent)ser.Deserialize(filename, out SerializationInfo serInfo);
        info = new Info(filename, serInfo);
      }
      if (content == null) throw new PersistenceException($"Cannot deserialize root element of {filename}");
      return content;
    }


    protected override void SaveContent(IStorableContent content, string filename, bool compressed, CancellationToken cancellationToken) {
      var ser = new ProtoBufSerializer();
      ser.Serialize(content, filename, cancellationToken);
    }
  }
}

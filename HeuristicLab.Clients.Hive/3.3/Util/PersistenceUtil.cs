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

using System;
using System.Collections.Generic;
using System.IO;
using HEAL.Fossil;

namespace HeuristicLab.Clients.Hive {
  public static class PersistenceUtil {
    public static byte[] Serialize(object obj, out IEnumerable<Type> types) {
      using (MemoryStream memStream = new MemoryStream()) {
        throw new NotImplementedException("Not supported by HEAL.Fossil yet."); // TODO
        // XmlGenerator.Serialize(obj, memStream, ConfigurationService.Instance.GetConfiguration(new XmlFormat()), false, out types);
        // byte[] jobByteArray = memStream.ToArray();
        // return jobByteArray;
      }
    }

    public static byte[] Serialize(object obj) {
      var ser = new ProtoBufSerializer();
      return ser.Serialize(obj);
    }

    public static T Deserialize<T>(byte[] sjob) {
      var ser = new ProtoBufSerializer();
      return (T)ser.Deserialize(sjob);
    }
  }
}

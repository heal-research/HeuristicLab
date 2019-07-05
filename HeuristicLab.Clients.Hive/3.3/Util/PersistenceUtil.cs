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
using System.IO.Compression;
using Google.Protobuf;
using HEAL.Attic;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Clients.Hive {
  public static class PersistenceUtil {
    public static byte[] Serialize(object obj, out IEnumerable<Type> types) {
      var ser = new ProtoBufSerializer();
      var bytes = ser.Serialize(obj, out SerializationInfo info);
      types = info.SerializedTypes;
      return bytes;
    }

    public static byte[] Serialize(object obj) {
      var ser = new ProtoBufSerializer();
      return ser.Serialize(obj);
    }

    public static T Deserialize<T>(byte[] sjob) {
      var ser = new ProtoBufSerializer();
      try {
        return (T)ser.Deserialize(sjob);
      } catch (PersistenceException e) {
        // Exceptions may arise because data in Hive was uploaded with former XML-based
        // persistence or with Attic prior to 1.2.
        if (e.InnerException is InvalidDataException) {
          // We assume the data was serialized with HEAL.Attic < 1.2 which did not use
          // DeflateStream, but as of 1.2 uses DeflateStream by default
          var compressedData = Compress(sjob);
          try {
            // retry with the job's data compressed
            return (T)ser.Deserialize(compressedData);
          } catch (PersistenceException e2) {
            if (e2.InnerException is InvalidProtocolBufferException
              || e2.InnerException is InvalidDataException) {
              // retry deserialize original bytes with old persistence
              return DeserializeWithXmlParser<T>(sjob);
            } else throw;
          }
        }
        if (e.InnerException is InvalidProtocolBufferException) {
          // We assume the data was not serialized with HEAL.Attic, but with the former
          // XML-based persistence
          return DeserializeWithXmlParser<T>(sjob);
        } else throw;
      }
    }

    private static byte[] Compress(byte[] sjob) {
      using (var memStream = new MemoryStream(sjob.Length)) {
        using (var deflateStream = new DeflateStream(memStream, CompressionMode.Compress)) {
          deflateStream.Write(sjob, 0, sjob.Length);
        }
        return memStream.ToArray();
      }
    }

    private static T DeserializeWithXmlParser<T>(byte[] sjob) {
      using (MemoryStream memStream = new MemoryStream(sjob)) {
        return XmlParser.Deserialize<T>(memStream);
      }
    }
  }
}

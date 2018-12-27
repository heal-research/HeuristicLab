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
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Default.Xml {


  /// <summary>
  /// Main entry point of persistence to XML. Use the static methods to serialize
  /// to a file or to a stream.
  /// </summary>
  public class ReadableXmlGenerator : XmlGenerator {

    protected Dictionary<int, string> typeCache = new Dictionary<int, string>();

    /// <summary>
    /// Formats the specified begin token.
    /// </summary>
    /// <param name="beginToken">The begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(BeginToken beginToken) {
      var dict = new Dictionary<string, string> {
          {"name", beginToken.Name},
          {"id", beginToken.Id.ToString()}};
      return CreateNodeStart(typeCache[beginToken.TypeId], dict);
    }

    /// <summary>
    /// Formats the specified end token.
    /// </summary>
    /// <param name="endToken">The end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(EndToken endToken) {
      return CreateNodeEnd(typeCache[endToken.TypeId]);
    }

    /// <summary>
    /// Formats the specified data token.
    /// </summary>
    /// <param name="dataToken">The data token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(PrimitiveToken dataToken) {
      var dict = new Dictionary<string, string> {
            {"name", dataToken.Name},
            {"id", dataToken.Id.ToString()}};
      return CreateNode(typeCache[dataToken.TypeId], dict,
        ((XmlString)dataToken.SerialData).Data);
    }

    /// <summary>
    /// Formats the specified ref token.
    /// </summary>
    /// <param name="refToken">The ref token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(ReferenceToken refToken) {
      return CreateNode(refToken.Id.ToString(),
        new Dictionary<string, string> {
          {"name", refToken.Name}});
    }

    /// <summary>
    /// Formats the specified null ref token.
    /// </summary>
    /// <param name="nullRefToken">The null ref token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(NullReferenceToken nullRefToken) {
      return CreateNode(XmlStringConstants.NULL,
        new Dictionary<string, string>{
          {"name", nullRefToken.Name}});
    }

    /// <summary>
    /// Formats the specified meta info begin token.
    /// </summary>
    /// <param name="metaInfoBeginToken">The meta info begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      return CreateNodeStart(XmlStringConstants.METAINFO);
    }

    /// <summary>
    /// Formats the specified meta info end token.
    /// </summary>
    /// <param name="metaInfoEndToken">The meta info end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      return CreateNodeEnd(XmlStringConstants.METAINFO);
    }

    /// <summary>
    /// Formats the specified token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(TypeToken token) {
      typeCache[token.Id] = TypeNameParser.Parse(token.TypeName).GetTypeNameInCode(false);
      return "";
    }

    /// <summary>
    /// Serialize an object into a file.
    /// The XML configuration is obtained from the <c>ConfigurationService</c>.
    /// The file is actually a ZIP file.
    /// Compression level is set to 5 and needed assemblies are not included.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="filename">The filename.</param>
    public new static void Serialize(object o, string filename) {
      Serialize(o, filename, ConfigurationService.Instance.GetConfiguration(new XmlFormat()));
    }

    /// <summary>
    /// Serializes the specified object into a file.
    /// Needed assemblies are not included, ZIP compression level is set to 5.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="config">The configuration.</param>
    public new static void Serialize(object obj, string filename, Configuration config) {
      try {
        string tempfile = Path.GetTempFileName();
        DateTime start = DateTime.Now;
        using (FileStream stream = File.Create(tempfile)) {
          Serialize(obj, stream, config);
        }
        Logger.Info(String.Format("easy serialization took {0} seconds",
          (DateTime.Now - start).TotalSeconds));
        File.Copy(tempfile, filename, true);
        File.Delete(tempfile);
      }
      catch (Exception) {
        Logger.Warn("Exception caught, no data has been written.");
        throw;
      }
    }

    /// <summary>
    /// Serializes the specified object into a stream using the <see cref="XmlFormat"/>
    /// obtained from the <see cref="ConfigurationService"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    public static void Serialize(object obj, Stream stream) {
      Serialize(obj, stream, ConfigurationService.Instance.GetConfiguration(new XmlFormat()));
    }


    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="config">The configuration.</param>
    public static void Serialize(object obj, Stream stream, Configuration config) {
      try {
        using (StreamWriter writer = new StreamWriter(stream)) {
          Serializer serializer = new Serializer(obj, config);
          serializer.InterleaveTypeInformation = true;
          ReadableXmlGenerator generator = new ReadableXmlGenerator();
          foreach (ISerializationToken token in serializer) {
            string line = generator.Format(token);
            writer.Write(line);
          }
          writer.Flush();
        }
      }
      catch (PersistenceException) {
        throw;
      }
      catch (Exception e) {
        throw new PersistenceException("Unexpected exception during Serialization.", e);
      }
    }
  }
}
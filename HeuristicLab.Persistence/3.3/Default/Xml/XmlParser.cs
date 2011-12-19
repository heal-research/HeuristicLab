#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// Main entry point of persistence loading from XML. Use the static
  /// methods to load from a file or stream.
  /// </summary>
  public class XmlParser : IEnumerable<ISerializationToken> {

    private readonly XmlTextReader reader;
    private delegate IEnumerator<ISerializationToken> Handler();
    private readonly Dictionary<string, Handler> handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlParser"/> class.
    /// </summary>
    /// <param name="input">The input.</param>
    public XmlParser(TextReader input) {
      reader = new XmlTextReader(input);
      reader.WhitespaceHandling = WhitespaceHandling.All;
      reader.Normalization = false;
      handlers = new Dictionary<string, Handler> {
                     {XmlStringConstants.PRIMITIVE, ParsePrimitive},
                     {XmlStringConstants.COMPOSITE, ParseComposite},
                     {XmlStringConstants.REFERENCE, ParseReference},
                     {XmlStringConstants.NULL, ParseNull},
                     {XmlStringConstants.METAINFO, ParseMetaInfo},
                     {XmlStringConstants.TYPE, ParseTypeInfo},
                   };
    }

    /// <summary>
    /// Returns an enumerator that iterates through the serialization tokens.
    /// </summary>
    /// <returns>
    /// An that can be used to iterate through the collection of serialization tokens.
    /// </returns>
    public IEnumerator<ISerializationToken> GetEnumerator() {
      while (reader.Read()) {
        if (!reader.IsStartElement()) {
          break;
        }
        IEnumerator<ISerializationToken> iterator;
        try {
          iterator = handlers[reader.Name].Invoke();
        }
        catch (KeyNotFoundException) {
          throw new PersistenceException(String.Format(
            "Invalid XML tag \"{0}\" in persistence file.",
            reader.Name));
        }
        while (iterator.MoveNext()) {
          yield return iterator.Current;
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the serialization tokens.
    /// </summary>
    /// <returns>
    /// An that can be used to iterate through the collection of serialization tokens.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    private IEnumerator<ISerializationToken> ParsePrimitive() {
      int? id = null;
      string idString = reader.GetAttribute("id");
      if (idString != null)
        id = int.Parse(idString);
      string name = reader.GetAttribute("name");
      int typeId = int.Parse(reader.GetAttribute("typeId"));
      string typeName = reader.GetAttribute("typeName");
      string serializer = reader.GetAttribute("serializer");
      if (typeName != null)
        yield return new TypeToken(typeId, typeName, serializer);
      XmlReader inner = reader.ReadSubtree();
      inner.Read();
      string xml = inner.ReadInnerXml();
      inner.Close();
      yield return new PrimitiveToken(name, typeId, id, new XmlString(xml));
    }

    private IEnumerator<ISerializationToken> ParseComposite() {
      string name = reader.GetAttribute("name");
      string idString = reader.GetAttribute("id");
      int? id = null;
      if (idString != null)
        id = int.Parse(idString);
      int typeId = int.Parse(reader.GetAttribute("typeId"));
      string typeName = reader.GetAttribute("typeName");
      string serializer = reader.GetAttribute("serializer");
      if (typeName != null)
        yield return new TypeToken(typeId, typeName, serializer);
      yield return new BeginToken(name, typeId, id);
      IEnumerator<ISerializationToken> iterator = GetEnumerator();
      while (iterator.MoveNext())
        yield return iterator.Current;
      yield return new EndToken(name, typeId, id);
    }

    private IEnumerator<ISerializationToken> ParseReference() {
      yield return new ReferenceToken(
        reader.GetAttribute("name"),
        int.Parse(reader.GetAttribute("ref")));
    }

    private IEnumerator<ISerializationToken> ParseNull() {
      yield return new NullReferenceToken(reader.GetAttribute("name"));
    }

    private IEnumerator<ISerializationToken> ParseMetaInfo() {
      yield return new MetaInfoBeginToken();
      IEnumerator<ISerializationToken> iterator = GetEnumerator();
      while (iterator.MoveNext())
        yield return iterator.Current;
      yield return new MetaInfoEndToken();
    }

    private IEnumerator<ISerializationToken> ParseTypeInfo() {
      yield return new TypeToken(
        int.Parse(reader.GetAttribute("id")),
        reader.GetAttribute("typeName"),
        reader.GetAttribute("serializer"));
    }

    /// <summary>
    /// Parses the type cache.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>A list of type mapping entries.</returns>
    public static List<TypeMapping> ParseTypeCache(TextReader reader) {
      try {
        var typeCache = new List<TypeMapping>();
        XmlReader xmlReader = XmlReader.Create(reader);
        while (xmlReader.Read()) {
          if (xmlReader.Name == XmlStringConstants.TYPE) {
            typeCache.Add(new TypeMapping(
              int.Parse(xmlReader.GetAttribute("id")),
              xmlReader.GetAttribute("typeName"),
              xmlReader.GetAttribute("serializer")));
          }
        }
        return typeCache;
      }
      catch (PersistenceException) {
        throw;
      }
      catch (Exception e) {
        throw new PersistenceException("Unexpected exception during type cache parsing.", e);
      }
    }

    /// <summary>
    /// Deserializes an object from the specified filename.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>A fresh object instance</returns>
    public static object Deserialize(string filename) {
      TimeSpan start = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
      try {
        using (ZipFile file = new ZipFile(filename)) {
          return Deserialize(file);
        }
      }
      finally {
        TimeSpan end = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
        Tracing.Logger.Info(string.Format(
          "deserialization of {0} took {1} seconds",
          filename, (end - start).TotalSeconds));
      }
    }

    /// <summary>
    /// Deserializes the specified filename.
    /// </summary>
    /// <typeparam name="T">object type expected from the serialized file</typeparam>
    /// <param name="filename">The filename.</param>
    /// <returns>A fresh object of type T</returns>
    public static T Deserialize<T>(string filename) {
      return (T)Deserialize(filename);
    }


    /// <summary>
    /// Deserializes an object from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>A fresh object instance.</returns>
    public static object Deserialize(Stream stream) {
      try {
        using (StreamReader reader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress))) {
          XmlParser parser = new XmlParser(reader);
          Deserializer deserializer = new Deserializer(new TypeMapping[] { });
          return deserializer.Deserialize(parser);
        }
      }
      catch (PersistenceException) {
        throw;
      }
      catch (Exception x) {
        throw new PersistenceException("Unexpected exception during deserialization", x);
      }
    }

    /// <summary>
    /// Deserializes an object from the specified stream.
    /// </summary>
    /// <typeparam name="T">object type expected from the serialized stream</typeparam>
    /// <param name="stream">The stream.</param>
    /// <returns>A fresh object instance.</returns>
    public static T Deserialize<T>(Stream stream) {
      return (T)Deserialize(stream);
    }

    private static object Deserialize(ZipFile zipFile) {
      try {
        ZipEntry typecache = zipFile.GetEntry("typecache.xml");
        if (typecache == null)
          throw new PersistenceException("file does not contain typecache.xml");
        Deserializer deSerializer = new Deserializer(ParseTypeCache(new StreamReader(zipFile.GetInputStream(typecache))));
        ZipEntry data = zipFile.GetEntry("data.xml");
        if (data == null)
          throw new PersistenceException("file does not contain data.xml");
        XmlParser parser = new XmlParser(
          new StreamReader(zipFile.GetInputStream(data)));
        object result = deSerializer.Deserialize(parser);
        zipFile.Close();
        return result;
      }
      catch (PersistenceException) {
        throw;
      }
      catch (Exception e) {
        throw new PersistenceException("Unexpected exception during deserialization", e);
      }
    }
  }
}
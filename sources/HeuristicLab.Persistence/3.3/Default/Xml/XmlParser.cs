using System.Xml;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.Xml {

  public class XmlParser : IEnumerable<ISerializationToken> {

    private readonly XmlReader reader;
    private delegate IEnumerator<ISerializationToken> Handler();
    private readonly Dictionary<string, Handler> handlers;

    public XmlParser(TextReader input) {
      XmlReaderSettings settings = new XmlReaderSettings {
        ConformanceLevel = ConformanceLevel.Document,
        IgnoreWhitespace = true,
        IgnoreComments = true
      };
      reader = XmlReader.Create(input, settings);
      handlers = new Dictionary<string, Handler> {
                     {XmlStringConstants.PRIMITIVE, ParsePrimitive},
                     {XmlStringConstants.COMPOSITE, ParseComposite},
                     {XmlStringConstants.REFERENCE, ParseReference},
                     {XmlStringConstants.NULL, ParseNull},
                     {XmlStringConstants.METAINFO, ParseMetaInfo},
                   };
    }

    public IEnumerator<ISerializationToken> GetEnumerator() {
      while (reader.Read()) {
        if (!reader.IsStartElement()) {
          break;
        }
        IEnumerator<ISerializationToken> iterator;
        try {
          iterator = handlers[reader.Name].Invoke();
        } catch (KeyNotFoundException) {
          throw new PersistenceException(String.Format(
            "Invalid XML tag \"{0}\" in persistence file.",
            reader.Name));
        }
        while (iterator.MoveNext()) {
          yield return iterator.Current;
        }
      }
    }

    private IEnumerator<ISerializationToken> ParsePrimitive() {
      int? id = null;
      string idString = reader.GetAttribute("id");
      if (idString != null)
        id = int.Parse(idString);
      string name = reader.GetAttribute("name");
      int typeId = int.Parse(reader.GetAttribute("typeId"));
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

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

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
      } catch (PersistenceException e) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException("Unexpected exception during type cache parsing.", e);
      }
    }

    public static object Deserialize(string filename) {
      return Deserialize(new ZipFile(filename));
    }

    public static object Deserialize(Stream stream) {
      return Deserialize(new ZipFile(stream));
    }

    private static object Deserialize(ZipFile zipFile) {
      try {
        Deserializer deSerializer = new Deserializer(
          ParseTypeCache(
          new StreamReader(
            zipFile.GetInputStream(zipFile.GetEntry("typecache.xml")))));
        XmlParser parser = new XmlParser(
          new StreamReader(zipFile.GetInputStream(zipFile.GetEntry("data.xml"))));
        object result = deSerializer.Deserialize(parser);
        zipFile.Close();
        return result;
      } catch (PersistenceException e) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException("Unexpected exception during deserialization", e);
      }
    }
  }
}
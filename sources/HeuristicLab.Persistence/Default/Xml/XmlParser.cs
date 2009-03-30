using System.Xml;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using ICSharpCode.SharpZipLib.Zip;

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
                     {XmlStrings.PRIMITIVE, ParsePrimitive},
                     {XmlStrings.COMPOSITE, ParseComposite},
                     {XmlStrings.REFERENCE, ParseReference},
                     {XmlStrings.NULL, ParseNull}
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
        }
        catch (KeyNotFoundException) {
          throw new InvalidOperationException(String.Format(
            "No handler for XML tag \"{0}\" installed",
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
      yield return new PrimitiveToken(
        reader.GetAttribute("name"),
        int.Parse(reader.GetAttribute("typeId")),
        reader.ReadString(),
        id);
    }

    private IEnumerator<ISerializationToken> ParseComposite() {
      string name = reader.GetAttribute("name");            
      string idString = reader.GetAttribute("id");
      int? id = null;
      int? typeId = null;
      if (idString != null)
        id = int.Parse(idString);
        typeId = int.Parse(reader.GetAttribute("typeId"));
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

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public static List<TypeMapping> ParseTypeCache(TextReader reader) {
      var typeCache = new List<TypeMapping>();
      XmlReader xmlReader = XmlReader.Create(reader);
      while ( xmlReader.Read() ) {
        if (xmlReader.Name == XmlStrings.TYPE) {
          typeCache.Add(new TypeMapping(
            int.Parse(xmlReader.GetAttribute("id")),
            xmlReader.GetAttribute("typeName"),
            xmlReader.GetAttribute("serializer")));
        }
      }
      return typeCache;
    }

    public static object DeSerialize(string filename) {
      ZipFile zipFile = new ZipFile(filename);      
      DeSerializer deSerializer = new DeSerializer(
        ParseTypeCache(
        new StreamReader(
          zipFile.GetInputStream(zipFile.GetEntry("typecache.xml")))));
      XmlParser parser = new XmlParser(
        new StreamReader(zipFile.GetInputStream(zipFile.GetEntry("data.xml"))));
      return deSerializer.DeSerialize(parser);      
    }
  }  
}

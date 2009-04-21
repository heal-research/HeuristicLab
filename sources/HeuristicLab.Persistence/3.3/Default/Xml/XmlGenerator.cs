using System.Collections.Generic;
using System;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using HeuristicLab.Tracing;
using log4net;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.Xml {

  public class XmlGenerator : GeneratorBase<string> {

    private int depth;
    private int Depth {
      get {
        return depth;
      }
      set {
        depth = value;
        prefix = new string(' ', depth * 2);
      }
    }

    private string prefix;


    public XmlGenerator() {
      Depth = 0;
    }

    private enum NodeType { Start, End, Inline } ;

    private static void AddXmlTagContent(StringBuilder sb, string name, Dictionary<string, object> attributes) {
      sb.Append(name);
      foreach (var attribute in attributes) {
        if (attribute.Value != null && !string.IsNullOrEmpty(attribute.Value.ToString())) {
          sb.Append(' ');
          sb.Append(attribute.Key);
          sb.Append("=\"");
          sb.Append(attribute.Value);
          sb.Append('"');
        }
      }
    }

    private static void AddXmlStartTag(StringBuilder sb, string name, Dictionary<string, object> attributes) {
      sb.Append('<');
      AddXmlTagContent(sb, name, attributes);
      sb.Append('>');
    }

    private static void AddXmlInlineTag(StringBuilder sb, string name, Dictionary<string, object> attributes) {
      sb.Append('<');
      AddXmlTagContent(sb, name, attributes);
      sb.Append("/>");
    }

    private static void AddXmlEndTag(StringBuilder sb, string name) {
      sb.Append("</");
      sb.Append(name);
      sb.Append(">");
    }

    private string CreateNodeStart(string name, Dictionary<string, object> attributes) {
      StringBuilder sb = new StringBuilder();
      sb.Append(prefix);
      Depth += 1;
      AddXmlStartTag(sb, name, attributes);
      sb.Append("\r\n");
      return sb.ToString();
    }

    private string CreateNodeStart(string name) {
      return CreateNodeStart(name, new Dictionary<string, object>());
    }

    private string CreateNodeEnd(string name) {
      Depth -= 1;
      StringBuilder sb = new StringBuilder();
      sb.Append(prefix);
      AddXmlEndTag(sb, name);
      sb.Append("\r\n");
      return sb.ToString();
    }

    private string CreateNode(string name, Dictionary<string, object> attributes) {
      StringBuilder sb = new StringBuilder();
      sb.Append(prefix);
      AddXmlInlineTag(sb, name, attributes);
      sb.Append("\r\n");
      return sb.ToString();
    }

    private string CreateNode(string name, Dictionary<string, object> attributes, string content) {
      StringBuilder sb = new StringBuilder();
      sb.Append(prefix);
      AddXmlStartTag(sb, name, attributes);
      sb.Append(content);
      sb.Append("</").Append(name).Append(">\r\n");
      return sb.ToString();
    }

    protected override string Format(BeginToken beginToken) {
      return CreateNodeStart(
        XmlStringConstants.COMPOSITE,
        new Dictionary<string, object> {
          {"name", beginToken.Name},
          {"typeId", beginToken.TypeId},
          {"id", beginToken.Id}});
    }

    protected override string Format(EndToken endToken) {
      return CreateNodeEnd(XmlStringConstants.COMPOSITE);
    }

    protected override string Format(PrimitiveToken dataToken) {
      return CreateNode(XmlStringConstants.PRIMITIVE,
        new Dictionary<string, object> {
            {"typeId", dataToken.TypeId},
            {"name", dataToken.Name},
            {"id", dataToken.Id}},
        ((XmlString)dataToken.SerialData).Data);
    }

    protected override string Format(ReferenceToken refToken) {
      return CreateNode(XmlStringConstants.REFERENCE,
        new Dictionary<string, object> {
          {"ref", refToken.Id},
          {"name", refToken.Name}});
    }

    protected override string Format(NullReferenceToken nullRefToken) {
      return CreateNode(XmlStringConstants.NULL,
        new Dictionary<string, object>{
          {"name", nullRefToken.Name}});
    }

    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      return CreateNodeStart(XmlStringConstants.METAINFO);
    }

    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      return CreateNodeEnd(XmlStringConstants.METAINFO);
    }

    public IEnumerable<string> Format(List<TypeMapping> typeCache) {
      yield return CreateNodeStart(XmlStringConstants.TYPECACHE);
      foreach (var mapping in typeCache)
        yield return CreateNode(
          XmlStringConstants.TYPE,
          mapping.GetDict());
      yield return CreateNodeEnd(XmlStringConstants.TYPECACHE);
    }

    public static void Serialize(object o, string filename) {
      Serialize(o, filename, ConfigurationService.Instance.GetDefaultConfig(new XmlFormat()));
    }

    public static void Serialize(object obj, string filename, Configuration config) {
      Serializer serializer = new Serializer(obj, config);
      XmlGenerator generator = new XmlGenerator();
      ZipOutputStream zipStream = new ZipOutputStream(File.Create(filename));
      zipStream.SetLevel(9);
      zipStream.PutNextEntry(new ZipEntry("data.xml"));
      StreamWriter writer = new StreamWriter(zipStream);
      ILog logger = Logger.GetDefaultLogger();
      foreach (ISerializationToken token in serializer) {
        string line = generator.Format(token);
        writer.Write(line);
        logger.Debug(line.TrimEnd());
      }
      writer.Flush();
      zipStream.PutNextEntry(new ZipEntry("typecache.xml"));
      writer = new StreamWriter(zipStream);
      foreach (string line in generator.Format(serializer.TypeCache)) {
        writer.Write(line);
        logger.Debug(line.TrimEnd());
      }
      writer.Flush();
      zipStream.Close();
    }

  }
}
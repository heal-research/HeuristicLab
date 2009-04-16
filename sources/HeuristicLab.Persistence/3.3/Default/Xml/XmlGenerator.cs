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

  struct XmlStrings {
    public const string PRIMITIVE = "PRIMITVE";
    public const string COMPOSITE = "COMPOSITE";
    public const string REFERENCE = "REFERENCE";
    public const string NULL = "NULL";
    public const string TYPECACHE = "TYPECACHE";
    public const string TYPE = "TYPE";
    public const string METAINFO = "METAINFO";
  }

  public class XmlGenerator : GeneratorBase<string> {

    private int depth;

    public XmlGenerator() {
      depth = 0;
    }

    private enum NodeType { Start, End, Inline } ;

    private static string FormatNode(string name,
        Dictionary<string, object> attributes,
        NodeType type) {
      return FormatNode(name, attributes, type, " ");
    }

    private static string FormatNode(string name,
        Dictionary<string, object> attributes,
        NodeType type, string space) {
      StringBuilder sb = new StringBuilder();
      sb.Append('<');
      if (type == NodeType.End)
        sb.Append('/');
      sb.Append(name);
      foreach (var attribute in attributes) {
        if (attribute.Value != null && !string.IsNullOrEmpty(attribute.Value.ToString())) {
          sb.Append(space);
          sb.Append(attribute.Key);
          sb.Append("=\"");
          sb.Append(attribute.Value);
          sb.Append('"');
        }
      }
      if (type == NodeType.Inline)
        sb.Append('/');
      sb.Append(">");
      return sb.ToString();
    }

    private string Prefix {
      get { return new string(' ', depth * 2); }
    }

    protected override string Format(BeginToken beginToken) {
      var attributes = new Dictionary<string, object> {
        {"name", beginToken.Name},
        {"typeId", beginToken.TypeId},
        {"id", beginToken.Id}};
      string result = Prefix +
                      FormatNode(XmlStrings.COMPOSITE, attributes, NodeType.Start) + "\r\n";
      depth += 1;
      return result;
    }

    protected override string Format(EndToken endToken) {
      depth -= 1;
      return Prefix + "</" + XmlStrings.COMPOSITE + ">\r\n";
    }

    protected override string Format(PrimitiveToken dataToken) {
      var attributes =
        new Dictionary<string, object> {
            {"typeId", dataToken.TypeId},
            {"name", dataToken.Name},
            {"id", dataToken.Id}};
      return Prefix +
        FormatNode(XmlStrings.PRIMITIVE, attributes, NodeType.Start) +
        ((XmlString)dataToken.SerialData).Data + "</" + XmlStrings.PRIMITIVE + ">\r\n";
    }

    protected override string Format(ReferenceToken refToken) {
      var attributes = new Dictionary<string, object> {
        {"ref", refToken.Id},
        {"name", refToken.Name}};
      return Prefix + FormatNode(XmlStrings.REFERENCE, attributes, NodeType.Inline) + "\r\n";
    }

    protected override string Format(NullReferenceToken nullRefToken) {
      var attributes = new Dictionary<string, object>{
        {"name", nullRefToken.Name}};
      return Prefix + FormatNode(XmlStrings.NULL, attributes, NodeType.Inline) + "\r\n";
    }

    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      string result = Prefix + "<" + XmlStrings.METAINFO + ">\r\n";
      depth += 1;
      return result;
    }

    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      depth -= 1;
      return Prefix + "</" + XmlStrings.METAINFO + ">\r\n";
    }

    public IEnumerable<string> Format(List<TypeMapping> typeCache) {
      yield return "<" + XmlStrings.TYPECACHE + ">";
      foreach (var mapping in typeCache)
        yield return "  " + FormatNode(
          XmlStrings.TYPE,
          mapping.GetDict(),
          NodeType.Inline,
          "\r\n    ");
      yield return "</" + XmlStrings.TYPECACHE + ">";
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
        logger.Debug(line);
      }
      writer.Flush();
      zipStream.PutNextEntry(new ZipEntry("typecache.xml"));
      writer = new StreamWriter(zipStream);
      foreach (string line in generator.Format(serializer.TypeCache)) {
        writer.WriteLine(line);
        logger.Debug(line);
      }
      writer.Flush();
      zipStream.Close();
    }

  }
}
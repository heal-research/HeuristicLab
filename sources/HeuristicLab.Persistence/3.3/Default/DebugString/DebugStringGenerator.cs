using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.DebugString {

  public class DebugStringGenerator : GeneratorBase<string> {

    private bool isSepReq;
    private readonly bool showRefs;

    public DebugStringGenerator() : this(true) { }

    public DebugStringGenerator(bool showRefs) {
      isSepReq = false;
      this.showRefs = showRefs;
    }

    protected override string Format(BeginToken beginToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(beginToken.Name)) {
        sb.Append(beginToken.Name);
        if (beginToken.Id != null && showRefs) {
          sb.Append('[');
          sb.Append(beginToken.Id);
          sb.Append(']');
        }
      }
      sb.Append("(");
      isSepReq = false;
      return sb.ToString();
    }

    protected override string Format(EndToken endToken) {
      isSepReq = true;
      return ")";
    }

    protected override string Format(PrimitiveToken primitiveToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(primitiveToken.Name)) {
        sb.Append(primitiveToken.Name);
        if (primitiveToken.Id != null && showRefs) {
          sb.Append('[');
          sb.Append(primitiveToken.Id);
          sb.Append(']');
        }
        sb.Append('=');
      }
      sb.Append(((DebugString)primitiveToken.SerialData).Data);
      isSepReq = true;
      return sb.ToString();
    }

    protected override string Format(ReferenceToken referenceToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(referenceToken.Name)) {
        sb.Append(referenceToken.Name);
        sb.Append('=');
      }
      sb.Append('{');
      sb.Append(referenceToken.Id);
      sb.Append('}');
      isSepReq = true;
      return sb.ToString();
    }

    protected override string Format(NullReferenceToken nullReferenceToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(nullReferenceToken.Name)) {
        sb.Append(nullReferenceToken.Name);
        sb.Append('=');
      }
      sb.Append("<null>");
      isSepReq = true;
      return sb.ToString();
    }

    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      return "[";
    }

    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      return "]";
    }

    protected override string Format(TypeToken typeToken) {
      return string.Empty;
    }

    public static string Serialize(object o) {
      return Serialize(o, ConfigurationService.Instance.GetDefaultConfig(new DebugStringFormat()));
    }

    public static string Serialize(object o, Configuration configuration) {
      Serializer s = new Serializer(o, configuration);
      DebugStringGenerator generator = new DebugStringGenerator();
      StringBuilder sb = new StringBuilder();
      foreach (ISerializationToken token in s) {
        sb.Append(generator.Format(token));
      }
      return sb.ToString();
    }
  }
}

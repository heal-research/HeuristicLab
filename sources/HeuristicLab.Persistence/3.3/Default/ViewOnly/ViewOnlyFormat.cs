using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces.Tokens;

namespace HeuristicLab.Persistence.Default.ViewOnly {

  public class ViewOnlyFormat : FormatBase {
    public override string Name { get { return "ViewOnly"; } }
    public static ViewOnlyFormat Instance = new ViewOnlyFormat();
  }

  public abstract class ValueType2ViewFormatter : IFormatter {
    public abstract Type Type { get; }
    public IFormat Format { get { return ViewOnlyFormat.Instance; } }
    public object DoFormat(object o) { return o; }
    public object Parse(object o) {
      throw new NotImplementedException();
    }
  }

  public class String2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(string); } }
  }

  public class Bool2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(bool); } }
  }

  public class Int2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(int); } }
  }

  public class Double2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(double); } }
  }

  public class DateTime2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(DateTime); } }
  }

  public class Type2ViewFormatter : ValueType2ViewFormatter {
    public override Type Type { get { return typeof(Type); } }
  }

  public class ViewOnlyGenerator : Generator<string> {

    private bool isSepReq;
    private readonly bool showRefs;

    public ViewOnlyGenerator() : this(false) { }

    public ViewOnlyGenerator(bool showRefs) {
      isSepReq = false;
      this.showRefs = showRefs;
    }    

    protected override string Format(BeginToken beginToken) {
      StringBuilder sb = new StringBuilder();      
      if (isSepReq)
        sb.Append(", ");
      if ( ! string.IsNullOrEmpty(beginToken.Name) ) {
        sb.Append(beginToken.Name);
        if ( beginToken.Id != null && showRefs ) {
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
      if ( ! string.IsNullOrEmpty(primitiveToken.Name) ) {
        sb.Append(primitiveToken.Name);
        if ( primitiveToken.Id != null && showRefs ) {
          sb.Append('[');
          sb.Append(primitiveToken.Id);
          sb.Append(']');
        }
        sb.Append('=');
      }
      sb.Append(primitiveToken.SerialData);
      isSepReq = true;      
      return sb.ToString();  
    }

    protected override string Format(ReferenceToken referenceToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if ( ! string.IsNullOrEmpty(referenceToken.Name) ) {
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

    public static string Serialize(object o) {
      return Serialize(o, ConfigurationService.Instance.GetDefaultConfig(ViewOnlyFormat.Instance));
    }

    public static string Serialize(object o, Configuration configuration) {
      Serializer s = new Serializer(o, configuration);      
      ViewOnlyGenerator generator = new ViewOnlyGenerator();
      StringBuilder sb = new StringBuilder();
      foreach (ISerializationToken token in s) {
        sb.Append(generator.Format(token));        
      }
      return sb.ToString();
    }
  }
}

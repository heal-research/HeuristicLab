using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.ViewOnly {

  public class String : ISerialData {
    public string Data { get; set; }
    public String(string s) {
      Data = s;
    }
  }

  public class ViewOnlyFormat : FormatBase<String> {
    public override string Name { get { return "ViewOnly"; } }    
  }

  public abstract class ValueType2ViewFormatterBase<T> : FormatterBase<T, String> {
    public override String Format(T o) { return new String(o.ToString()); }
    public override T Parse(String s) {
      throw new NotImplementedException();
    }
  }

  [EmptyStorableClass]
  public class String2ViewFormatter : ValueType2ViewFormatterBase<string> { }

  [EmptyStorableClass]
  public class Bool2ViewFormatter : ValueType2ViewFormatterBase<bool> { }

  [EmptyStorableClass]
  public class Int2ViewFormatter : ValueType2ViewFormatterBase<int> { }

  [EmptyStorableClass]
  public class Double2ViewFormatter : ValueType2ViewFormatterBase<double> { }

  [EmptyStorableClass]
  public class DateTime2ViewFormatter : ValueType2ViewFormatterBase<DateTime> { }

  [EmptyStorableClass]
  public class Type2ViewFormatter : ValueType2ViewFormatterBase<Type> { }

  [EmptyStorableClass]
  public class Float2ViewFormatter : ValueType2ViewFormatterBase<float> { }
    
  public class ViewOnlyGenerator : GeneratorBase<string> {

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
      sb.Append(((String)primitiveToken.SerialData).Data);
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
      return Serialize(o, ConfigurationService.Instance.GetDefaultConfig(new ViewOnlyFormat()));
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

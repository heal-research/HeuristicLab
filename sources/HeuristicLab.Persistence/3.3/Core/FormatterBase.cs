using System;
using System.Text;

namespace HeuristicLab.Persistence.Interfaces {

  public abstract class FormatterBase<Source, SerialData> : IFormatter<Source, SerialData> where SerialData : ISerialData {

    public abstract SerialData Format(Source o);

    public abstract Source Parse(SerialData t);

    public Type SerialDataType { get { return typeof(SerialData); } }

    public Type SourceType { get { return typeof(Source); } }

    ISerialData IFormatter.Format(object o) {
      return Format((Source)o);
    }

    object IFormatter.Parse(ISerialData o) {
      return Parse((SerialData)o);
    }

    public override string ToString() {
      return new StringBuilder()
        .Append(this.GetType().Name)
        .Append('(')
        .Append(SourceType.Name)
        .Append("->")
        .Append(SerialDataType.Name)
        .ToString();
    }

  }

}
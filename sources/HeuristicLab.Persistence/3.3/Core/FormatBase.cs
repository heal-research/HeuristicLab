using System;

namespace HeuristicLab.Persistence.Interfaces {

  public abstract class FormatBase<SerialDataFormat> : IFormat<SerialDataFormat> where SerialDataFormat : ISerialData {

    public abstract string Name { get; }

    public Type SerialDataType { get { return typeof(SerialDataFormat); } }

    public bool Equals(FormatBase<SerialDataFormat> f) {
      if (f == null)
        return false;
      return f.Name == this.Name;
    }

    public override bool Equals(object obj) {
      FormatBase<SerialDataFormat> f = obj as FormatBase<SerialDataFormat>;      
      return Equals(f);
    }
    
  }

}
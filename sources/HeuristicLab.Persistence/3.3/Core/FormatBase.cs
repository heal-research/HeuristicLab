using System;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Common base class for defining a new serialization format.
  /// </summary>  
  [StorableClass(StorableClassType.Empty)]
  public abstract class FormatBase<SerialDataFormat> : IFormat<SerialDataFormat> where SerialDataFormat : ISerialData {

    /// <summary>
    /// Gets the format's name.
    /// </summary>
    /// <value>The format's name.</value>
    public abstract string Name { get; }
    
    /// <summary>
    /// Datatype that describes the atoms used for serialization serialization.
    /// </summary>
    public Type SerialDataType { get { return typeof(SerialDataFormat); } }

    /// <summary>
    /// Compares formats by name.
    /// </summary>    
    public bool Equals(FormatBase<SerialDataFormat> f) {
      if (f == null)
        return false;
      return f.Name == this.Name;
    }

    /// <summary>
    /// Compares foramts by name.
    /// </summary>
    public override bool Equals(object obj) {
      FormatBase<SerialDataFormat> f = obj as FormatBase<SerialDataFormat>;
      return Equals(f);
    }

    public override int GetHashCode() {
      return Name.GetHashCode();
    }

  }

}
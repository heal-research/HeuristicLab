using System;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Interface of a new serialization output format. Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>
  public interface IFormat {
    string Name { get; }
    Type SerialDataType { get; }
  }

  /// <summary>
  /// Marker interface for new serialization output format.  Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>  
  public interface IFormat<SerialDataFormat> : IFormat where SerialDataFormat : ISerialData {
  }

}
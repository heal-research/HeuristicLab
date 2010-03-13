using System;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Interface of a new serialization output format. Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>
  public interface IFormat {

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }

    /// <summary>
    /// Gets the type of the serial data.
    /// </summary>
    /// <value>The type of the serial data.</value>
    Type SerialDataType { get; }
  }

  /// <summary>
  /// Marker interface for new serialization output format.  Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>  
  public interface IFormat<SerialDataFormat> : IFormat where SerialDataFormat : ISerialData {
  }

}
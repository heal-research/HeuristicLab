using System;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Marker interface of serial data formatters. Transform data of type SourceType
  /// into the serialization format SerialDataType. Derive from FormatterBase instead
  /// of implementing this interface.
  /// </summary>
  public interface IFormatter {
    Type SerialDataType { get; }
    Type SourceType { get; }
    ISerialData Format(object o);
    object Parse(ISerialData o);
  }

  /// <summary>
  /// Marker interface of serial data formatters. Transform data of type Source
  /// into the serialization format SerialData. Derive from FormatterBase instead
  /// of implementing this interface.
  /// </summary>  
  public interface IFormatter<Source, SerialData> : IFormatter where SerialData : ISerialData {
    SerialData Format(Source o);
    Source Parse(SerialData t);
  }

}
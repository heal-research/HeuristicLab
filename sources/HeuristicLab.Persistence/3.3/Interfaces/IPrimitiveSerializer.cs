using System;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Marker interface primitive serializers. Transform data of type SourceType
  /// into the serialization format SerialDataType. Derive from PrimitiveSerializerBase instead
  /// of implementing this interface.
  /// </summary>
  public interface IPrimitiveSerializer {
    Type SerialDataType { get; }
    Type SourceType { get; }
    ISerialData Format(object o);
    object Parse(ISerialData o);
  }

  /// <summary>
  /// Marker interface primitive serializers. Transform data of type SourceType
  /// into the serialization format SerialDataType. Derive from PrimitiveSerializerBase instead
  /// of implementing this interface.
  /// </summary>
  public interface IPrimitiveSerializer<Source, SerialData> : IPrimitiveSerializer where SerialData : ISerialData {
    SerialData Format(Source o);
    Source Parse(SerialData t);
  }

}
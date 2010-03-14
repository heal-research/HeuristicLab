using System;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Base class for primitive serializers. These are serializers that map
  /// directly to a single datatype and directly produce a serializable object.
  /// </summary>
  /// <typeparam name="Source">The source type.</typeparam>
  /// <typeparam name="SerialData">The serialized type.</typeparam>
  [StorableClass]
  public abstract class PrimitiveSerializerBase<Source, SerialData> :
      IPrimitiveSerializer<Source, SerialData>
      where SerialData : ISerialData {

    /// <summary>
    /// Formats the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    public abstract SerialData Format(Source o);

    /// <summary>
    /// Parses the specified serialized data back into an object.
    /// </summary>
    /// <param name="t">The serial data.</param>
    /// <returns>A newly created object representing the serialized data.</returns>
    public abstract Source Parse(SerialData data);

    /// <summary>
    /// Gets the type of the serial data.
    /// </summary>
    /// <value>The type of the serial data.</value>
    public Type SerialDataType { get { return typeof(SerialData); } }

    /// <summary>
    /// Gets the type of the source.
    /// </summary>
    /// <value>The type of the source.</value>
    public Type SourceType { get { return typeof(Source); } }

    /// <summary>
    /// Formats the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    ISerialData IPrimitiveSerializer.Format(object o) {
      return Format((Source)o);
    }

    /// <summary>
    /// Parses the specified serialized data back into an object.
    /// </summary>
    /// <param name="t">The serial data.</param>
    /// <returns>A newly created object representing the serialized data.</returns>
    object IPrimitiveSerializer.Parse(ISerialData o) {
      return Parse((SerialData)o);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
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
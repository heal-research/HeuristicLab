using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Encapsulated the serialization of a single primitive value.
  /// </summary>
  public class PrimitiveToken : SerializationTokenBase {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int TypeId;

    /// <summary>
    /// The object's id.
    /// </summary>
    public readonly int? Id;

    /// <summary>
    /// The serialized data.
    /// </summary>
    public readonly ISerialData SerialData;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveToken"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="typeId">The type's id.</param>
    /// <param name="id">The onbject's id.</param>
    /// <param name="serialData">The serialized data.</param>
    public PrimitiveToken(string name, int typeId, int? id, ISerialData serialData)
      : base(name) {
      TypeId = typeId;
      Id = id;
      SerialData = serialData;
    }
  }

}
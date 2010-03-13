using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Association of id, type name and serializer
  /// </summary>
  public class TypeMapping {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The type's full name.
    /// </summary>
    public readonly string TypeName;


    /// <summary>
    /// The full name of the serializes used to serialize the
    /// <see cref="TypeName"/>.
    /// </summary>
    public readonly string Serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapping"/> class.
    /// </summary>
    /// <param name="id">The type's id.</param>
    /// <param name="typeName">The type's full name.</param>
    /// <param name="serializer">The full name of the serializer use to
    /// serialize this type.</param>
    public TypeMapping(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }

    /// <summary>
    /// Creates a dictionary that conatins all properties
    /// and values of this instance.
    /// </summary>
    /// <returns>A dictionary containing all properties
    /// and values of this instance.</returns>
    public Dictionary<string, object> GetDict() {
      return new Dictionary<string, object> {
        {"id", Id},
        {"typeName", TypeName},
        {"serializer", Serializer}};
    }

  }

}
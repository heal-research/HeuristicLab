using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// A token containing type information and mapping.
  /// </summary>
  public class TypeToken : ISerializationToken {

    /// <summary>
    /// The type id.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The type's full name.
    /// </summary>
    public readonly string TypeName;

    /// <summary>
    /// The full type name of the serialized used to
    /// serialize the type.
    /// </summary>
    public readonly string Serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeToken"/> class.
    /// </summary>
    /// <param name="id">The type id.</param>
    /// <param name="typeName">Full name of the type.</param>
    /// <param name="serializer">The full name of the serializer
    /// used to serialize the type.</param>
    public TypeToken(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }
  }
}

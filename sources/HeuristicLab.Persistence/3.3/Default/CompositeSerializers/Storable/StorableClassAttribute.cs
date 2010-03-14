using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Specifies which memebrs are selected for serialization by the StorableSerializer
  /// </summary>
  public enum StorableClassType {

    /// <summary>
    /// Serialize only fields and properties that have been marked
    /// with the [Storable] attribute. This is the default value.
    /// </summary>
    MarkedOnly,

    /// <summary>
    /// Serialize all fields but ignore the 
    /// [Storable] attribute on properties.
    /// </summary>
    [Obsolete("not implemented yet")]
    AllFields,

    /// <summary>
    /// Serialize all properties but ignore the
    /// [Storable] attirbute on fields.
    /// </summary>
    [Obsolete("not implemented yet")]
    AllProperties,

    /// <summary>
    /// Serialize all fields and all properties
    /// but ignore the [Storable] on all members.
    /// </summary>
    [Obsolete("not implemnted yet")]
    AllFieldsAndAllProperties
  };


  /// <summary>
  /// Mark a class to be considered by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class StorableClassAttribute : Attribute {


    /// <summary>
    /// Specify how members are selected for serialization.
    /// </summary>
    public StorableClassType Type { get; set; }

    /// <summary>
    /// Mark a class to be serialize by the <c>StorableSerizlier</c>
    /// </summary>
    /// <param name="type">The storable class type.</param>
    public StorableClassAttribute(StorableClassType type) {
      Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorableClassAttribute"/> class.
    /// The default value for <see cref="StorableClassType"/> is
    /// <see cref="StorableClassType.MarkedOnly"/>.
    /// </summary>
    public StorableClassAttribute() {
    }

  }
}


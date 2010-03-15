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
    /// 
    /// This does not include generated backing fields
    /// for automatic properties.
    /// </summary>    
    AllFields,

    /// <summary>
    /// Serialize all properties but ignore the
    /// [Storable] attribute on fields.
    /// </summary>    
    AllProperties,

    /// <summary>
    /// Serialize all fields and all properties
    /// regardless of the [Storable] attribute.
    /// 
    /// This does not include generated backing fields
    /// for automatic properties, but uses the property
    /// accessors instead.
    /// </summary>    
    AllFieldsAndAllProperties
  };  
}


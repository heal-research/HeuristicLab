using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Interfaces {

  public interface ICompositeSerializer {

    /// <summary>
    /// Defines the Priorty of this composite serializer. Higher number means
    /// higher prioriy. Negative numbers are fallback serializers.
    /// All default generic composite serializers have priority 100. Specializations
    /// have priority 200 so they will  be tried first. Priorities are
    /// only considered for default configurations.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines for every type whether the composite serializer is applicable.
    /// </summary>    
    bool CanSerialize(Type type);

    /// <summary>
    /// Generate MetaInfo necessary for instance creation. (i.e.
    /// array dimensions).
    /// </summary>    
    IEnumerable<Tag> CreateMetaInfo(object obj);

    /// <summary>
    /// Decompose an object into KeyValuePairs, the Key can be null,
    /// the order in which elements are generated is guaranteed to be
    /// the same as they will be supplied to the Populate method.
    /// </summary>    
    IEnumerable<Tag> Decompose(object obj);

    /// <summary>
    /// Create an instance of the object using the provided meta information.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    object CreateInstance(Type type, IEnumerable<Tag> metaInfo);

    /// <summary>
    /// Compose an object from the KeyValuePairs previously generated
    /// in DeCompose. The order in which the values are supplied is
    /// the same as they where generated. Keys might be null.
    /// </summary>    
    void Populate(object instance, IEnumerable<Tag> tags, Type type);
  }

}
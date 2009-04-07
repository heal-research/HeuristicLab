using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Interfaces {

  public interface IDecomposer {

    /// <summary>
    /// Determines for every type whether the decomposer is applicable.
    /// </summary>    
    bool CanDecompose(Type type);

    /// <summary>
    /// Decompose an object into KeyValuePairs, the Key can be null,
    /// the order in which elements are generated is guaranteed to be
    /// the same as they are supplied in the Compose method.
    /// </summary>    
    IEnumerable<Tag> DeCompose(object obj);

    /// <summary>
    /// Create an instance of the object if possible. May return null
    /// in which case the Populate method must create the instance.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    object CreateInstance(Type type);

    /// <summary>
    /// Compose an object from the KeyValuePairs previously generated
    /// in DeCompose. The order in which the values are supplied is
    /// the same as they where generated. Keys might be null.
    /// </summary>    
    object Populate(object instance, IEnumerable<Tag> tags, Type type);
  }  

}
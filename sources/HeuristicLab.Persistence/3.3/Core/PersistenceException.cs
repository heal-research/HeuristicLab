using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Exception thrown by components inside the persistence framework.
  /// </summary>
  [Serializable]  
  public class PersistenceException : Exception {

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    public PersistenceException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public PersistenceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PersistenceException(string message, Exception innerException) :
      base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerExceptions">The inner exceptions.</param>
    public PersistenceException(string message, IEnumerable<Exception> innerExceptions)
      : base(message) {
      int i = 0;
      foreach (var x in innerExceptions) {
        i += 1;
        this.Data.Add("Inner Exception " + i, x);
      }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// <PermissionSet>
    /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
    /// </PermissionSet>
    public override string ToString() {
      var sb = new StringBuilder()
        .Append(base.ToString())
        .Append('\n');
      foreach (Exception x in Data.Values) {
        sb.Append(x.ToString()).Append('\n');
      }
      return sb.ToString();
    }
  }
  
}
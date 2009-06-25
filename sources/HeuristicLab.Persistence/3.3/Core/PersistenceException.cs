using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  [Serializable]  
  public class PersistenceException : Exception {
    public PersistenceException() : base() { }
    public PersistenceException(string message) : base(message) { }
    public PersistenceException(string message, Exception innerException) :  base(message, innerException) { }
    public PersistenceException(string message, IEnumerable<Exception> innerExceptions)
      : base(message) {
      int i = 0;
      foreach (var x in innerExceptions) {
        i += 1;
        this.Data.Add("Inner Exception " + i, x);
      }
    }
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
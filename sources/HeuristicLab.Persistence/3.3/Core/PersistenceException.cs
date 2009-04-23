using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Core {

  public class PersistenceException : Exception {
    public PersistenceException() : base() { }
    public PersistenceException(string message) : base(message) { }
    public PersistenceException(string message, Exception innerException) :  base(message, innerException) { }
  }
  
}
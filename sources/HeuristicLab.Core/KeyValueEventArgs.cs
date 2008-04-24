using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicLab.Core {
  public class KeyValueEventArgs : EventArgs {

    private IItem key;
    public IItem Key {
      get { return key; }
    }

    private IItem value;
    public IItem Value {
      get { return value; }
    }

    public KeyValueEventArgs(IItem key, IItem value) {
      this.key = key;
      this.value = value; 
    }
  }
}

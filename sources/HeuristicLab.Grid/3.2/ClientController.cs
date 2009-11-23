using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Grid {

  class ClientController {
    public GridClient Client { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public bool Running {
      get {
        return !Client.Stopped;
      }
      set {
        if (value == Running)
          return;
        if (!value)          
          Client.Stop();
      }
    }         
  }
}

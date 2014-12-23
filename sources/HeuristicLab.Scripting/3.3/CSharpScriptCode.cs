// use 'vars' to access variables in the script's variable store (e.g. vars.x = 5)
// use 'vars[string]' to access variables via runtime strings (e.g. vars[""x""] = 5)
// use 'vars.Contains(string)' to check if a variable exists
// use 'vars.Clear()' to remove all variables
// use 'foreach (KeyValuePair<string, object> v in vars) { ... }' to iterate over all variables
// use 'variables' to work with IEnumerable<T> extension methods on the script's variable store

using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

public class MyScript : HeuristicLab.Scripting.CSharpScriptBase {
  public override void Main() {
    // type your code here
  }

  // implement further classes and methods

}
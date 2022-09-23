using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.PluginInfrastructure {
  [Serializable]
  public class StringArgument : CommandLineArgument<string> {

    public StringArgument(string value)
      : base(value) {
    }

    protected override bool CheckValidity() {
      return true;
    }
  }
}

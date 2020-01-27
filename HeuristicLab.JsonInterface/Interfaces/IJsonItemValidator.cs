using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IJsonItemValidator {
    bool Validate(ref IList<IJsonItem> faultyItems);
  }
}

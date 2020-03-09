using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public interface IValueLookupJsonItemVM : ILookupJsonItemVM {
    IJsonItem JsonItemReference { get; }
  }
}

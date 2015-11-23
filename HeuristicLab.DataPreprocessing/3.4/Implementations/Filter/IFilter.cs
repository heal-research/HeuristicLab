using HeuristicLab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataPreprocessing.Filter
{
  public interface IFilter : IConstraint
  {
    new bool[] Check();
    new bool[] Check(out string errorMessage);
  }
}

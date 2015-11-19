using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataPreprocessing.Interfaces
{
  public interface IFilteredPreprocessingData : ITransactionalPreprocessingData
  {
    void SetFilter(bool[] rowFilters);
    void PersistFilter();
    void ResetFilter();
    bool IsFiltered { get; }

    event EventHandler FilterChanged;
   }
}

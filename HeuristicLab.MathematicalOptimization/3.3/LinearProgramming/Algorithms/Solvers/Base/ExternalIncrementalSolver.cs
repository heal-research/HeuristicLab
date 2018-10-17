using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  [StorableClass]
  public class ExternalIncrementalSolver : IncrementalSolver, IExternalSolver {
    protected const string FileDialogFilter = "Dynamic-Link Library (*.dll)|*.dll|All Files (*.*)|*.*";

    [Storable]
    protected IFixedValueParameter<FileValue> libraryNameParam;

    public ExternalIncrementalSolver() {
    }

    protected ExternalIncrementalSolver(ExternalIncrementalSolver original, Cloner cloner)
      : base(original, cloner) {
      libraryNameParam = cloner.Clone(original.libraryNameParam);
    }

    public string LibraryName {
      get => libraryNameParam?.Value.Value;
      set => libraryNameParam.Value.Value = value;
    }
  }
}
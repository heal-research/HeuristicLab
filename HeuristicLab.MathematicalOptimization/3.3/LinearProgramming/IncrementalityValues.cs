namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  public enum IncrementalityValues {

    // Start solve from scratch.
    INCREMENTALITY_OFF = 0,

    // Reuse results from previous solve as much as the underlying
    // solver allows.
    INCREMENTALITY_ON = 1
  };
}
namespace HeuristicLab.ExactOptimization.LinearProgramming {

  public enum ResultStatus {
    Optimal,        // optimal.
    Feasible,       // feasible, or stopped by limit.
    Infeasible,     // proven infeasible.
    Unbounded,      // proven unbounded.
    Abnormal,       // abnormal, i.e., error of some kind.
    ModelInvalid,   // the model is trivially invalid (NaN coefficients, etc).
    NotSolved = 6,  // not been solved yet.
    OptimalWithinTolerance = int.MaxValue  // optimal within gap tolerance but objective != bound
  }
}

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  // Enumeration of parameters that take integer or categorical values.
  public enum IntegerParam {

    // Advanced usage: presolve mode.
    PRESOLVE = 1000,

    // Algorithm to solve linear programs.
    LP_ALGORITHM = 1001,

    // Advanced usage: incrementality from one solve to the next.
    INCREMENTALITY = 1002,

    // Advanced usage: enable or disable matrix scaling.
    SCALING = 1003
  };
}
namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  // Enumeration of parameters that take continuous values.
  public enum DoubleParam {

    // Limit for relative MIP gap.
    RELATIVE_MIP_GAP = 0,

    // Advanced usage: tolerance for primal feasibility of basic
    // solutions. This does not control the integer feasibility
    // tolerance of integer solutions for MIP or the tolerance used
    // during presolve.
    PRIMAL_TOLERANCE = 1,

    // Advanced usage: tolerance for dual feasibility of basic solutions.
    DUAL_TOLERANCE = 2
  };
}
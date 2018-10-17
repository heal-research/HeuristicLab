namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  public interface IExternalSolver : ISolver {
    string LibraryName { get; set; }
  }
}
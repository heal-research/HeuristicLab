using Google.OrTools.LinearSolver;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Problems;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  public class CompiledLinearProgrammingProblemDefinition : CompiledProblemDefinition, ILinearProgrammingProblemDefinition {
    private Variable x;
    private Variable y;

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    public void BuildModel(Solver solver) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Example model taken from https://developers.google.com/optimization/mip/integer_opt
      // Define the decision variables
      x = solver.MakeIntVar(0, 3.5, "x");
      y = solver.MakeIntVar(0, double.PositiveInfinity, "y");
      // Define the constraints
      solver.Add(x + 7 * y <= 17.5);
      // Define the objective
      solver.Maximize(x + 10 * y);
    }

    public void Analyze(Solver solver, ResultCollection results) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the solution variables of the decision variables
      results.AddOrUpdateResult("x", new DoubleValue(x.SolutionValue()));
      results.AddOrUpdateResult("y", new DoubleValue(y.SolutionValue()));
      // The decision variables can also be retrieved from the solver
      //results.AddOrUpdateResult("x", new DoubleValue(solver.LookupVariableOrNull("x").SolutionValue()));
      //results.AddOrUpdateResult("y", new DoubleValue(solver.LookupVariableOrNull("y").SolutionValue()));
    }

    // Implement further classes and methods
  }
}
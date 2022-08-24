using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Interpreter {
  public class ThresholdIntervalArithBoundsEstimator : IntervalArithBoundsEstimator {
    public override double GetConstraintViolation(ISymbolicExpressionTree tree, IntervalCollection variableRanges, ShapeConstraint constraint) {
      var estimation = GetModelBound(tree, variableRanges);
      
      var error = 0d;
      if (!constraint.Interval.Contains(estimation.LowerBound)) {
        var lbError = Math.Abs(estimation.LowerBound - constraint.Interval.LowerBound);
        error += CalcBoundViolation(lbError, constraint.Threshold.LowerBound);
      }

      if (!constraint.Interval.Contains(estimation.UpperBound)) {
        var ubError = Math.Abs(estimation.UpperBound - constraint.Interval.UpperBound);
        error += CalcBoundViolation(ubError, constraint.Threshold.UpperBound);
      }

      return error / 2.0;
    }

    private double CalcBoundViolation(double error, double threshold) {
      threshold = Math.Abs(threshold);

      if (double.IsNaN(error)) return 1.0;
      if (double.IsInfinity(error) && !double.IsInfinity(threshold)) return 1.0;
      if (double.IsInfinity(threshold)) return 0;
      if (error <= 0) return 0;
      if (error > threshold) return 1.0;
      if (threshold > 0) return Math.Min(1.0, error / threshold);
      return 1.0;
    }
  }
}

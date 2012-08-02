#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.TestFunctions;
using HeuristicLab.Analysis;

namespace HeuristicLab.Algorithms.GradientDescent {
  public class LevenbergMarquardtMove : SingleSuccessorOperator {
    private const string EvaluatorParameterName = "Evaluator";
    private const string ResultsParameterName = "Results";
    private const string RealVectorParameterName = "RealVector";
    private const string QualityParameterName = "Quality";

    private const string QualityResultsName = "Quality";


    public ILookupParameter<ISingleObjectiveTestFunctionProblemEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>)Parameters[EvaluatorParameterName]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters[RealVectorParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }

    [StorableConstructor]
    protected LevenbergMarquardtMove(bool deserializing) : base(deserializing) { }
    protected LevenbergMarquardtMove(LevenbergMarquardtMove original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LevenbergMarquardtMove(this, cloner);
    }

    public LevenbergMarquardtMove()
      : base() {
      Parameters.Add(new LookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>(EvaluatorParameterName, ""));
      Parameters.Add(new LookupParameter<RealVector>(RealVectorParameterName, ""));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, ""));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, ""));
    }

    public override IOperation Apply() {
      double epsg = 0;
      double epsf = 0;
      double epsx = 0;
      int maxits = 0;
      double diffstep = .1;

      double[] solution = RealVectorParameter.ActualValue.ToArray();

      alglib.minlmstate state;
      alglib.minlmreport report;
      
      alglib.minlmcreatev(solution.Length, solution, diffstep, out state);
      alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
      alglib.minlmsetxrep(state, true);
      alglib.minlmoptimize(state, CreateCallBack(EvaluatorParameter.ActualValue), CreateReportProgress(ResultParameter.ActualValue), null);
      alglib.minlmresults(state, out solution, out report);

      RealVectorParameter.ActualValue = new RealVector(solution);
      return base.Apply();
    }

    private alglib.ndimensional_fvec CreateCallBack(ISingleObjectiveTestFunctionProblemEvaluator evaluator) {
      return (double[] arg, double[] fi, object obj) => {
        RealVector r = new RealVector(arg);
        RealVectorParameter.ActualValue = r;

        IExecutionContext context = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
        evaluator.Execute(context, CancellationToken);
        QualityParameter.ExecutionContext = context;
        fi[0] = QualityParameter.ActualValue.Value;
      };
    }

    private alglib.ndimensional_rep CreateReportProgress(ResultCollection results) {
      return (double[] arg, double func, object obj) => {
        if (!results.ContainsKey(QualityResultsName)) {
          DataTable table = new DataTable(QualityResultsName);
          table.Rows.Add(new DataRow("Quality"));
          results.Add(new Result(QualityResultsName, table));
        }

        DataTable resultsTable = (DataTable)results[QualityResultsName].Value;
        resultsTable.Rows["Quality"].Values.Add(func);
      };
    }
  }
}

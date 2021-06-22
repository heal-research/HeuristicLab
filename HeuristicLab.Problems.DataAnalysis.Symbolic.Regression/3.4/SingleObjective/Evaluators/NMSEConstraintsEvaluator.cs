#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("NMSE Evaluator with shape-constraints", "Calculates NMSE of a symbolic regression solution and checks constraints the fitness is a combination of NMSE and constraint violations.")]
  [StorableType("27473973-DD8D-4375-997D-942E2280AE8E")]
  public class NMSEConstraintsEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    #region Parameter/Properties

    private const string OptimizeParametersParameterName = "OptimizeParameters";
    private const string ParameterOptimizationIterationsParameterName = "ParameterOptimizationIterations";
    private const string UseConstraintsParameterName = "UseConstraintsEvaluation";
    private const string UseSoftConstraintsParameterName = "UseSoftConstraintsEvaluation";
    private const string BoundsEstimatorParameterName = "BoundsEstimator";
    private const string PenaltyFactorParameterName = "PenaltyFactor";
    private const string GenerationOfConvergenceParameterName = "Generation of Convergence";
    private const string AlphaParameterName = "Alpha";
    private const string ResultCollectionParameterName = "Results";
    private const string GenerationsEntry = "Generations";
    private const string LPValueParameterName = "Low Pass Value";
    private const string UseDynamicPenaltyImpl1ParameterName = "UseDynamicPenaltyImpl1";
    private const string UseDynamicPenaltyImpl2ParameterName = "UseDynamicPenaltyImpl2";
    private const string UseDynamicPenaltyImpl3ParameterName = "UseDynamicPenaltyImpl3";
    private const string UseAdditivePenaltyParameterName = "UseAdditivePenalty";
    private const string RisingPenaltyParameterName = "RisingPenalty";
    private const string StepSizeParameterName = "Step Size";
    private const string MaximumStepsParameterName = "Maximum Steps";
    private const string StartpenaltyParameterName = "Start penalty";


    public IFixedValueParameter<BoolValue> OptimizerParametersParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[OptimizeParametersParameterName];

    public IFixedValueParameter<IntValue> ConstantOptimizationIterationsParameter =>
      (IFixedValueParameter<IntValue>)Parameters[ParameterOptimizationIterationsParameterName];

    public IFixedValueParameter<BoolValue> UseConstraintsParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseConstraintsParameterName];

    public IFixedValueParameter<BoolValue> UseSoftConstraintsParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseSoftConstraintsParameterName];

    public IValueParameter<IBoundsEstimator> BoundsEstimatorParameter =>
      (IValueParameter<IBoundsEstimator>)Parameters[BoundsEstimatorParameterName];

    public IFixedValueParameter<DoubleValue> PenaltyFactorParameter =>
      (IFixedValueParameter<DoubleValue>)Parameters[PenaltyFactorParameterName];

    public IFixedValueParameter<IntValue> GenerationOfConvergenceParameter =>
      (IFixedValueParameter<IntValue>)Parameters[GenerationOfConvergenceParameterName];

    public IFixedValueParameter<DoubleValue> AlphaParameter =>
      (IFixedValueParameter<DoubleValue>)Parameters[AlphaParameterName];

    public ILookupParameter<ResultCollection> ResultCollectionParameter =>
      (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName];

    public IResultParameter<DataTable> LPValueParameter {
      get {
        if (Parameters.TryGetValue(LPValueParameterName, out IParameter p))
          return (IResultParameter<DataTable>)p;
        return null;
      }
    }

    public IFixedValueParameter<BoolValue> UseDynamicPenaltyImpl1Parameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseDynamicPenaltyImpl1ParameterName];

    public IFixedValueParameter<BoolValue> UseDynamicPenaltyImpl2Parameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseDynamicPenaltyImpl2ParameterName];

    public IFixedValueParameter<BoolValue> UseDynamicPenaltyImpl3Parameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseDynamicPenaltyImpl3ParameterName];

    public IFixedValueParameter<BoolValue> UseAdditivePenaltyParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[UseAdditivePenaltyParameterName];

    public IFixedValueParameter<IntValue> StepSizeParameter =>
      (IFixedValueParameter<IntValue>)Parameters[StepSizeParameterName];

    public IFixedValueParameter<IntValue> MaximumStepsParameter =>
      (IFixedValueParameter<IntValue>)Parameters[MaximumStepsParameterName];

    public IResultParameter<DataTable> RisingPenaltyParameter {
      get {
        if (Parameters.TryGetValue(RisingPenaltyParameterName, out IParameter p))
          return (IResultParameter<DataTable>)p;
        return null;
      }
    }

    public IFixedValueParameter<DoubleValue> StartpenaltyParameter =>
      (IFixedValueParameter<DoubleValue>)Parameters[StartpenaltyParameterName];

    public bool OptimizeParameters {
      get => OptimizerParametersParameter.Value.Value;
      set => OptimizerParametersParameter.Value.Value = value;
    }

    public int ConstantOptimizationIterations {
      get => ConstantOptimizationIterationsParameter.Value.Value;
      set => ConstantOptimizationIterationsParameter.Value.Value = value;
    }

    public bool UseConstraints {
      get => UseConstraintsParameter.Value.Value;
      set => UseConstraintsParameter.Value.Value = value;
    }

    public bool UseSoftConstraints {
      get => UseSoftConstraintsParameter.Value.Value;
      set => UseSoftConstraintsParameter.Value.Value = value;
    }

    public IBoundsEstimator BoundsEstimator {
      get => BoundsEstimatorParameter.Value;
      set => BoundsEstimatorParameter.Value = value;
    }

    public double PenaltyFactor {
      get => PenaltyFactorParameter.Value.Value;
      set => PenaltyFactorParameter.Value.Value = value;
    }

    public int GenerationOfConvergence {
      get => GenerationOfConvergenceParameter.Value.Value;
      set => GenerationOfConvergenceParameter.Value.Value = value;
    }

    public double Alpha {
      get => AlphaParameter.Value.Value;
      set => AlphaParameter.Value.Value = value;
    }

    public ResultCollection ResultCollection => 
      ResultCollectionParameter.ActualValue;

    private IntValue Generations {
      get {
        IResult result;
        ResultCollection.TryGetValue(GenerationsEntry, out result);
        if (result == null) return new IntValue(0);
        return result.Value == null ? new IntValue(0) : (IntValue)result.Value;
      }
    }

    public override bool Maximization => false; // NMSE is minimized

    #endregion

    #region Constructors/Cloning

    [StorableConstructor]
    protected NMSEConstraintsEvaluator(StorableConstructorFlag _) : base(_) { }

    protected NMSEConstraintsEvaluator(
      NMSEConstraintsEvaluator original, Cloner cloner) : base(original, cloner) { }

    public NMSEConstraintsEvaluator() {
      Parameters.Add(new FixedValueParameter<BoolValue>(OptimizeParametersParameterName,
        "Define whether optimization of numeric parameters is active or not (default: false).", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseConstraintsParameterName,
        "Define whether evaluation of constraints is active or not (default: true).", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName,
        "Define how many parameter optimization steps should be performed (default: 10).", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseSoftConstraintsParameterName,
        "Define whether the constraints are penalized by soft or hard constraints (default: false).", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IBoundsEstimator>(BoundsEstimatorParameterName,
        "The estimator which is used to estimate output ranges of models (default: interval arithmetic).", new IntervalArithBoundsEstimator()));
      Parameters.Add(new FixedValueParameter<DoubleValue>(PenaltyFactorParameterName,
        "Punishment factor for constraint violations for soft constraint handling (fitness = NMSE + penaltyFactor * avg(violations)) (default: 1.0)", new DoubleValue(1.0)));
      Parameters.Add(new FixedValueParameter<IntValue>(GenerationOfConvergenceParameterName, "", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(AlphaParameterName, "", new DoubleValue(0.9)));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultCollectionParameterName, "The result collection to store the analysis results."));

      Parameters.Add(new FixedValueParameter<BoolValue>(UseDynamicPenaltyImpl1ParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseDynamicPenaltyImpl2ParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseDynamicPenaltyImpl3ParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseAdditivePenaltyParameterName, "", new BoolValue(false)));
      

      Parameters.Add(new FixedValueParameter<IntValue>(StepSizeParameterName,
        "Defines the step size for the increasing penalty multiplier.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumStepsParameterName,
        "Defines maximum steps for the increasing penalty multiplier.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(StartpenaltyParameterName,
        "The start value for the penalty multiplier.", new DoubleValue(0.5)));

      /*
      Parameters.Add(new ResultParameter<DataTable>(RisingPenaltyParameterName,
        "Shows the behavior of the penalty multiplier."));
      RisingPenaltyParameter.DefaultValue = new DataTable(RisingPenaltyParameterName) {
        VisualProperties = {
          XAxisTitle = "Generations",
          YAxisTitle = "penalty Multiplier"
        }
      };


      Parameters.Add(new ResultParameter<DataTable>(LPValueParameterName,
        "Low Pass Value"));
      LPValueParameter.DefaultValue = new DataTable(LPValueParameterName) {
        VisualProperties = {
          XAxisTitle = "Generations",
          YAxisTitle = "Value"
        }
      };*/
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NMSEConstraintsEvaluator(this, cloner);
    }

    #endregion

    public override IOperation InstrumentedApply() {
      var rows = GenerateRowsToEvaluate();
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;
      
      if (OptimizeParameters) {
        SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, tree, problemData, rows,
          false, ConstantOptimizationIterations, true,
          estimationLimits.Lower, estimationLimits.Upper);
      } else {
        if (applyLinearScaling) {
          var rootNode = new ProgramRootSymbol().CreateTreeNode();
          var startNode = new StartSymbol().CreateTreeNode();
          var offset = tree.Root.GetSubtree(0) //Start
                                .GetSubtree(0); //Offset
          var scaling = offset.GetSubtree(0);

          //Check if tree contains offset and scaling nodes
          if (!(offset.Symbol is Addition) || !(scaling.Symbol is Multiplication))
            throw new ArgumentException($"{ItemName} can only be used with IntervalArithmeticGrammar.");

          var t = (ISymbolicExpressionTreeNode)scaling.GetSubtree(0).Clone();
          rootNode.AddSubtree(startNode);
          startNode.AddSubtree(t);
          var newTree = new SymbolicExpressionTree(rootNode);

          //calculate alpha and beta for scaling
          var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(newTree, problemData.Dataset, rows);

          var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
          OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out var alpha, out var beta,
            out var errorState);

          if (errorState == OnlineCalculatorError.None) {
            //Set alpha and beta to the scaling nodes from ia grammar
            var offsetParameter = offset.GetSubtree(1) as ConstantTreeNode;
            offsetParameter.Value = alpha;
            var scalingParameter = scaling.GetSubtree(1) as ConstantTreeNode;
            scalingParameter.Value = beta;
          }
        } // else: alpha and beta are evolved
      }

      var quality = Calculate(interpreter, tree, estimationLimits.Lower, estimationLimits.Upper, problemData, rows,
        BoundsEstimator, UseConstraints, UseSoftConstraints, 
        UseDynamicPenaltyImpl1Parameter.Value.Value, UseDynamicPenaltyImpl2Parameter.Value.Value, 
        UseDynamicPenaltyImpl3Parameter.Value.Value, UseAdditivePenaltyParameter.Value.Value,
        PenaltyFactor,
        StepSizeParameter.Value.Value, StartpenaltyParameter.Value.Value, MaximumStepsParameter.Value.Value, 
        RisingPenaltyParameter?.ActualValue,
        Generations.Value, GenerationOfConvergence, Alpha,
        LPValueParameter);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public override void InitializeState() {
      oldValue = 0.0;
      actualValue = 0.0;
      oldGeneration = 0;
      base.InitializeState();
    }

    // bei mehrmaligen ausführungen bleibt der state!
    private static int oldGeneration = 0;
    private static double oldValue = 0.0;
    private static double actualValue = 0.0;

    public static double Calculate(
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree tree,
      double lowerEstimationLimit, double upperEstimationLimit,
      IRegressionProblemData problemData, IEnumerable<int> rows,
      IBoundsEstimator estimator,
      bool useConstraints, bool useSoftConstraints = false, 
      bool useDynamicConstraints1 = false, bool useDynamicConstraints2 = false, bool useDynamicConstraints3 = false,
      bool useAdditivePenalty = false,
      double penaltyFactor = 1.0,
      int stepSize = 1, double startpenalty = 0.5, int maximumSteps = 1000, DataTable penaltyDataTable = null,
      int generation = 0, int generationOfConvergence = 100, double alpha = 0.9,
      IResultParameter<DataTable> lpValueParameter = null) {

      double risingPenalty = 1.0;
      if (useDynamicConstraints1) {
        risingPenalty = LinearDiscreteDoubleValueModifier.Apply(0, startpenalty, 1.0,
          (int)(generation / stepSize) * stepSize, 0, maximumSteps);
      }

      if (oldGeneration != generation) {
        oldGeneration = generation;

        if(lpValueParameter != null) {
          var LPValueParameterDataTable = lpValueParameter.ActualValue;
          if (LPValueParameterDataTable.Rows.Count == 0)
            LPValueParameterDataTable.Rows.Add(new DataRow(LPValueParameterName));

          LPValueParameterDataTable.Rows[LPValueParameterName]
            .Values
            .Add(oldValue);
        }
        
        if (penaltyDataTable != null && useDynamicConstraints1) {
          if (penaltyDataTable.Rows.Count == 0)
            penaltyDataTable.Rows.Add(new DataRow("LinearDiscreteDoubleValueModifier"));
          penaltyDataTable.Rows["LinearDiscreteDoubleValueModifier"].Values.Add(risingPenalty);
        }

        oldValue = actualValue;
      }

      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var constraints = problemData.ShapeConstraints.EnabledConstraints;
      var intervalCollection = problemData.VariableRanges;

      var boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
      var nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues,
        out var errorState);

      if (errorState != OnlineCalculatorError.None) {
        actualValue = alpha * 1.0 + (1.0 - alpha) * actualValue;
        return 10000.0;
      }

      if (!useConstraints)
        return nmse;


      if(useDynamicConstraints2) {
        foreach(var c in constraints) {
          if(!double.IsNegativeInfinity(c.DynInterval.LowerBound) && !double.IsPositiveInfinity(c.DynInterval.UpperBound)) {
            int step = (int)(generation / stepSize) * stepSize;
            var lb = LinearDiscreteDoubleValueModifier.Apply(0, c.DynInterval.LowerBound, c.TargetInterval.LowerBound, step, 0, maximumSteps);
            var ub = LinearDiscreteDoubleValueModifier.Apply(0, c.DynInterval.UpperBound, c.TargetInterval.UpperBound, step, 0, maximumSteps);
            c.Interval = new Interval(lb, ub);
          }
        }
      } else {
        foreach (var c in constraints) {
          c.Interval = new Interval(c.TargetInterval.LowerBound, c.TargetInterval.UpperBound);
        }
      }

      var constraintViolations = IntervalUtil.GetConstraintViolations(constraints, estimator, intervalCollection, tree);
      var constraintBounds = IntervalUtil.GetModelBounds(constraints, estimator, intervalCollection, tree);

      if (constraintViolations.Any(x => double.IsNaN(x) || double.IsInfinity(x))) {
        actualValue = alpha * 1.0 + (1.0 - alpha) * actualValue;
        return 10000.0;
      }


      /*
      if(constraintViolations.Any(x => x > 0.0)) {
        actualValue = alpha * 1.0 + (1.0 - alpha) * actualValue;
      } else {
        actualValue *= (1.0 - alpha);
      }*/

      if (useSoftConstraints) {
        if (penaltyFactor < 0.0)
          throw new ArgumentException("The parameter has to be greater or equal 0.0!", nameof(penaltyFactor));


        var errors = constraints
          .Zip(constraintBounds, CalcSoftConstraintError);

        var weightedViolationSum = constraints
          .Zip(errors, (c, e) => c.Weight * e)
          .Average();

        /*
        var weightedViolationSum = constraints
          .Zip(constraintViolations, (c, v) => c.Weight * v)
          .Average();
        */

        actualValue = alpha * errors.Average() + (1.0 - alpha) * actualValue;

        var violation = (weightedViolationSum * penaltyFactor);
        if (useDynamicConstraints1)
          violation *= risingPenalty;


        if (useDynamicConstraints3)
          violation *= oldValue;

        if (useAdditivePenalty)
          nmse += violation;
        else
          nmse += nmse * violation;


        return nmse;
          //Math.Min(nmse, 1.0) + 
          //(Math.Min(nmse, 1.0) * 
          //(//(penaltyFactor * oldValue) * 
          /*Math.Min(penaltyFactor, penaltyFactor * ((generation + 1) / generationOfConvergence)) * */ /* penaltyFactor rises over time */
          //penaltyFactor * weightedViolationSum);
      } else if (constraintViolations.Any(x => x > 0.0)) {
        return 1.0;
      } // globale constraints -> wenn diese verletzt werden -> nmse = 1.0 ????
        // analyzer -> avg. quality von lösung die nix verletzen


      return nmse;
    }

    private static double CalcSoftConstraintError(ShapeConstraint constraint, Interval bounds) {
      if (!constraint.Interval.Contains(bounds)) {

        // get the absolute threshold bounds
        var thresholdLb = Math.Abs(constraint.Threshold.LowerBound);
        var thresholdUb = Math.Abs(constraint.Threshold.UpperBound);

        // calc the absolute bound errors
        var errorLb = 0.0;//Math.Abs(Math.Abs(b.LowerBound) - Math.Abs(c.Interval.LowerBound));
        var errorUb = 0.0;//Math.Abs(Math.Abs(b.UpperBound) - Math.Abs(c.Interval.UpperBound));

        if (!constraint.Interval.Contains(bounds.LowerBound)) {
          errorLb = Math.Abs(bounds.LowerBound - constraint.Interval.LowerBound); // immer einfach 0 als "Mitte"?
        }

        if (!constraint.Interval.Contains(bounds.UpperBound)) {
          errorUb = Math.Abs(bounds.UpperBound - constraint.Interval.UpperBound);
        }

        double relativeLb;
        if (double.IsInfinity(thresholdLb))
          relativeLb = 0.0;
        if (thresholdLb > 0.0) {
          relativeLb = errorLb / thresholdLb;
          relativeLb = double.IsNaN(relativeLb) ? 1.0 : Math.Min(relativeLb, 1.0);
        } else
          relativeLb = 1.0;

        double relativeUb;
        if (double.IsInfinity(thresholdUb))
          relativeUb = 0.0;
        else if (thresholdUb > 0.0) {
          relativeUb = errorUb / thresholdUb;
          relativeUb = double.IsNaN(relativeUb) ? 1.0 : Math.Min(relativeUb, 1.0);
        } else
          relativeUb = 1.0;

        var error = (relativeLb + relativeUb) / 2.0;
        //actualValue = alpha * error + (1.0 - alpha) * actualValue;
        return error; //* constraint.Weight;
      }
      //actualValue *= (1.0 - alpha);
      return 0.0;
    }

    public override double Evaluate(
      IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData,
      IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      var nmse = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree,
        EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper,
        problemData, rows, BoundsEstimator, UseConstraints, UseSoftConstraints, 
        UseDynamicPenaltyImpl1Parameter.Value.Value, UseDynamicPenaltyImpl2Parameter.Value.Value,
        UseDynamicPenaltyImpl3Parameter.Value.Value, UseAdditivePenaltyParameter.Value.Value,
        PenaltyFactor,
        StepSizeParameter.Value.Value, StartpenaltyParameter.Value.Value, MaximumStepsParameter.Value.Value);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return nmse;
    }
  }
}
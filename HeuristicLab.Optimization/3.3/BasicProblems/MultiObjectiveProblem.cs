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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [StorableType("6F2EC371-0309-4848-B7B1-C9B9C7E3436F")]
  public abstract class MultiObjectiveProblem<TEncoding, TEncodedSolution> :
    Problem<TEncoding, TEncodedSolution, MultiObjectiveEvaluator<TEncodedSolution>>,
    IMultiObjectiveProblem<TEncoding, TEncodedSolution>,
    IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    #region Parameternames
    public const string BestKnownFrontParameterName = "BestKnownFront";
    public const string ReferencePointParameterName = "ReferencePoint";
    #endregion

    #region Parameterproperties
    [Storable] public IValueParameter<BoolArray> MaximizationParameter { get; }
    public IValueParameter<DoubleMatrix> BestKnownFrontParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters[BestKnownFrontParameterName]; }
    }
    public IValueParameter<DoubleArray> ReferencePointParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[ReferencePointParameterName]; }
    }
    #endregion


    [StorableConstructor]
    protected MultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }

    protected MultiObjectiveProblem(MultiObjectiveProblem<TEncoding, TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      MaximizationParameter = cloner.Clone(original.MaximizationParameter);
      ParameterizeOperators();
    }

    protected MultiObjectiveProblem() : base() {
      MaximizationParameter = new ValueParameter<BoolArray>("Maximization", "The dimensions correspond to the different objectives: False if the objective should be minimized, true if it should be maximized..", new BoolArray(new bool[] { }, @readonly: true));
      Parameters.Add(MaximizationParameter);
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>(BestKnownFrontParameterName, "A double matrix representing the best known qualites for this problem (aka points on the Pareto front). Points are to be given in a row-wise fashion."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(ReferencePointParameterName, "The refrence point for hypervolume calculations on this problem"));
      Operators.Add(Evaluator);
      Operators.Add(new MultiObjectiveAnalyzer<TEncodedSolution>());
      ParameterizeOperators();
    }

    protected MultiObjectiveProblem(TEncoding encoding) : base(encoding) {
      MaximizationParameter = new ValueParameter<BoolArray>("Maximization", "The dimensions correspond to the different objectives: False if the objective should be minimized, true if it should be maximized..", new BoolArray(new bool[] { }, @readonly: true));
      Parameters.Add(MaximizationParameter);
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>(BestKnownFrontParameterName, "A double matrix representing the best known qualites for this problem (aka points on the Pareto front). Points are to be given in a row-wise fashion."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(ReferencePointParameterName, "The refrence point for hypervolume calculations on this problem"));
      Operators.Add(Evaluator);
      Operators.Add(new MultiObjectiveAnalyzer<TEncodedSolution>());
      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public int Objectives {
      get { return Maximization.Length; }
    }
    public bool[] Maximization {
      get { return MaximizationParameter.Value.CloneAsArray(); }
      protected set {
        if (MaximizationParameter.Value.SequenceEqual(value)) return;
        MaximizationParameter.ForceValue(new BoolArray(value, @readonly: true));
        OnMaximizationChanged();
      }
    }

    public virtual IReadOnlyList<double[]> BestKnownFront {
      get {
        if (!Parameters.ContainsKey(BestKnownFrontParameterName)) return null;
        var mat = BestKnownFrontParameter.Value;
        if (mat == null) return null;
        var v = new double[mat.Rows][];
        for (var i = 0; i < mat.Rows; i++) {
          var r = v[i] = new double[mat.Columns];
          for (var j = 0; j < mat.Columns; j++) {
            r[j] = mat[i, j];
          }
        }
        return v;
      }
      set {
        if (value == null || value.Count == 0) {
          BestKnownFrontParameter.Value = new DoubleMatrix();
          return;
        }
        var mat = new DoubleMatrix(value.Count, value[0].Length);
        for (int i = 0; i < value.Count; i++) {
          for (int j = 0; j < value[i].Length; j++) {
            mat[i, j] = value[i][j];
          }
        }

        BestKnownFrontParameter.Value = mat;
      }
    }
    public virtual double[] ReferencePoint {
      get { return ReferencePointParameter.Value != null ? ReferencePointParameter.Value.CloneAsArray() : null; }
      set { ReferencePointParameter.Value = new DoubleArray(value); }
    }

    public virtual double[] Evaluate(TEncodedSolution solution, IRandom random) {
      return Evaluate(solution, random, CancellationToken.None);
    }

    public abstract double[] Evaluate(TEncodedSolution solution, IRandom random, CancellationToken cancellationToken);
    public virtual void Analyze(TEncodedSolution[] solutions, double[][] qualities, ResultCollection results, IRandom random) { }


    protected override void OnOperatorsChanged() {
      if (Encoding != null) {
        PruneSingleObjectiveOperators(Encoding);
        var combinedEncoding = Encoding as CombinedEncoding;
        if (combinedEncoding != null) {
          foreach (var encoding in combinedEncoding.Encodings.ToList()) {
            PruneSingleObjectiveOperators(encoding);
          }
        }
      }
      base.OnOperatorsChanged();
    }

    private void PruneSingleObjectiveOperators(IEncoding encoding) {
      if (encoding != null && encoding.Operators.Any(x => x is ISingleObjectiveOperator && !(x is IMultiObjectiveOperator)))
        encoding.Operators = encoding.Operators.Where(x => !(x is ISingleObjectiveOperator) || x is IMultiObjectiveOperator).ToList();

      foreach (var multiOp in Encoding.Operators.OfType<IMultiOperator>()) {
        foreach (var soOp in multiOp.Operators.Where(x => x is ISingleObjectiveOperator).ToList()) {
          multiOp.RemoveOperator(soOp);
        }
      }
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<IMultiObjectiveEvaluationOperator<TEncodedSolution>>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<IMultiObjectiveAnalysisOperator<TEncodedSolution>>())
        op.AnalyzeAction = Analyze;
    }


    #region IMultiObjectiveHeuristicOptimizationProblem Members
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion

    public event EventHandler MaximizationChanged;
    protected void OnMaximizationChanged() {
      MaximizationChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
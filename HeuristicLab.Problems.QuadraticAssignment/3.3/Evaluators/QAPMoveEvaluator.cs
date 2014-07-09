#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// A base class for operators which evaluate QAP solutions.
  /// </summary>
  [Item("QAPMoveEvaluator", "A base class for operators which evaluate moves.")]
  [StorableClass]
  public abstract class QAPMoveEvaluator : SingleSuccessorOperator, IQAPMoveEvaluator {

    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }
    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }

    [StorableConstructor]
    protected QAPMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPMoveEvaluator(QAPMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected QAPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a QAP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a QAP solution."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The matrix which contains the distances between the facilities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The matrix with the weights between the facilities, that is how strongly they're connected to each other."));
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Swap2MoveHardTabuCriterion", @"For relative postion encoded permutations it prevents readding of previously deleted edges as well as deleting previously added edges.
  For absolute position encoded permutations it prevents moving a number if it was previously moved.

If the aspiration condition is activated, a move will not be considered tabu against a move in the tabu list if it leads to a better solution than the quality recorded with the move in the tabu list.")]
  [StorableClass]
  public class Swap2MoveHardTabuCriterion : SingleSuccessorOperator, IPermutationSwap2MoveOperator, ITabuChecker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (LookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (LookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (ILookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueParameter<BoolValue> UseAspirationCriterionParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseAspirationCriterion"]; }
    }

    public BoolValue UseAspirationCriterion {
      get { return UseAspirationCriterionParameter.Value; }
      set { UseAspirationCriterionParameter.Value = value; }
    }

    [StorableConstructor]
    protected Swap2MoveHardTabuCriterion(bool deserializing) : base(deserializing) { }
    protected Swap2MoveHardTabuCriterion(Swap2MoveHardTabuCriterion original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveHardTabuCriterion()
      : base() {
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveHardTabuCriterion(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      Swap2Move move = Swap2MoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      int length = permutation.Length;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;

      if (permutation.PermutationType != PermutationTypes.Absolute)
        isTabu = EvaluateRelativeTabuState(tabuList, move, permutation, moveQuality, maximization, useAspiration);
      else isTabu = EvaluateAbsoluteTabuState(tabuList, move, permutation, moveQuality, maximization, useAspiration);

      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }

    private static bool EvaluateRelativeTabuState(ItemList<IItem> tabuList, Swap2Move move, Permutation permutation, double moveQuality, bool maximization, bool useAspiration) {
      bool isTabu = false;
      StandardEdgeEqualityComparer eq = new StandardEdgeEqualityComparer();
      bool bidirectional = permutation.PermutationType == PermutationTypes.RelativeUndirected;
      List<Edge> deleted = new List<Edge>();
      deleted.Add(new Edge(permutation.GetCircular(move.Index1 - 1), permutation[move.Index1], bidirectional));
      deleted.Add(new Edge(permutation[move.Index1], permutation.GetCircular(move.Index1 + 1), bidirectional));
      deleted.Add(new Edge(permutation.GetCircular(move.Index2 - 1), permutation[move.Index2], bidirectional));
      deleted.Add(new Edge(permutation[move.Index2], permutation.GetCircular(move.Index2 + 1), bidirectional));
      List<Edge> added = new List<Edge>();
      added.Add(new Edge(permutation.GetCircular(move.Index1 - 1), permutation[move.Index2], bidirectional));
      added.Add(new Edge(permutation[move.Index2], permutation.GetCircular(move.Index1 + 1), bidirectional));
      added.Add(new Edge(permutation.GetCircular(move.Index2 - 1), permutation[move.Index1], bidirectional));
      added.Add(new Edge(permutation[move.Index1], permutation.GetCircular(move.Index2 + 1), bidirectional));

      foreach (IItem tabuMove in tabuList) {
        Swap2MoveRelativeAttribute relAttrib = (tabuMove as Swap2MoveRelativeAttribute);
        if (relAttrib != null
          && (!useAspiration
              || maximization && moveQuality <= relAttrib.MoveQuality
              || !maximization && moveQuality >= relAttrib.MoveQuality)) {
          for (int i = 0; i < relAttrib.AddedEdges.Count; i++) {
            isTabu = eq.Equals(relAttrib.AddedEdges[i], deleted[0])
                  || eq.Equals(relAttrib.AddedEdges[i], deleted[1])
                  || eq.Equals(relAttrib.AddedEdges[i], deleted[2])
                  || eq.Equals(relAttrib.AddedEdges[i], deleted[3]);
            if (isTabu) break;
          }
          if (isTabu) break;
          for (int i = 0; i < relAttrib.DeletedEdges.Count; i++) {
            isTabu = eq.Equals(relAttrib.DeletedEdges[i], added[0])
                  || eq.Equals(relAttrib.DeletedEdges[i], added[1])
                  || eq.Equals(relAttrib.DeletedEdges[i], added[2])
                  || eq.Equals(relAttrib.DeletedEdges[i], added[3]);
            if (isTabu) break;
          }
        }
        if (isTabu) break;
      }
      return isTabu;
    }

    private bool EvaluateAbsoluteTabuState(ItemList<IItem> tabuList, Swap2Move move, Permutation permutation, double moveQuality, bool maximization, bool useAspiration) {
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        Swap2MoveAbsoluteAttribute attrib = (tabuMove as Swap2MoveAbsoluteAttribute);
        if (attrib != null
          && (!useAspiration
              || maximization && moveQuality <= attrib.MoveQuality
              || !maximization && moveQuality >= attrib.MoveQuality)) {
          int i1 = move.Index1;
          int n1 = permutation[move.Index1];
          int i2 = move.Index2;
          int n2 = permutation[move.Index2];
          if (attrib != null) {
            if (attrib.Number1 == n1 || attrib.Number1 == n2
              || attrib.Number2 == n2 || attrib.Number2 == n1)
              isTabu = true;
          }
        }
        if (isTabu) break;
      }
      return isTabu;
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Swap2MoveTabuMaker", "Declares a given swap-2 move as tabu, by adding its attributes to the tabu list and also store the solution quality or the move quality (whichever is better).")]
  [StorableClass]
  public class Swap2MoveTabuMaker : TabuMaker, IPermutationSwap2MoveOperator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (LookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }

    [StorableConstructor]
    protected Swap2MoveTabuMaker(bool deserializing) : base(deserializing) { }
    protected Swap2MoveTabuMaker(Swap2MoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move that was made."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      Swap2Move move = Swap2MoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      double baseQuality = moveQuality;
      if (maximization && quality > moveQuality || !maximization && quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality
      if (permutation.PermutationType == PermutationTypes.Absolute)
        return new Swap2MoveAbsoluteAttribute(move.Index1, permutation[move.Index1], move.Index2, permutation[move.Index2], baseQuality);
      else {
        Swap2MoveRelativeAttribute attrib = new Swap2MoveRelativeAttribute(baseQuality);
        bool bidirectional = permutation.PermutationType == PermutationTypes.RelativeUndirected;
        attrib.DeletedEdges.Add(new Edge(permutation.GetCircular(move.Index1 - 1), permutation[move.Index1], bidirectional));
        attrib.DeletedEdges.Add(new Edge(permutation[move.Index1], permutation.GetCircular(move.Index1 + 1), bidirectional));
        attrib.DeletedEdges.Add(new Edge(permutation.GetCircular(move.Index2 - 1), permutation[move.Index2], bidirectional));
        attrib.DeletedEdges.Add(new Edge(permutation[move.Index2], permutation.GetCircular(move.Index2 + 1), bidirectional));
        attrib.AddedEdges.Add(new Edge(permutation.GetCircular(move.Index1 - 1), permutation[move.Index2], bidirectional));
        attrib.AddedEdges.Add(new Edge(permutation[move.Index2], permutation.GetCircular(move.Index1 + 1), bidirectional));
        attrib.AddedEdges.Add(new Edge(permutation.GetCircular(move.Index2 - 1), permutation[move.Index1], bidirectional));
        attrib.AddedEdges.Add(new Edge(permutation[move.Index1], permutation.GetCircular(move.Index2 + 1), bidirectional));
        return attrib;
      }
    }
  }
}

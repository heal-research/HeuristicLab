#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Swap2MoveMaker", "Peforms a swap-2 move on a given grouping and updates the quality.")]
  [StorableClass]
  public class Swap2MoveMaker : SingleSuccessorOperator, ILinearLinkageSwap2MoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (ILookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }
    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    [StorableConstructor]
    protected Swap2MoveMaker(bool deserializing) : base(deserializing) { }
    protected Swap2MoveMaker(Swap2MoveMaker original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move to apply."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The linear linkage encoded solution to which the move should be applied."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveMaker(this, cloner);
    }

    public static void Apply(LinearLinkage lle, Swap2Move move) {
      var groups = lle.GetGroups().ToList();
      int g1 = -1, g2 = -1, g1Idx = -1, g2Idx = -1;
      for (var i = 0; i < groups.Count; i++) {
        if (g1 < 0) {
          g1Idx = groups[i].IndexOf(move.Item1);
          if (g1Idx >= 0) {
            g1 = i;
            continue;
          }
        }
        if (g2 < 0) {
          g2Idx = groups[i].IndexOf(move.Item2);
          if (g2Idx >= 0) g2 = i;
        }
      }

      // can happen if (for some reason) the items belong to the same group
      if (g1 < 0 || g2 < 0) throw new InvalidOperationException("Swap2MoveMaker: Cannot apply swap move, items are not found in different groups.");

      var h = groups[g1][g1Idx];
      groups[g1][g1Idx] = groups[g2][g2Idx];
      groups[g2][g2Idx] = h;
      lle.SetGroups(groups);
    }

    public override IOperation Apply() {
      var move = Swap2MoveParameter.ActualValue;
      var lle = LLEParameter.ActualValue;
      var moveQuality = MoveQualityParameter.ActualValue;
      var quality = QualityParameter.ActualValue;

      Apply(lle, move);

      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}

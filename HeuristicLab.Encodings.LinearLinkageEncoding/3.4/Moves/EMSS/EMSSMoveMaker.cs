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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("EMSSMoveMaker", "Applies an EMSS move to the lle grouping (extract, merge, shift, and split.")]
  [StorableClass]
  public class EMSSMoveMaker : SingleSuccessorOperator, ILinearLinkageEMSSMoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<EMSSMove> EMSSMoveParameter {
      get { return (ILookupParameter<EMSSMove>)Parameters["EMSSMove"]; }
    }
    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    [StorableConstructor]
    protected EMSSMoveMaker(bool deserializing) : base(deserializing) { }
    protected EMSSMoveMaker(EMSSMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public EMSSMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<EMSSMove>("EMSSMove", "The move to apply."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The linear linkage encoded solution to which the move should be applied."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EMSSMoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      var move = EMSSMoveParameter.ActualValue;
      var lle = LLEParameter.ActualValue;
      var moveQuality = MoveQualityParameter.ActualValue;
      var quality = QualityParameter.ActualValue;

      move.Apply(lle);

      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}

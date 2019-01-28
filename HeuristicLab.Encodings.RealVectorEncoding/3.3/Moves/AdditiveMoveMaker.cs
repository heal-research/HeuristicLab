#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMoveMaker", "Peforms an additive move on a given real vector and updates the quality.")]
  [StorableType("DFEC364D-EFF9-456C-A85C-095F454719C8")]
  public class AdditiveMoveMaker : SingleSuccessorOperator, IAdditiveRealVectorMoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (ILookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    [StorableConstructor]
    protected AdditiveMoveMaker(StorableConstructorFlag _) : base(_) { }
    protected AdditiveMoveMaker(AdditiveMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public AdditiveMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      AdditiveMove move = AdditiveMoveParameter.ActualValue;
      RealVector realVector = RealVectorParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      realVector[move.Dimension] += move.MoveDistance;
      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}

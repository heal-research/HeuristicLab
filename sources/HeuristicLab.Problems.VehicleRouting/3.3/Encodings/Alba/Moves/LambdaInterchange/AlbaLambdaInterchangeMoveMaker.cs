#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaLambdaInterchangeMoveMaker", "Peforms a lambda interchange moves on a given VRP encoding and updates the quality. It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaLambdaInterchangeMoveMaker : AlbaMoveMaker, IAlbaLambdaInterchangeMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<AlbaLambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<AlbaLambdaInterchangeMove>)Parameters["AlbaLambdaInterchangeMove"]; }
    }

    [StorableConstructor]
    protected AlbaLambdaInterchangeMoveMaker(bool deserializing) : base(deserializing) { }
    protected AlbaLambdaInterchangeMoveMaker(AlbaLambdaInterchangeMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }
    public AlbaLambdaInterchangeMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<AlbaLambdaInterchangeMove>("AlbaLambdaInterchangeMove", "The move to make."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeMoveMaker(this, cloner);
    }

    public static void Apply(AlbaEncoding solution, AlbaLambdaInterchangeMove move) {
      AlbaLambdaInterchangeManipulator.Apply(
        solution,
        move.Tour1, move.Position1, move.Length1,
        move.Tour2, move.Position2, move.Length2);
    }

    public override IOperation Apply() {
      IOperation next = base.Apply();

      AlbaLambdaInterchangeMove move = LambdaInterchangeMoveParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      //perform move
      VRPToursParameter.ActualValue = move.MakeMove();

      quality.Value = moveQuality.Value;

      return next;
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("LambdaInterchangeMoveMaker", "Peforms a lambda interchange moves on a given Alba VRP encoding and updates the quality.")]
  [StorableClass]
  public class LambdaInterchangeMoveMaker : AlbaMoveMaker, IAlbaLambdaInterchangeMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<LambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<LambdaInterchangeMove>)Parameters["LambdaInterchangeMove"]; }
    }

    [StorableConstructor]
    private LambdaInterchangeMoveMaker(bool deserializing) : base(deserializing) { }

    public LambdaInterchangeMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<LambdaInterchangeMove>("LambdaInterchangeMove", "The move to make."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
    }

    public static void Apply(AlbaEncoding solution, LambdaInterchangeMove move) {
      LambdaInterchangeManipulator.Apply(
        solution,
        move.Tour1, move.Position1, move.Length1,
        move.Tour2, move.Position2, move.Length2);
    }

    public override IOperation Apply() {
      IOperation next = base.Apply();
      
      AlbaEncoding solution = VRPToursParameter.ActualValue as AlbaEncoding;

      LambdaInterchangeMove move = LambdaInterchangeMoveParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
     
      //perform move
      Apply(solution, move);

      quality.Value = moveQuality.Value;

      return next;
    }
  }
}

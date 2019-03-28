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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PRVUniformOnePositionManipulator", "Represents a manipulation operation inserting parts of the individual at another position.")]
  [StorableType("5C6A1D79-88DD-42D7-94C1-57C16BA44148")]
  public class PRVUniformOnePositionManipulator : PRVManipulator {

    [StorableConstructor]
    protected PRVUniformOnePositionManipulator(StorableConstructorFlag _) : base(_) { }
    protected PRVUniformOnePositionManipulator(PRVUniformOnePositionManipulator original, Cloner cloner) : base(original, cloner) { }
    public PRVUniformOnePositionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVUniformOnePositionManipulator(this, cloner);
    }

    public static void Apply(IRandom random, PRVEncoding individual, int numberOfRules) {
      UniformOnePositionManipulator.Apply(random, individual.PriorityRulesVector, new IntMatrix(new int[,] { { 0, numberOfRules } }));
    }

    protected override void Manipulate(IRandom random, PRVEncoding individual, int numberOfRules) {
      Apply(random, individual, numberOfRules);
    }
  }
}

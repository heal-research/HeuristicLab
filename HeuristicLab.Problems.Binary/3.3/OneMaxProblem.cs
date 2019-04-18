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

using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;

namespace HeuristicLab.Problems.Binary {
  [Item("One Max Problem", "Represents a problem whose objective is to maximize the number of true values.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 210)]
  [StorableType("A290ADDE-33F5-4607-ABC0-19349CD0FBF1")]
  public class OneMaxProblem : BinaryVectorProblem {
    public override bool Maximization {
      get { return true; }
    }

    public OneMaxProblem() : base() {
      Encoding.Length = 10;
      BestKnownQuality = Encoding.Length;
    }

    [StorableConstructor]
    protected OneMaxProblem(StorableConstructorFlag _) : base(_) { }

    protected OneMaxProblem(OneMaxProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxProblem(this, cloner);
    }

    public override double Evaluate(BinaryVector vector, IRandom random) {
      return vector.Count(b => b);
    }

    protected override void LengthParameter_ValueChanged(object sender, EventArgs e) {
      base.LengthParameter_ValueChanged(sender, e);
      BestKnownQuality = Length;
    }
  }
}

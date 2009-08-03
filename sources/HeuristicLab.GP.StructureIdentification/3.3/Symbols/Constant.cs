#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.GP.StructureIdentification {
  public sealed class Constant : Terminal {
    public const string VALUE = "Value";

    public override string Description {
      get { return "Returns the value of local variable 'Value'."; }
    }

    public Constant()
      : base() {
      SetupInitialization();
      SetupManipulation();
    }

    public override IFunctionTree GetTreeNode() {
      return new ConstantFunctionTree(this);
    }

    private void SetupInitialization() {
      // initialization operator
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor initSeq = new SequentialProcessor();
      UniformRandomizer randomizer = new UniformRandomizer();
      randomizer.Min = -20.0;
      randomizer.Max = 20.0;

      combinedOp.OperatorGraph.AddOperator(initSeq);
      combinedOp.OperatorGraph.AddOperator(randomizer);
      combinedOp.OperatorGraph.InitialOperator = initSeq;
      initSeq.AddSubOperator(randomizer);
      Initializer = combinedOp;
    }

    private void SetupManipulation() {
      // manipulation operator
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor manipulationSeq = new SequentialProcessor();
      NormalRandomAdder valueAdder = new NormalRandomAdder();
      valueAdder.Mu = 0.0;
      valueAdder.Sigma = 1.0;

      combinedOp.OperatorGraph.AddOperator(manipulationSeq);
      combinedOp.OperatorGraph.AddOperator(valueAdder);
      combinedOp.OperatorGraph.InitialOperator = manipulationSeq;
      manipulationSeq.AddSubOperator(valueAdder);
      Manipulator = combinedOp;
    }
  }
}

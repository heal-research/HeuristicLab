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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public class StructIdProblemInjector : OperatorBase {
    public override string Description {
      get { return @"Injects the necessary variables for a structure identification problem."; }
    }

    public StructIdProblemInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Set to false as structure identification is a minimization problem", typeof(BoolData), VariableKind.New));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.New));
      AddVariable(new Variable("Dataset", new Dataset()));
      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("TargetVariable", new IntData()));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "MaxTreeHeight", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("MaxTreeHeight", new IntData(1)));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "MaxTreeSize", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("MaxTreeSize", new IntData(1)));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "First sample to evaluate in training", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("TrainingSamplesStart", new IntData(0)));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "Last sample to evaluate in training", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("TrainingSamplesEnd", new IntData(0)));
    }

    public override IView CreateView() {
      return new StructIdProblemInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      scope.AddVariable(new Variable(GetVariableInfo("Maximization").ActualName, new BoolData(false)));
      scope.AddVariable(new Variable(GetVariableInfo("Dataset").ActualName, (IItem)GetVariable("Dataset").Value.Clone()));
      scope.AddVariable(new Variable(GetVariableInfo("TargetVariable").ActualName, (IItem)GetVariable("TargetVariable").Value.Clone()));
      scope.AddVariable(new Variable(GetVariableInfo("MaxTreeHeight").ActualName, (IItem)GetVariable("MaxTreeHeight").Value.Clone()));
      scope.AddVariable(new Variable(GetVariableInfo("MaxTreeSize").ActualName, (IItem)GetVariable("MaxTreeSize").Value.Clone()));
      scope.AddVariable(new Variable(GetVariableInfo("TrainingSamplesStart").ActualName, (IItem)GetVariable("TrainingSamplesStart").Value.Clone()));
      scope.AddVariable(new Variable(GetVariableInfo("TrainingSamplesEnd").ActualName, (IItem)GetVariable("TrainingSamplesEnd").Value.Clone()));
      return null;
    }
  }
}

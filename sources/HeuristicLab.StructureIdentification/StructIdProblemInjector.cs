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
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.New));
      GetVariableInfo("Dataset").Local = true;
      AddVariable(new Variable("Dataset", new Dataset()));

      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), VariableKind.New));
      GetVariableInfo("TargetVariable").Local = true;
      AddVariable(new Variable("TargetVariable", new IntData()));

      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "TrainingSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("TrainingSamplesStart").Local = true;
      AddVariable(new Variable("TrainingSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "TrainingSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("TrainingSamplesEnd").Local = true;
      AddVariable(new Variable("TrainingSamplesEnd", new IntData()));

      AddVariableInfo(new VariableInfo("ValidationSamplesStart", "ValidationSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("ValidationSamplesStart").Local = true;
      AddVariable(new Variable("ValidationSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("ValidationSamplesEnd", "ValidationSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("ValidationSamplesEnd").Local = true;
      AddVariable(new Variable("ValidationSamplesEnd", new IntData()));

      AddVariableInfo(new VariableInfo("TestSamplesStart", "TestSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("TestSamplesStart").Local = true;
      AddVariable(new Variable("TestSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("TestSamplesEnd", "TestSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("TestSamplesEnd").Local = true;
      AddVariable(new Variable("TestSamplesEnd", new IntData()));
    }

    public override IView CreateView() {
      return new StructIdProblemInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      foreach(VariableInfo info in VariableInfos) {
        if(info.Local) {
          scope.AddVariable(new Variable(info.ActualName, (IItem)GetVariable(info.FormalName).Value.Clone()));
        }
      }
      return null;
    }
  }
}

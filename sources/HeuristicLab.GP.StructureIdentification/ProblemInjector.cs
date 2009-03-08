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
using Core = HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  public class ProblemInjector : Core.OperatorBase {
    public override string Description {
      get { return @"Injects the necessary variables for a structure identification problem."; }
    }

    public ProblemInjector()
      : base() {
      AddVariableInfo(new Core.VariableInfo("Dataset", "Dataset", typeof(Dataset), Core.VariableKind.New));
      GetVariableInfo("Dataset").Local = true;
      AddVariable(new Core.Variable("Dataset", new Dataset()));

      AddVariableInfo(new Core.VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("TargetVariable").Local = true;
      AddVariable(new Core.Variable("TargetVariable", new IntData()));

      AddVariableInfo(new Core.VariableInfo("AllowedFeatures", "Indexes of allowed input variables", typeof(ItemList<IntData>), Core.VariableKind.New));
      GetVariableInfo("AllowedFeatures").Local = true;
      AddVariable(new Core.Variable("AllowedFeatures", new ItemList<IntData>()));

      AddVariableInfo(new Core.VariableInfo("TrainingSamplesStart", "TrainingSamplesStart", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("TrainingSamplesStart").Local = true;
      AddVariable(new Core.Variable("TrainingSamplesStart", new IntData()));

      AddVariableInfo(new Core.VariableInfo("TrainingSamplesEnd", "TrainingSamplesEnd", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("TrainingSamplesEnd").Local = true;
      AddVariable(new Core.Variable("TrainingSamplesEnd", new IntData()));

      AddVariableInfo(new Core.VariableInfo("ValidationSamplesStart", "ValidationSamplesStart", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("ValidationSamplesStart").Local = true;
      AddVariable(new Core.Variable("ValidationSamplesStart", new IntData()));

      AddVariableInfo(new Core.VariableInfo("ValidationSamplesEnd", "ValidationSamplesEnd", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("ValidationSamplesEnd").Local = true;
      AddVariable(new Core.Variable("ValidationSamplesEnd", new IntData()));

      AddVariableInfo(new Core.VariableInfo("TestSamplesStart", "TestSamplesStart", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("TestSamplesStart").Local = true;
      AddVariable(new Core.Variable("TestSamplesStart", new IntData()));

      AddVariableInfo(new Core.VariableInfo("TestSamplesEnd", "TestSamplesEnd", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("TestSamplesEnd").Local = true;
      AddVariable(new Core.Variable("TestSamplesEnd", new IntData()));
    }

    public override Core.IView CreateView() {
      return new ProblemInjectorView(this);
    }

    public override Core.IOperation Apply(Core.IScope scope) {
      foreach(Core.VariableInfo info in VariableInfos) {
        if(info.Local) {
          Core.IVariable var = GetVariable(info.FormalName);
          if(var != null) scope.AddVariable(new Core.Variable(info.ActualName, (Core.IItem)var.Value.Clone()));
        }
      }
      return null;
    }
  }
}

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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class TimeSeriesProblemInjector : ProblemInjector {

    public override string Description {
      get {
        return "Problem injector for time series problems.";
      }
    }

    public TimeSeriesProblemInjector()
      : base() {
      AddVariableInfo(new Core.VariableInfo("Autoregressive", "Autoregressive modelling includes previous values of the target variable to predict future values.", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo("Autoregressive").Local = true;
      AddVariable(new Core.Variable("Autoregressive", new BoolData()));

      AddVariableInfo(new Core.VariableInfo("MaxTimeOffset", "MaxTimeOffset", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("MaxTimeOffset").Local = true;
      AddVariable(new Core.Variable("MaxTimeOffset", new IntData(0)));

      AddVariableInfo(new Core.VariableInfo("MinTimeOffset", "MinTimeOffset", typeof(IntData), Core.VariableKind.New));
      GetVariableInfo("MinTimeOffset").Local = true;
      AddVariable(new Core.Variable("MinTimeOffset", new IntData(-1)));
    }
  }
}

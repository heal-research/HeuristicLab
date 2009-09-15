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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Modeling;
using HeuristicLab.GP;
using HeuristicLab.Random;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP.StructureIdentification.Classification;

namespace HeuristicLab.LinearRegression {
  public class LinearTimeSeriesPrognosis : LinearRegression, ITimeSeriesAlgorithm {

    public override string Name { get { return "LinearTimeSeriesPrognosis"; } }
    public int MaxTimeOffset {
      get { return GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data = value; }
    }

    public int MinTimeOffset {
      get { return GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data = value; }
    }


    public LinearTimeSeriesPrognosis()
      : base() {
      MaxTimeOffset = -1;
      MinTimeOffset = -1;
    }

    protected override IOperator CreateProblemInjector() {
      return DefaultTimeSeriesOperators.CreateProblemInjector();
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector injector = base.CreateGlobalInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("MinTimeOffset", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTimeOffset", new IntData()));
      return injector;
    }

    protected override IOperator CreateModelAnalyzerOperator() {
      return DefaultTimeSeriesOperators.CreatePostProcessingOperator();
    }

    protected internal virtual IAnalyzerModel CreateLRModel(IScope bestModelScope) {
      var model = new AnalyzerModel();
      DefaultTimeSeriesOperators.PopulateAnalyzerModel(bestModelScope, model);
      return model;
    }
  }
}

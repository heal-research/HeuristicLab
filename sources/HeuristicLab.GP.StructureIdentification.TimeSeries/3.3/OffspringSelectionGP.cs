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
using HeuristicLab.Logging;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class OffspringSelectionGP : HeuristicLab.GP.StructureIdentification.OffspringSelectionGP, ITimeSeriesAlgorithm {
    public override IOperator ProblemInjector {
      get {
        CombinedOperator algo = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
        return algo.OperatorGraph.InitialOperator.SubOperators[1];
      }
      set {
        CombinedOperator algo = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
        algo.OperatorGraph.InitialOperator.RemoveSubOperator(1);
        value.Name = "ProblemInjector";
        algo.OperatorGraph.InitialOperator.AddSubOperator(value, 1);
      }
    }

    protected override IOperator CreateFunctionLibraryInjector() {
      return new FunctionLibraryInjector();
    }

    protected override IOperator CreateProblemInjector() {
      return new ProblemInjector();
    }

    protected override IOperator CreateBestSolutionProcessor() {
      IOperator seq = base.CreateBestSolutionProcessor();
      seq.AddSubOperator(StandardGP.BestSolutionProcessor);
      return seq;
    }

    public override IEditor CreateEditor() {
      return new OffspringSelectionGpEditor(this);
    }

    public override IView CreateView() {
      return new OffspringSelectionGpEditor(this);
    }
  }
}

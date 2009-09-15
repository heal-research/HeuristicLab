
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
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorClassification : SupportVectorRegression, IClassificationAlgorithm {

    public override string Name { get { return "SupportVectorClassification"; } }

    public SupportVectorClassification()
      : base() {
      SvmType = "NU_SVC";
      CostList = new DoubleArrayData(new double[] { 1.0 });
      MaxCostIndex = 1;
    }

    protected override IOperator CreateModelAnalyzerOperator() {
      return DefaultClassificationOperators.CreatePostProcessingOperator();
    }


    protected override IAnalyzerModel CreateSVMModel(IScope bestModelScope) {
      IAnalyzerModel model = new AnalyzerModel();
      model.SetMetaData("Cost", bestModelScope.GetVariableValue<DoubleData>("Cost", false).Data);
      model.SetMetaData("Nu", bestModelScope.GetVariableValue<DoubleData>("Nu", false).Data);
      DefaultClassificationOperators.PopulateAnalyzerModel(bestModelScope, model);
      return model;
    }
  }
}

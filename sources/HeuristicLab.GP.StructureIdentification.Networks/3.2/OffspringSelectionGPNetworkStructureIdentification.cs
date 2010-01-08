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

using HeuristicLab.Core;
using HeuristicLab.Modeling;
using HeuristicLab.Operators;
using HeuristicLab.Data;

namespace HeuristicLab.GP.StructureIdentification.Networks {
  public class OffspringSelectionGPNetworkStructureIdentification : HeuristicLab.GP.StructureIdentification.OffspringSelectionGPRegression, IClassificationAlgorithm {

    public override string Name {
      get {
        return base.Name + " - Network StructureIdentification";
      }
    }

    //protected override IOperator CreateProblemInjector() {
    //  return DefaultNetworkStructureIdentificationOperators.CreateProblemInjector();
    //}

    //protected override IOperator CreateModelAnalyzerOperator() {
    //  return DefaultNetworkStructureIdentificationOperators.CreatePostProcessingOperator();
    //}

    //protected override IAnalyzerModel CreateGPModel() {
    //  IScope bestModelScope = Engine.GlobalScope.SubScopes[0];
    //  var model = new AnalyzerModel();

    //  model.SetMetaData("SelectionPressure", bestModelScope.GetVariableValue<DoubleData>("SelectionPressure", false).Data);
    //  DefaultStructureIdentificationOperators.PopulateAnalyzerModel(bestModelScope, model);
    //  DefaultNetworkStructureIdentificationOperators.PopulateAnalyzerModel(bestModelScope, model);

    //  return model;
    //}
  }
}

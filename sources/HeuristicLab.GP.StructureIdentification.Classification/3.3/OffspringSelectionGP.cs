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

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class OffspringSelectionGP : HeuristicLab.GP.StructureIdentification.OffspringSelectionGP, IClassificationAlgorithm {

    protected override IOperator CreateProblemInjector() {
      return DefaultClassificationAlgorithmOperators.CreateProblemInjector();
    }

    protected override IOperator CreatePostProcessingOperator() {
      return DefaultClassificationAlgorithmOperators.CreatePostProcessingOperator();
    }

    protected override IAnalyzerModel CreateGPModel() {
      IAnalyzerModel model = base.CreateGPModel();
      DefaultClassificationAlgorithmOperators.SetModelData(model, Engine.GlobalScope.SubScopes[0]);
      return model;
    }
  }
}

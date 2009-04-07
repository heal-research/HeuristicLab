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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Server {
  public class RandomDispatcher : DispatcherBase {
    private Random random;
    public RandomDispatcher(IStore store)
      : base(store) {
      random = new Random();
    }

    public override Algorithm SelectAlgorithm(DataSet dataSet, int targetVariable, Algorithm[] possibleAlgorithms) {
      return possibleAlgorithms[random.Next(possibleAlgorithms.Length)];
    }

    public override ModelComplexity SelectComplexity(DataSet dataSet, int targetVariable, Algorithm algorithm, ModelComplexity[] possibleComplexities) {
      return possibleComplexities[random.Next(possibleComplexities.Length)];
    }

    public override Entity SelectDataSet(Entity[] datasets) {
      return datasets[random.Next(datasets.Length)];
    }

    public override int SelectTargetVariable(DataSet dataSet, int[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }
  }
}

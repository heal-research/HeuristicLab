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
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public class RandomDispatcher : DispatcherBase {
    private Random random;
    public RandomDispatcher(IStore store)
      : base(store) {
      random = new Random();
    }

    public override IAlgorithm SelectAlgorithm(DataSet dataSet, int targetVariable, LearningTask learningTask) {
      DiscoveryService ds = new DiscoveryService();
      IAlgorithm[] algos = ds.GetInstances<IAlgorithm>();
      switch (learningTask) {
        case LearningTask.Regression: {
            var regressionAlgos = algos.Where(a => (a as IClassificationAlgorithm) == null && (a as ITimeSeriesAlgorithm) == null);
            if (regressionAlgos.Count() == 0) return null;
            return regressionAlgos.ElementAt(random.Next(regressionAlgos.Count()));
          }
        case LearningTask.Classification: {
            var classificationAlgos = algos.Where(a => (a as IClassificationAlgorithm) != null);
            if (classificationAlgos.Count() == 0) return null;
            return classificationAlgos.ElementAt(random.Next(classificationAlgos.Count()));
          }
        case LearningTask.TimeSeries: {
            var timeSeriesAlgos = algos.Where(a => (a as ITimeSeriesAlgorithm) != null);
            if (timeSeriesAlgos.Count() == 0) return null;
            return timeSeriesAlgos.ElementAt(random.Next(timeSeriesAlgos.Count()));
          }
      }
      return null;
    }

    public override Entity SelectDataSet(Entity[] datasets) {
      return datasets[random.Next(datasets.Length)];
    }

    public override int SelectTargetVariable(DataSet dataSet, int[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }
  }
}

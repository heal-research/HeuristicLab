#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeatureSelectionInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Feature Selection Problems"; }
    }
    public override string Description {
      get { return "A set of artificial feature selection benchmark problems"; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
      var sizes = new int[] { 50, 100, 200 };
      var pp = new double[] { 0.1, 0.25, 0.5 };
      var noiseRatios = new double[] { 0.01, 0.05, 0.1, 0.2 };
      foreach (var size in sizes) {
        foreach (var p in pp) {
          foreach (var noiseRatio in noiseRatios) {
            descriptorList.Add(new FeatureSelection(size, p, noiseRatio));
          }
        }
      }
      return descriptorList;
    }
  }
}

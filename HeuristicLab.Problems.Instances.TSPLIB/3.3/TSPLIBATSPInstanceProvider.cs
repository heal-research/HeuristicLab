#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBATSPInstanceProvider : TSPLIBInstanceProvider<ATSPData> {

    public override string Name {
      get { return "TSPLIB (asymmetric TSP)"; }
    }

    public override string Description {
      get { return "Traveling Salesman Problem Library"; }
    }

    protected override string FileExtension { get { return "atsp"; } }

    protected override ATSPData LoadInstance(TSPLIBParser parser, IDataDescriptor descriptor = null) {
      var instance = new ATSPData();

      parser.Parse();
      instance.Dimension = parser.Dimension;
      instance.Coordinates = parser.DisplayVertices != null ? parser.DisplayVertices : parser.Vertices;
      instance.Distances = parser.Distances;

      instance.Name = parser.Name;
      instance.Description = parser.Comment
        + Environment.NewLine + Environment.NewLine
        + GetInstanceDescription();

      return instance;
    }

    protected override void LoadSolution(TSPLIBParser parser, ATSPData instance) {
      parser.Parse();
      instance.BestKnownTour = parser.Tour.FirstOrDefault();
    }
  }
}

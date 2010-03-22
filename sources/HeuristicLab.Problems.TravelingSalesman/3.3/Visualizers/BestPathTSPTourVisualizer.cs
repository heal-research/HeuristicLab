#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator for visualizing the best tour of Traveling Salesman Problems given in path representation using city coordinates.
  /// </summary>
  [Item("BestPathTSPTourVisualizer", "An operator for visualizing the best tour of Traveling Salesman Problems given in path representation using city coordinates.")]
  [StorableClass]
  public sealed class BestPathTSPTourVisualizer : SingleSuccessorOperator, IPathCoordinatesTSPSolutionsVisualizer {
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<ItemArray<Permutation>> PermutationParameter {
      get { return (ILookupParameter<ItemArray<Permutation>>)Parameters["Permutation"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public ILookupParameter<PathTSPTour> PathTSPTourParameter {
      get { return (ILookupParameter<PathTSPTour>)Parameters["PathTSPTour"]; }
    }
    ILookupParameter ISolutionsVisualizer.VisualizationParameter {
      get { return PathTSPTourParameter; }
    }

    public BestPathTSPTourVisualizer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new SubScopesLookupParameter<Permutation>("Permutation", "The TSP solutions given in path representation from which the best solution should be visualized."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The qualities of the TSP solutions which should be visualized."));
      Parameters.Add(new LookupParameter<PathTSPTour>("PathTSPTour", "The visual representation of the best TSP solution."));
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      ItemArray<Permutation> permutations = PermutationParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      PathTSPTour tour = PathTSPTourParameter.ActualValue;
      if (tour == null) PathTSPTourParameter.ActualValue = new PathTSPTour(coordinates, permutations[i]);
      else {
        tour.Coordinates = coordinates;
        tour.Permutation = permutations[i];
      }
      return base.Apply();
    }
  }
}

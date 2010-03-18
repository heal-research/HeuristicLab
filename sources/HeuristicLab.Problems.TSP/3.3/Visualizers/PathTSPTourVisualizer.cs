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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// An operator for visualizing tours of Traveling Salesman Problems given in path representation.
  /// </summary>
  [Item("PathTSPTourVisualizer", "An operator for visualizing tours of Traveling Salesman Problems given in path representation.")]
  [StorableClass]
  public sealed class PathTSPTourVisualizer : SingleSuccessorOperator, IPathTSPTourVisualizer {
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<IItem> VisualizationParameter {
      get { return (ILookupParameter<IItem>)Parameters["Visualization"]; }
    }

    public PathTSPTourVisualizer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be visualized."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "An item which represents the visualization of the given TSP solution."));
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      TSPTour tour = new TSPTour(coordinates, permutation);
      VisualizationParameter.ActualValue = tour;
      return base.Apply();
    }
  }
}

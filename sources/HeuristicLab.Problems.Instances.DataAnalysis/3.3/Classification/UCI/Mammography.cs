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
namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Mammography : IUCIDataDescriptor {
    public string Name { get { return "Mammography"; } }
    public string Description {
      get {
        return "Data Set Information: Features are computed from a digitized image of a fine needle aspirate (FNA) of a breast mass."
        + "Website: http://archive.ics.uci.edu/ml/datasets/Breast+Cancer+Wisconsin+%28Diagnostic%29" + Environment.NewLine
        + "Attribute Information:" + Environment.NewLine
        + "1) ID number" + Environment.NewLine
        + "2) Diagnosis (M = malignant, B = benign)" + Environment.NewLine
        + "3-32)" + Environment.NewLine + Environment.NewLine
        + "Ten real-valued features are computed for each cell nucleus:" + Environment.NewLine + Environment.NewLine
        + "a) radius (mean of distances from center to points on the perimeter)" + Environment.NewLine
        + "b) texture (standard deviation of gray-scale values)" + Environment.NewLine
        + "c) perimeter" + Environment.NewLine
        + "d) area" + Environment.NewLine
        + "e) smoothness (local variation in radius lengths)" + Environment.NewLine
        + "f) compactness (perimeter^2 / area - 1.0)" + Environment.NewLine
        + "g) concavity (severity of concave portions of the contour)" + Environment.NewLine
        + "h) concave points (number of concave portions of the contour)" + Environment.NewLine
        + "i) symmetry" + Environment.NewLine
        + "j) fractal dimension (\"coastline approximation\" - 1)";
      }
    }
    public string Donor { get { return "M. Elter"; } }
    public int Year { get { return 2007; } }
  }
}

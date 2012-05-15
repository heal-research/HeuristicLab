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
namespace HeuristicLab.Problems.Instances.Classification {
  public class Iris : IDataDescriptor {
    public string Name { get { return "iris"; } }
    public string Description {
      get {
        return "Data Set Information: This is perhaps the best known database to be found in the pattern "
        + "recognition literature. Fisher's paper is a classic in the field and is referenced frequently to this "
        + "day. (See Duda & Hart, for example.) The data set contains 3 classes of 50 instances each, where each class "
        + "refers to a type of iris plant. One class is linearly separable from the other 2; the latter are NOT linearly "
        + "separable from each other." + Environment.NewLine
        + "Website: http://archive.ics.uci.edu/ml/datasets/Iris" + Environment.NewLine
        + "Attribute Information:" + Environment.NewLine
        + "1. sepal length in cm" + Environment.NewLine
        + "2. sepal width in cm" + Environment.NewLine
        + "3. petal length in cm" + Environment.NewLine
        + "4. petal width in cm" + Environment.NewLine
        + "5. class:" + Environment.NewLine
        + "-- Iris Setosa" + Environment.NewLine
        + "-- Iris Versicolour" + Environment.NewLine
        + "-- Iris Virginica" + Environment.NewLine + Environment.NewLine
        + "Note: Iris Setosa = 0; Iris Versicolour = 1; Iris Virginica = 2;";
      }
    }
  }
}

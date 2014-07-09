#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class Iris : UCIDataDescriptor {
    public override string Filename { get { return "Iris"; } }
    public override string Description {
      get {
        return "Data Set Information:" + Environment.NewLine
        + "This is perhaps the best known database to be found in the pattern " + Environment.NewLine
        + "recognition literature. Fisher's paper is a classic in the field and "
        + "is referenced frequently to this day. (See Duda & Hart, for example.) "
        + "The data set contains 3 classes of 50 instances each, where each class "
        + "refers to a type of iris plant. One class is linearly separable from the "
        + "other 2; the latter are NOT linearly separable from each other." + Environment.NewLine
        + "Predicted attribute: class of iris plant." + Environment.NewLine
        + "This is an exceedingly simple domain." + Environment.NewLine + Environment.NewLine
        + "The classes have been converted in the following way" + Environment.NewLine
        + "Iris-setosa     = 0" + Environment.NewLine
        + "Iris-versicolor = 1" + Environment.NewLine
        + "Iris-virginica  = 2";
      }
    }
    public override string Donor { get { return "M. Marshall"; } }
    public override int Year { get { return 1988; } }

    protected override string TargetVariable { get { return "class"; } }
    protected override string[] VariableNames {
      get { return new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "class" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 150; } }
  }
}

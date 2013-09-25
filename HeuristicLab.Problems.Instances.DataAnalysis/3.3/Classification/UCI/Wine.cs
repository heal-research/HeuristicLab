#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class Wine : UCIDataDescriptor {
    public override string Filename { get { return "Wine"; } }
    public override string Description {
      get {
        return "These data are the results of a chemical analysis of wines grown in the same region " +
        "in Italy but derived from three different cultivars. The analysis determined the quantities " +
        "of 13 constituents found in each of the three types of wines." + Environment.NewLine + Environment.NewLine +
        "I think that the initial data set had around 30 variables, but for some reason I only have the " +
        "13 dimensional version. I had a list of what the 30 or so variables were, but a.) I lost it, and b.), " +
        "I would not know which 13 variables are included in the set." + Environment.NewLine + Environment.NewLine +
        "The attributes are (dontated by Riccardo Leardi, riclea '@' anchem.unige.it )" + Environment.NewLine +
        "1) Alcohol" + Environment.NewLine +
        "2) Malic acid" + Environment.NewLine +
        "3) Ash" + Environment.NewLine +
        "4) Alcalinity of ash" + Environment.NewLine +
        "5) Magnesium" + Environment.NewLine +
        "6) Total phenols" + Environment.NewLine +
        "7) Flavanoids" + Environment.NewLine +
        "8) Nonflavanoid phenols" + Environment.NewLine +
        "9) Proanthocyanins" + Environment.NewLine +
        "10)Color intensity" + Environment.NewLine +
        "11)Hue" + Environment.NewLine +
        "12)OD280/OD315 of diluted wines" + Environment.NewLine +
        "13)Proline" + Environment.NewLine + Environment.NewLine +
        "In a classification context, this is a well posed problem with \"well behaved\" class structures. A " +
        "good data set for first testing of a new classifier, but not very challenging. ";
      }
    }
    public override string Donor { get { return "S. Aeberhard"; } }
    public override int Year { get { return 1991; } }

    protected override string TargetVariable { get { return "Class"; } }
    protected override string[] VariableNames {
      get { return new string[] { "Alcohol", "Malic acid", "Ash", "Alcalinity of ash", "Magnesium", "Total phenols", "Flavanoids", "Nonflavanoid phenols", "Proanthocyanins", "Color intensity", "Hue", "OD280/OD315 of diluted wines", "Proline", "Class" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "Alcohol", "Malic acid", "Ash", "Alcalinity of ash", "Magnesium", "Total phenols", "Flavanoids", "Nonflavanoid phenols", "Proanthocyanins", "Color intensity", "Hue", "OD280/OD315 of diluted wines", "Proline" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 119; } }
    protected override int TestPartitionStart { get { return 119; } }
    protected override int TestPartitionEnd { get { return 178; } }
  }
}

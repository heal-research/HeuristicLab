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
  public class WisconsinDiagnosticBreastCancer : UCIDataDescriptor {
    public override string Filename { get { return "Wisconsin Diagnostic Breast Cancer"; } }
    public override string Description {
      get {
        return "Data Set Information:" + Environment.NewLine
        + "Results:" + Environment.NewLine
        + "- predicting field 2, diagnosis: B = benign, M = malignant" + Environment.NewLine
        + "- sets are linearly separable using all 30 input features" + Environment.NewLine
        + "- best predictive accuracy obtained using one separating plane" + Environment.NewLine
        + "  in the 3-D space of Worst Area, Worst Smoothness and" + Environment.NewLine
        + "  Mean Texture.  Estimated accuracy 97.5% using repeated" + Environment.NewLine
        + "  10-fold crossvalidations.  Classifier has correctly" + Environment.NewLine
        + "  diagnosed 176 consecutive new patients as of November 1995." + Environment.NewLine + Environment.NewLine
        + "The classes have been converted in the following way" + Environment.NewLine
        + "B = benign    = 0" + Environment.NewLine
        + "M = malignant = 1";
      }
    }
    public override string Donor { get { return "Nick Street"; } }
    public override int Year { get { return 1995; } }

    protected override string TargetVariable { get { return "Diagnosis"; } }
    protected override string[] VariableNames {
      get { return new string[] { "ID number", "Diagnosis", "mean radius", "mean texture", "mean perimeter", "mean area", "mean smoothness", "mean compactness", "mean concavity", "mean concave points", "mean symmetry", "mean fractal dimension", "radius SE", "texture SE", "perimeter SE", "area SE", "smoothness SE", "compactness SE", "concavity SE", "concave points SE", "symmetry SE", "fractal dimension SE", "worst radius", "worst texture", "worst perimeter", "worst area", "worst smoothness", "worst compactness", "worst concavity", "worst concave points", "worst symmetry", "worst fractal dimension" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "mean radius", "mean texture", "mean perimeter", "mean area", "mean smoothness", "mean compactness", "mean concavity", "mean concave points", "mean symmetry", "mean fractal dimension", "radius SE", "texture SE", "perimeter SE", "area SE", "smoothness SE", "compactness SE", "concavity SE", "concave points SE", "symmetry SE", "fractal dimension SE", "worst radius", "worst texture", "worst perimeter", "worst area", "worst smoothness", "worst compactness", "worst concavity", "worst concave points", "worst symmetry", "worst fractal dimension" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 380; } }
    protected override int TestPartitionStart { get { return 380; } }
    protected override int TestPartitionEnd { get { return 569; } }
  }
}

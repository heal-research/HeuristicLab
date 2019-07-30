#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class Thyroid : UCIDataDescriptor {
    public override string Filename { get { return "Thyroid"; } }
    public override string Description {
      get {
        return "Thyroid gland data. ('normal', hypo and hyper functioning)" + Environment.NewLine + Environment.NewLine +
        "Attr. no :" + Environment.NewLine +
        "1: Class attribute (1 = normal, 2 = hyper, 3 = hypo)" + Environment.NewLine +
        "2: T3-resin uptake test. (A percentage)" + Environment.NewLine +
        "3: Total Serum thyroxin as measured by the isotopic displacement method." + Environment.NewLine +
        "4: Total serum triiodothyronine as measured by radioimmuno	assay." + Environment.NewLine +
        "5: basal thyroid-stimulating hormone (TSH) as measured by radioimmuno assay." + Environment.NewLine +
        "6: Maximal absolute difference of TSH value after injection of 200 micro grams of " +
        "thyrotropin-releasing hormone as compared to the basal value." + Environment.NewLine +
        "All attributes are continuous.";
      }
    }
    public override string Donor { get { return "S. Aeberhard"; } }
    public override int Year { get { return 1992; } }

    protected override string TargetVariable { get { return "X000"; } }
    protected override string[] VariableNames {
      get { return new string[] { "X001", "X002", "X003", "X004", "X005", "X000" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "X001", "X002", "X003", "X004", "X005" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 143; } }
    protected override int TestPartitionStart { get { return 143; } }
    protected override int TestPartitionEnd { get { return 215; } }
  }
}

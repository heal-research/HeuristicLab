#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class Parkinsons : UCIDataDescriptor {
    public override string Filename { get { return "Parkinsons"; } }
    public override string Description {
      get {
        return "Data Set Information:" + Environment.NewLine
        + "This dataset is composed of a range of biomedical voice measurements from 31 people, 23 with "
        + "Parkinson's disease (PD). Each column in the table is a particular voice measure, and each row "
        + "corresponds one of 195 voice recording from these individuals (\"name\" column). The main aim of "
        + "the data is to discriminate healthy people from those with PD, according to \"status\" column which "
        + "is set to 0 for healthy and 1 for PD." + Environment.NewLine
        + "Further details are contained in the following reference -- if you use this dataset, please cite: " + Environment.NewLine
        + "Max A. Little, Patrick E. McSharry, Eric J. Hunter, Lorraine O. Ramig (2008), 'Suitability of "
        + "dysphonia measurements for telemonitoring of Parkinson's disease', IEEE Transactions on Biomedical "
        + "Engineering (to appear)." + Environment.NewLine
        + "Note: The column \"name\" has been removed and the column \"status\" has been moved to the end.";
      }
    }
    public override string Donor { get { return "M. Little"; } }
    public override int Year { get { return 2008; } }

    protected override string TargetVariable { get { return "status"; } }
    protected override string[] VariableNames {
      get { return new string[] { "MDVP:Fo(Hz)", "MDVP:Fhi(Hz)", "MDVP:Flo(Hz)", "MDVP:Jitter(%)", "MDVP:Jitter(Abs)", "MDVP:RAP", "MDVP:PPQ", "Jitter:DDP", "MDVP:Shimmer", "MDVP:Shimmer(dB)", "Shimmer:APQ3", "Shimmer:APQ5", "MDVP:APQ", "Shimmer:DDA", "NHR", "HNR", "RPDE", "DFA", "spread1", "spread2", "D2", "PPE", "status" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "MDVP:Fo(Hz)", "MDVP:Fhi(Hz)", "MDVP:Flo(Hz)", "MDVP:Jitter(%)", "MDVP:Jitter(Abs)", "MDVP:RAP", "MDVP:PPQ", "Jitter:DDP", "MDVP:Shimmer", "MDVP:Shimmer(dB)", "Shimmer:APQ3", "Shimmer:APQ5", "MDVP:APQ", "Shimmer:DDA", "NHR", "HNR", "RPDE", "DFA", "spread1", "spread2", "D2", "PPE" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 130; } }
    protected override int TestPartitionStart { get { return 130; } }
    protected override int TestPartitionEnd { get { return 195; } }
  }
}

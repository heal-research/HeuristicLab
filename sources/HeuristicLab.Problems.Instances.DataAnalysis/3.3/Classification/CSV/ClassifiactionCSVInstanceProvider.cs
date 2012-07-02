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
using System.Collections.Generic;
using System.IO;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;
namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class ClassificationCSVInstanceProvider : ClassificationInstanceProvider {
    public override string Name {
      get { return "CSV Problem Provider"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac/hl/core/wiki/UsersFAQ#DataAnalysisImportFileFormat"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override bool CanSaveData {
      get { return true; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new List<IDataDescriptor>();
    }

    public override void SaveData(IClassificationProblemData instance, string path) {
      StringBuilder strBuilder = new StringBuilder();

      foreach (var variable in instance.InputVariables) {
        strBuilder.Append(variable + ";");
      }
      strBuilder.Remove(strBuilder.Length - 1, 1);
      strBuilder.AppendLine();

      Dataset dataset = instance.Dataset;

      for (int i = 0; i < dataset.Rows; i++) {
        for (int j = 0; j < dataset.Columns; j++) {
          strBuilder.Append(dataset.GetValue(i, j) + ";");
        }
        strBuilder.Remove(strBuilder.Length - 1, 1);
        strBuilder.AppendLine();
      }

      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(strBuilder);
      }
    }

    public override IClassificationProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }
  }
}

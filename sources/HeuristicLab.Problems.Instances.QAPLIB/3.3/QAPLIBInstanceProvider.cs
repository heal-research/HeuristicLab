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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HeuristicLab.Problems.Instances.QAPLIB {
  public class QAPLIBInstanceProvider : ProblemInstanceProvider<QAPData> {
    #region Reversed instances
    // These instances specified their best known solution in the wrong order
    private static HashSet<string> reversedSolutions = new HashSet<string>(new string[] {
      "bur26a",
      "bur26b",
      "bur26c",
      "bur26d",
      "bur26e",
      "bur26f",
      "bur26g",
      "bur26h",
      "chr12a",
      "chr12b",
      "chr12c",
      "chr15a",
      "chr15b",
      "chr15c",
      "chr18a",
      "chr18b",
      "chr20a",
      "chr20b",
      "chr20c",
      "chr22a",
      "chr22b",
      "chr25a",
      "esc16a",
      "esc16b",
      "esc16c",
      "esc16d",
      "esc16e",
      "esc16g",
      "esc16h",
      "esc16i",
      "esc16j",
      "esc32a",
      "esc32e",
      "esc32f",
      "esc32g",
      "had12",
      "had14",
      "had16",
      "had18",
      "had20",
      "kra32",
      "lipa20a",
      "lipa30a",
      "lipa40a",
      "lipa50a",
      "lipa60a",
      "lipa70a",
      "lipa80a",
      "lipa90a",
      "nug12",
      "nug14",
      "nug15",
      "nug16a",
      "nug16b",
      "nug17",
      "nug18",
      "nug20",
      "nug21",
      "nug22",
      "nug24",
      "nug25",
      "nug27",
      "nug28",
      "rou12",
      "rou15",
      "rou20",
      "scr12",
      "scr15",
      "scr20",
      "sko100a",
      "sko100b",
      "sko100c",
      "sko100d",
      "sko100e",
      "sko100f",
      "sko49",
      "sko81",
      "sko90",
      "ste36a",
      "ste36b",
      "tai100a",
      "tai100b",
      "tai12a",
      "tai12b",
      "tai150b",
      "tai15a",
      "tai15b",
      "tai17a",
      "tai20a",
      "tai20b",
      "tai256c",
      "tai25a",
      "tai25b",
      "tai30b",
      "tai35b",
      "tai40b",
      "tai50a",
      "tai50b",
      "tai60a",
      "tai60b",
      "tai64c",
      "tai80a",
      "tai80b",
      "wil100"
    });
    #endregion

    public override string Name {
      get { return "QAPLIB"; }
    }

    public override string Description {
      get { return "Quadratic Assignment Problem Library"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://www.seas.upenn.edu/qaplib/"); }
    }

    public override string ReferencePublication {
      get {
        return @"R. E. Burkard, S. E. Karisch, and F. Rendl. 1997.
QAPLIB - A Quadratic Assignment Problem Library.
Journal of Global Optimization, 10, pp. 391-403.";
      }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var solutions = Assembly.GetExecutingAssembly()
        .GetManifestResourceNames()
        .Where(x => x.EndsWith(".sln"))
        .ToDictionary(x => Path.GetFileNameWithoutExtension(x) + ".dat", x => x);

      return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".dat"))
          .OrderBy(x => x)
          .Select(x => new QAPLIBDataDescriptor(GetPrettyName(x), GetDescription(), x, solutions.ContainsKey(x) ? solutions[x] : String.Empty));
    }

    public override QAPData LoadData(IDataDescriptor id) {
      var descriptor = (QAPLIBDataDescriptor)id;
      using (var stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(descriptor.InstanceIdentifier)) {
        var parser = new QAPLIBParser();
        parser.Parse(stream);
        var instance = Load(parser);
        instance.Name = id.Name;
        instance.Description = id.Description;

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          using (Stream solStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(descriptor.SolutionIdentifier)) {
            var slnParser = new QAPLIBSolutionParser();
            slnParser.Parse(solStream, true);
            if (slnParser.Error != null) throw slnParser.Error;

            int[] assignment = slnParser.Assignment;
            if (reversedSolutions.Contains(instance.Name)) {
              assignment = (int[])slnParser.Assignment.Clone();
              for (int i = 0; i < assignment.Length; i++)
                assignment[slnParser.Assignment[i]] = i;
            }
            instance.BestKnownAssignment = assignment;
            instance.BestKnownQuality = slnParser.Quality;
          }
        }
        return instance;
      }
    }

    public override QAPData LoadData(string path) {
      var parser = new QAPLIBParser();
      parser.Parse(path);
      var instance = Load(parser);
      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();
      return instance;
    }

    public override void SaveData(QAPData instance, string path) {
      throw new NotSupportedException();
    }

    private QAPData Load(QAPLIBParser parser) {
      var instance = new QAPData();
      instance.Dimension = parser.Size;
      instance.Distances = parser.Distances;
      instance.Weights = parser.Weights;
      return instance;
    }

    private string GetPrettyName(string instanceIdentifier) {
      return Regex.Match(instanceIdentifier, GetType().Namespace + @"\.Data\.(.*)\.dat").Groups[1].Captures[0].Value;
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}

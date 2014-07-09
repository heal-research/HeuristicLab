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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using HeuristicLab.Problems.Instances;
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  public class EcbProblemInstanceProvider : ProblemInstanceProvider<IProblemData> {
    private class EcbDataDescriptor : IDataDescriptor {
      public string Name { get; set; }
      public string Description { get; set; }
    }

    public override string Name {
      get { return "European Central Bank FX Data Provider"; }
    }

    public override string Description {
      get { return "Downloads exchange rate data from the ECB"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://www.ecb.europa.eu/stats/eurofxref/"); }
    }

    public override string ReferencePublication {
      get { return string.Empty; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var l = new List<IDataDescriptor>();
      try {
        using (var client = new WebClient()) {
          var s = client.OpenRead("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml");
          if (s == null) return l;

          using (var reader = new XmlTextReader(s)) {
            reader.MoveToContent();
            reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            do {
              l.Add(new EcbDataDescriptor() { Name = "EUR / " + reader.GetAttribute("currency"), Description = string.Empty });
            } while (reader.ReadToNextSibling("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref"));
          }
        }
      }
      catch (Exception) {
      }
      return l;
    }

    public override IProblemData LoadData(IDataDescriptor descriptor) {
      var values = new List<IList>();
      var tList = new List<DateTime>();
      var dList = new List<double>();
      values.Add(tList);
      values.Add(dList);
      using (var client = new WebClient()) {
        var s = client.OpenRead("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml");
        if (s != null)

          using (var reader = new XmlTextReader(s)) {
            reader.MoveToContent();
            reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            // foreach time
            do {
              reader.MoveToAttribute("time");
              tList.Add(reader.ReadContentAsDateTime());
              reader.MoveToElement();
              reader.ReadToDescendant("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
              // foreach currencys
              do {
                // find matching entry
                if (descriptor.Name.Contains(reader.GetAttribute("currency"))) {
                  reader.MoveToAttribute("rate");
                  dList.Add(reader.ReadContentAsDouble());

                  reader.MoveToElement();
                  // skip remaining siblings
                  while (reader.ReadToNextSibling("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")) ;
                  break;
                }
              } while (reader.ReadToNextSibling("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref"));
            } while (reader.ReadToNextSibling("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref"));
          }
      }
      // keep only the rows with data for this exchange rate
      if (tList.Count > dList.Count)
        tList.RemoveRange(dList.Count, tList.Count - dList.Count);
      else if (dList.Count > tList.Count)
        dList.RemoveRange(tList.Count, dList.Count - tList.Count);

      // entries in ECB XML are ordered most recent first => reverse lists
      tList.Reverse();
      dList.Reverse();

      // calculate exchange rate deltas
      var changes = new[] { 0.0 } // first element
        .Concat(dList.Zip(dList.Skip(1), (prev, cur) => cur - prev)).ToList();
      values.Add(changes);

      var targetVariable = "d(" + descriptor.Name + ")";
      var allowedInputVariables = new string[] { targetVariable };

      var ds = new Dataset(new string[] { "Day", descriptor.Name, targetVariable }, values);
      return new ProblemData(ds, allowedInputVariables, targetVariable);
    }
  }
}

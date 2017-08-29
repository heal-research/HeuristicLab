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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Core;

namespace HeuristicLab.HiveDrain {
  /// <summary>
  /// Retrieves all jobs and starts downloading their tasks
  /// </summary>
  public class JobDownloader {
    public string RootLocation { get; set; }

    public string NamePattern { get; set; }

    public bool OneFile { get; set; }

    private ILog log;

    public JobDownloader(string location, string pattern, ILog log, bool oneFile = false) {
      RootLocation = location;
      NamePattern = pattern;
      this.log = log;
      OneFile = oneFile;
    }

    public void Start() {
      var jobsLoaded = HiveServiceLocator.Instance.CallHiveService<IEnumerable<Job>>(s => s.GetJobs());
      Semaphore limitSemaphore = new Semaphore(HeuristicLabHiveDrainApplication.MaxParallelDownloads, HeuristicLabHiveDrainApplication.MaxParallelDownloads);

      foreach (Job j in jobsLoaded) {
        if ((!String.IsNullOrEmpty(NamePattern) && j.Name.Contains(NamePattern)) ||
            String.IsNullOrEmpty(NamePattern)) {
          string jobPath = Path.Combine(RootLocation, String.Format("{0} - {1}", j.Name, j.Id));
          log.LogMessage(String.Format("\"{0}\": {1}", j.Name, j.Id));

          if (OneFile) {
            JobTaskOneFileDownloader taskDownloader = new JobTaskOneFileDownloader(jobPath, j, limitSemaphore, log);
            taskDownloader.Start();
          } else {
            JobTaskDownloader taskDownloader = new JobTaskDownloader(jobPath, j, limitSemaphore, log);
            taskDownloader.Start();
          }
        } else {
          log.LogMessage(String.Format("\"{0}\": {1} ---> ignored", j.Name, j.Id));
        }
      }
    }
  }
}

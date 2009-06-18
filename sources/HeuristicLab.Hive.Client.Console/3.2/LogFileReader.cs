#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Console {
  class LogFileReader {
    string filename = "";
    FileSystemWatcher fileSystemWatcher = null;
    long previousSeekPosition;

    public delegate void MoreDataHandler(object sender, string newData);
    public event MoreDataHandler MoreData;
    private int maxBytes = 1024 * 16;
    public int MaxBytes {
      get { return this.maxBytes; }
      set { this.maxBytes = value; }
    }

    public LogFileReader(string filename) {
      this.filename = filename;
    }

    public void Start() {
      FileInfo targetFile = new FileInfo(this.filename);

      previousSeekPosition = 0;

      fileSystemWatcher = new FileSystemWatcher();
      fileSystemWatcher.IncludeSubdirectories = false;
      fileSystemWatcher.Path = targetFile.DirectoryName;
      fileSystemWatcher.Filter = targetFile.Name;

      if (!targetFile.Exists) {
        fileSystemWatcher.Created += new FileSystemEventHandler(TargetFile_Created);
        fileSystemWatcher.EnableRaisingEvents = true;
      } else {
        TargetFile_Changed(null, null);
        StartMonitoring();
      }
    }

    public void Stop() {
      fileSystemWatcher.EnableRaisingEvents = false;
      fileSystemWatcher.Dispose();
    }

    public string ReadFullFile() {
      using (StreamReader streamReader = new StreamReader(this.filename)) {
        return streamReader.ReadToEnd();
      }
    }

    public void StartMonitoring() {
        fileSystemWatcher.Changed += new FileSystemEventHandler(TargetFile_Changed);
      fileSystemWatcher.Renamed += new RenamedEventHandler(TargetFile_Renamed);
      fileSystemWatcher.EnableRaisingEvents = true;
    }

    public void TargetFile_Renamed(object source, FileSystemEventArgs e)
    {
        Stop();
        Start();
    }

    public void TargetFile_Created(object source, FileSystemEventArgs e) {
      StartMonitoring();
    }

    public void TargetFile_Changed(object source, FileSystemEventArgs e) {
      //lock (this)
      {
        //read from current seek position to end of file
        byte[] bytesRead = new byte[maxBytes];
        FileStream fs = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (fs.Length > maxBytes) {
          this.previousSeekPosition = fs.Length - maxBytes;
        }
        this.previousSeekPosition = (int)fs.Seek(this.previousSeekPosition, SeekOrigin.Begin);
        int numBytes = fs.Read(bytesRead, 0, maxBytes);
        fs.Close();
        this.previousSeekPosition += numBytes;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < numBytes; i++) {
          sb.Append((char)bytesRead[i]);
        }

        //call delegates with the string
        this.MoreData(this, sb.ToString());
      }
    }
  }
}

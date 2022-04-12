using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Google.ProtocolBuffers.ProtoGen;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HeuristicLab.Build {
  public class CreateProtoMessages : Task {
    [Required]
    public string ProtosDir { get; set; }
    [Required]
    public string SolutionDir { get; set; }

    public override bool Execute() {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return true; }

      var protoFiles = Directory.EnumerateFiles(ProtosDir, "*.proto");
      foreach (var protoFile in protoFiles) {
        ProgramPreprocess.Run($"--proto_path={ProtosDir}", protoFile, "--include_imports", $"-output_directory={ProtosDir}", $"--protoc_dir={Path.Combine(SolutionDir, "build", "bin")}");
      }
      return true;
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CommandLineInterface
{
  public class Runner
  {
    private readonly ProtoBufSerializer serializer;
    private Task executableTask;

    public Runner() {
      serializer = new ProtoBufSerializer();
    }

    public void Run(ICommandLineArgument[] args) {
      StringBuilder errorLog = new StringBuilder();
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

      if(args.Length == 3 && args[1].Valid && args[1] is OpenArgument hlFile && (args[2] is OpenArgument || args[2] is StringArgument)) {
        var hlFilePath = hlFile.Value;
        var saveFilePath = args[2].Value.ToString();

        var obj = serializer.Deserialize(hlFilePath);
        if(obj is IExecutable executable) {
          executable.Paused += (s,e) => Save(executable, saveFilePath, true);
          executable.Stopped += (s,e) => cancellationTokenSource.Cancel();

          executableTask = executable.StartAsync();

          while(!executableTask.IsCompleted) {
            try {
              Task.Delay(TimeSpan.FromHours(1), cancellationTokenSource.Token).Wait();
              if (!executableTask.IsCompleted) {
                executable.Pause();
              }
            } catch (Exception) { }
          }
          Save(executable, saveFilePath, false);
        } else {
          errorLog.AppendLine("File contains no 'IExecutable' object.");
        }
      } else {
        errorLog.AppendLine("Missing or invalid file argument(s).");
        errorLog.AppendLine("The first file argument needs to be an existing and valid .hl-File.");
        errorLog.AppendLine("The second file argument is the output file (the program will create it).");
        errorLog.AppendLine("Usage: HeuristicLab.exe /start:CLIRunner <Path-Of-HL-File> <Path-Of-Out-File>");
      }

      Console.WriteLine(errorLog.ToString());
      File.AppendAllText("HL-ErrorLog.txt", errorLog.ToString());
    }

    private void Save(IExecutable executable, string saveFilePath, bool startAgain) {
      serializer.Serialize(executable, saveFilePath);
      if(startAgain)
        executableTask = executable.StartAsync();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.ExternalEvaluation.Service;
using Google.ProtocolBuffers;

namespace ExternalEvaluation.Service.Host {
  class Program {
    static void Main(string[] args) {
      ServerSocketListenerFactory socket = new ServerSocketListenerFactory(9991);
      PollService service = new PollService(socket);
      service.Start();
      while (true) {
        Console.WriteLine("Waiting for message...");
        var solution = service.GetSolution();
        if (solution == null) break;
        Console.WriteLine("Received: " + solution.SolutionId);
        service.SendQuality(solution, 1);
        Console.WriteLine("Replied!");
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Common {
  public class MessageContainer {
    public MessageQueue.MessageType Message { get; set; }
    public int JobId { get; set; }

    public MessageContainer(MessageQueue.MessageType message) {
      Message = message;
      JobId = 0;
    }
    public MessageContainer(MessageQueue.MessageType message, int jobId) {
      Message = message;
      JobId = jobId;
    }

  }
}

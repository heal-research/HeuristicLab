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

using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Net.Mail;
using System.IO;

namespace HeuristicLab.CEDMA.Operators {
  public class ItemMailer : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public ItemMailer()
      : base() {
      AddVariableInfo(new VariableInfo("Server", "SMTP server", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("To", "Recipient address", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Item", "The item to send", typeof(IItem), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      string server = GetVariableValue<StringData>("Server", scope, true).Data;
      string to = GetVariableValue<StringData>("To", scope, true).Data;
      IItem item = GetVariableValue<IItem>("Item", scope, true);

      MailMessage message = new MailMessage();
      message.To.Add(to);
      message.Subject = "HL3 Item - " + GetVariableInfo("Item").ActualName;
      message.From = new MailAddress(@"robot@heuristiclab.com");
      using(MemoryStream memStream = new MemoryStream()) {
        PersistenceManager.Save(item, memStream);
        memStream.Flush();
        memStream.Position = 0;
        message.Attachments.Add(new Attachment(memStream, "Scope"));

        SmtpClient smtpClient = new SmtpClient(server, 2525);
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Send(message);
      }
      return null;
    }
  }
}

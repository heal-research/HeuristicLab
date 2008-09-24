using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class SocketData : ItemBase, IDataStream {
    private TcpNetworkDriverConfiguration config;
    public IDriverConfiguration Configuration {
      get { return config; }
      set { config = (TcpNetworkDriverConfiguration)value; }
    }

    private TcpListener tcpListener;
    private TcpClient tcpIn;
    private TcpClient tcpOut;
    private string buffer;

    public SocketData() {
      tcpListener = null;
      tcpIn = null;
      tcpOut = null;
      config = null;
      buffer = "";
    }

    #region clone & persistence
    // A socket cannot be cloned
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SocketData clone = new SocketData();
      clonedObjects.Add(Guid, clone);
      clone.tcpIn = tcpIn;
      clone.config = (TcpNetworkDriverConfiguration)Auxiliary.Clone(config, clonedObjects);
      clone.buffer = buffer;
      return clone;
    }

    // A socket cannot be persisted
    // but information can be persisted that will allow it to be recreated
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode configNode = PersistenceManager.Persist("Configuration", config, document, persistedObjects);
      node.AppendChild(configNode);
      XmlNode bufferNode = document.CreateNode(XmlNodeType.Element, "Buffer", null);
      bufferNode.InnerText = buffer;
      node.AppendChild(bufferNode);
      return node;
    }

    // A socket cannot be persisted
    // but information can be persisted that will allow it to be recreated
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Configuration = (TcpNetworkDriverConfiguration)PersistenceManager.Restore(node.SelectSingleNode("Configuration"), restoredObjects);
      buffer = node.SelectSingleNode("Buffer").InnerText;
      if (tcpListener == null) StartListening();
    }
    #endregion

    private void StartListening() {
      tcpListener = new TcpListener(IPAddress.Any, config.SourcePort.Data);
      tcpListener.Start();
    }

    public void Initialize(IDriverConfiguration configuration) {
      Configuration = configuration;
      StartListening();
    }

    public bool Connect() {
      if (tcpIn == null || !tcpIn.Connected) {
        if (tcpListener.Pending())
          tcpIn = tcpListener.AcceptTcpClient();
      }
      if (tcpOut == null || !tcpOut.Connected) {
        tcpOut = new TcpClient();
        try {
          tcpOut.Connect(new IPEndPoint(IPAddress.Parse(config.TargetIPAddress.Data), config.TargetPort.Data));
        } catch (SocketException) {
          tcpOut.Close();
          tcpOut = null;
        }
      }
      return (tcpIn != null && tcpIn.Connected) && (tcpOut != null && tcpOut.Connected);
    }

    public void Close() {
      tcpListener.Stop();
      if ((tcpOut != null && tcpOut.Connected) && (tcpIn != null && tcpIn.Connected)) {
        Write("CLOSING");
        if (Read() != null)
          throw new InvalidOperationException("ERROR in SocketData: Out of sync during Close()");
      }
      if (tcpIn != null) {
        tcpIn.Close();
        tcpIn = null;
      }
      if (tcpOut != null) {
        tcpOut.Close();
        tcpOut = null;
      }
      buffer = "";
    }

    public void Write(string s) {
      if (tcpOut == null || !tcpOut.Connected) Connect();
      NetworkStream outStream = tcpOut.GetStream();

      byte[] sendBytes = Encoding.ASCII.GetBytes(s + "\n.\n");
      outStream.Write(sendBytes, 0, sendBytes.Length);
      outStream.Flush();
      return;
    }

    public string Read() {
      if (tcpIn == null || !tcpIn.Connected) Connect();
      NetworkStream inStream = tcpIn.GetStream();
      byte[] receivedBytes;
      int count = 0;
      int messageEnd = -1;
      int bufferCount = 0;
      if (buffer.Length > 0) {
        byte[] bufferBytes = Encoding.ASCII.GetBytes(buffer);
        receivedBytes = new byte[1000 + bufferBytes.Length];
        Array.Copy(bufferBytes, receivedBytes, bufferBytes.Length);
        messageEnd = TerminationCheck(receivedBytes, 0, bufferBytes.Length);
        count = bufferBytes.Length;
        bufferCount = count;
        buffer = "";
      } else
        receivedBytes = new byte[1000];

      if (messageEnd < 0) {
        byte[] tmp = new byte[1000];
        bool done;
        do {
          int recvd = inStream.Read(tmp, 0, 1000);
          if (recvd == 0) break;
          count += recvd;
          if (count > receivedBytes.Length) {
            byte[] h = receivedBytes;
            receivedBytes = new byte[2 * h.Length];
            Array.Copy(h, receivedBytes, count - recvd);
          }
          Array.Copy(tmp, 0, receivedBytes, count - recvd, recvd);
          if (count < 3) done = false;
          else done = (receivedBytes[count - 1] == 10 && receivedBytes[count - 2] == 46 && receivedBytes[count - 3] == 10); // \n.\n
        } while (!done);
        messageEnd = TerminationCheck(receivedBytes, bufferCount, count - bufferCount);
      }
      if (messageEnd < 0) throw new InvalidOperationException("ERROR: message was not received");
      if (messageEnd < count - 3)
        buffer = Encoding.ASCII.GetString(receivedBytes, messageEnd + 3, count - messageEnd - 3);
      string result = Encoding.ASCII.GetString(receivedBytes, 0, messageEnd);
      if (result.Equals("CLOSING")) return null;
      else return result;
    }

    private int TerminationCheck(byte[] buffer, int start, int length) {
      for (int i = start ; i < start + length - 2 ; i++) {
        if (buffer[i] == 10 && buffer[i + 1] == 46 && buffer[i + 2] == 10) return i;
      }
      return -1;
    }
  }
}

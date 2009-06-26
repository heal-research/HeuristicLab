using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts {
  [DataContract]
  public class StreamedObject<T> : Stream {
    private T encapsulatedObject;

    private Stream stream = 
      new MemoryStream();

    public StreamedObject(T encapsulatedObject) {
      this.encapsulatedObject = encapsulatedObject;

      BinaryFormatter formatter =
        new BinaryFormatter();
      formatter.Serialize(this.stream, this.encapsulatedObject);

      this.stream.Seek(0, SeekOrigin.Begin);
    }

    public T EncapsulatedObject {
      get {
        return this.encapsulatedObject;
      }
    }

    public override bool CanRead {
      get { return this.stream.CanRead; }
    }

    public override bool CanSeek {
      get { throw new NotImplementedException(); }
    }

    public override bool CanWrite {
      get { throw new NotImplementedException(); }
    }

    public override void Flush() {
      throw new NotImplementedException();
    }

    public override long Length {
      get { return this.stream.Length; }
    }

    public override long Position {
      get {
        return this.stream.Position;
      }
      set {
        if(this.stream.Position != value)
          throw new NotImplementedException();
      }
    }

    public override int Read(byte[] buffer, int offset, int count) {
      int read = this.stream.Read(buffer, offset, count);

      return read;
    }

    public override long Seek(long offset, SeekOrigin origin) {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count) {
      throw new NotImplementedException();
    }

    public override void Close() {
      this.stream.Close();
      base.Close();
    }
    protected override void Dispose(bool disposing) {
      this.stream.Dispose();
      base.Dispose(disposing);
    }
  }
}

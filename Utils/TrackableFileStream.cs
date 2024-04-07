using System.Text;

namespace Utils {
  public class TrackableFileStream : FileStream {
    public TrackableFileStream(string path, FileMode mode, FileAccess access) : base(path, mode, access) {
    }

    public string? ReadLine() {
      string result;
      MemoryStream ms = new();
      int current;

      while((current = ReadByte()) != -1) {
        if((char)current == '\n') break;
        ms.WriteByte((byte)current);
      }

      result = Encoding.Default.GetString(ms.ToArray());

      if(result.Length == 0) {
        return null;
      }

      return result;
    }
  }
}
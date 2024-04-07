using System.Text;
using Models;
using Utils;

namespace FileHandler {
  public delegate T FromString<T>(string str);
  public delegate bool FindFunction<T>(T item);

  public class FileHandler<T> {
    private readonly string fileName;
    private readonly FromString<T> fromString;
    private TextWriter? tw;
    private List<T> list;
    private int itemsBuffered;
    public int ItemsBuffered {
      get => itemsBuffered;
      private set {
        if(value < 0) {
          throw new Exception("invalid value for items buffered!");
        }
        itemsBuffered = value;
      }
    }

    public FileHandler(string fname, FromString<T> fs) {
      fileName = fname;
      list = [];
      fromString = fs;
    }

    // add an item to be saved in underlying file
    public bool Add(T item) {
      if(item == null) {
        return false;
      }
      list.Add(item);
      ItemsBuffered++;
      return true;
    }

    // pops the last buffered object
    public T Pop() {
      T item = list[ItemsBuffered-1];
      list.RemoveAt(ItemsBuffered-1);
      ItemsBuffered--;
      return item;
    }

    // save the buffered objects in the file
    public bool Save() {
      tw = File.AppendText(fileName!);
      foreach(var item in list) {
        tw.WriteLine(item);
      }
      tw.Flush();
      tw.Close();
      Console.WriteLine($"{ItemsBuffered} items appended in {fileName}");
      ItemsBuffered = 0;
      return true;
    }

    // returns the buffered objects
    public List<T> Buffered() {
      return list;
    }

    public void Reset() {
      ItemsBuffered = 0;
      list = [];
    }

    public List<T> Read() {
      List<T> l = [];
      string[] tr = File.ReadAllLines(fileName);
      foreach(var line in tr) {
        l.Add(fromString(line));
      }
      return l;
    }

    public bool Delete(FindFunction<T> find) {
      TrackableFileStream fs = new(fileName!, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      FileStream tempF = File.Open("temp.txt", FileMode.Create, FileAccess.Write);
      StreamWriter writerTemp = new(tempF, Encoding.UTF8);

      string? line;

      while((line = fs.ReadLine()) != null) {
        if(!find(fromString(line))) {
          writerTemp.Write(line);
        }
      } 

      writerTemp.Flush();
      fs.Close();
      tempF.Close();

      File.Delete(fileName!);
      File.Move("temp.txt", fileName!);

      return true;
    }

    // read and parse each line from the underlying file and return it
    private List<T> GetFromFile() {
      List<T> l = [];
      string? line;
      using StreamReader sr = new(fileName);

      while((line = sr.ReadLine()) != null) {
        l.Add(fromString(line));
      }

      return l;
    }

    // read and parse each line (up to count) from the underlying file and return it
    private List<T> GetFromFile(int count) {
      List<T> l = [];
      string? line;
      using StreamReader sr = new(fileName);
      int index = 0;

      while((line = sr.ReadLine()) != null && index < count) {
        l.Add(fromString(line));
        index++;
      }

      return l;
    }

    public T? Find(FindFunction<T> find) {
      List<T> l = [];
      string? line;
      using StreamReader sr = new(fileName);

      while((line = sr.ReadLine()) != null) {
        T current = fromString(line);
        
        if(find(current)) {
          return current;
        }
      }

      return default;
    }
  }
}
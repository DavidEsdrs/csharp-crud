using System.IO;
using System.Text.RegularExpressions;
using Models;

namespace MyFirstProject {
  public enum Choose {
    Invalid = 0,
    None,
    Read,
    Write,
    Save,
    Reset,
    Delete,
    Update,
  }

  public partial class Program {
    public static void Main() {
      var fh = new FileHandler.FileHandler<Person>("file.txt", ParseString);

      Choose chose = 0;

      do {
        Console.Write("give a input > ");
        var line = Console.ReadLine();
        
        try {
          chose = (Choose)Convert.ToInt32(line);
        } catch {
          Console.WriteLine("invalid input!");
          continue;
        }

        switch(chose) {
          case Choose.None:
            Console.WriteLine("bye :)");
            break;
          case Choose.Read:
            Read(fh);
            break;
          case Choose.Write:
            Write(fh);
            break;
          case Choose.Save:
            fh.Save();
            break;
          case Choose.Reset:
            fh.Reset();
            break;
          case Choose.Delete:
            bool success = Delete(fh);
            Console.WriteLine(success ? "operation sucessful" : "operation didn't succeed!");
            break;
          case Choose.Update:
            fh.Reset();
            break;
          default:
            Console.WriteLine("no valid number chose, exitting execution");
            break;
        }

      } while(chose != Choose.None);
    }

    private static void Read(FileHandler.FileHandler<Person> fh) {
      var list = fh.Read();

      if(list.Count == 0) {
        Console.WriteLine("the list is empty");
      } else {
        foreach(var p in list) {
          Console.WriteLine(p);
        }

        Console.WriteLine($"{fh.ItemsBuffered} items buffered");
        Console.WriteLine($"{list.Count} items read");
      }
    }

    private static void Write(FileHandler.FileHandler<Person> fh) {
      int age;
      string? name;

      Console.Write("give a value for name > ");
      name = Console.ReadLine();

      if(name?.Length < 2 || name?.Length > 20) {
        throw new Exception("invalid value for name!");
      } 

      Console.Write("give a value for age > ");
      var ageAsString = Console.ReadLine();

      try {
        age = Convert.ToInt32(ageAsString);
      }

      catch {
        throw new Exception("invalid value for age");
      }

      var p = new Person(name!, age);

      fh.Add(p);

      Console.WriteLine($"{fh.ItemsBuffered} items buffered");
    }

    public static Person ParseString(string str) {  
      string name = "";
      int age = 0;

      string[] segments = str.Split(',');
      
      if(segments.Length != 2) {
        return new();
      }

      name = segments[0];
      age = Convert.ToInt32(segments[1]);

      return new(name, age);
    }

    public static bool Delete(FileHandler.FileHandler<Person> fh) {
      string? name;

      Console.Write("give a name to delete > ");
      name = Console.ReadLine();

      if(name?.Length < 2 || name?.Length > 20) {
        throw new Exception("invalid value for name!");
      }

      return fh.Delete((Person p) => p.Name == name);
    }
  }
}
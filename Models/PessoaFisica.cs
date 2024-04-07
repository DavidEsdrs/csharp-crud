using System;

namespace Models {
  public class Person {
    private string name;
    public string Name {
      get => name;
      set {
        if (value.Length < 2 || value.Length > 20) {
          throw new Exception("invalid value for name");
        }

        name = value;
      }
    }

    private int age;
    public int Age {
      get => age;
      set {
        if (value < 0 || value > 120) {
          throw new Exception("invalid value for age");
        }
        age = value;
      }
    }

    public override string ToString() {
      return $"{Name},{Age}";
    }

    public Person(string name, int age) {
      this.name = name;
      this.age = age;
    }

    public Person() {
      name = "";
      age = 0;
    }
  }
}
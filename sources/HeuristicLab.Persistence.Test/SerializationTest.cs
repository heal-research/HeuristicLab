using System;
using System.Xml.Serialization;
using System.Collections;

namespace Persistence.Test {

  [Serializable]
  public class Employer {

    public string name;
    public ArrayList employees;

    public Employer() { }

    public Employer(string name) {
      this.name = name;
      employees = new ArrayList();
    }

  }

  [Serializable]
  public class Employee {

    public string firstName;
    public string lastName;
    public Employer employer;

    public Employee() { }

    public Employee(string firstName, string lastName, Employer employer) {
      this.firstName = firstName;
      this.lastName = lastName;
      this.employer = employer;
    }

  }
   
  class SerializationTest {
    public static void Main() {

      Employer company = new Employer("FH OOE F&E GmbH.");
      company.employees.Add(new Employee("John", "Smith", company));
      company.employees.Add(new Employee("Max", "Mustermann", company));
      company.employees.Add(new Employee("Erik", "Pitzer", company));

      XmlSerializer s = new XmlSerializer(typeof(Employer));
      s.Serialize(Console.Out, company);
      Console.ReadLine();
    }
  }
}

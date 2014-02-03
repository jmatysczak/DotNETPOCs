using System;

namespace ASPNETPOCs.DISetupOnPageParse {
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class InjectDep : Attribute {
    public string Name;
    public InjectDep(string name) {
      this.Name = name;
    }
  }
}

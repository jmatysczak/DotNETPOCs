using System;

namespace ASPNETPOCs.DISetupOnPageParse {
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class InjectDepAttribute : Attribute {
    public string Name;
    public InjectDepAttribute(string name) {
      this.Name = name;
    }
  }
}

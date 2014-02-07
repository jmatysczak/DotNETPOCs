using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace InterfaceProxying {
  class Program {
    static void Main(string[] args) {
      (new ExampleProxy(typeof(InterfaceToProxy)).GetTransparentProxy() as InterfaceToProxy).AnEvent += (sender, e) => { };
    }
  }

  interface InterfaceToProxy {
    event EventHandler AnEvent;
  }

  class ExampleProxy : RealProxy {
    public ExampleProxy(Type t) : base(t) { }

    public override IMessage Invoke(IMessage msg) {
      Console.WriteLine("Properties:");
      foreach(object key in msg.Properties.Keys) {
        Console.WriteLine("   {0}: {1}", key, msg.Properties[key]);
      }

      IMethodCallMessage call = (IMethodCallMessage)msg;
      Console.WriteLine("Method Call Properties:");
      Console.WriteLine("   ArgCount: {0}", call.ArgCount);
      Console.WriteLine("   Args: {0}", call.Args);
      Console.WriteLine("   HasVarArgs: {0}", call.HasVarArgs);
      Console.WriteLine("   InArgCount: {0}", call.InArgCount);
      Console.WriteLine("   InArgs: {0}", call.InArgs);
      Console.WriteLine("   LogicalCallContext: {0}", call.LogicalCallContext);
      Console.WriteLine("   MethodBase: {0}", call.MethodBase);
      Console.WriteLine("   MethodName: {0}", call.MethodName);
      Console.WriteLine("   MethodSignature: {0}", call.MethodSignature);
      Console.WriteLine("   TypeName: {0}", call.TypeName);
      Console.WriteLine("   Uri: {0}", call.Uri);

      return new ReturnMessage(null, null, 0, call.LogicalCallContext, call);
    }
  }
}

using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace InterfaceProxying {
  class Program {
    static void Main(string[] args) {
      string theOutArg;
      var theReturnValue = (new ExampleProxy(typeof(InterfaceToProxy)).GetTransparentProxy() as InterfaceToProxy).AMethod("Hello", out theOutArg);
      Console.WriteLine("theOutArg: {0}", theOutArg);
      Console.WriteLine("theReturnValue: {0}", theReturnValue);
    }
  }

  interface InterfaceToProxy {
    string AMethod(string anInArg, out string anOutArg);
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

      var returnValue = call.InArgs[0].ToString() + " World";
      // Sneaky. The second and third parameters must include the in parameters...
      return new ReturnMessage(returnValue, new object[] { null, returnValue + " Out" }, 2, call.LogicalCallContext, call);
    }
  }
}

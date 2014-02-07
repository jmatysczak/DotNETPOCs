### Proof of concepts/examples/investigations in .NET.

#### ASPNETPOCs/DISetupOnPageParse
Add code to the classes that render ASPXs and ASCXs to perform dependency injection.

The code behind of each ASPX and ASCX has a control builder attribute that specifies a control build to use. The control builders override the ProcessGeneratedCode method and modify code dom of the builder method.

#### ASPNETPOCs/ServerSentEvents
Client and server example of server sent events.

The client is a simple HTML page that creates an EventSource, listens to its lifecycle events, and writes to the page.

The server is an asynchronous http handler. The advantage of using an asynchronous versus a synchronous http handler is that a thread won't be tied up for the life time of the request.

I am not sure this code should be used in a real application since technically IIS7- and .NET 4.0- don't support server sent events. So while this example works, using this technique might cause problems.

#### ASPNETPOCs/StripWhiteSpace
Remove unnecessary white space in the markup of ASPXs and ASCXs as they are converted to classes.

The code behind of each ASPX and ASCX has a control builder attribute that specifies a control build to use. The control builders override the AllowWhitespaceLiterals and AppendLiteralString methods to remove whitespace.

#### InterfaceProxying
An example of how to proxy an interface and handle a method call.
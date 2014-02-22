### Proof of concepts/examples/investigations in .NET.

#### ASPNETHosting
Example of how to host ASP.NET, add a module to modify page, and process a page.

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

#### GetASPNETFileAsType
Example of compiling and rendering an ASPX page outside of the ASP.NET process.

Useful to unit test rendering.

#### InterfaceProxying
Example of how to proxy an interface and handle a method call.

#### SortingAndPagingPerformance
Various implementations of sorting and paging a list. I wanted to implement a priority queue backed by a binary heap and see the performance numbers.
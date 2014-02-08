using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace GetASPNETFileAsType {
  class Program {
    static void Main(string[] args) {
      // Create the path to ASPNETPOCs project so the ASPX can be found.
      var currentDir = Directory.GetCurrentDirectory();
      var appPhysicalSourceDir = Path.Combine(currentDir.Substring(0, currentDir.IndexOf("GetASPNETFileAsType")), "ASPNETPOCs");
      Console.WriteLine("appPhysicalSourceDir: {0}", appPhysicalSourceDir);

      // The virtual path is arbitrary unless you want to precompile a specific app.
      var appVirtualDir = "/GetASPNETFileAsType";
      ClientBuildManager manager = new ClientBuildManager(appVirtualDir, appPhysicalSourceDir);

      // "GetCompiledType" will return null if the Type's dependencies can not be resolved.
      // This means the ASPNETPOCs project needs to be references so the ASPX's code behind class can be resolved.
      var pageType = manager.GetCompiledType("~/StripWhiteSpace/StripWhiteSpacePage.aspx");

      // Should return the Type that "~/StripWhiteSpace/StripWhiteSpacePage.aspx" was compiled to.
      Console.WriteLine("pageType: {0}", pageType);
      // Writes the location of the assembly the ASPX's class is in.
      Console.WriteLine("pageType.Assembly.Location: {0}", pageType.Assembly.Location);

      // Create an instance from the type. Since it is a Page, it will implement IHttpHandler.
      var handler = (IHttpHandler)Activator.CreateInstance(pageType);

      // This is where you can wire up any dependencies if you need to.

      // Setup the call context to process the page in.
      // The rendered page will be written to "Console.Out" in this case.
      // For unit testing you can use a StringWriter and then parse it with something like Html Agility Pack.
      var context = new HttpContext(new SimpleWorkerRequest(appVirtualDir, appPhysicalSourceDir, "StripWhiteSpace/StripWhiteSpacePage.aspx", "", Console.Out));
      // The next line is necessary to avoid some null reference and parse exceptions.
      context.Request.Browser = new HttpBrowserCapabilities { Capabilities = new Hashtable { { "tables", "true" } } };

      // By default the response is buffered.
      // So either turn buffering off or make sure you call Flush.
      // Flush is called below.
      //context.Response.BufferOutput = false;

      // The following enables trace information to be written with the page and raises the "TraceFinished" event. The default is false.
      /*
      context.Trace.IsEnabled = true;
      context.Trace.TraceFinished += (sender, e) => {
        foreach(TraceContextRecord r in e.TraceRecords) {
          Console.WriteLine("Category: {0}, IsWarning: {1}, Message: {2}, ErrorInfo:{3}{4}", r.Category, r.IsWarning, r.Message, r.ErrorInfo == null ? "" : "\r\n", r.ErrorInfo);
        }
      };
      */

      // Renders the page.
      handler.ProcessRequest(context);

      // Flush the response. You either need this line or "context.Response.BufferOutput = false;" above.
      context.Response.Flush();

      Console.WriteLine();
    }
  }
}

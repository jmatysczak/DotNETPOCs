using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;

namespace ASPNETHosting {
  class Program {
    static void Main(string[] args) {
      ExampleAspNetHost host = null;

      try {
        var currentDir = Directory.GetCurrentDirectory();

        // The web app root and its bin directory of the app this program will host.
        // This will be under the directory this exe is running in.
        var aspnetRootDir = Path.Combine(currentDir, "aspnet");
        var aspnetRootBinDir = Path.Combine(aspnetRootDir, "bin");

        // Delete the web app root directory if it exists and create it.
        if(Directory.Exists(aspnetRootDir)) Directory.Delete(aspnetRootDir, true);
        Directory.CreateDirectory(aspnetRootBinDir);

        // The directory of the web app we're going to use. We're going to copy the necessary files from it to aspnetRootDir and aspnetRootBinDir.
        // You can just use the directory where the web app exists, but you have to be careful about cleaning up any modifications to its web.config.
        var aspnetSourceDir = Path.Combine(currentDir.Substring(0, currentDir.IndexOf("ASPNETHosting")), "ASPNETPOCs");
        // Copy the web app artifacts we want.
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "bin"), "ASPNETPOCs.dll"), Path.Combine(aspnetRootBinDir, "ASPNETPOCs.dll"));
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "DISetupOnPageParse"), "DIPage.aspx"), Path.Combine(aspnetRootDir, "DIPage.aspx"));
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "DISetupOnPageParse"), "DIUserControl.ascx"), Path.Combine(aspnetRootDir, "DIUserControl.ascx"));

        // Put a copy of this exe in the web app's bin directory.
        // This is just a short cut.
        // You can use a separate dll to hold the host (ExampleAspNetHost) and any other code (ExampleHttpModule).
        var thisAssemblysLocation = Assembly.GetExecutingAssembly().Location;
        File.Copy(thisAssemblysLocation, Path.Combine(aspnetRootBinDir, Path.GetFileName(thisAssemblysLocation)));

        // Customize the web.config.
        // In this example we're writing the entire web.config because the target web app doesn't have one.
        // If you web app has a web.config, open it, modify it, and overwrite it.
        File.WriteAllText(Path.Combine(aspnetRootDir, "web.config"), @"
<configuration>
  <system.web>
    <httpModules>
      <add name='ExampleHttpModule' type='" + typeof(ExampleHttpModule).AssemblyQualifiedName + @"'/>
    </httpModules>
  </system.web>
</configuration>
        ");

        // Create the asp.net host.
        // "ExampleAspNetHost" must be in an assembly in the bin (or the gac, but I haven't tried this).
        // "ExampleVirtualRoot" is arbirary.
        // "aspnetRootDir" is the web app root.
        host = (ExampleAspNetHost)ApplicationHost.CreateApplicationHost(typeof(ExampleAspNetHost), "/ExampleVirtualRoot", aspnetRootDir);

        // Process a page.
        host.ProcessRequest("DIPage.aspx");
      } finally {
        if(host != null) {
          AppDomain.Unload(host.GetAppDomain());
        }
      }
    }

    public class ExampleAspNetHost : MarshalByRefObject {
      public void ProcessRequest(string page) {
        HttpRuntime.ProcessRequest(new SimpleWorkerRequest(page, null, Console.Out));
      }

      public AppDomain GetAppDomain() {
        return Thread.GetDomain();
      }
    }
  }

  public class ExampleHttpModule : IHttpModule {
    public void Dispose() { }

    public void Init(HttpApplication context) {
      context.PreRequestHandlerExecute += delegate {
        var page = (Page)HttpContext.Current.CurrentHandler;
        page.PreRender += delegate {
          var name = page.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic);
          name.SetValue(page, "Johnny 'Hosted' Doe", null);
        };
      };
    }
  }
}

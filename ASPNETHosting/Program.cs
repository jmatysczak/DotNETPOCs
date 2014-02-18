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
        var aspnetRootDir = Path.Combine(currentDir, "aspnet");
        var aspnetRootBinDir = Path.Combine(aspnetRootDir, "bin");

        if(Directory.Exists(aspnetRootDir)) Directory.Delete(aspnetRootDir, true);
        Directory.CreateDirectory(aspnetRootBinDir);

        var aspnetSourceDir = Path.Combine(currentDir.Substring(0, currentDir.IndexOf("ASPNETHosting")), "ASPNETPOCs");
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "bin"), "ASPNETPOCs.dll"), Path.Combine(aspnetRootBinDir, "ASPNETPOCs.dll"));
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "DISetupOnPageParse"), "DIPage.aspx"), Path.Combine(aspnetRootDir, "DIPage.aspx"));
        File.Copy(Path.Combine(Path.Combine(aspnetSourceDir, "DISetupOnPageParse"), "DIUserControl.ascx"), Path.Combine(aspnetRootDir, "DIUserControl.ascx"));

        var thisAssemblysLocation = Assembly.GetExecutingAssembly().Location;
        File.Copy(thisAssemblysLocation, Path.Combine(aspnetRootBinDir, Path.GetFileName(thisAssemblysLocation)));

        File.WriteAllText(Path.Combine(aspnetRootDir, "web.config"), @"
<configuration>
  <system.web>
    <httpModules>
      <add name='ExampleHttpModule' type='" + typeof(ExampleHttpModule).AssemblyQualifiedName + @"'/>
    </httpModules>
  </system.web>
</configuration>
        ");

        host = (ExampleAspNetHost)ApplicationHost.CreateApplicationHost(typeof(ExampleAspNetHost), "/ExampleVirtualRoot", aspnetRootDir);

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
    public void Dispose() {
      Console.WriteLine(this.GetType().FullName + ".Dispose()");
    }

    public void Init(HttpApplication context) {
      Console.WriteLine(this.GetType().FullName + ".Init(HttpApplication)");
      context.PostRequestHandlerExecute += delegate {
        Page page = (Page)HttpContext.Current.CurrentHandler;
      };
    }
  }
}

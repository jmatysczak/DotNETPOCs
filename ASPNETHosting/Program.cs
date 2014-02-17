using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Xml;

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

        //ExampleAspNetHost.AddModulesToWebConfig(true);

        host = (ExampleAspNetHost)ApplicationHost.CreateApplicationHost(typeof(ExampleAspNetHost), "/ExampleVirtualRoot", aspnetRootDir);

        host.ProcessRequest("DIPage.aspx");
      } finally {
        if(host != null) {
          AppDomain.Unload(host.GetAppDomain());
        }
      }
    }

    public class ExampleAspNetHost : MarshalByRefObject {
      public const string WEB_ROOT_DIR = @"C:\MyData\PD\WorkLenz\Trunk\WorkLenz\WorkLenz\";

      public static void AddModulesToWebConfig(bool add) {
        string webConfig = Path.Combine(WEB_ROOT_DIR, "web.config");
        XmlDocument doc = new XmlDocument();
        doc.Load(webConfig);

        XmlNode assemblies = doc.SelectSingleNode("//assemblies");
        if(add) {
          assemblies.InnerXml += "<add assembly='" + typeof(ExampleHttpModule).Assembly.FullName + "'/>";
        } else {
          assemblies.RemoveChild(assemblies.LastChild);
        }

        XmlNode httpModules = doc.SelectSingleNode("//httpModules");
        if(add) {
          httpModules.InnerXml += "<add name='ExampleHttpModule' type='" + typeof(ExampleHttpModule).FullName + "'/>";
        } else {
          httpModules.RemoveChild(httpModules.LastChild);
        }

        doc.Save(webConfig);
      }

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

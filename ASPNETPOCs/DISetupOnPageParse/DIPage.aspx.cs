using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  [FileLevelControlBuilder(typeof(DIFileLevelPageControlBuilder))]
  public partial class DIPage : Page {
    [InjectDep("John Doe")]
    protected string Name { get; set; }
  }
}

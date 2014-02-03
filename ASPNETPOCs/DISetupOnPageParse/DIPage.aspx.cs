using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  [FileLevelControlBuilder(typeof(DIFileLevelControlBuilder))]
  public partial class DIPage : Page {
    [InjectDep("World")]
    protected string Name { get; set; }
  }
}

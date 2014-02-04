using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  [FileLevelControlBuilder(typeof(DIFileLevelUserControlBuilder))]
  public partial class DIUserControl : UserControl {
    [InjectDep("Jane Doe")]
    protected string Name { get; set; }
  }
}
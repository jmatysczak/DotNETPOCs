using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPNETPOCs.DISetupOnPageParse {
  [ControlBuilder(typeof(DIControlBuilder))]
  public class DICustomControl : Label {
    [InjectDep("World")]
    public string CustomText { set { this.Text = value + " (from custom control)"; } }
  }
}

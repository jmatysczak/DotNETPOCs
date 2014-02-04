using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPNETPOCs.DISetupOnPageParse {
  [ControlBuilder(typeof(DIControlBuilder))]
  public class DICustomControl : Label {
    [InjectDep("Jane Roe")]
    public string CustomText { set { this.Text = value; } }
  }
}

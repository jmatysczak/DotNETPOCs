using System.Web.UI;

namespace ASPNETPOCs.StripWhiteSpace {
  public class StripWhiteSpaceUserControlControlBuilder : FileLevelUserControlBuilder {
    public override bool AllowWhitespaceLiterals() {
      return false;
    }

    public override void AppendLiteralString(string s) {
      base.AppendLiteralString(StripWhiteSpacePageControlBuilder.StripWhiteSpace(s));
    }
  }
}

using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace ASPNETPOCs.StripWhiteSpace {
  public class StripWhiteSpacePageControlBuilder : FileLevelPageControlBuilder {
    public override bool AllowWhitespaceLiterals() {
      return false;
    }

    private static readonly Regex WHITE_SPACE_BETWEEN_ELEMENTS = new Regex(@"(?<=>)\s+|\s+(?=<)", RegexOptions.Compiled);

    public override void AppendLiteralString(string s) {
      if(HttpContext.Current == null || !HttpContext.Current.IsDebuggingEnabled) s = WHITE_SPACE_BETWEEN_ELEMENTS.Replace(s, "").Trim();
      base.AppendLiteralString(s);
    }
  }
}

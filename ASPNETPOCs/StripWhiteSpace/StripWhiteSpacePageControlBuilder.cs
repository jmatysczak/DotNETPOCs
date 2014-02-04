using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace ASPNETPOCs.StripWhiteSpace {
  public class StripWhiteSpacePageControlBuilder : FileLevelPageControlBuilder {
    public override bool AllowWhitespaceLiterals() {
      return false;
    }

    public override void AppendLiteralString(string s) {
      base.AppendLiteralString(StripWhiteSpace(s));
    }

    private static readonly Regex WHITE_SPACE_BETWEEN_ELEMENTS = new Regex(@"(?<=>)\s+|\s+(?=<)", RegexOptions.Compiled);
    public static string StripWhiteSpace(string s) {
      if(HttpContext.Current == null || !HttpContext.Current.IsDebuggingEnabled) s = WHITE_SPACE_BETWEEN_ELEMENTS.Replace(s, "").Trim();
      return s;
    }
  }
}

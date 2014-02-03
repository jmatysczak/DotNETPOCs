﻿using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  [FileLevelControlBuilder(typeof(DIFileLevelPageControlBuilder))]
  public partial class DIPage : Page {
    [InjectDep("World")]
    protected string Name { get; set; }
  }
}

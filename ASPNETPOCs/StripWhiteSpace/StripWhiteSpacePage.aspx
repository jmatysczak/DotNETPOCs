<%@ Page Language="C#" CodeBehind="StripWhiteSpacePage.aspx.cs" Inherits="ASPNETPOCs.StripWhiteSpace.StripWhiteSpacePage" %>
<%@ Register TagPrefix="uc" TagName="StripWhiteSpaceUserControl" Src="StripWhiteSpaceUserControl.ascx" %>

<!doctype html>

<html lang="en">
  <head>
    <meta charset="utf-8">

    <title>Strip White Space</title>
  </head>
  <body>
    <div>
      Change the debug attribute of the compilation element in the web.config, reload, and view the source.
    </div>
    <uc:StripWhiteSpaceUserControl runat="server" />
  </body>
</html>

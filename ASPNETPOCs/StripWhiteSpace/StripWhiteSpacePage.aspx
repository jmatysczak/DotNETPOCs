<%@ Page Language="C#" CodeBehind="StripWhiteSpacePage.aspx.cs" Inherits="ASPNETPOCs.StripWhiteSpace.StripWhiteSpacePage" %>
<%@ Register TagPrefix="uc" TagName="StripWhiteSpaceUserControl" Src="StripWhiteSpaceUserControl.ascx" %>

<!doctype html>

<html lang="en">
  <head>
    <meta charset="utf-8">

    <title>Strip White Space</title>
  </head>
  <body>
    <h1>
      Hello World
    </h1>
    <uc:StripWhiteSpaceUserControl runat="server" />
  </body>
</html>

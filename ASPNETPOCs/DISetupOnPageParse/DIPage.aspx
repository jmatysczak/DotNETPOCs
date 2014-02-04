<%@ Page Language="C#" CodeBehind="DIPage.aspx.cs" Inherits="ASPNETPOCs.DISetupOnPageParse.DIPage" %>
<%@ Register TagPrefix="uc" TagName="DIUserControl" Src="DIUserControl.ascx" %>
<%@ Register TagPrefix="uc" Namespace="ASPNETPOCs.DISetupOnPageParse" Assembly="ASPNETPOCs" %>

<!doctype html>

<html lang="en">
  <head>
    <meta charset="utf-8">

    <title>Page With DI</title>
  </head>
  <body>
    <div>
      The value of the page's Name property: <%=Name%><br/>
      The page's class: <%= GetType() %><br/>
      The page's class's assembly: <%= GetType().Assembly.Location %><br/>
      Take a look at the class's "__BuildControlTree" method with an assembly browser like ILSpy.
    </div>
    <p/>
    <uc:DIUserControl runat="server" />
    <p/>
    <h2>Hello <uc:DICustomControl runat="server" /></h2>
  </body>
</html>

<%@ Page Language="C#" CodeBehind="DIPage.aspx.cs" Inherits="ASPNETPOCs.DISetupOnPageParse.DIPage" %>
<%--
<%@ Register TagPrefix="uc" TagName="DIUserControl" Src="DIUserControl.ascx" %>
--%>

<!doctype html>

<html lang="en">
  <head>
    <meta charset="utf-8">

    <title>Page With DI</title>
  </head>
  <body>
    <h1>Hello <%=Name%></h1>
<%--
    <uc:DIUserControl runat="server" />
--%>
    <h2><%= System.Reflection.Assembly.GetExecutingAssembly().Location %></h2>
  </body>
</html>

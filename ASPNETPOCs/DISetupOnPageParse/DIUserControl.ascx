<%@ Control Language="C#" CodeBehind="DIUserControl.ascx.cs" Inherits="ASPNETPOCs.DISetupOnPageParse.DIUserControl" %>
    <div>
      The value of the user control's Name property: <%=Name%><br/>
      The user control's class: <%= GetType() %><br/>
      The user control's class's assembly: <%= GetType().Assembly.Location %><br/>
      Take a look at the class's "__BuildControlTree" method with an assembly browser like ILSpy.
    </div>

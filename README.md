#### Proof of concepts/examples/investigations in .NET.

##### ASPNETPOCs/StripWhiteSpace
Remove unnecessary white space in the markup of ASPXs and ASCXs as they are converted to classes.

The code behind of each ASPX and ASCX has a control builder attribute that specifies a control build to use. The control builders overrid the AllowWhitespaceLiterals and AppendLiteralString methods to remove whitespace.

##### ASPNETPOCs/DISetupOnPageParse
Add code to the classes that render ASPXs and ASCXs to perform dependency injection.

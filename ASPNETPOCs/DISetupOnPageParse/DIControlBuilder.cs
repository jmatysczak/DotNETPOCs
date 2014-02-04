using System;
using System.CodeDom;
using System.Reflection;
using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  public class DIControlBuilder : ControlBuilder {
    public override void ProcessGeneratedCode(
      CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType,
      CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod) {
      ExampleDependencyInjectionLogic(buildMethod);
      base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }

    public static void ExampleDependencyInjectionLogic(CodeMemberMethod buildMethod) {
      // Non file based controls are different than file based controls.
      // Non file based controls are instantiated with "new" inside of a method and returned.
      // They are declared in the first statement.
      CodeVariableDeclarationStatement declaration = (CodeVariableDeclarationStatement)buildMethod.Statements[0];
      // Use the type of the declaration to get the type of the control.
      Type type = Type.GetType(declaration.Type.BaseType);
      // Standard reflection to get all of the properties. You could also look for fields.
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      foreach(PropertyInfo property in properties) {
        // See if the property has the specific attribute we care about.
        object[] injectDeps = property.GetCustomAttributes(typeof(InjectDepAttribute), true);
        if(injectDeps.Length == 1) {
          // Create the code dom to perform the injection.
          // In this example, set the property to the literal the attribute was given.
          CodeStatement setProperty = new CodeAssignStatement(
            new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(declaration.Name), property.Name),
            new CodePrimitiveExpression((injectDeps[0] as InjectDepAttribute).Name + " (the builder method this is set in is '" + buildMethod.Name + "')")
          );
          // Add the statement to the list of statements that make up the builder method.
          // The last statement is the return statement that returns the control.
          // So we insert out injection statement(s) right before it.
          buildMethod.Statements.Insert(buildMethod.Statements.Count - 1, setProperty);
        }
      }
    }
  }
}

using System;
using System.CodeDom;
using System.Reflection;
using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  public class DIFileLevelPageControlBuilder : FileLevelPageControlBuilder {
    public override void ProcessGeneratedCode(
      CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType,
      CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod) {
      DIFileLevelPageControlBuilder.ExampleDependencyInjectionLogic(derivedType, buildMethod);
      base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }

    public static void ExampleDependencyInjectionLogic(CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod) {
      // "derivedType.BaseTypes" are those types a file based control (ASPX or ASCX inherit from.
      // The first type is the code behind.
      Type type = Type.GetType(derivedType.BaseTypes[0].BaseType);
      // Standard reflection to get all of the properties. You could also look for fields.
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      foreach(PropertyInfo property in properties) {
        // See if the property has a specific attribute.
        object[] injectDeps = property.GetCustomAttributes(typeof(InjectDepAttribute), true);
        if(injectDeps.Length == 1) {
          // Create the code dom to perform the injection.
          // In this example, set the property to the literal the attribute was given.
          CodeStatement setProperty = new CodeAssignStatement(
            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property.Name),
            new CodePrimitiveExpression((injectDeps[0] as InjectDepAttribute).Name)
          );
          // Add the statement to the list of statements that make up the builder method.
          buildMethod.Statements.Add(setProperty);
        }
      }
    }
  }
}

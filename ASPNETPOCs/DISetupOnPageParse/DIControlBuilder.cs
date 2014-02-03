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
      CodeVariableDeclarationStatement declaration = (CodeVariableDeclarationStatement)buildMethod.Statements[0];
      Type type = Type.GetType(declaration.Type.BaseType);
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      foreach(PropertyInfo property in properties) {
        object[] injectDeps = property.GetCustomAttributes(typeof(InjectDepAttribute), true);
        if(injectDeps.Length == 1) {
          CodeStatement setProperty = new CodeAssignStatement(
            new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(declaration.Name), property.Name),
            new CodePrimitiveExpression(" (Injected) " + (injectDeps[0] as InjectDepAttribute).Name)
          );
          buildMethod.Statements.Insert(buildMethod.Statements.Count - 1, setProperty);
        }
      }
    }
  }
}

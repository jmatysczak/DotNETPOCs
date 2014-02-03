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
      Type type = Type.GetType(derivedType.BaseTypes[0].BaseType);
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      foreach(PropertyInfo property in properties) {
        object[] injectDeps = property.GetCustomAttributes(typeof(InjectDepAttribute), true);
        if(injectDeps.Length == 1) {
          CodeStatement setProperty = new CodeAssignStatement(
            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property.Name),
            new CodePrimitiveExpression(" (Injected) " + (injectDeps[0] as InjectDepAttribute).Name)
            );
          buildMethod.Statements.Add(setProperty);
        }
      }
    }
  }
}

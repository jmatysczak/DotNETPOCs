using System;
using System.CodeDom;
using System.Reflection;
using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  public class DIFileLevelControlBuilder : FileLevelPageControlBuilder {
    public override void ProcessGeneratedCode(
      CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType,
      CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod) {
      Type type = Type.GetType(derivedType.BaseTypes[0].BaseType);
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      foreach(PropertyInfo property in properties) {
        object[] injectDeps = property.GetCustomAttributes(typeof(InjectDep), true);
        if(injectDeps.Length == 1) {
          CodeStatement setProperty = new CodeAssignStatement(
            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property.Name),
            new CodePrimitiveExpression(" (Injected) " + (injectDeps[0] as InjectDep).Name)
            );
          buildMethod.Statements.Add(setProperty);
        }
      }
      base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }
  }
}

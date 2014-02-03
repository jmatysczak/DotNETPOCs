using System.CodeDom;
using System.Web.UI;

namespace ASPNETPOCs.DISetupOnPageParse {
  public class DIFileLevelUserControlBuilder : FileLevelUserControlBuilder {
    public override void ProcessGeneratedCode(
      CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType,
      CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod) {
      DIFileLevelPageControlBuilder.ExampleDependencyInjectionLogic(derivedType, buildMethod);
      base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }
  }
}

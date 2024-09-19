using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace OLFramework.Generator
{
	[Generator]
	public class SyncVarGenerator : ISourceGenerator
	{
		
		public void Execute(GeneratorExecutionContext context)
		{
			foreach (var tree in context.Compilation.SyntaxTrees)
			{
				context.AddSource("test.g", "//test");
				var root = tree.GetRoot();
				var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
				foreach (var node in classNodes)
				{
					foreach (var attrlist in node.AttributeLists)
					{
						foreach (var attr in attrlist.Attributes)
						{
							if (attr.Name.ToString() == "EnableSyncVar")
							{
								context.AddSource($"{node.Identifier.ToString()}.g", GenerateSyncVar(node));
							}
						}
					}
				}
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{

		}

		private string GenerateSyncVar(ClassDeclarationSyntax classNode)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("//auto generated");
			if (classNode.Parent == null) return "";
			foreach(var usingNode in classNode.Parent.DescendantNodes().OfType<UsingDirectiveSyntax>())
			{
				if (usingNode.Name == null) continue;
				sb.AppendLine($"using {usingNode.Name.ToString()};");
			}

			sb.AppendLine($"public partial class {classNode.Identifier.ToString()} : MonoBehaviour");
			sb.AppendLine("{");
			sb.AppendLine("\tprivate void InitSyncVars()");
			sb.AppendLine("\t{");
			foreach (var field in classNode.ChildNodes().OfType<FieldDeclarationSyntax>())
			{
				foreach(var variableDef in field.ChildNodes().OfType<VariableDeclarationSyntax>())
				{
					string type_name = variableDef.ChildNodes().OfType<GenericNameSyntax>().First().Identifier.ToString();
					string argT = variableDef.ChildNodes().OfType<TypeArgumentListSyntax>().First()
						.ChildNodes().OfType<IdentifierNameSyntax>().First().Identifier.ToString();
					if (type_name == "SyncVar")
					{
						foreach(var variable in variableDef.ChildNodes().OfType<VariableDeclaratorSyntax>())
						{
							string varName = variable.Identifier.ToString();
							sb.AppendLine($"\t\t{varName} = new SyncVar<{argT}>(\"{varName}\",GetComponent<NetworkIdentify>(),Set{varName})");
						}
					}
                }
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			return sb.ToString();
		}
	}
}

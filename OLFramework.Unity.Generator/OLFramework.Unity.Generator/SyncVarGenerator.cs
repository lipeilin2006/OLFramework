using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
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
				if (field.Declaration.Type.ToString() == "SyncVar")
				{
					foreach (var varNode in field.Declaration.Variables) {
						string varName = varNode.Identifier.ToString();
						sb.AppendLine($"\t\t{varName} = new SyncVar(\"{varName}\",gameObject,Set{varName});");
					}
				}
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			return sb.ToString();
		}
	}
}

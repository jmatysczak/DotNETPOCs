using System;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace AjaxMinAST {
	class Program {
		static void Main(string[] args) {
			var input = File.ReadAllText("input.js");

			Console.WriteLine("--- Raw ---");
			Console.WriteLine(input);
			Console.WriteLine("\r\n");

			Console.WriteLine("--- Minified ---");
			Console.WriteLine(new Minifier().MinifyJavaScript(input));
			Console.WriteLine("\r\n");

			Console.WriteLine("--- AST ---");
			var parser = new JSParser();
			parser.CompilerError += (_, ea) => Console.WriteLine(ea.Error);
			
			var functions = parser.Parse(input);
			var functionContext = parser.Parse("var functionContext = {};");

			new ObjectLiteralVisitor(functions).Visit(functionContext);

			OutputVisitor.Apply(Console.Out, functionContext, new CodeSettings() { MinifyCode = false, OutputMode = OutputMode.MultipleLines });
		}
	}

	class ObjectLiteralVisitor : TreeVisitor {
		Block functions;

		public ObjectLiteralVisitor(Block functions) {
			this.functions = functions;
		}
	
		public override void Visit(ObjectLiteral node) {
			base.Visit(node);
			foreach(var function in this.functions.Children) {
				node.Properties.Insert(node.Properties.Count, new ObjectLiteralProperty(node.Context) {
					Name = new ObjectLiteralField("Name", PrimitiveType.String, node.Context),
					Value = function
				});
			}
		}
	}
}

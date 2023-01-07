using System.Text;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: tools <output directory>");
    Environment.Exit(64);
}

var outputDir = args[0];

DefineAst("Expression", new[]
{
    "Binary   : Expression Left, Token TokenOperator, Expression Right",
    "Grouping : Expression Expression",
    "Literal  : object Value",
    "Unary    : Token TokenOperator, Expression Right"
});

void DefineAst(string baseName, IEnumerable<string> types)
{
    var path = $"{outputDir}/{baseName}.cs";
    using var streamWriter = new StreamWriter(path, false, Encoding.UTF8);
    
    streamWriter.WriteLine("using Loxsharp;");
    streamWriter.WriteLine();

    streamWriter.WriteLine($"internal abstract class {baseName}\n{{");

    foreach (var type in types)
    {
        var className = type.Split(":")[0].Trim();
        var fields = type.Split(":")[1].Trim(); 
        DefineType(streamWriter, baseName, className, fields);
        streamWriter.WriteLine();
    }
    
    streamWriter.WriteLine("}");
}

void DefineType(TextWriter streamWriter, string baseName, string className, string fieldList)
{
    streamWriter.WriteLine($"   internal sealed class {className} : {baseName}\n   {{");

    // Constructor.
    streamWriter.WriteLine($"       public {className} ({fieldList})\n       {{");
    var fields = fieldList.Split(", ");
    foreach (var field in fields) 
    {
        var name = field.Split(" ")[1];
        streamWriter.WriteLine($"           this.{name} = {name};");
    }
    
    streamWriter.WriteLine("       }");
    
    // Fields.
    foreach (var field in fields) 
    {
        streamWriter.WriteLine();
        streamWriter.WriteLine($"      public {field} {{ get; }}");
    }

    streamWriter.WriteLine("   }");
}
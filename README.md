# csharp-call-graph

A simple tool to generate a call graph for calls of C# code. We store call graph into Neo4j Database which can be used to do Code Analysis.

You can use this command line utility by passing the following arguments:


CsharpCallGraphToNeo4j.exe  <msbuild-path> <project-file/solution-file> <neo4j-bolt-url> <neo4j-username> <neo4j-password> [project-name-filter]
example :
```
CsharpCallGraphToNeo4j.exe  "C:\Program Files\dotnet\sdk\5.0.100-rc.2.20479.15" "D:\github\MSBuildLocator\MSBuildLocator.sln" "bolt://localhost:7687" neo4j pass
```

Your project method call graph will be uploaded into Neo4j database with this simple schema , which can be later derived into more levels. Below is the schema for generated nodes.


## Schema
* #### Method Node 
 
  Method node contains lot information about the method 
 ```
 
 :Method  {
 
 
hashCode: "1920986537", // hashCode based on method signature 
method_name //"ToString",
document_id    //"be32cfb6-55b9-4fe8-877a-144b9c1a8bf8",
project_name  //"Microsoft.CodeAnalysis(netstandard2.0)",
project_id "eeb8ad3a-6eb4-4c4f-9170-e99045097406",


"classname": "Microsoft.CodeAnalysis.CodeAnalysisResourcesLocalizableErrorArgument",
"project_path": "D:\github\roslyn\src\Compilers\Core\Portable\Microsoft.CodeAnalysis.csproj",

method_symbol_name:  //"ToString",
solution_id   //"41a8df19-af06-4a5a-ba35-e27e649e74e2",
method_symbol_containing_namespace // "Microsoft.CodeAnalysis",
document_name //"CodeAnalysisResourcesLocalizableErrorArgument.cs",
document_path //"D:\github\roslyn\src\Compilers\Core\Portable\CodeAnalysisResourcesLocalizableErrorArgument.cs",



textspan_start //"737",
textspan_end // "834",


method_symbol_isStatic //true or false
method_symbol_isExtern" //true or false
method_symbol_isSealed" //true or false
method_symbol_isImplcitlyDeclared //true or false
method_symbol_isAsync //true or false
method_symbol_isGenericMethod  //true or false



method_symbol_containing_module  // "Microsoft.CodeAnalysis.dll",
method_symbol_associated_symbol: "",

method_symbol_defination //"Microsoft.CodeAnalysis.CodeAnalysisResourcesLocalizableErrorArgument.ToString()",
method_symbol_orginal_defination  //"Microsoft.CodeAnalysis.CodeAnalysisResourcesLocalizableErrorArgument.ToString()",


method_symbol_ReturnType //string,
method_symbol_isVirtual //False,
method_symbol_isAbstract //False,
method_symbol_containing_assembly //"Microsoft.CodeAnalysis, Version=42.42.42.42, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
method_symbol_isOverride  //"True",
method_symbol_HidesBaseMethodsByName  // "False"



name //"1920986537"   We have choosed hashCode as the label for method node

  }
}
```

* #### METHOD_CALL Relationship
```
(:Method)-[:METHOD_CALL]->(:Method)
```



## Further More

If you want to derive more Class Dependency Graph from Call Graph or do more with the data, you see this tutorial. [code-analysis-tutorial-neo4j](https://github.com/vinitsiriya/code-analysis-tutorial-neo4j)






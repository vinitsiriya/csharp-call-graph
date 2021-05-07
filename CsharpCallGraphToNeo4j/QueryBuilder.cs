using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpCallGraphToNeo4j
{
    public class QueryBuilder
    {
        public class QueryContext
        {
           public Workspace workspace;
            public Project proj;
            public Document doc;
           // public IMethodSymbol methodSymbol;
            //public MethodDeclarationSyntax method;
           // public Location location;

            public QueryContext(Workspace workspace, Project proj, Document doc)
            {
                this.workspace = workspace;
                this.proj = proj;
                this.doc = doc;
               // this.methodSymbol = methodSymbol;
                //this.method = method;
              //  this.location = location;
            }

           
        }
       

        public QueryBuilder()
        {
           
        }

        public static int toHashCode(IMethodSymbol symbol, String[] otherproperties)
        {


            String name = symbol.ContainingNamespace + symbol.Name;
            var parameters = symbol.Parameters.Select(x => x.Type).ToArray();
            int hash = 7;
            hash = 31 * hash + (int)name.GetHashCode();
            foreach (var p in parameters)
                hash = 31 * hash + (p == null ? 0 : p.GetHashCode());
            foreach (var p in otherproperties)
                hash = 31 * hash + (p == null ? 0 : p.GetHashCode());
            return hash;

        }

        public static string GetKeyValueForMethod(QueryContext queryContext, IMethodSymbol methodSymbol, MethodDeclarationSyntax method)
        {
            Workspace Workspace = queryContext.workspace;
            Project proj = queryContext.proj;
            Document doc = queryContext.doc;
       
            //Location location = queryContext.location;

            //var invokes = method.DescendantNodes().Where(x=>x is InvocationExpressionSyntax).ToList();
            String method_name = method.Identifier.Text;

            String solution_id = Workspace.CurrentSolution.Id.Id.ToString();

            String project_path = proj.FilePath;
            String project_name = proj.Name;
            String project_assemblypath = proj.AssemblyName;
            String project_id = proj.Id.Id.ToString();

            String document_id = doc.Id.Id.ToString();
            String document_name = doc.Name.ToString();
            String document_path = doc.FilePath.ToString();
            String document_folders = string.Join(",", doc.Folders);

            String textspan_start = method.Span.Start.ToString() ;//location.GetMappedLineSpan().Span.Start.ToString();
            String textspan_end = method.Span.End.ToString();//location.GetMappedLineSpan().Span.End.ToString();




            String hashCode = toHashCode(methodSymbol, new String[] { solution_id, project_id, document_id, doc.FilePath }).ToString();

            // String method_parameter_list =   method.ParameterList.Parameters.Select(x=>x.ToString());
            String method_languge = method.Language;

            int method_arity = method.Arity;

            String method_declaration_line1 = "";
            {
                String methodstring = method.ToString();
                if (methodstring.Contains("\n"))

                {
                    var i = methodstring.IndexOf("\n");
                    method_declaration_line1 = methodstring.Substring(0, i + 1);
                }

            }
            String method_symbol_name = methodSymbol.Name;


            String method_symbol_defination = methodSymbol.ToString(); //Microsoft.CodeAnalysis.SemanticModel.GetSymbolInfo(Microsoft.CodeAnalysis.SyntaxNode, System.Threading.CancellationToken)
            String method_symbol_orginal_defination = methodSymbol.OriginalDefinition.ToString();
            String method_symbol_associated_symbol = "";



            if (methodSymbol.AssociatedSymbol != null)
                method_symbol_associated_symbol = methodSymbol.AssociatedSymbol.ToString();

            String method_symbol_containing_module = methodSymbol.ContainingModule.ToString();
            String method_symbol_containing_namespace = methodSymbol.ContainingNamespace.ToString();
            String method_symbol_containing_assembly = methodSymbol.ContainingAssembly.ToString(); //ContainingAssembly
            bool method_symbol_HidesBaseMethodsByName = methodSymbol.HidesBaseMethodsByName;
            bool method_symbol_isAbstract = methodSymbol.IsAbstract;
            bool method_symbol_isAsync = methodSymbol.IsAsync;
            bool method_symbol_isGenericMethod = methodSymbol.IsGenericMethod;
            bool method_symbol_isImplcitlyDeclared = methodSymbol.IsImplicitlyDeclared;
            bool method_symbol_isOverride = methodSymbol.IsOverride;
            bool method_symbol_isSealed = methodSymbol.IsSealed;
            bool method_symbol_isStatic = methodSymbol.IsStatic;
            bool method_symbol_isVirtual = methodSymbol.IsVirtual;
            bool method_symbol_isExtern = methodSymbol.IsExtern;
            String method_symbol_ReturnType = methodSymbol.ReturnType.ToString();
            String className = methodSymbol.ContainingType.ToString();





            String[] keys = new String[]{
                      "name",
                    "method_name",
                    "solution_id",
                    "project_path",
                    "project_name",
                    "project_id",
                    "document_id",
                    "document_name",
                    "document_path",
                    "document_folders",
                    "textspan_start",
                    "textspan_end",

                    "hashCode",

                    "method_arity",
                    "method_declaration_line1",
                    "method_symbol_name",
                    "method_symbol_defination",
                    "method_symbol_orginal_defination",
                    "method_symbol_associated_symbol",
                    "method_symbol_containing_module",
                    "method_symbol_containing_namespace",
                    "method_symbol_containing_assembly",
                    "method_symbol_associated_symbol",
                    "method_symbol_containing_module",
                    "method_symbol_containing_namespace",
                    "method_symbol_containing_assembly",

                    "method_symbol_HidesBaseMethodsByName",
                    "method_symbol_isAbstract",
                    "method_symbol_isAsync",
                    "method_symbol_isGenericMethod",
                    "method_symbol_isImplcitlyDeclared",
                    "method_symbol_isOverride",
                    "method_symbol_isSealed",
                    "method_symbol_isStatic",
                    "method_symbol_isVirtual",
                    "method_symbol_isExtern",
                    "method_symbol_ReturnType",
                     "class_name"

                    };


            String[] values = new String[]{
                      hashCode,
                        method_name,
                        solution_id,
                        project_path,
                        project_name,
                        project_id,
                        document_id,
                        document_name,
                        document_path,
                        document_folders,

                        textspan_start,
                        textspan_end,
                          hashCode ,

                        method_arity.ToString(),
                        method_declaration_line1,
                        method_symbol_name.ToString(),
                        method_symbol_defination.ToString(),
                        method_symbol_orginal_defination.ToString(),
                        method_symbol_associated_symbol,
                        method_symbol_containing_module,
                        method_symbol_containing_namespace,
                        method_symbol_containing_assembly,
                        method_symbol_associated_symbol,
                        method_symbol_containing_module,
                        method_symbol_containing_namespace,
                        method_symbol_containing_assembly,
                        method_symbol_HidesBaseMethodsByName.ToString(),
                        method_symbol_isAbstract.ToString(),
                        method_symbol_isAsync.ToString(),
                        method_symbol_isGenericMethod.ToString(),
                        method_symbol_isImplcitlyDeclared.ToString(),
                        method_symbol_isOverride.ToString(),
                        method_symbol_isSealed.ToString(),
                        method_symbol_isStatic.ToString(),
                        method_symbol_isVirtual.ToString(),
                        method_symbol_isExtern.ToString(),
                        method_symbol_ReturnType.ToString(),
                         className




                    };

            String querypart = "";

            for (int i = 0; i < values.Length; i++)
            {

                querypart += keys[i] + ":'" + values[i] + "',\n";
            }


            return querypart;
        }


        public static String GetKeyValueForMetaMethod(QueryContext queryContext, SymbolInfo sym)
        {

           

            var original_defination = sym.Symbol.OriginalDefinition.ToString();
            var containing_assembly = sym.Symbol.ContainingAssembly.ToString();
            var containing_module = sym.Symbol.ContainingModule.ToString();
            var containing_namespace = sym.Symbol.ContainingNamespace.ToString();


            String[] keys = new String[] { "original_defination", "containing_assembly",
                                           "containing_module" , "containing_namespace" };
            String[] values = new String[] { original_defination, containing_assembly, containing_module, containing_module };

           
            String querypart = "";

            for (int i = 0; i < values.Length; i++)
            {

                querypart += keys[i] + ":'" + values[i] + "',\n";
            }


            return querypart;
        }




    }
}

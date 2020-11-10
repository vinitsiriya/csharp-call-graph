using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CsharpCallGraphToNeo4j.QueryBuilder;

namespace CsharpCallGraphToNeo4j
{
  public  class CallGraphWorker
    {
        Workspace workspace;
        String neo4jUrl;
        String neo4jUsername;
        String neo4jPassword;

           
        public CallGraphWorker(Workspace workspace, String neo4jUrl,String neo4jUsername, String neo4jPassword)
        {
            this.workspace = workspace;
            this.neo4jUrl = neo4jUrl;
            this.neo4jUsername = neo4jUsername;
            this.neo4jPassword = neo4jPassword;
        }


        public void Run(Project[] projects, int MaxDegreeOfParallelism_Project = 1 ,int MaxDegreeOfParallelism_Document = 1 )
        {



            List<Task> tasksinparallel = new List<Task>();
            processProjects(projects, MaxDegreeOfParallelism_Project, MaxDegreeOfParallelism_Document);



        }


        private void processProjects(Project[] projects, int MaxDegreeOfParallelism_Project, int MaxDegreeOfParallelism_Document = 1)
        {
            Parallel.ForEach(projects, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism_Project },
           (project) =>
           {
               processProject(project, MaxDegreeOfParallelism_Document);
           });
         }



        private void processProject(Project project, int MaxDegreeOfParallelism_Document = 1)
        {

             processDocuments(project.Documents.ToArray(), MaxDegreeOfParallelism_Document);

        }


        private void processDocuments(Document[] documents, int MaxDegreeOfParallelism_Document = 1)
        {
            Parallel.ForEach(documents, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism_Document },
            (document) =>
            {
               processDocument(document,document.Project).ConfigureAwait(true).GetAwaiter().GetResult();

            });
        }

        private async Task processDocument(Document document,Project containingProject)
        {
           
                var semmodel = await document.GetSemanticModelAsync();
                var syntaxTree = semmodel.SyntaxTree;
                var syntaxRoot = await semmodel.SyntaxTree.GetRootAsync();
                var methods = syntaxRoot.DescendantNodes() 
                                        .Where(x => x is MethodDeclarationSyntax)
                                        .Select(x => (MethodDeclarationSyntax)x).ToList();


                var driver = GraphDatabase.Driver(neo4jUrl, AuthTokens.Basic(neo4jUsername, neo4jPassword));
                var session = driver.Session();

                //List<Task> methodstask = new List<Task>();

                foreach (var method in methods)

                {

                    //toNeo4jQuery(Workspace Workspace,Project proj,Document doc,SemanticModel sem,MethodDeclarationSyntax method)
                    // Func<Task<int>> querypa = async () => {

                    var methodSymbol = (IMethodSymbol)semmodel.GetDeclaredSymbol(method);


                    var invokes = method.DescendantNodes().Where(x => x is InvocationExpressionSyntax).Select(x => (InvocationExpressionSyntax)x);
                    QueryContext query1 = new QueryContext(workspace, containingProject, document, (IMethodSymbol)methodSymbol, method);
                    var query1KeyValue = QueryBuilder.GetKeyValueFor(query1);


                    foreach (var invoke in invokes)
                    {
                        var sym = semmodel.GetSymbolInfo(invoke);
                        if (sym.Symbol != null)
                        {



                            foreach (var syntaxRef in sym.Symbol.DeclaringSyntaxReferences)
                            {
                                if (syntaxRef != null)
                                {
                                    var syntaxnode = syntaxRef.GetSyntax();
                                    //localfunction
                                    if (syntaxnode != null && syntaxnode is MethodDeclarationSyntax)
                                    {

                                        var invMethodSyntax = (MethodDeclarationSyntax)syntaxnode;
                                        var invdoc = workspace.CurrentSolution.GetDocument(syntaxRef.SyntaxTree);
                                        var invsem = await invdoc.GetSemanticModelAsync();
                                        var invsym = invsem.GetDeclaredSymbol(invMethodSyntax);
                                        QueryContext query2 = new QueryContext(workspace, invdoc.Project, invdoc, (IMethodSymbol)invsym, invMethodSyntax);
                                        var query2KeyValue = QueryBuilder.GetKeyValueFor(query2);





                                        String querystr = @"
                                                                                        MERGE (a:Method 

                                                                                            {

                                                                                                   " + query1KeyValue.Trim().TrimEnd(',') + @"           

                                                                                            })

                                                                                          
                                                                                        MERGE

                                                                                           (b:Method 
                                                                                             {

                                                                                                   " + query2KeyValue.Trim().TrimEnd(',') + @"           

                                                                                            })
    
    
                                                                                           
    
   
    
                                                                                          MERGE (a)-[r:METHOD_CALL]->(b)
                                                                                             return *
    
    
    
                                                                                 ";
                                        String querystr2 = querystr.Replace("\\", "\\\\");

                                        session.Run(querystr2);




                                    }
                                    else
                                        Debug.Write("call syntax not found / Calls to Local function are not yet supported ");
                                }

                            }
                        }

                    }



            






                }

            
        }


     

 


    }
}

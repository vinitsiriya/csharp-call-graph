using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpCallGraphToNeo4j
{
  public  class CallGraphToNeo4j
    {

        public static int DEGREE_PARALLELISM_PROJECT = 1;
        public static int DEGREE_PARALLELISM_DOCUMENT = 4; // YOU CAN TWEAK THE DEGREE OF PARALLELISM


        MSBuildWorkspace workspace;
       public CallGraphToNeo4j(MSBuildWorkspace workspace)
        {
            this.workspace = workspace;

        }

        public  void LoadProject(String proj_file)
        {
            workspace.OpenProjectAsync(proj_file).GetAwaiter().GetResult();
        }

        public void LoadSolution(String solution_file)
        {
            workspace.OpenSolutionAsync(solution_file).GetAwaiter().GetResult();
        }

        public  void Run(String neo4jUrl, String neo4jusername, String neo4jpassword, String namefilter_contains=""){

   

           

           







   
            Project[] projects;
            
            if(namefilter_contains.Equals( ""))
            projects = workspace.CurrentSolution.Projects.ToArray(); //.Where();
            else
            {
                projects = workspace.CurrentSolution.Projects.Where(x => x.Name.Contains(namefilter_contains)).ToArray();
            }

            Run(projects, neo4jUrl, neo4jusername, neo4jpassword);
           


        }

        public void Run(Project[] projects, String neo4jUrl, String neo4jusername, String neo4jpassword)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();     


            CallGraphWorker worker = new CallGraphWorker(workspace, neo4jUrl, neo4jusername, neo4jpassword);

            



            worker.Run(projects, DEGREE_PARALLELISM_PROJECT, DEGREE_PARALLELISM_DOCUMENT);



            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");


        }




    }
}

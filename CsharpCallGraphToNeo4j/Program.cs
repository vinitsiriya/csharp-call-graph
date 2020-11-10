using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace CsharpCallGraphToNeo4j
{
    class Program
    {

        static  String[] DLL_LOOKUP_PATHS = 
            new string[] { MSBUILD_PATH };
        static String MSBUILD_PATH; // It can be path for dotnet sdk, generally it is like C:\Program Files\dotnet\sdk\5.0.100-rc.2.20479.15\



        static void Main(string[] args)
        {
 

            if (args.Length < 5)
            {
                Console.WriteLine("Arguments : <msbuild-path> <project-file/solution-file> <neo4j-url> <neo4j-username> <neo4j-password> [project-name-filter]");
                Console.WriteLine("project-name-filter : optional argument, keyword present in project-name or project name");
               Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            String MsbuildPath = args[0];          
            String project_solution_file = args[1];
            String neo4jUrl = args[2];
            String neo4jUsername = args[3];
            String neo4jPassword = args[4];
            String namefilter = "";
            if (args.Length > 5)
            {
                namefilter = args[5];
   
            }


            //check arguments 
            if (!Directory.Exists(MsbuildPath))
            {
                Console.WriteLine("MsBuildpath is not valid");
                return;
            }
            if (!File.Exists(project_solution_file))
            {
                Console.WriteLine("Solution/Project file is not valid.");
            }
            bool isvalidproj = (project_solution_file.Trim().EndsWith(".csproj") || project_solution_file.Trim().EndsWith(".sln"));
            if (!isvalidproj)
            {
                Console.WriteLine("pass .csproj/.sln file is not valid.");
            }


            Console.WriteLine("Program Loaded with parameters:");
            Console.WriteLine("msbuildpath:" + MsbuildPath);
            Console.WriteLine("project_solution_file:" + project_solution_file);
            Console.WriteLine("neo4jUrl:" + neo4jUrl);
            Console.WriteLine("neo4jUsername:" + neo4jUsername);
            Console.WriteLine("neo4jPassword: <not-shown>");
            Console.WriteLine("namefilter:" + namefilter);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();



            //Load MSBuild
            MSBUILD_PATH = MsbuildPath;
            DLL_LOOKUP_PATHS[0] = MSBUILD_PATH;
            var instances = MSBuildLocator.QueryVisualStudioInstances();
            var instance = instances.First();
            MSBuildLocator.RegisterMSBuildPath(MSBUILD_PATH);
            AssemblyLoadContext.Default.Resolving += Default_Resolving;






                DLL_LOOKUP_PATHS[0] = MSBUILD_PATH;
            //String AnyMorePathForLookup = ""; Add to DLL_LOOKUP_PATH


            Run_CallGraphToNeo4j(project_solution_file, neo4jUrl, neo4jUsername, neo4jPassword);







        }

    



        static void Run_CallGraphToNeo4j(String project_solution_file,String neo4jUrl,String neo4jusername,String neo4jpassword,String namefilter="")
        {

            Console.WriteLine("Loading Projects...");
            Console.WriteLine();


            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            if (project_solution_file.Trim().EndsWith(".csproj"))
                workspace.OpenProjectAsync(project_solution_file).ConfigureAwait(true).GetAwaiter().GetResult();
            else if(project_solution_file.Trim().EndsWith(".sln"))
                workspace.OpenSolutionAsync(project_solution_file).ConfigureAwait(true).GetAwaiter().GetResult();


          


            Project[] projects_loaded = workspace.CurrentSolution.Projects.ToArray();
            Project[] projects;
            if (namefilter == "")
                projects = workspace.CurrentSolution.Projects.ToArray();
            else
               projects = workspace.CurrentSolution.Projects.Where(x=>x.Name.Contains(namefilter)).ToArray();



            Console.WriteLine("Loaded Projects:");

            for (int x = 0; x < projects_loaded.Length; x++  )
            {
                var project = projects_loaded[x];
                Console.WriteLine(x+". "+project.Name.ToString());
            }




            Console.WriteLine("Selected Projects:");

            for (int x = 0; x < projects.Length; x++)
            {
                var project = projects[x];
                Console.WriteLine(x + ". " + project.Name);
            } 


            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();




            Console.WriteLine("Errors while loading Ppojects:");

            foreach (var d in workspace.Diagnostics)
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(d.Message + "\n");
                Console.WriteLine("--------------------------------------");
                //  error = true;
            }



            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();



            CallGraphToNeo4j callGraphToNeo4J = new CallGraphToNeo4j(workspace);
            callGraphToNeo4J.Run( neo4jUrl, neo4jusername, neo4jpassword,namefilter);

        }




        /* These functions are needed .NET Core to resolve assemblies. We are using it to resolve and load MSBuild Assemblies.
         * Please make change in global paths.
         * 
         * 
         */

        private static System.Reflection.Assembly Default_Resolving(AssemblyLoadContext assemblyLoadContext, System.Reflection.AssemblyName assemblyName)
        {
            try
            {

                return TryLoadAssembly(assemblyName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
          
            return null;
        }


        static Assembly TryLoadAssembly(AssemblyName assemblyName)
        {

            Assembly assembly;


            var lookupFolder = DLL_LOOKUP_PATHS;
            foreach (var d in lookupFolder)
            {
                string str = Path.Combine(d, assemblyName.Name + ".dll");
                if (File.Exists(str))
                {

                    assembly = Assembly.LoadFrom(str);

                    return assembly;
                }
            }

            return (Assembly)null;

        }




  


    }
    

}


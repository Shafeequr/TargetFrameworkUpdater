using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;

namespace FrameworkUpdater
{
    public class Migrator
    {
        public String vSolutionPath = null;
        EnvDTE.DTE dte = (EnvDTE.DTE)Activator.CreateInstance(System.Type.GetTypeFromProgID("VisualStudio.DTE", true), true);
        public Migrator(String SolutionPath)
        {
            this.vSolutionPath = SolutionPath;
        }
        public void Migrate(String TargetFramework)
        {
            this.dte.Solution.Open(this.vSolutionPath);
            int vProjectCounts = dte.Solution.Projects.Count;
            for (int i = 1; i <= vProjectCounts; i++)
            {
                EnvDTE.Project oProject = dte.Solution.Projects.Item(i);
                oProject.Properties.Item("TargetFrameworkMoniker").Value = TargetFramework;
            }
            dte.Solution.Close();
        }
        public Model.Solution CheckProjectVersion()
        {
            Model.Solution oSolution = new Model.Solution();
            this.dte.Solution.Open(this.vSolutionPath);
            SolutionBuild oSolutionBuild = this.dte.Solution.SolutionBuild;
            oSolution.StartUpProject = oSolutionBuild.StartupProjects[0].ToString();
            oSolution.StartUpProject = oSolution.StartUpProject.Split('\\')[0];
            List<Model.Projects> oProjectDetails = new List<Model.Projects>();
            foreach (EnvDTE.Project oProject in this.dte.Solution.Projects)
            {
                if (oProject.Name == oSolution.StartUpProject)
                {
                    String vStartUpProjectPath = Path.GetDirectoryName(this.vSolutionPath) + "\\" + oSolution.StartUpProject;
                    oSolution.Framework = oProject.Properties.Item("TargetFrameworkMoniker").Value.ToString();
                    oProjectDetails.Add(new Model.Projects() { ProjectName = oProject.Name, FrameworkVersion = oProject.Properties.Item("TargetFrameworkMoniker").Value.ToString() });
                }
                else
                {
                    oProjectDetails.Add(new Model.Projects() { ProjectName = oProject.Name, FrameworkVersion = oProject.Properties.Item("TargetFrameworkMoniker").Value.ToString() });
                }
            }
            int vVersionCount = (from projects in oProjectDetails select projects.FrameworkVersion).Distinct().Count();
            if(vVersionCount > 1)
            {
                oSolution.AreAllProjectofSameVersion = false;
            }
            else
            {
                oSolution.AreAllProjectofSameVersion = true;
            }
            this.dte.Solution.Close();
            return oSolution;
        }
    }
}

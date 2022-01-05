using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Confluence_OfflineSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("I expect the project path as parameter");
                return;
            }

            string projectFolder = args[0];
            if (Directory.Exists(projectFolder) == false)
            {
                Console.WriteLine($"The path to your project does not exist - {projectFolder}");
                return;
            }


            //------------------------------------------------------------------------
            //>> Copy files required
            CopyRequiredFiles(projectFolder);


            //------------------------------------------------------------------------
            //>> Create documents

            string sTmp;
            int currentId = 1, iTmp;
            char openBracket = '{', closeBracket = '}';
            StringBuilder documentFiller = new StringBuilder();
            string[] listHtmlFiles = Directory.GetFiles(projectFolder, "*.html");

            foreach (string htmlFile in listHtmlFiles)
            {
                Console.WriteLine($"Analyze: {htmlFile}");
                string htmlContent = File.ReadAllText(htmlFile);

                //--> Step 1: Creazione dell'oggettto docuemnt in js
                int docId = currentId++;

                string docPage = Path.GetFileName(htmlFile);

                sTmp = "<title>";
                iTmp = htmlContent.IndexOf(sTmp);
                string docTitle = htmlContent.Substring(iTmp + sTmp.Length, htmlContent.IndexOf("</title>") - iTmp - sTmp.Length);

                string docBody = Regex.Replace(htmlContent, @"<(.|\n)*?>", "").Replace("\t", "").Replace("\n", "").Replace("  ", "").Replace("\r", "");

                documentFiller.Append($"var doc{docId} = {openBracket} ID: \"{docId}\", Title: \"{docTitle}\", Body: \"{docBody}\", Page: \"{docPage}\" {closeBracket}; \r\n index.addDoc(doc{docId});\r\n");


                //--> Step 2: Se il file non include i file ncessari lo faccio io
                if (htmlContent.Contains("<script type='text/javascript' src='js/documentsFiller.js'></script>") == false)
                {
                    iTmp = htmlContent.IndexOf("<META http-");

                    string htmlContentWithIncludes = htmlContent.Substring(0, iTmp);
                    htmlContentWithIncludes += "\t<link rel='stylesheet' href='styles/offline-search.css' type='text/css' />\r\n";
                    htmlContentWithIncludes += "\t<script type='text/javascript' src='js/jquery-3.5.1.min.js'></script>\r\n";
                    htmlContentWithIncludes += "\t<script type='text/javascript' src='js/elasticlunr.min.js'></script>\r\n";
                    htmlContentWithIncludes += "\t<script type='text/javascript' src='js/offline-search.js'></script>\r\n";
                    htmlContentWithIncludes += "\t<script type='text/javascript' src='js/documentsFiller.js'></script>\r\n";
                    htmlContentWithIncludes += htmlContent.Substring(iTmp);

                    File.WriteAllText(htmlFile, htmlContentWithIncludes);

                }
            }

            File.WriteAllText(Path.Combine(projectFolder, "js", "documentsFiller.js"), documentFiller.ToString());
        }


        static void CopyRequiredFiles(string projectFolder)
        {
            string pathFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");

            string jsPath = Path.Combine(projectFolder, "js");
            if (!Directory.Exists(jsPath)) Directory.CreateDirectory(jsPath);

            string cssPath = Path.Combine(projectFolder, "styles");
            if (!Directory.Exists(cssPath)) Directory.CreateDirectory(cssPath);


            DirectoryInfo dir = new DirectoryInfo(pathFiles);
            FileInfo[] files = dir.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".js")) files[i].CopyTo(Path.Combine(jsPath, files[i].Name), true);
                else if (files[i].Name.EndsWith(".css")) files[i].CopyTo(Path.Combine(cssPath, files[i].Name), true);
            }
        }

    }
}

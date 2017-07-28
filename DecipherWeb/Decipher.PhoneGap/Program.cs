using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Decipher.PhoneGap
{
    class Program
    {
        static void Main(string[] args)
        {
            string toVersion = String.Empty;
            string fromVersion = String.Empty;
            Console.WriteLine("What version to you want to copy FROM? (Web, Android or iOS)");
            fromVersion = Console.ReadLine();
            Console.WriteLine("What version do you want to copy TO? (Web, Android or iOS)");
            toVersion = Console.ReadLine();
            if (!String.IsNullOrEmpty(fromVersion) && !String.IsNullOrEmpty(toVersion))
            {
                string webBase = ConfigurationManager.AppSettings["WebBase"];
                string appBase = ConfigurationManager.AppSettings["AppBase"];
                string androidBase = ConfigurationManager.AppSettings["AndroidBase"];
                string iosBase = ConfigurationManager.AppSettings["iOSBase"];
                string fromRoot = String.Empty;
                switch (fromVersion)
                {
                    case "App":
                        fromRoot = appBase;
                        break;
                    case "Android":
                        fromRoot = androidBase;
                        break;
                    case "iOS":
                        fromRoot = iosBase;
                        break;
                    default:
                        // Web
                        fromRoot = webBase;
                        break;
                }
                string toRoot = String.Empty;
                switch (toVersion)
                {
                    case "App":
                        toRoot = appBase;
                        break;
                    case "iOS":
                        toRoot = iosBase;
                        break;
                    case "Android":
                        toRoot = androidBase;
                        break;
                    default:
                        // Web
                        toRoot = webBase;
                        break;
                }
                // Copy css files
                string[] cssFiles = System.IO.Directory.GetFiles(fromRoot + @"\css");
                foreach (string cssFile in cssFiles)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(cssFile);
                    System.IO.File.Copy(cssFile, toRoot + @"\css\" + info.Name, true);
                }
                // Copy images
                string[] imgFiles = System.IO.Directory.GetFiles(fromRoot + @"\img");
                foreach (string imgFile in imgFiles)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(imgFile);
                    System.IO.File.Copy(imgFile, toRoot + @"\img\" + info.Name, true);
                }
                // Copy js files
                string[] jsFiles = System.IO.Directory.GetFiles(fromRoot + @"\js", "*.js");
                foreach (string jsFile in jsFiles)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(jsFile);
                    System.IO.File.Copy(jsFile, toRoot + @"\js\" + info.Name, true);
                }
                // Client directory
                string[] clientDir = System.IO.Directory.GetDirectories(fromRoot + @"\client");
                foreach (string dir in clientDir)
                {
                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dir);
                    if (dirInfo.Name == "scripts")
                    {
                        // scripts folders
                        string[] scriptsDir = System.IO.Directory.GetDirectories(dir);
                        foreach (string scriptDir in scriptsDir)
                        {
                            System.IO.DirectoryInfo scDirInfo = new System.IO.DirectoryInfo(scriptDir);
                            if (scDirInfo.Name != "app")
                            {
                                // don't copy config and routing as they may be different between web and app
                                string[] scriptFiles = System.IO.Directory.GetFiles(scriptDir);
                                foreach (string scriptFile in scriptFiles)
                                {
                                    System.IO.FileInfo scriptInfo = new System.IO.FileInfo(scriptFile);
                                    System.IO.File.Copy(scriptFile, toRoot + @"\client\scripts\" + scDirInfo.Name + @"\" + scriptInfo.Name, true);
                                }
                            }
                        }
                    }
                    else if (dirInfo.Name == "views")
                    {
                        // view folders
                        string[] viewsDir = System.IO.Directory.GetDirectories(dir);
                        foreach (string viewDir in viewsDir)
                        {
                            System.IO.DirectoryInfo viewDirInfo = new System.IO.DirectoryInfo(viewDir);
                            // files in view folder
                            string[] viewFiles = System.IO.Directory.GetFiles(viewDir);
                            foreach (string viewFile in viewFiles)
                            {
                                System.IO.FileInfo viewFileInfo = new System.IO.FileInfo(viewFile);
                                System.IO.File.Copy(viewFile, toRoot + @"\client\views\" + viewDirInfo.Name + @"\" + viewFileInfo.Name, true);
                            }
                            // folders in view folder
                            string[] viewFolders = System.IO.Directory.GetDirectories(viewDir);
                            foreach (string viewFolder in viewFolders)
                            {
                                System.IO.DirectoryInfo viewFolderInfo = new System.IO.DirectoryInfo(viewFolder);
                                // files in subfolder
                                string[] viewFolderFiles = System.IO.Directory.GetFiles(viewFolder);
                                foreach (string viewFolderFile in viewFolderFiles)
                                {
                                    System.IO.FileInfo viewFolderFileInfo = new System.IO.FileInfo(viewFolderFile);
                                    System.IO.File.Copy(viewFolderFile, toRoot + @"\client\views\" + viewDirInfo.Name + @"\" + viewFolderInfo.Name + @"\" + viewFolderFileInfo.Name, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

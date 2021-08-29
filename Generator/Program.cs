using System;
using System.IO;
using System.Collections.Generic;

namespace Generator
{
    public class SiteResource
    {
        private string file;

        public SiteResource(string file)
        {
            this.file = file;
        }

        public bool Ignorable {
            get {
                foreach (string path_part in file.Split(Path.DirectorySeparatorChar)) {
                    if (path_part.StartsWith(".")) {
                        return true;
                    }
                }
                return false;
            }
        }

        public override string ToString()
        {
            if (Ignorable)
            {
                return "[I]" + file;
            }
            else 
            { 
                return file;
            }
        }

        public void Publish(string source, string dest) {
            if (Ignorable) {
                return;
            }

            var file_source = source + Path.DirectorySeparatorChar + file;
            /*
            if (File.Exists(file_source)) {
                return;
            }
            */

            System.Console.WriteLine(
                file_source +
                " => " +
                dest + Path.DirectorySeparatorChar + file
            );


            /*
            File.Copy(
                file_source,
                dest + Path.DirectorySeparatorChar + file
            );
            */
        }
    }


    public class Site {
        private string root;
        private string template = "Template";
        private string output = "Output";

        public Site(string root)
        {
            this.root = root;
        }

        public static Site Create(string root)
        {
            System.Console.WriteLine("Creating site in " + root);
            return new Site(root);
        }

        public void Build() {
            System.Console.WriteLine("Building In " + root);
            Directory.CreateDirectory(root + Path.DirectorySeparatorChar + output);
            var full_source_path = root + Path.DirectorySeparatorChar + template;
            var site_file_paths = ProcessDirectory(full_source_path);
            var site_resources = site_file_paths
                .ConvertAll<SiteResource>(file => new SiteResource(file.Split(full_source_path)[1].Substring(1)));
            site_resources.ForEach(resource => {
                //System.Console.WriteLine(resource.ToString());
                resource.Publish(full_source_path, root + Path.DirectorySeparatorChar + output);
            });
        }

        public void Clean() {

            Directory.Delete(root + Path.DirectorySeparatorChar + output, true);
        }

        private List<string> ProcessDirectory(string root_directory) { 
            var file_list = new List<string>(Directory.GetFiles(root_directory));
            var directories = Directory.GetDirectories(root_directory);
            foreach (string directory_name in directories) {
                file_list.AddRange(ProcessDirectory(directory_name));
            }
            return file_list;
        }

    }

    class CLI {

        public static void Execute(string[] args) 
        { 
            if (args[0] == null) { 
                throw new ArgumentException("Not a valid command must provide an option.");
            }

            if (args[0] == "new") {
                if (args[1] == null) { 
                    throw new ArgumentException("Must Provide a Site Path");
                }

                Site.Create(args[1]);
                System.Environment.Exit(0);
            }

            if (args[0] == "build") {
                var root = args.Length > 1 ? args[1] : System.Environment.CurrentDirectory;
                var site = new Site(root);
                site.Build();
                System.Environment.Exit(0);
            }

            if (args[0] == "clean") {
                var root = args.Length > 1 ? args[1] : System.Environment.CurrentDirectory;
                var site = new Site(root);
                site.Clean();
                System.Environment.Exit(0);
            }

            throw new ArgumentException(args[1] + "Not a valid command");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CLI.Execute(args);
        }
    }
}

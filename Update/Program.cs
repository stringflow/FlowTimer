using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Update {

    public class Program {

        public const string URL = "http://gunnermaniac.com/flowtimer/";

        public static int Main(string[] args) {
            if(args.Length >= 1) {
                if(args[0].ToLower() == "-latest") {
                    return GetLatestBuild();
                } else if(args[0].ToLower() == "-update") {
                    if(args.Length <= 3) {
                        Console.WriteLine("Usage: -update (build version) (current exe) <process id>");
                        return -1;
                    }

                    try {
                        Update(args[1], Path.GetDirectoryName(args[2]), Path.GetFileName(args[2]), args.Length >= 4 ? int.Parse(args[3]) : -1);
                        return 0;
                    } catch(Exception e) {
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }

            return -1;
        }

        private static int GetLatestBuild() {
            List<string> files = new List<string>();
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);

            using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
                    string html = reader.ReadToEnd();

                    Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                    MatchCollection matches = regex.Matches(html);

                    if(matches.Count > 0) {
                        foreach(Match match in matches) {
                            if(match.Success) {
                                string[] matchData = match.Groups[0].ToString().Split('\"');
                                files.Add(matchData[1]);
                            }
                        }
                    }
                }
            }

            List<int> versions = files.Skip(2).Select(val => int.Parse(Path.GetFileNameWithoutExtension(val))).OrderBy(val => val).ToList();
            return versions.Last();
        }

        private static void Update(string build, string directory, string fileName, int processId = -1) {
            string download = URL + build + ".exe";
            string src = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\new.exe";
            string dest = directory + "\\" + fileName;

            if(processId != -1) {
                Process process = Process.GetProcessById(processId);
                process.Kill();
            }

            using(WebClient client = new WebClient()) {
                client.DownloadFile(download, src);
            }

            File.Delete(dest);
            File.Move(src, dest);

            if(processId != -1) {
                Process.Start(dest);
                MessageBox.Show("FlowTimer has been successfully updated to build " + build + "!", "Update Sucessful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

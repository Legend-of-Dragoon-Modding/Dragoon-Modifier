using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class ModVersion {

        public static void Check() {
            try {
                using (TimedWebClient client = new TimedWebClient { Timeout = 1000}) {
                    client.Headers.Add("user-agent", "Anything");
                    string json = client.DownloadString("https://api.github.com/repos/Zychronix/Dragoon-Modifier/releases/latest");
                    var mod_version = JsonSerializer.Deserialize<ModVersionJson>(json);
                    string new_version = mod_version.tag_name.Replace("v", "");
                    Version v1 = new Version(new_version);
                    Version v2 = new Version(Constants.VERSION);
                    if (v1.CompareTo(v2) > 0) {
                        Constants.WriteOutput($"Current version {Constants.VERSION} is outdated. You can download version {new_version} at {mod_version.html_url}");
                        Constants.WriteGLog($"Newer version ({new_version}) available.");
                    }
                }
            } catch (Exception ex) {
                Constants.WriteDebug(ex);
            }
            
        }

        private class TimedWebClient : WebClient {
            public int Timeout { get; set; }

            public TimedWebClient() {
                this.Timeout = 5000;
            }

            protected override WebRequest GetWebRequest(Uri address) {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.Timeout;
                return objWebRequest;
            }

        }

        public class ModVersionJson {
            public string name { get; set; }
            public string tag_name { get; set; }
            public string html_url { get; set; }
        }
    }
}

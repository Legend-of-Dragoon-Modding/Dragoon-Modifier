using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class ModVersion {
        public static bool IsCurrent(string currentVersion, out string newVersion, out string uri) {
            using (TimedWebClient client = new TimedWebClient { Timeout = 1500 }) {
                client.Headers.Add("user-agent", "Anything");
                string json = client.DownloadString("https://api.github.com/repos/Zychronix/Dragoon-Modifier/releases/latest");
                var modVersion = JsonSerializer.Deserialize<ModVersionJson>(json);
                newVersion = modVersion.tag_name.Replace("v", "");
                uri = modVersion.html_url;
                Version v1 = new Version(newVersion);
                Version v2 = new Version(currentVersion);
                if (v1.CompareTo(v2) > 0) {
                    return false;
                }
                return true;
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

        private class ModVersionJson {
            public string name { get; set; }
            public string tag_name { get; set; }
            public string html_url { get; set; }
        }
    }
}

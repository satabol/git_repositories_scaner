using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace git_repositories_scanner
{
    public class Settings
    {
        public IList<string> Paths { get; set; }
        string Version { get; set; }
    }
}

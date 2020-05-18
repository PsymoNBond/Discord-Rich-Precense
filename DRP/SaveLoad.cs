using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRP
{
    public class UserData
    {
        public string Details { get; set; }
        public string State { get; set; }
        public string LargeImageKey { get; set; }
        public string SmallImageKey { get; set; }
        public string LargeImageText { get; set; }
        public string SmallImageText { get; set; }
        public string ClientID { get; set; }
        public string EndTime { get; set; }
        public bool isEnd { get; set; }
        public bool isLoadOnStartCB { get; set; }
    }
}

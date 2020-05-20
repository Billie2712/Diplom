using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Model
{
    public class KddFeatures
    {
        public int duration;
        public string protocol;
        public string service;
        public int src_bytes;
        public int dst_bytes;
        public string flag;
        public int land;
        public int wrong_fragment;
        public int urgent;
        public int hot;
        public int num_failed_logins;
        public int logged_in;
    }
}

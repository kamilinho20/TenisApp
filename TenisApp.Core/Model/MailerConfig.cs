using System;
using System.Collections.Generic;
using System.Text;

namespace TenisApp.Core.Model
{
    public class MailerConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}

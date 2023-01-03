using System;

namespace matcrm.data.Models.Dto
{
    public class RecapchaDto
    {
        public string response { get; set; }

        public string secret { get; set; }

        public string remoteip { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }
        public bool success { get; set; }
        public bool credit { get; set; }
        public float scrore { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MarkAsReadUnReadResponse
    {
        public int count { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}

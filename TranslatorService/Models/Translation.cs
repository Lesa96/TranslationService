using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranslatorService.Models
{
    public class Translation
    {
        public string Language { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
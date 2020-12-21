using System;
using System.Collections.Generic;

namespace App.Models
{
    public class SubmissionDBO
    {
        public int ID { get; set;}
        public string User { get; set; }
        public Uri ImageUri { get; set; }
        public Uri Job { get; set; } = default;
    }
}
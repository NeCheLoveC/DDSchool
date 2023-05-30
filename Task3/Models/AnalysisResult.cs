using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task3.Models
{
    public class AnalysisResult
    {
        public IDictionary<String, int> words;

        public AnalysisResult(IDictionary<String, int> words)
        {
            this.words = words;
        }
    }
}
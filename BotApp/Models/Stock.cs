using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotApp.Models
{

    public class StockLUIS
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }
    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }

        public string startIndex { get; set; }

        public string endIndex { get; set; }

        public string score { get; set; }
    }


}
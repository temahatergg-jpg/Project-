using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string TitleRu { get; set; }
        public string ShortStatement { get; set; }
        public string ShortIdea { get; set; }
        public string PolygonUrl { get; set; }
        public bool CodeforcesPrepared { get; set; }
        public bool YandexPrepared { get; set; }
        public int Difficulty { get; set; }
        public string Note { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<int> ContestIds { get; set; } = new List<int>();
    }
}

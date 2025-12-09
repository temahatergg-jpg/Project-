namespace WindowsFormsApp1
{
    public class ContestItem
    {
        public int Id { get; set; }
        public string NameRu { get; set; }
        public short Year { get; set; }

        public override string ToString()
        {
            return NameRu + " (" + Year + ")";
        }
    }
}
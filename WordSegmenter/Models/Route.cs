namespace WordSegmenter.Models
{
    public class Route
    {
        public int To { get; set; }
        public double Weight { get; set; }
        public Route(int to, double weight)
        {
            Weight = weight;
            To = to;
        }
    }
}
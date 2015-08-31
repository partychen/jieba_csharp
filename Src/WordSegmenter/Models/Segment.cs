namespace WordSegmenter.Models
{
    public class Segment
    {
        public int To { get; set; }
        public double Weight { get; set; }
        public Segment(int to, double weight)
        {
            Weight = weight;
            To = to;
        }
    }

    public class Route
    {
        public Segment[] Segments { get; set; }
        public Route(Segment[] segments)
        {
            Segments = segments;
        }
    }
}
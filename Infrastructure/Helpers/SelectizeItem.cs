namespace Infrastructure.Helpers
{
    public class SelectizeItem<TValue>
    {
        public SelectizeItem(string t, TValue v)
        {
            text = t;
            value = v;
        }

        public SelectizeItem() { }
        public string text { get; set; }
        public TValue value { get; set; }
    }
}
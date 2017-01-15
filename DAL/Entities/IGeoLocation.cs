namespace DAL.Entities
{
    public interface IGeoLocation
    {
        string Name { get; set; }
        string Address { get; set; }
        decimal Long { get; set; }
        decimal Lat { get; set; }
    }
}
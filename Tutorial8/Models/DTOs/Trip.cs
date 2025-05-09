namespace Tutorial8.Models.DTOs
{
    public class Trip
    {
        public int TripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int MaxParticipants { get; set; }
        public int CountryId { get; set; }
        
    }
}
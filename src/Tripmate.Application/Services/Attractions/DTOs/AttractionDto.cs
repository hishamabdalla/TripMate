namespace Tripmate.Application.Services.Attractions.DTOs
{
    public class AttractionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; } = string.Empty;
        public string OpeningHours { get; set; }
        public string TicketPrice { get; set; }      
    }
}

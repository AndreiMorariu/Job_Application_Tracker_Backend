namespace Application_Tracker.Contracts.DTO
{
    public record ApplicationDTO
    {
        public Guid Id { get; set; }

        public string Company { get; set; }

        public string Position { get; set; }

        public string Location { get; set; }

        public string Link { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }
    }
}

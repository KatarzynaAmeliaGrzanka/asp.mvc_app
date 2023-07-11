namespace LibraryLab2.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string Author { get; set; }
        public int Date { get; set; }
        public string? User { get; set; }
        public string? Reserved { get; set; }
        public string? Leased { get; set; }

        public Book()
        {

        }
    }


}

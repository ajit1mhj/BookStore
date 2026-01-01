namespace BookStore.Models.DTOS
{
    public class StockDisplayModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public string? BookName { get; set; }
    }
}

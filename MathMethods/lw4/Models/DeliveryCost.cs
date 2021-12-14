namespace LW4.Models
{
    public class DeliveryCost
    {
        public int Value { get; set; }

        public int Price { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSelected { get; set; }

        public Needs Needs { get; set; }

        public Stocks Stocks { get; set; }

    }
}

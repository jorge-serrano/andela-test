namespace Campaign
{
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return $"Customer Name {Name} - City {City}";
        }
    }
}

// See https://aka.ms/new-console-template for more information
using Campaign;
using System.Linq;

Console.WriteLine("Hello, World!");
var events = new List<Event>{
new Event{ Name = "Phantom of the Opera", City = "New York"},
new Event{ Name = "Metallica", City = "Los Angeles"},
new Event{ Name = "Metallica", City = "New York"},
new Event{ Name = "Metallica", City = "Boston"},
new Event{ Name = "LadyGaGa", City = "New York"},
new Event{ Name = "LadyGaGa", City = "Boston"},
new Event{ Name = "LadyGaGa", City = "Chicago"},
new Event{ Name = "LadyGaGa", City = "San Francisco"},
};
var customers = new List<Customer>{
new Customer{ Name = "Nathan", City = "New York"},
new Customer{ Name = "Bob", City = "Boston"},
new Customer{ Name = "Cindy", City = "Chicago"},
new Customer{ Name = "Lisa", City = "Los Angeles"}
};
Dictionary<string, int> _distances = new Dictionary<string, int>();
//SendAllEventsToClientOrderedBy(events, customers,e=>e.City);
SendNearestEvents(events, customers);

void SendNearestEvents(List<Event> events, List<Customer> customers)
{
    var crossJoinQuery =
    from e in events
    from c in customers
    select new
    {
        CustomerName= c.Name,
        c.City,
        Event = e,
        Distance = GetDistance(c.City,e.City)
    };
    var orderedResults = crossJoinQuery.OrderBy(r => r.CustomerName).ThenBy(nr => nr.Distance);
    foreach (var item in customers)
    {
        var top5Events = orderedResults.Where(r => r.CustomerName == item.Name).Take(5);
        foreach (var ev in top5Events)
        {
            AddToEmail(item, ev.Event);
        }
    }

}

void SendAllEventsToClient(List<Event> events, List<Customer> customers)
{
    var data = JoinData(events.OrderBy(e=>e.City).ToArray(),customers);
    foreach (var item in data)
    {
        AddToEmail(item.Customer, item.Event);
    }
 }
void SendAllEventsToClientOrderedBy(List<Event> events, List<Customer> customers, Func<Event, object> orderClause)
{
    var data = JoinData(events.OrderBy(orderClause).Select(e => e).ToArray(), customers);
    foreach (var item in data)
    {
        AddToEmail(item.Customer, item.Event);
    }
}
dynamic JoinData(IEnumerable<Event> events, IEnumerable<Customer> customers)
{
    return events.Join(customers, e => e.City, c => c.City, (e, c) => new { Customer = c, Event = e, Distance = GetDistance(c.City, e.City) });
}
void AddToEmail(Customer customer, Event eventToSend)
{
    Console.WriteLine($"Event {eventToSend} sent to customer {customer}");
}
int GetDistance(string fromCity, string toCity)
{
    if (fromCity == toCity)
        return 0;
    var distanceKey = $"{fromCity}-{toCity}";
    if (_distances.ContainsKey(distanceKey))
        return _distances[distanceKey];

    int retries = 10;
    while (retries-- > 0)
    {
        try
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var distance = rand.Next(1, 600);
            _distances.Add(distanceKey, distance);
            return distance;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    throw new InvalidOperationException();
   
}

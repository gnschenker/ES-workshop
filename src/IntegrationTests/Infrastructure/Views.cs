using System;

namespace IntegrationTests.Infrastructure
{
    public class Foo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public int Counter { get; set; }
    }

    public class Bar
    {
        public string Id { get; set; }
        public decimal AverageWeight { get; set; }
        public int Counter { get; set; }
    }

    public class Baz
    {
        public string Id { get; set; }
        public decimal AverageWeight { get; set; }
        public int Counter { get; set; }
    }

    public class Baz2
    {
        public string Id { get; set; }
        public double AverageWeight { get; set; }
        public int Counter { get; set; }
    }
}
using System;

namespace Projects.Contracts.Events
{
    public class SampleStarted
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Step1Executed
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class SampleApproved
    {
        public Guid Id { get; set; }
    }

    public class SampleCancelled
    {
        public Guid Id { get; set; }
    }
}
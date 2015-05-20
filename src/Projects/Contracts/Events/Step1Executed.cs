using System;

namespace Projects.Contracts.Events
{
    public class Step1Executed
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }
}
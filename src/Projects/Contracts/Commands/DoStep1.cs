using System;

namespace Projects.Contracts.Commands
{
    public class DoStep1
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }
}
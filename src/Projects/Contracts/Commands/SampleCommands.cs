using System;

namespace Projects.Contracts.Commands
{
    public class StartSample
    {
        public string Name { get; set; }
    }

    public class DoStep1
    {
        public Guid SampleId { get; set; }
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class ApproveSample
    {
        public Guid SampleId { get; set; }
    }

    public class CancelSample
    {
        public Guid SampleId { get; set; }
    }
}
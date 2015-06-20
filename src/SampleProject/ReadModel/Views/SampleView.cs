using System;
using SampleProject.Domain.Samples;

namespace SampleProject.ReadModel.Views
{
    public class SampleView
    {
        public Guid Id { get; set; }
        public SampleStatus Status { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
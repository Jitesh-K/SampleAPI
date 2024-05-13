using System;
using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Entities
{
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid().ToString(); // Assign a new GUID as the Id when the order is created
        }

        [Key]
        public string? Id { get; private set; }

        [StringLength(100, ErrorMessage = "Description must be at most 100 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
        public string? Name { get; set; }

        public DateTime EntryDate { get; set; }
        public bool Invoiced { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}

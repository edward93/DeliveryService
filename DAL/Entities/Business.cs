using System;

namespace DAL.Entities
{
    /// <summary>
    /// This is a mock version.
    /// </summary>
    public class Business : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
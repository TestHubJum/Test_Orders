namespace Domain.Entities
{
    public class BaseEntity
    {
        public long Id { get; set; }    
        public DateTime CreatAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}

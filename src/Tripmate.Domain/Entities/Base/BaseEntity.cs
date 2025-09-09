namespace Tripmate.Domain.Entities.Base
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
    }
}

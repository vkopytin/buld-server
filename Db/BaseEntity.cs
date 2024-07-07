using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

namespace Repository;

public abstract class BaseEntity<T> : IBaseEntity<T>
{
    [Key]
    [BsonId]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual T Id { get; set; }

    object IBaseEntity.Id
    {
        get { return this.Id; }
        set { this.Id = (T)value; }
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


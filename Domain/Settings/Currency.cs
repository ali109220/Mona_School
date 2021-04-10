using Core.SharedDomain.IndexEntity;

namespace Domain.Settings
{
    public class Currency : IndexEntity
    {
        public virtual string Symbol { get; set; }
        public virtual bool IsActive { get; set; }
    }
}

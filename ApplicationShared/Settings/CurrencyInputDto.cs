using ApplicationShared.Constants;

namespace ApplicationShared.Settings
{
    public class CurrencyInputDto : IndexDto
    {
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }
}

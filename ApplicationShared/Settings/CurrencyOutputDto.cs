using ApplicationShared.Constants;
using System.Collections.Generic;

namespace ApplicationShared.Settings
{
    public class CurrencyDto : OutputIndexDto
    {
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }
    public class CurrencyOutputDto
    {
        public IEnumerable<CurrencyDto> Currencies { get; set; }
        public int AllCount { get; set; }
    }
}

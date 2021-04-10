using Microsoft.Extensions.Localization;
using TenisApp.Core.Interface;

namespace TenisApp.Localization
{
    public class SharedResource : ILocalizer
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SharedResource(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
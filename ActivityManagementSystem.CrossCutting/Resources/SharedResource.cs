using Microsoft.Extensions.Localization;
using System;
namespace ActivityManagementSystem.CrossCutting.Resources
{
    public class SharedResource : ISharedResource
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        public SharedResource(IStringLocalizer<SharedResource> localizer) =>
            _localizer = localizer;


        public string ErrorCommon => GetString(nameof(ErrorCommon));

        public string ErrorDbConnect => GetString(nameof(ErrorDbConnect));

        public string ErrorInMethod => GetString(nameof(ErrorInMethod));

        public string InfoMethodParams => GetString(nameof(InfoMethodParams));

        public string InfoProcessEnd => GetString(nameof(InfoProcessEnd));

        public string InfoProcessStart => GetString(nameof(InfoProcessStart));

        public string InfoParamsReceived => GetString(nameof(InfoParamsReceived));
        public string InfoResultReceived => GetString(nameof(InfoResultReceived));

        public string InfoProcessUpdate => GetString(nameof(InfoProcessUpdate));
        private string GetString(string name) =>
            _localizer[name].Value;
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Correspondence.Api.Utils
{
    public static class QueryStringParams
    {
        public static int? GetIntValue(this IQueryCollection query, string paramName)
        {
            StringValues queryValue;
            if (query.TryGetValue(paramName, out queryValue))
            {
                int value;
                if (!string.IsNullOrEmpty(queryValue) && int.TryParse(queryValue, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }
}

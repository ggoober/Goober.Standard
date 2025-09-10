using System.Collections.Generic;

namespace Goober.Http.Models
{
    public class HttpResponseContextModel<TResponse>
    {
        public TResponse Response { get; set; }
        public List<KeyValuePair<string, string>> HeaderValues { get; set; }
    }
}

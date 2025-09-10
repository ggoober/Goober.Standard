using System.Collections.Generic;

namespace Goober.Http.Models.Parameters
{
    public class IndusoftHttpParameters
    {
        public List<IndusoftAuthorizationEndPointParameters> AuthorizationEndPoints { get; set; } = new List<IndusoftAuthorizationEndPointParameters>();
    }
}

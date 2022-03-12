using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.API.TigerGraph
{
    internal sealed class TigerGraphAPIClient : APIClient
    {
        private static readonly Lazy<TigerGraphAPIClient> lazy
            = new Lazy<TigerGraphAPIClient>(() => new TigerGraphAPIClient());
        public static TigerGraphAPIClient Instance => lazy.Value;
        private TigerGraphAPIClient() { }
    }
}

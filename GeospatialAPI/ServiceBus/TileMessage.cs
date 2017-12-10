using System;
using System.Collections.Generic;
using System.Text;

namespace GeospatialAPI.ServiceBus
{
    public class TileMessage
    {
        public string ClientName { get; set; }
        public string SessionId { get; set; }
        public int LatencyTime { get; set; }

        public string Result { get; set; }
        public DateTime ResultsReceived { get; set; }
    }
}

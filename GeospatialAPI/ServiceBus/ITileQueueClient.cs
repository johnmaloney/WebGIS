using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeospatialAPI.ServiceBus
{
    public interface ITileQueueClient
    {
        Task SendMessages(TileMessage tile);
    }
}

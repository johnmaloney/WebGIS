using GeospatialAPI.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeospatialAPI.Controllers
{
    [Route("[controller]")]
    public class TileController : Controller
    {
        #region Fields

        private readonly ITileQueueClient queueClient;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public TileController(ITileQueueClient client)
        {
            this.queueClient = client;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok(new TileMessage { SessionId = new Random().Next().ToString() });
            ///  This is the session Id that the tile client will pass up //
            //return this.Ok(Guid.NewGuid());
        }

        [HttpPost]
        public virtual IActionResult MakeTile([FromBody]TileMessage tile)
        {
            this.queueClient.SendMessages(tile);
            return this.Created("TileRequestCreated", $"Request for Tile sent for: {tile.ClientName}");
        }

        #endregion
    }
}

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
            return this.Ok();
        }

        [HttpPost]
        public virtual IActionResult MakeTile([FromBody]GeoMessage tile)
        {
            this.queueClient.SendMessages(tile);
            return this.Created("TileRequestCreated", tile);
        }

        #endregion
    }
}

using GeospatialAPI.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeospatialAPI.Controllers
{
    [Route("[controller]")]
    public class QueueController : Controller
    {
        #region Fields

        private readonly IQueueObserverClient observerClient;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public QueueController(IQueueObserverClient observerClient)
        {
            this.observerClient = observerClient;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var id = new Random().Next().ToString();
            return this.Ok(new GeoMessage { ClientName = $"Client #{id}", SessionId = id, LatencyTime = 30000 });
        }

        [HttpGet("{sessionId}")]
        public IActionResult Get(string sessionId)
        {
            // There are two cases for this, one where the QueueObserverClient //
            // is NULL, meaning this is a responder //  
            if (observerClient == null)
                return this.NotFound();
            else
            {
                var currentQueue = observerClient.Results.Where(t => t.SessionId == sessionId);
                return this.Ok(currentQueue.OrderByDescending(t=> t.ResultsReceived));
            }
        }

        [HttpPut]
        public IActionResult ClearSession(GeoMessage tile)
        {
            // There are two cases for this, one where the QueueObserverClient //
            // is NULL, meaning this is a responder //  
            if (observerClient == null)
                return this.NotFound();
            else
            {
                var currentQueue = observerClient.Results.RemoveAll(t => t.SessionId == tile.SessionId);
                return this.Accepted();
            }
        }

        #endregion
    }
}

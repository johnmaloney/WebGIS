using GeospatialAPI.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeospatialAPI.Controllers
{
    [Route("[controller]")]
    public class AnalysisController : Controller
    { 
        #region Fields

        private readonly IAnalysisQueueClient queueClient;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public AnalysisController(IAnalysisQueueClient client)
        {
            this.queueClient = client;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok();
        }

        [HttpPost]
        public virtual IActionResult StartAnalysis([FromBody]GeoMessage message)
        {
            this.queueClient.SendMessages(message);
            return this.Created("Analysis Started", message);
        }

        #endregion
    }
}

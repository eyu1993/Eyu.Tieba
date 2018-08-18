using Eyu.Tieba.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eyu.Tieba.WebAPI.Controllers
{
    [Authorize]
    public class BlackController : ApiController
    {
        public IUnitOfWork UOW { get; set; }
        public BlackController(IUnitOfWork unitOfWork)
        {
            this.UOW = unitOfWork;
        }
        public async Task<IHttpActionResult> Get()
        {
            //var z = UOW.BlackListRepository.GetAll();
            return Ok();
        }
    }
}

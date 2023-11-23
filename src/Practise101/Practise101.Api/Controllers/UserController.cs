using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Practise101.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
    }
}

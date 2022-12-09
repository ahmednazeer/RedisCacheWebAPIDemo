using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisCacheWebAPIDemo.Core.contracts;
using RedisCacheWebAPIDemo.Data;
using RedisCacheWebAPIDemo.Data.Entities;

namespace RedisCache.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DriversController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly ICacheReaderService _cacheReaderService;

    public DriversController(AppDBContext context,ICacheReaderService cacheReaderService)
    {
        this._context = context;
        this._cacheReaderService = cacheReaderService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<Driver>> Get(){
        var drivers = _cacheReaderService.ReadCache<IEnumerable<Driver>>("drivers");
        if(drivers is  null || drivers.Count() == 0)
        {
            drivers = _context.Drivers.ToList();
            //update redis
            var res=_cacheReaderService.SetCacheItem("drivers", drivers, DateTimeOffset.Now.AddMinutes(5));

        }
        return Ok(drivers);
    }
    [HttpPost]
    public ActionResult<Driver> Post(Driver driver)
    {
        _context.Drivers.Add(driver);
        _context.SaveChanges();

        var drivers = _cacheReaderService.ReadCache<IEnumerable<Driver>>("drivers")?.ToList();
        if (drivers is null)
        {
            drivers = new List<Driver>() { driver };
            //update redis
            _cacheReaderService.SetCacheItem("drivers", drivers, DateTimeOffset.Now.AddMinutes(5));

        }
        drivers.Add(driver);
        var res = _cacheReaderService.SetCacheItem("drivers", drivers, DateTimeOffset.Now.AddSeconds(60));

        return Created(string.Empty,driver);
    }

    [HttpDelete("{id}")]
    public ActionResult<Driver> Delete(int id)
    {
        //var deleteResult=_context.Drivers.Remove()
        var deleteResult =(bool) _cacheReaderService.RemoveCacheItem("drivers");
        
        return deleteResult ? Ok("Deleted Succesfully"):BadRequest();
    }

}

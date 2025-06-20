using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace VierGewinntApi.Controllers;
using VierGewinntApi;

[ApiController]
[EnableCors("AllowSpecificOrigin")]
[Route("[controller]")]

public class VierGewinntController : ControllerBase
{
    private readonly ILogger<VierGewinntController> _logger;
    private readonly BackEndService _backEndService;

    private readonly CounterService _zähler;
    

    public VierGewinntController(ILogger<VierGewinntController> logger, BackEndService BackEndService, CounterService CounterService)
    {
        _logger = logger;
        _backEndService = BackEndService;
        _zähler = CounterService;
    }
    
    [HttpGet,Route("GetCounter")]
    public int GetCounter(){
         _zähler.Increase();
         _zähler.Increase();
         _zähler.Increase();
         return _zähler.Count;
    }
    [HttpGet, Route("GetGame")]
    public ActionResult<VierGewinnt> GetGame()
    {
        return _backEndService.GetGame();
    }

    [HttpPost, Route("MakeGameReady")]
    public ActionResult<VierGewinnt> MakeGameReady( [FromBody]int gamemode)
    {
        return _backEndService.MakeGameReady(gamemode);
    }
    [HttpPost, Route("HandleClick")]
    public ActionResult<VierGewinnt> HandleClick( [FromBody] ZugDatenDto zug)
    {
        return _backEndService.HandleClick(zug.Spalte, zug.Zeile);
    }
    
}
public class ZugDatenDto{
    public int Spalte { get; set; }
    public int Zeile { get; set; }
}
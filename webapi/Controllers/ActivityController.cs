using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivityController : ControllerBase
{
    private TestContext _db;
    private LeaderboardService _leaderboardService;
    private IMemoryCache _cache;

    public ActivityController(TestContext db, LeaderboardService leaderboardService, IMemoryCache cache)
    {
        _db = db;
        _leaderboardService = leaderboardService;
        _cache = cache;
    }

    [HttpGet]
    [Route("")]
    public ActionResult<List<PlayerActivity>> Get()
    {
        return Ok(_db.PlayerActivities.ToList());
    }

    [HttpGet]
    [Route("{uuid}")]
    public ActionResult<List<PlayerActivity>> GetByPlayerId(string uuid)
    {
        if (string.IsNullOrEmpty(uuid))
        {
            return Ok(_db.PlayerActivities.ToList());
        }
        var result = _db.PlayerActivities.Where(p => p.PlayerId.Contains(uuid)).ToList();
        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("GetScore")]
    public ActionResult<List<PlayerActivity>> GetScoreByDate(string playerId, DateTime? fromDate, DateTime? toDate)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return BadRequest("No valid PlayerUUID in Request.");
        }
        if (fromDate == null)
        {
            fromDate = DateTime.MinValue;
        }
        if (toDate == null)
        {
            toDate = DateTime.MaxValue;
        }

        int result = _leaderboardService.GetPlayerScore(playerId, fromDate.Value, toDate.Value);

        return Ok(result);
    }



    [HttpPost]
    [Route("AddActivity")]
    public ActionResult<PlayerActivity> AddActivity([FromBody] PlayerActivity activity)
    {
        if (!ValidateActivity(activity, out string error))
        {
            return BadRequest(error);
        }

        activity.Date = DateTime.UtcNow;

        _db.PlayerActivities.Add(activity);
        _db.SaveChanges();

        //invalidated cache
        _cache.Remove("LeaderBoardCache");

        return Ok(activity);
    }

    private bool ValidateActivity(PlayerActivity activity, out string error)
    {
        error = "";

        if (!Guid.TryParse(activity.PlayerId, out _))
        {
            error = "Player ID was not a valid GUID.";
            return false;
        }

        if (activity.LogType == Enums.ActivityLogType.EARNED_POINTS)
        {
            if (!int.TryParse(activity.Data, out int i))
            {
                error = "Score was not a valid Number.";
                return false;
            }
        }
        if (activity.LogType == Enums.ActivityLogType.RECEIVED_REWARD || activity.LogType == Enums.ActivityLogType.STARTED_CRAFTING)
        {
            if (activity.Data.Length != Guid.NewGuid().ToString().Length)
            {
                error = "ItemId was not a valid ID";
                return false;
            }
        }
        return true;
    }

}

using webapi.Models;
using webapi.Enums;

public interface ILeaderboardService
{
    int GetPlayerScore(string playerId, DateTime fromDate, DateTime toDate);
    List<PlayerDetails> GetPlayerDetailsWithScore();
}

public class LeaderboardService : ILeaderboardService
{
    private readonly TestContext _db;

    public LeaderboardService(TestContext db)
    {
        _db = db;
    }

    public int GetPlayerScore(string playerId, DateTime fromDate, DateTime toDate)
    {
        var scoreActivitiesInDateRange = _db.PlayerActivities.Where(p => p.LogType == ActivityLogType.EARNED_POINTS &&
                                                                 p.Date >= fromDate &&
                                                                 p.Date <= toDate &&
                                                                 p.PlayerId == playerId);

        int result = SumScore(scoreActivitiesInDateRange.ToList());
        return result;
    }

    public List<PlayerDetails> GetPlayerDetailsWithScore()
    {
        var activities = _db.PlayerActivities.Where(a => a.LogType == ActivityLogType.EARNED_POINTS)
                                             .Select(a => new { a.PlayerId, a.Data })
                                             .ToList();

        var scores = activities.GroupBy(a => a.PlayerId)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Sum(a => int.TryParse(a.Data, out var val) ? val : 0)
                                );

        var players = _db.PlayerDetails.ToList();

        var result = players
            .Select(p => new PlayerDetails(p.Name, p.Uuid, scores.ContainsKey(p.Uuid) ? scores[p.Uuid] : 0))
            .OrderByDescending(p => p.Score)
            .ToList();

        return result;
    }

    private int SumScore(List<PlayerActivity> activities)
    {
        int scoreInRange = 0;
        foreach (var activity in activities)
        {
            if (int.TryParse(activity.Data, out int score)) { scoreInRange += score; };
        }
        return scoreInRange;
    }


}
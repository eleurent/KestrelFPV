using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RaceScore : MonoBehaviour
{
    public const string PlayerTimeProp = "time";
	public const string PlayerLapProp = "lap";
}


static class ScoreExtensions
{
    public static void SetTime(this PhotonPlayer player, float time)
    {
		Hashtable prop = new Hashtable() {{RaceScore.PlayerTimeProp, time}};
		player.SetCustomProperties(prop);
    }
	
    public static float GetTime(this PhotonPlayer player)
    {
        object time;
		if (player.customProperties.TryGetValue(RaceScore.PlayerTimeProp, out time))
        {
			return (float)time;
        }
        return 0;
    }

	public static void SetLap(this PhotonPlayer player, int lap)
	{
		Hashtable prop = new Hashtable() {{RaceScore.PlayerLapProp, lap}};
		player.SetCustomProperties(prop);
	}
	
	public static int GetLap(this PhotonPlayer player)
	{
		object lap;
		if (player.customProperties.TryGetValue(RaceScore.PlayerLapProp, out lap))
		{
			return (int)lap;
		}
		return 0;
	}
}
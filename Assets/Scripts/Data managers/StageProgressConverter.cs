using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class StageProgressConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(GameProgress.StageProgress);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        string type = jo["type"]?.ToString();

        GameProgress.StageProgress result = type switch
        {
            "Asteroid" => new GameProgress.AsteroidStageProgress(),
            "Cable" => new GameProgress.CableConnectionStageProgress(),
            "Equipment" => new GameProgress.EquipmentRecoveryStageProgress(),
            _ => new GameProgress.StageProgress()
        };

        serializer.Populate(jo.CreateReader(), result);
        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JObject jo = new JObject();

        if (value is GameProgress.AsteroidStageProgress asteroid)
        {
            jo["type"] = "Asteroid";
            jo["isCompleted"] = asteroid.isCompleted;
            jo["score"] = asteroid.score;
            jo["timeTaken"] = asteroid.timeTaken;
            jo["mistakes"] = asteroid.mistakes;
            jo["incorrectAsteroids"] = asteroid.incorrectAsteroids;
            jo["bonusAsteroids"] = asteroid.bonusAsteroids;
            jo["selectedTime"] = asteroid.selectedTime;
        }
        else if (value is GameProgress.CableConnectionStageProgress cable)
        {
            jo["type"] = "Cable";
            jo["isCompleted"] = cable.isCompleted;
            jo["score"] = cable.score;
            jo["timeTaken"] = cable.timeTaken;
            jo["mistakes"] = cable.mistakes;
            jo["selectedTime"] = cable.selectedTime;
        }
        else if (value is GameProgress.EquipmentRecoveryStageProgress equip)
        {
            jo["type"] = "Equipment";
            jo["isCompleted"] = equip.isCompleted;
            jo["score"] = equip.score;
            jo["timeTaken"] = equip.timeTaken;
            jo["mistakes"] = equip.mistakes;
            jo["selectedTime"] = equip.selectedTime;
        }

        else if (value is GameProgress.StageProgress baseStage)
        {
            jo["type"] = "Stage";
            jo["isCompleted"] = baseStage.isCompleted;
            jo["score"] = baseStage.score;
            jo["timeTaken"] = baseStage.timeTaken;
            jo["mistakes"] = baseStage.mistakes;
            jo["selectedTime"] = baseStage.selectedTime;
        }

        jo.WriteTo(writer);
    }

}

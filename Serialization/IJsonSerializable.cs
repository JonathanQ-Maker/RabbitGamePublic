using Newtonsoft.Json.Linq;

public interface IJsonSerializable
{
    public void Deserialize(JObject data);

    public void Serialize(JObject data);
}
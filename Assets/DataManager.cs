

public class DataManager
{
    private static int[,,] worldData;
    private static string code;

    public static int[,,] GetWorldData()
    {
        return worldData;
    }

    public static void SetWorldData(int[,,] data)
    {
        worldData = data;
    }

    public static string GetCode()
    {
        return code;
    }

    public static void SetCode(string data)
    {
        code = data;
    }
}

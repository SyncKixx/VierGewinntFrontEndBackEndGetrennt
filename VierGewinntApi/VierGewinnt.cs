using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
//Datenstruktur c# man braucht dies aber in beiden also sowohl c# als auch typescript
namespace VierGewinntApi;

public class VierGewinnt
{
    public int gamemode { get; set; }

    public int currentSpieler { get; set; }

    public int winner  {get; set;}

    public string[][]? spielfeld { get; set; }
    //zeilen und spalten zu einem array mit der größe machen
    public int[]? zeilen { get; set; }
    public int[]? spalten { get; set; }
}
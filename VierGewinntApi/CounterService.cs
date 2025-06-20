using System;
using System.ComponentModel.Design.Serialization;

namespace VierGewinntApi;

public class CounterService
{
    public int Count { get; set; }


    public int Increase(){
        return Count += 1;
    }
}

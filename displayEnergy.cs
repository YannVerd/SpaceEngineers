const string decimalFormat = "#0.00";
const double maxInputTurbine = 371.59;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    //This makes the program run automatically every 10 ticks.
}

public void Main() 
{
    
    IMyTextPanel panel = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
    panel.WriteText("");
    panel.ContentType = ContentType.SCRIPT;
    List<IMyBatteryBlock> batteriesList = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlockGroupWithName("Batteries").GetBlocksOfType<IMyBatteryBlock>(batteriesList);
    List<IMyWindTurbine> windTurbinesList = new List<IMyWindTurbine>();
    GridTerminalSystem.GetBlockGroupWithName("WindTurbines").GetBlocksOfType<IMyWindTurbine>(windTurbinesList);

    var frame = panel.DrawFrame(); // frame activation for text customization

    // create lines
    var lines = new []
    {
        new { Data = "Energy Informations \n\n", Position = new Vector2(100, 20), Scale = 1.5f, FontColor = Color.Yellow },
        new { Data = BatteriesTotalInfos(batteriesList), Position = new Vector2(10, 70), Scale = 1.0f, FontColor = Color.White},
        new { Data = WindTurbinesTotalInfos(windTurbinesList), Position = new Vector2(10, 200), Scale = 1.0f, FontColor = Color.White }
    };

    // display lines on panelq
    foreach(var line in lines)
    {
        var textSprite = new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = line.Data,
            Position = line.Position,
            Color = line.FontColor,
            FontId = "White",
            RotationOrScale = line.Scale,
            Alignment = TextAlignment.LEFT
        };

        frame.Add(textSprite);
    };

    frame.Dispose();

}

string BatteryInfosToText(float currentInput, float maxInput, float currentStoredPower, float maxStoredPower, float currentOutput)
{   
    float time = 0;
    float flux = currentInput - currentOutput;
    if(flux >= 0){
        time = (maxStoredPower -currentStoredPower)/(currentInput - currentOutput);
    }else {
        time = currentStoredPower/currentOutput;
    }
     
    string textInfos = "";
    textInfos += "Batteries flux: "+ flux.ToString(decimalFormat)+" / "+ maxInput.ToString() + " MW\n";
    textInfos += "Stored Power: "+ currentStoredPower.ToString(decimalFormat) + " / " + maxStoredPower.ToString() + " MWh ( "+(currentStoredPower/maxStoredPower*100).ToString(decimalFormat) +" % "+")\n";
    if(flux > 0){
        textInfos += "recharging batteries \n";
        textInfos += "Recharging time : "+ TranslateTimestoHMinFormat(time) + "\n\n";
    }else {
        textInfos += "Batteries discharge \n";
        textInfos += "Discharging time : "+ TranslateTimestoHMinFormat(time) +"\n\n";
    }
    
    return textInfos;

}

string BatteriesTotalInfos(List<IMyBatteryBlock> list)
{
    string textTotal = "";
    float maxStoredPower = 0;
    float currentStoredPower = 0;
    float currentInput = 0;
    float maxInput = 0;
    float currentOutput = 0;

    for (int i = 0; i < list.Count; i++)
    {
        maxStoredPower += list[i].MaxStoredPower;
        currentStoredPower += list[i].CurrentStoredPower;
        currentInput += list[i].CurrentInput;
        maxInput += list[i].MaxInput;
        currentOutput += list[i].CurrentOutput;
    }

    textTotal += BatteryInfosToText(currentInput, maxInput, currentStoredPower, maxStoredPower, currentOutput);
    return textTotal;
}

string WindTurbinesTotalInfos(List<IMyWindTurbine> list)
{
    string textTotal = "";
    float currentOutput = 0;
    double maxOutput = (maxInputTurbine * list.Count)/1000; // MW

     for (int i = 0; i < list.Count; i++)
    {  
        currentOutput += list[i].CurrentOutput;      
    }

    textTotal += "Wind Turbines : "+ currentOutput.ToString(decimalFormat) + " / " + maxOutput.ToString(decimalFormat) +" MW\n";
  
    return textTotal;

}

string TranslateTimestoHMinFormat(float time)
{
    if(time%1 != 0){
        return time.ToString("#0") + " Hours " + (time%1 * 60).ToString("00") + " Min";
    }else {
        return time.ToString("#0") + " Hours";
    }
}
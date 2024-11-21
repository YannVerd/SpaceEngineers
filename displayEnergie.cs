public void Program()
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
        // for y position : +50 is ok
        new { Data = "Energy Informations \n\n", Position = new Vector2(100, 20), Scale = 1.5f, FontColor = Color.Yellow },
        new { Data = BatteriesTotalInfos(batteriesList), Position = new Vector2(50, 70), Scale = 1.0f, FontColor = Color.White},
        new { Data = WindTurbinesTotalInfos(windTurbinesList), Position = new Vector2(50, 120), Scale = 1.0f, FontColor = Color.White }
    };

    // display lines on panel
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
    float rechargeTime = 0;
    if(currentInput - currentOutput >= 0){
        rechargeTime= (maxStoredPower -currentStoredPower)/(currentInput - currentOutput);
    }
     
    string textInfos = "";
    textInfos += "Input: "+ currentInput.ToString()+ " / "+ maxInput.ToString() +" MW\n";
    textInfos += "Stored: "+ currentStoredPower.ToString() + " / " + maxStoredPower.ToString() + " MWh ( "+(currentStoredPower/maxStoredPower*100).ToString() +" % "+")\n";
    if(rechargeTime > 0){
        textInfos += "Recharging time : "+ rechargeTime.ToString() + " hours\n";
    }else {
        textInfos += "Recharging time : discharges\n\n";
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
    float efficiency = 0;
    float currentOutput = 0;
    float ratio = 0;

     for (int i = 0; i < list.Count; i++)
    {
        efficiency += list[i].Effectivity;
        currentOutput += list[i].CurrentOutput;
        ratio += list[i].CurrentOutputRatio;
    }

    textTotal += "Production Efficiency : " + efficiency/list.Count + " %\n";
    textTotal += "Production CurrentOutput : "+ currentOutput + " MW\n";
    textTotal += "Production Ratio : "+ ratio + "\n\n";
    return textTotal;

}
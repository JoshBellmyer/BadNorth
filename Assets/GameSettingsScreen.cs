using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsScreen : UIScreen
{
    public void OnBack()
    {
        manager.SetUIScreen("Title Screen");
    }
}

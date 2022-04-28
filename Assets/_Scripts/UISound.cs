using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour, ISelectHandler, ISubmitHandler {

    public Sound selectSound = Sound.MenuMove;
    public Sound submitSound = Sound.MenuClick;


    public void OnSelect (BaseEventData eventData) {
        SoundPlayer.PlaySoundLocal(selectSound, 1, false);
    }

    public void OnSubmit (BaseEventData eventData) {
        SoundPlayer.PlaySoundLocal(submitSound, 0.9f, false);
    }
}
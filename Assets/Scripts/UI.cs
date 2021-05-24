using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public int octave;
    public Waves waves;
    public Text inputX;
    public Text inputY;
    public Text inputWavelength;
    public Slider sliderSteepness;

    public void UpdateSteepness(System.Single steepness)
    {
        waves.UpdateSteepness(steepness, octave);
    }

    public void UpdateWavelength()
    {
        waves.UpdateWavelength(float.Parse(inputWavelength.text), octave);
    }

    public void UpdateVectorX()
    {
        UpdateVector(float.Parse(inputX.text), float.Parse(inputY.text));
    }

    public void UpdateVectorY()
    {
        UpdateVector(float.Parse(inputX.text), float.Parse(inputY.text));
    }

    public void UpdateVector(float x, float y)
    {
        waves.UpdateVector(new Vector2(x, y), octave);
    }

    public void UpdateAllUIValues(Vector2 direction, float wavelength, float steepness)
    {
        inputX.GetComponentInParent<InputField>().text = direction.x.ToString();
        inputY.GetComponentInParent<InputField>().text = direction.y.ToString();
        sliderSteepness.value = steepness;
        inputWavelength.GetComponentInParent<InputField>().text = wavelength.ToString();
    }
}

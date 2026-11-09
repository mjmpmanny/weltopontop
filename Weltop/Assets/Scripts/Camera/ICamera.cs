using UnityEngine;
using UnityEngine.UIElements;

public interface ICamera
{
    public void SetSensitivity(float _sensitivity);
    public void SwitchTo();
    public void SwitchFrom();

    public void RotateTo(Quaternion rot);
}

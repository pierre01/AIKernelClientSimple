﻿using CommunityToolkit.Mvvm.ComponentModel;
using LightsAPICommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfHousePassiveClient.ViewModels;


public partial class LightViewModel : ObservableObject
{
    private readonly Light _light;
    private static readonly ColorConverter _colorConverter = new();
    public event EventHandler<bool> LightSwitched;

    public LightViewModel(Light light)
    {
        _light = light;

        var hexBright = (int)Math.Round(light.Brightness * 2.55);
        var hexOriginPoint = (int)Math.Round(hexBright * 0.75);
        //string hexBrightStr = $"#{hexBright:X2}{light.Color}";
        LightColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{hexBright:X2}{light.Color}"));
        OriginPointColor = (Color)ColorConverter.ConvertFromString($"#{hexOriginPoint:X2}{light.Color}");
        EndPointColor = (Color)ColorConverter.ConvertFromString($"#00{light.Color}");
        Brightness = light.Brightness;
        HexColor = light.Color;
        SwitchLightOnOrOff(light);

    }

    public void Update(Light light)
    {
        if (_light.LightId != light.LightId)
            return;
        var isOn = light.State == LightState.On;
        if (IsOn == isOn && HexColor == light.Color && Brightness == light.Brightness)
            return;

        bool colorModified = false;
        // If the color or brightness has changed, update the color
        if (HexColor != light.Color || Brightness != light.Brightness)
        {
            UpdateLightAndHalo(light);
            Brightness = light.Brightness;
            HexColor = light.Color;
            colorModified = true;
        }

        if (IsOn == isOn && !colorModified)
            return;
        IsOn = isOn;
        SwitchLightOnOrOff(light);
    }

    private void SwitchLightOnOrOff(Light light)
    {
        var isOn = light.State == LightState.On;
        if (isOn == false)
        {
            LightColor = new SolidColorBrush(Colors.Transparent);
            OriginPointColor = Colors.Transparent;
            OnPropertyChanged(nameof(LightColor));
            OnPropertyChanged(nameof(OriginPointColor));
            OnPropertyChanged(nameof(EndPointColor));
            LightSwitched?.Invoke(this, false);
        }
        else
        {
            UpdateLightAndHalo(light);
            OnPropertyChanged(nameof(LightColor));
            OnPropertyChanged(nameof(OriginPointColor));
            OnPropertyChanged(nameof(EndPointColor));
            LightSwitched?.Invoke(this, false);
        }
    }

    private void UpdateLightAndHalo(Light light)
    {
        // Make Brightness a number between 0 and 255
        var hexBright = (int)Math.Round(light.Brightness * 2.55);
        var hexOriginPoint = (int)Math.Round(hexBright * 0.75);
        LightColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{hexBright:X2}{light.Color}"));
        OriginPointColor = (Color)ColorConverter.ConvertFromString($"#{hexOriginPoint:X2}{light.Color}");
        EndPointColor = (Color)ColorConverter.ConvertFromString($"#00{light.Color}");
    }

    public int Id => _light.LightId;
    public string Name => _light.Name;
    public int RoomId => _light.RoomId;

    public Brush LightColor { get; set; }
    public Color OriginPointColor { get; set; }
    public Color EndPointColor { get; set; }

    public string HexColor
    {
        get => _light.Color;
        set
        {
            SetProperty(_light.Color, value, _light, (light, v) => light.Color = v);
            OnPropertyChanged(nameof(LightColor));
            OnPropertyChanged(nameof(OriginPointColor));
            OnPropertyChanged(nameof(EndPointColor));
        }
    }

    private bool _isOn;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            SetProperty(_isOn, value, _light, (light, v) =>
            {
                _isOn = v;
                light.State = v ? LightState.On : LightState.Off;
            });
        }
    }

    public int Brightness
    {
        get => _light.Brightness;
        set
        {
            SetProperty(_light.Brightness, value, _light, (light, v) => light.Brightness = v);
            OnPropertyChanged(nameof(LightColor));
            OnPropertyChanged(nameof(OriginPointColor));
            OnPropertyChanged(nameof(EndPointColor));
        }
    }
}

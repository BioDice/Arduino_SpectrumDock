using Aurora;
using Aurora.Devices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;

public class SpectrumDockDeviceScript
{
  public string devicename = "Spectrum Dock";
  public string SerialPort = "COM4";
  public int BaudRate = 9600;
  public bool enabled = true; // Switch to True, to enable it in Aurora
  public int TotalLeds = 9;

  private Color col_black = Color.Black;
  private Color col_white = Color.White;
  private Color col_green = Color.Green;
  private static SerialPort port;
  private static Logger logger;

  /*
   * DeviceKeys: Aurora class that contains metadata about which key it is and what color
   * int: The led number on the ledstrip
   */
  Dictionary<DeviceKeys, int> CheckedKeys;

  public bool Initialize()
  {
    try
    {
      logger = new Logger();
      port = new SerialPort(SerialPort, BaudRate, Parity.Even, 8, StopBits.One); // Check the COM port your device is connected to and change accordingly
      port.Open();
      InitCheckedKeys();
      PaintItWhatEver(col_green);
      return true;
    }
    catch (Exception ex)
    {
      logger.LogLine(ex.ToString());
      return false;
    }
  }

  public void InitCheckedKeys()
  {
    CheckedKeys = new Dictionary<DeviceKeys, int>();
    CheckedKeys.Add(DeviceKeys.LEFT_CONTROL, 0);
    CheckedKeys.Add(DeviceKeys.CAPS_LOCK, 3);
    CheckedKeys.Add(DeviceKeys.TILDE, 6);

    CheckedKeys.Add(DeviceKeys.RIGHT_CONTROL, 1);
    CheckedKeys.Add(DeviceKeys.ENTER, 4);
    CheckedKeys.Add(DeviceKeys.BACKSPACE, 7);

    CheckedKeys.Add(DeviceKeys.NUM_ENTER, 2);
    CheckedKeys.Add(DeviceKeys.NUM_PLUS, 5);
    CheckedKeys.Add(DeviceKeys.NUM_MINUS, 8);
  }


  public void Reset()
  {
    PaintItWhite(); // just for debugging
  }

  public void Shutdown()
  {
    PaintItBlack();
    port.Close();
  }

  public bool UpdateDevice(Dictionary<DeviceKeys, Color> keyColors, bool forced)
  {
    try
    {
      foreach (KeyValuePair<DeviceKeys, Color> key in keyColors)
      {
        //Iterate over each key and color and send them to your device
        if (CheckedKeys.ContainsKey(key.Key))
        {
          Color currentColor = key.Value;
          int ledNr = CheckedKeys[key.Key];
          SendColorToDevice(ledNr, currentColor);
        }
      }

      return true;
    }
    catch (Exception ex)
    {
      return false;
    }
  }

  private void PaintItBlack()
  {
    SetAllLeds(col_black);
  }

  private void PaintItWhite()
  {
    SetAllLeds(col_white);
  }

  private void PaintItWhatEver(Color color)
  {
    SetAllLeds(color);
  }

  private void SetAllLeds(Color color)
  {
    for (int i = 0; i < TotalLeds; i++)
    {
      SendColorToDevice(i, color);
    }
  }

  //Custom method to send the color to the device
  private void SendColorToDevice(int ledNr, Color color)
  {
    byte[] data = { (byte)ledNr, color.R, color.G, color.B };

    port.Write(data, 0, 4);
    logger.LogLine(String.Format("Sending colors: {0} | {1} {2} {3}", ledNr, color.R, color.G, color.B));
  }

}
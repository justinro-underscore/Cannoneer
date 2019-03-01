using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public enum InputType
    {
        // PC
        KEYBOARD,
        JOYSTICK,
        // Phone
        PHONE_TILT,
        PHONE_JOYSTICK,
    }
    public InputType inputType = InputType.PHONE_TILT;
    public Vector2 tiltCalibration = new Vector2();

    public Image phoneTilt_shootUpImg;
    public Image phoneTilt_shootDownImg;

    private bool running = false;
    private Vector2 move = new Vector2();
    private bool fire = false;
    private bool fireUp = false;

    public void SetRunning(bool run)
    {
        running = run;
        // Reset the UI
        phoneTilt_shootUpImg.color = new Color(1, 1, 1, 0);
        phoneTilt_shootDownImg.color = new Color(1, 1, 1, 0);
    }

    void FixedUpdate()
    {
        if (running)
        {
            SetMovement();
            SetFireCannonball();
            UpdateUI();
        }
    }

    private void SetMovement()
    {
        switch (inputType)
        {
            case InputType.KEYBOARD:
                move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                break;
            case InputType.PHONE_TILT:
                move = new Vector2(Input.acceleration.x, Input.acceleration.y) - tiltCalibration; // TODO May have to swap x and y
                move *= 2; // To get rid of sluggish feeling
                break;
            case InputType.JOYSTICK:
            case InputType.PHONE_JOYSTICK:
            default:
                move = new Vector2();
                break;
        }
    }

    /**
     * Gets the movement vector from the desired form of input
     * @return A 2d Vector containing x and y information
     */
    public Vector2 GetMovement()
    {
        return move;
    }

    private void SetFireCannonball()
    {
        switch (inputType)
        {
            case InputType.KEYBOARD:
                if (Input.GetKey("o"))
                {
                    fire = fireUp = true;
                }
                else if (Input.GetKey("l"))
                {
                    fire = true;
                    fireUp = false;
                }
                else
                {
                    fire = fireUp = false;
                }
                break;
            case InputType.PHONE_TILT:
                fire = Input.GetMouseButton(0);
                fireUp = Input.mousePosition.y > (Screen.height / 2);
                break;
            case InputType.JOYSTICK:
            case InputType.PHONE_JOYSTICK:
            default:
                fire = fireUp = false;
                break;
        }
    }

    /**
     * Gets information regarding if we should fire the cannons
     * @return A 2d Vector of the form:
     *    (bool: True if should fire, bool: True if we should fire up, False if fire down)
     */
    public Vector2Int GetFireCannonball()
    {
        return new Vector2Int(fire ? 1 : 0, fireUp ? 1 : 0);
    }

    private void UpdateUI()
    {
        switch (inputType)
        {
            case InputType.PHONE_TILT:
                if(fire)
                {
                    if (fireUp)
                    {
                        phoneTilt_shootUpImg.color = new Color(1, 1, 1, 1);
                        phoneTilt_shootDownImg.color = new Color(1, 1, 1, 0.25f);
                    }
                    else
                    {
                        phoneTilt_shootUpImg.color = new Color(1, 1, 1, 0.25f);
                        phoneTilt_shootDownImg.color = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    phoneTilt_shootUpImg.color = new Color(1, 1, 1, 0.25f);
                    phoneTilt_shootDownImg.color = new Color(1, 1, 1, 0.25f);
                }
                break;
            case InputType.PHONE_JOYSTICK:
                break;
            case InputType.KEYBOARD:
            case InputType.JOYSTICK:
            default:
                break;
        }
    }
}

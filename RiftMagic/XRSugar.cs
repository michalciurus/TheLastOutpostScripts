using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class XRSugar {

    static XRSugar() {

        BehaviorForwarder.OnUpdate.AddListener(XRSugar.OnUpdate);

    }

    private static bool wasRightPrimaryUpInLastFrame = true;

    public static bool wasRightPrimaryButtonPressedDown = false;

    public static bool wasRightSecondaryButtonUpInLastFrame = true;

    public static bool wasRightSecondaryButtonPressedDown = false;

    public static bool wasRightTriggerPressedDown = false;

    public static bool wasRightTriggerUpInLastFrame = true;

    static void OnUpdate() {
        
        wasRightPrimaryButtonPressedDown = XRSugar.RightPrimaryButton() && wasRightPrimaryUpInLastFrame;

        wasRightPrimaryUpInLastFrame = !XRSugar.RightPrimaryButton();

        wasRightSecondaryButtonPressedDown = XRSugar.RightSecondaryButton() && wasRightSecondaryButtonUpInLastFrame;

        wasRightSecondaryButtonUpInLastFrame = !XRSugar.RightSecondaryButton();
        

        if (XRSugar.Right().HasValue)
        {
            wasRightTriggerPressedDown = Trigger(Right().Value) >= 0.2f && wasRightTriggerUpInLastFrame;
            wasRightTriggerUpInLastFrame = XRSugar.Trigger(Right().Value) < 0.2f;
        }
        else
        {
            wasRightTriggerPressedDown = false;
            wasRightTriggerUpInLastFrame = true;
        }
        
        
    }

    public static InputDevice? Right() {
        var gc = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, gc);

        if (gc.Count == 0) {
            return null;
        }

        return gc[0];
    }

    public static InputDevice? Left() {
        var gc = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, gc);

        if (gc.Count == 0) {
            return null;
        }

        return gc[0];
    }

    public static InputDevice? CenterEye() {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.CenterEye, leftHandDevices);

        if (leftHandDevices.Count == 0) {
            return null;
        }

        return leftHandDevices[0];
    }


    public static Vector2 LeftAxis() {

        var left = XRSugar.Left();

        if (left == null) {
            return Vector2.zero;
        }

        Vector2 a;

        left.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out a);

        return a;
    }

    public static Vector3 Position(InputDevice device) {

        if (device == null) {
            return Vector3.zero;
        }

        Vector3 b;

        device.TryGetFeatureValue(CommonUsages.devicePosition, out b);

        return b;
    }

    public static Quaternion Rotation(InputDevice device) {

        if (device == null) {
            return Quaternion.identity;
        }

        Quaternion b;

        device.TryGetFeatureValue(CommonUsages.deviceRotation, out b);

        return b;
    }

    public static Vector2 RightAxis() {

        var right = XRSugar.Right();

        if (right == null) {
            return Vector2.zero;
        }

        Vector2 a;

        right.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out a);

        return a;
    }

    public static bool RightPrimaryButton() {
        var right = XRSugar.Right();

        if (right == null) {
            return false;
        }

        bool a;

        right.Value.TryGetFeatureValue(CommonUsages.primaryButton, out a);

        return a;
    }
    
    public static bool LeftPrimaryButton() {
        var left = XRSugar.Left();

        if (left == null) {
            return false;
        }

        bool a;

        left.Value.TryGetFeatureValue(CommonUsages.primaryButton, out a);

        return a;
    }

    public static bool RightSecondaryButton() {
        var right = XRSugar.Right();

        if (right == null) {
            return false;
        }

        bool a;

        right.Value.TryGetFeatureValue(CommonUsages.secondaryButton, out a);

        return a;
    }
    
    public static bool LeftMenuButton() {
        var right = XRSugar.Left();

        if (right == null) {
            return false;
        }

        bool a;

        right.Value.TryGetFeatureValue(CommonUsages.menuButton, out a);

        return a;
    }

    public static bool RightAxisRightOn() {

        var axis = XRSugar.RightAxis();

        return axis.x > 0;
    }

    public static bool RightAxisLeftOn() {

        var axis = XRSugar.RightAxis();

        return axis.x < 0;
    }

    public static float Trigger(InputDevice device) {

        if (device == null) {
            return 0;
        }

        float a = 0.0f;

        device.TryGetFeatureValue(CommonUsages.trigger, out a);


        return a;
    }

    public static float Grip(InputDevice device) {

        if (device == null) {
            return 0;
        }

        float a = 0.0f;

        device.TryGetFeatureValue(CommonUsages.grip, out a);

        return a;
    }

    public static Vector3 Velocity(InputDevice device) {

        if (device == null) {
            return Vector3.zero;
        }

        Vector3 a;

        device.TryGetFeatureValue(CommonUsages.deviceVelocity, out a);

        return a;
    }

    public static Vector3 AngularVelocity(InputDevice device) {

        if (device == null) {
            return Vector3.zero;
        }

        Vector3 a;

        device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out a);

        return a;
    }
}

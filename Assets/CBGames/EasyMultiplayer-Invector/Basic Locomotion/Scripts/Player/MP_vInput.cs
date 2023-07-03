using EMI.Player;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
#if MOBILE_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace Invector.vCharacterController
{
    public class vInput : MonoBehaviour
    {
        public delegate void OnChangeInputType(InputDevice type);
        public event OnChangeInputType onChangeInputType;
        private static vInput _instance;
        public static vInput instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<vInput>();
                    if (_instance == null)
                    {
                        new GameObject("vInputType", typeof(vInput));
                        return vInput.instance;
                    }
                }
                return _instance;
            }
        }

        public vHUDController hud;

        void Start()
        {
            if (hud == null) hud = vHUDController.instance;

        }

        private InputDevice _inputType = InputDevice.MouseKeyboard;
        [HideInInspector]
        public InputDevice inputDevice
        {
            get { return _inputType; }
            set
            {
                _inputType = value;
                OnChangeInput();
            }
        }

        void OnGUI()
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    if (isJoystickInput())
                    {
                        inputDevice = InputDevice.Joystick;

                        if (hud != null)
                        {
                            hud.controllerInput = true;
                            hud.ShowText("Control scheme changed to Controller", 2f, 0.5f);
                        }
                    }
                    else if (isMobileInput())
                    {
                        inputDevice = InputDevice.Mobile;
                        if (hud != null)
                        {
                            hud.controllerInput = true;
                            hud.ShowText("Control scheme changed to Mobile", 2f, 0.5f);
                        }
                    }
                    break;
                case InputDevice.Joystick:
                    if (isMouseKeyboard())
                    {
                        inputDevice = InputDevice.MouseKeyboard;
                        if (hud != null)
                        {
                            hud.controllerInput = false;
                            hud.ShowText("Control scheme changed to Keyboard/Mouse", 2f, 0.5f);
                        }
                    }
                    else if (isMobileInput())
                    {
                        inputDevice = InputDevice.Mobile;
                        if (hud != null)
                        {
                            hud.controllerInput = true;
                            hud.ShowText("Control scheme changed to Mobile", 2f, 0.5f);
                        }
                    }
                    break;
                case InputDevice.Mobile:
                    if (isMouseKeyboard())
                    {
                        inputDevice = InputDevice.MouseKeyboard;
                        if (hud != null)
                        {
                            hud.controllerInput = false;
                            hud.ShowText("Control scheme changed to Keyboard/Mouse", 2f, 0.5f);
                        }
                    }
                    else if (isJoystickInput())
                    {
                        inputDevice = InputDevice.Joystick;
                        if (hud != null)
                        {
                            hud.controllerInput = true;
                            hud.ShowText("Control scheme changed to Controller", 2f, 0.5f);
                        }
                    }
                    break;
            }
        }

        private bool isMobileInput()
        {
#if UNITY_EDITOR && UNITY_MOBILE
            if (EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                return true;
            }
		
#elif MOBILE_INPUT
            if (EventSystem.current.IsPointerOverGameObject() || (Input.touches.Length > 0))
                return true;
#endif
            return false;
        }

        private bool isMouseKeyboard()
        {
#if MOBILE_INPUT
            return false;
#else
            // mouse & keyboard buttons
            if (Event.current.isKey || Event.current.isMouse)
                return true;
            // mouse movement
            if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f)
                return true;

            return false;
#endif
        }

        private bool isJoystickInput()
        {
            // joystick buttons
            if (Input.GetKey(KeyCode.Joystick1Button0) ||
                Input.GetKey(KeyCode.Joystick1Button1) ||
                Input.GetKey(KeyCode.Joystick1Button2) ||
                Input.GetKey(KeyCode.Joystick1Button3) ||
                Input.GetKey(KeyCode.Joystick1Button4) ||
                Input.GetKey(KeyCode.Joystick1Button5) ||
                Input.GetKey(KeyCode.Joystick1Button6) ||
                Input.GetKey(KeyCode.Joystick1Button7) ||
                Input.GetKey(KeyCode.Joystick1Button8) ||
                Input.GetKey(KeyCode.Joystick1Button9) ||
                Input.GetKey(KeyCode.Joystick1Button10) ||
                Input.GetKey(KeyCode.Joystick1Button11) ||
                Input.GetKey(KeyCode.Joystick1Button12) ||
                Input.GetKey(KeyCode.Joystick1Button13) ||
                Input.GetKey(KeyCode.Joystick1Button14) ||
                Input.GetKey(KeyCode.Joystick1Button15) ||
                Input.GetKey(KeyCode.Joystick1Button16) ||
                Input.GetKey(KeyCode.Joystick1Button17) ||
                Input.GetKey(KeyCode.Joystick1Button18) ||
                Input.GetKey(KeyCode.Joystick1Button19))
            {
                return true;
            }

            // joystick axis
            if (Input.GetAxis("LeftAnalogHorizontal") != 0.0f ||
                Input.GetAxis("LeftAnalogVertical") != 0.0f ||
                Input.GetAxis("RightAnalogHorizontal") != 0.0f ||
                Input.GetAxis("RightAnalogVertical") != 0.0f ||
                Input.GetAxis("LT") != 0.0f ||
                Input.GetAxis("RT") != 0.0f ||
                Input.GetAxis("D-Pad Horizontal") != 0.0f ||
                Input.GetAxis("D-Pad Vertical") != 0.0f)
            {
                return true;
            }
            return false;
        }

        void OnChangeInput()
        {
            if (onChangeInputType != null)
            {
                onChangeInputType(inputDevice);
            }
        }
    }

    /// <summary>
    /// INPUT TYPE - check in real time if you are using a joystick, mobile or mouse/keyboard
    /// </summary>
    [HideInInspector]
    public enum InputDevice
    {
        MouseKeyboard,
        Joystick,
        Mobile
    };

    [System.Serializable]
    public class GenericInput
    {
        // custom added values
        [SerializeField] public BasicNetworkCalls nc;
        [SerializeField] public string variableName;
        protected bool seperateActions = false;
        protected Action callback;
        [SerializeField] public float curFloatValue = 0.0f;
        [SerializeField] public float prevFloatValue = 0.0f;
        [SerializeField] public bool curBoolValue = false;
        [SerializeField] public bool prevBoolValue = false;

        protected InputDevice inputDevice { get { return vInput.instance.inputDevice; } }
        public bool useInput = true;
        [SerializeField]
        private bool isAxisInUse;

        [SerializeField]
        public string keyboard;
        [SerializeField]
        public bool keyboardAxis;
        [SerializeField]
        public string joystick;
        [SerializeField]
        public bool joystickAxis;
        [SerializeField]
        public string mobile;
        [SerializeField]
        public bool mobileAxis;

        [SerializeField]
        public bool joystickAxisInvert;
        [SerializeField]
        public bool keyboardAxisInvert;
        [SerializeField]
        public bool mobileAxisInvert;

        public float timeButtonWasPressed;
        public float lastTimeTheButtonWasPressed;
        public bool inButtomTimer;
        private float multTapTimer;
        private int multTapCounter;
        protected bool prevBtnValue = false;
        protected bool holdingBtn = false;

        public bool isAxis
        {
            get
            {
                bool value = false;
                switch (inputDevice)
                {
                    case InputDevice.Joystick:
                        value = joystickAxis;
                        break;
                    case InputDevice.MouseKeyboard:
                        value = keyboardAxis;
                        break;
                    case InputDevice.Mobile:
                        value = mobileAxis;
                        break;
                }
                return value;
            }
        }

        public bool isAxisInvert
        {
            get
            {
                bool value = false;
                switch (inputDevice)
                {
                    case InputDevice.Joystick:
                        value = joystickAxisInvert;
                        break;
                    case InputDevice.MouseKeyboard:
                        value = keyboardAxisInvert;
                        break;
                    case InputDevice.Mobile:
                        value = mobileAxisInvert;
                        break;
                }
                return value;
            }
        }

        /// <summary>
        /// Initialise a new GenericInput
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="joystick"></param>
        /// <param name="mobile"></param>
        public GenericInput(string keyboard, string joystick, string mobile)
        {
            this.keyboard = keyboard;
            this.joystick = joystick;
            this.mobile = mobile;
        }

        /// <summary>
        /// Initialise a new GenericInput
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="joystick"></param>
        /// <param name="mobile"></param>
        public GenericInput(string keyboard, bool keyboardAxis, string joystick, bool joystickAxis, string mobile, bool mobileAxis)
        {
            this.keyboard = keyboard;
            this.keyboardAxis = keyboardAxis;
            this.joystick = joystick;
            this.joystickAxis = joystickAxis;
            this.mobile = mobile;
            this.mobileAxis = mobileAxis;
        }

        /// <summary>
        /// Initialise a new GenericInput
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="joystick"></param>
        /// <param name="mobile"></param>
        public GenericInput(string keyboard, bool keyboardAxis, bool keyboardInvert, string joystick, bool joystickAxis, bool joystickInvert, string mobile, bool mobileAxis, bool mobileInvert)
        {
            this.keyboard = keyboard;
            this.keyboardAxis = keyboardAxis;
            this.keyboardAxisInvert = keyboardInvert;
            this.joystick = joystick;
            this.joystickAxis = joystickAxis;
            this.joystickAxisInvert = joystickInvert;
            this.mobile = mobile;
            this.mobileAxis = mobileAxis;
            this.mobileAxisInvert = mobileInvert;
        }

        /// <summary>
        /// Button Name
        /// </summary>
        public string buttonName
        {
            get
            {
                if (vInput.instance != null)
                {
                    if (vInput.instance.inputDevice == InputDevice.MouseKeyboard) return keyboard.ToString();
                    else if (vInput.instance.inputDevice == InputDevice.Joystick) return joystick;
                    else return mobile;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if button is a Key
        /// </summary>
        public bool isKey
        {
            get
            {
                if (vInput.instance != null)
                {
                    if (System.Enum.IsDefined(typeof(KeyCode), buttonName))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get <see cref="KeyCode"/> value
        /// </summary>
        public KeyCode key
        {
            get
            {
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), buttonName);
            }
        }

        /// <summary>
        /// Set the NetworkCalls component to update (if nothing is set nothing will be transmitted)
        /// </summary>
        /// <param name="nc"></param>
        public void SetNetworkCalls(BasicNetworkCalls nc, string variableName, bool seperateActions=false)
        {
            this.nc = nc;
            this.variableName = variableName;
            this.seperateActions = seperateActions;
        }

        public BasicNetworkCalls GetNetworkCalls()
        {
            return nc;
        }

        /// <summary>
        /// If you want send the result of the key press across the network but will also, if 
        /// the server, call a callback function
        /// </summary>
        /// <param name="nc">The network calls component</param>
        /// <param name="variableName">The name of this variable</param>
        /// <param name="callback">A function to execute when the key press matches the resolver.</param>
        public void SetNetworkCalls(BasicNetworkCalls nc, string variableName,  Action callback)
        {
            this.nc = nc;
            this.variableName = variableName;
            if (NetworkServer.active) // callbacks are only available on the server.
            {
                this.callback = callback;
            }
        }

        /// <summary>
        /// Get Button and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <returns></returns>
        public bool GetButton()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButton();

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                return (bool)nc.GetValue(this.variableName, typeof(bool));
            }
            else if (nc == null || (nc && nc.hasAuthority && NetworkClient.active))
            {
                // mobile
                if (inputDevice == InputDevice.Mobile)
                {
                    #if MOBILE_INPUT
                    curBoolValue = CrossPlatformInputManager.GetButton(this.buttonName)
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(this.variableName, true);
                    }
                    return curBoolValue;
                    #endif
                }
                // keyboard/mouse
                else if (inputDevice == InputDevice.MouseKeyboard)
                {
                    if (isKey)
                    {
                        curBoolValue = Input.GetKey(key);
                    }
                    else
                    {
                        curBoolValue = Input.GetButton(this.buttonName);
                    }
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(this.variableName, curBoolValue);
                    }
                    return curBoolValue;
                }
                // joystick
                else if (inputDevice == InputDevice.Joystick)
                {
                    curBoolValue = Input.GetButton(this.buttonName);
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(this.variableName, curBoolValue);
                    }
                    return curBoolValue;
                }

                if (false != prevBoolValue)
                {
                    prevBoolValue = false;
                    if (nc) nc.UpdateInput(this.variableName, false);
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Get ButtonDown and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <returns></returns>
        public bool GetButtonDown()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButtonDown();

            string varName = (seperateActions) ? this.variableName + "_down" : this.variableName;
            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && !nc.hasAuthority) || (NetworkServer.active && !NetworkClient.active)))
            {
                bool boolValue = (bool)nc.GetValue(varName, typeof(bool));
                if (prevBtnValue == boolValue && boolValue == true) // this if check prevents a double tap of the key because Updates are so fast
                {
                    return false;
                }
                else if (prevBtnValue != boolValue)
                {
                    prevBtnValue = boolValue;
                    if (NetworkServer.active)
                    {
                        callback?.Invoke();
                    }
                }
                return prevBtnValue;
            }
            // mobile
            else if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                if (nc == null || nc != null && nc.hasAuthority)
                {
                    if (CrossPlatformInputManager.GetButtonDown(this.buttonName))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard && (nc == null || (nc != null && NetworkClient.active && nc.hasAuthority)))
            {
                if (isKey)
                {
                    if (Input.GetKeyDown(key))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
                else if (nc == null || nc != null && NetworkClient.active && nc.hasAuthority)
                {
                    if (Input.GetButtonDown(this.buttonName))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick && (nc == null || (nc != null && NetworkClient.active && nc.hasAuthority)))
            {
                if (Input.GetButtonDown(this.buttonName))
                {
                    curBoolValue = true;
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(varName, true);
                    }
                    return true;
                }
            }
            curBoolValue = false;
            if (curBoolValue != prevBoolValue)
            {
                prevBoolValue = curBoolValue;
                if (nc) nc.UpdateInput(varName, false);
            }

            return false;
        }

        /// <summary>
        /// Get Button Up and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <returns></returns>
        public bool GetButtonUp()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButtonUp();

            string varName = (seperateActions) ? this.variableName + "_up" : this.variableName;
            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                bool boolValue = (bool)nc.GetValue(varName, typeof(bool));
                if (prevBtnValue == boolValue && boolValue == true)
                {
                    return false;
                }
                else if (prevBtnValue != boolValue)
                {
                    prevBtnValue = boolValue;
                    if (NetworkServer.active) // this prevents calling the callback twice. Once for pressing and once for releasing
                    {
                        callback?.Invoke();
                    }
                }
                return prevBtnValue;
            }
            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                if (nc == null || nc != null && nc.hasAuthority)
                {
                    if (CrossPlatformInputManager.GetButtonUp(this.buttonName))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKeyUp(key))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
                else
                {
                    if (Input.GetButtonUp(this.buttonName))
                    {
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, true);
                        }
                        return true;
                    }
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButtonUp(this.buttonName))
                {
                    curBoolValue = true;
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(varName, true);
                    }
                    return true;
                }
            }
            else if (nc != null && NetworkClient.active && nc.hasAuthority)
            {
                curBoolValue = false;
                if (curBoolValue != prevBoolValue)
                {
                    prevBoolValue = curBoolValue;
                    if (nc) nc.UpdateInput(varName, false);
                }
            }
            return false;
        }

        /// <summary>
        /// Get Axis and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <returns></returns>
        public float GetAxis()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || isKey) return 0;

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                return (float)nc.GetValue(this.variableName, typeof(float));
            }

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                curFloatValue = CrossPlatformInputManager.GetAxis(this.buttonName);
                if (nc == null || nc != null && nc.hasAuthority)
                {
                    if (curBoolValue != prevFloatValue)
                    {
                        prevFloatValue = curBoolValue;
                        if (nc) nc.UpdateInput(this.variableName, curFloatValue);
                    }
                }
                return value;
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard && (nc == null || (nc != null && nc.hasAuthority)))
            {
                curFloatValue = Input.GetAxis(this.buttonName);
                if (curFloatValue != prevFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, curFloatValue);
                }
                return curFloatValue;
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick && (nc == null || (nc != null && nc.hasAuthority)))
            {
                curFloatValue = Input.GetAxis(this.buttonName);
                if (curFloatValue != prevFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, curFloatValue);
                }
                return curFloatValue;
            }
            else if (nc != null && NetworkClient.active && nc.hasAuthority)
            {
                curFloatValue = 0;
                if (curFloatValue != prevFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, curFloatValue);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Axis Raw and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <returns></returns>
        public float GetAxisRaw()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || isKey) return 0;

            if ((nc != null && NetworkClient.active && !nc.hasAuthority) || (NetworkServer.active && !NetworkClient.active))
            {
                float value = (float)nc.GetValue(this.variableName, typeof(float));
                return value;
            }
            // mobile
            else if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                curFloatValue = CrossPlatformInputManager.GetAxisRaw(this.buttonName);
                if (nc != null && nc.hasAuthority)
                {
                    if (prevFloatValue != curFloatValue)
                    {
                        prevFloatValue = curFloatValue;
                        if (nc) nc.UpdateInput(this.variableName, (float)curFloatValue);
                    }
                }
                return curFloatValue;
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard && (nc == null || (nc != null && nc.hasAuthority)))
            {
                curFloatValue = Input.GetAxisRaw(this.buttonName);
                if (prevFloatValue != curFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, (float)curFloatValue);
                }
                return curFloatValue;
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick && (nc == null || (nc != null && nc.hasAuthority)))
            {
                curFloatValue = Input.GetAxisRaw(this.buttonName);
                if (prevFloatValue != curFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, (float)curFloatValue);
                }
                return curFloatValue;
            }
            else if (nc != null && NetworkClient.active && nc.hasAuthority)
            {
                curFloatValue = 0;
                if (prevFloatValue != curFloatValue)
                {
                    prevFloatValue = curFloatValue;
                    if (nc) nc.UpdateInput(this.variableName, (float)curFloatValue);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Double Button Down Check if button is pressed Within the defined time and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <param name="inputTime"></param>
        /// <returns></returns>
        public bool GetDoubleButtonDown(float inputTime = 1)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                bool boolValue = (bool)nc.GetValue(this.variableName + "_double", typeof(bool));
                if (prevBtnValue == boolValue && boolValue == true)
                {
                    return false;
                }
                else if (prevBtnValue != boolValue)
                {
                    prevBtnValue = boolValue;
                }
                return prevBtnValue;
            }
            else if (nc == null || (NetworkClient.active && nc != null && nc.hasAuthority))
            {
                if (multTapCounter == 0 && GetButtonDown())
                {
                    multTapTimer = Time.time;
                    multTapCounter = 1;
                    if (nc) nc.UpdateInput(this.variableName + "_double", false);
                    return false;
                }

                if (multTapCounter == 1 && GetButtonDown())
                {
                    var time = multTapTimer + inputTime;
                    var valid = (Time.time < time);
                    multTapTimer = 0;
                    multTapCounter = 0;
                    if (nc) nc.UpdateInput(this.variableName + "_double", valid);
                    return valid;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Buttom Timer Check if a button is pressed for defined time and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <param name="inputTime"> time to check button press</param>
        /// <returns></returns>
        public bool GetButtonTimer(float inputTime = 2)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;

            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                holdingBtn = (bool)nc.GetValue(this.variableName + "_hold", typeof(bool));
                if (holdingBtn != prevBoolValue)
                {
                    prevBoolValue = holdingBtn;
                    if (holdingBtn == true && !inButtomTimer)
                    {
                        lastTimeTheButtonWasPressed = Time.time + 0.1f;
                        timeButtonWasPressed = Time.time;
                        inButtomTimer = true;
                    }
                }
            }
            else if (nc == null || (nc != null && nc.hasAuthority && NetworkClient.active))
            {
                if (GetButtonDown() && !inButtomTimer)
                {
                    holdingBtn = true;
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                    timeButtonWasPressed = Time.time;
                    inButtomTimer = true;
                }
                else if (GetButtonUp())
                {
                    holdingBtn = false;
                }
                if (holdingBtn != prevBtnValue)
                {
                    prevBtnValue = holdingBtn;
                    if (nc) nc.UpdateInput(this.variableName + "_hold", holdingBtn);
                }
            }
            if (inButtomTimer)
            {
                var time = timeButtonWasPressed + inputTime;
                var valid = (time - Time.time <= 0);

                if (!holdingBtn || lastTimeTheButtonWasPressed < Time.time)
                {
                    inButtomTimer = false;
                    return false;
                }
                else
                {
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                }
                if (valid)
                {
                    inButtomTimer = false;
                    if (holdingBtn != false)
                    {
                        holdingBtn = false;
                        if (nc) nc.UpdateInput(this.variableName + "_hold", false);
                    }
                }

                return valid;
            }

            return false;
        }

        /// <summary>
        /// Get Buttom Timer Check if a button is pressed for defined time and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <param name="inputTime"> time to check button press</param>
        /// <returns></returns>
        public bool GetButtonTimer(ref float currentTimer, float inputTime = 2)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;

            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                holdingBtn = (bool)nc.GetValue(this.variableName + "_hold", typeof(bool));
                if (holdingBtn != prevBoolValue)
                {
                    prevBoolValue = holdingBtn;
                    if (holdingBtn == true && !inButtomTimer)
                    {
                        lastTimeTheButtonWasPressed = Time.time + 0.1f;
                        timeButtonWasPressed = Time.time;
                        inButtomTimer = true;
                    }
                }
            }
            else if (nc == null || (nc != null && nc.hasAuthority && NetworkClient.active))
            {
                if (GetButtonDown() && !inButtomTimer)
                {
                    holdingBtn = true;
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                    timeButtonWasPressed = Time.time;
                    inButtomTimer = true;
                }
                else if (GetButtonUp())
                {
                    holdingBtn = false;
                }
                if (holdingBtn != prevBtnValue)
                {
                    prevBtnValue = holdingBtn;
                    if (nc) nc.UpdateInput(this.variableName + "_hold", holdingBtn);
                }
            }
            if (inButtomTimer)
            {
                var time = timeButtonWasPressed + inputTime;
                currentTimer = time - Time.time;
                var valid = (time - Time.time <= 0);

                if (!holdingBtn || lastTimeTheButtonWasPressed < Time.time)
                {
                    inButtomTimer = false;
                    return false;
                }
                else
                {
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                }
                if (valid)
                {
                    inButtomTimer = false;
                    if (nc && nc.hasAuthority && NetworkClient.active)
                    {
                        holdingBtn = false;
                        nc.UpdateInput(this.variableName + "_hold", false);
                    }
                }
                
                return valid;
            }

            return false;
        }

        /// <summary>
        /// Get Buttom Timer Check if a button is pressed for defined time and if owner send results to server who will transmit it to other clients
        /// </summary>
        /// <param name="inputTime"> time to check button press</param>
        /// <returns></returns>
        public bool GetButtonTimer(ref float currentTimer, ref bool upAfterPressed, float inputTime = 2)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                holdingBtn = (bool)nc.GetValue(this.variableName + "_hold", typeof(bool));
                if (holdingBtn != prevBoolValue)
                {
                    prevBoolValue = holdingBtn;
                    if (holdingBtn == true)
                    {
                        prevBoolValue = holdingBtn;
                        lastTimeTheButtonWasPressed = Time.time + 0.1f;
                        timeButtonWasPressed = Time.time;
                        inButtomTimer = true;
                    }
                }
            }
            else if (nc == null || (nc != null && nc.hasAuthority && NetworkClient.active))
            {
                if (GetButtonDown())
                {
                    holdingBtn = true;
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                    timeButtonWasPressed = Time.time;
                    inButtomTimer = true;
                }
                else if (GetButtonUp())
                {
                    holdingBtn = false;
                }
                if (holdingBtn != prevBtnValue)
                {
                    prevBtnValue = holdingBtn;
                    nc.UpdateInput(this.variableName + "_hold", holdingBtn);
                }
            }
            if (inButtomTimer)
            {
                var time = timeButtonWasPressed + inputTime;
                currentTimer = (inputTime - (time - Time.time)) / inputTime;
                var valid = (time - Time.time <= 0);

                if (!holdingBtn || lastTimeTheButtonWasPressed < Time.time)
                {
                    inButtomTimer = false;
                    upAfterPressed = true;
                    return false;
                }
                else
                {
                    upAfterPressed = false;
                    lastTimeTheButtonWasPressed = Time.time + 0.1f;
                }
                if (valid)
                {
                    inButtomTimer = false;
                    holdingBtn = false;
                    if (holdingBtn != prevBoolValue)
                    {
                        prevBoolValue = holdingBtn;
                        if (nc && nc.hasAuthority && NetworkClient.active) 
                            nc.UpdateInput(this.variableName + "_hold", false);
                    }
                }
                    
                return valid;
            }
                
            return false;
        }

        /// <summary>
        /// Get Axis like a button and if owner send results to server who will transmit it to other clients   
        /// </summary>
        /// <param name="value">Value to check need to be diferent 0</param>
        /// <returns></returns>
        public bool GetAxisButton(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxisInvert) value *= -1f;

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                return (bool)nc.GetValue(this.variableName, typeof(bool));
            }
            else if (nc == null || (nc != null && nc.hasAuthority))
            {
                if (value > 0)
                {
                    return GetAxisRaw() >= value;
                }
                else if (value < 0)
                {
                    return GetAxisRaw() <= value;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Axis like a buttonDown and if owner send results to server who will transmit it to other clients   
        /// </summary>
        /// <param name="value">Value to check need to be diferent 0</param>
        /// <returns></returns>
        public bool GetAxisButtonDown(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxisInvert) value *= -1f;
            string varName = (seperateActions) ? this.variableName + "_axisdown" : this.variableName;
            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                return (bool)nc.GetValue(varName, typeof(bool));
            }
            else if (nc == null || (nc != null && nc.hasAuthority))
            {
                if (value > 0)
                {
                    if (!isAxisInUse && GetAxisRaw() >= value)
                    {
                        isAxisInUse = true;
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, curBoolValue);
                        }
                        return true;
                    }
                    else if (isAxisInUse && GetAxisRaw() == 0)
                    {
                        isAxisInUse = false;
                    }
                }
                else if (value < 0)
                {
                    if (!isAxisInUse && GetAxisRaw() <= value)
                    {
                        isAxisInUse = true;
                        curBoolValue = true;
                        if (curBoolValue != prevBoolValue)
                        {
                            prevBoolValue = curBoolValue;
                            if (nc) nc.UpdateInput(varName, curBoolValue);
                        }
                        return true;
                    }
                    else if (isAxisInUse && GetAxisRaw() == 0)
                    {
                        isAxisInUse = false;
                    }
                }
            }
            else if (nc != null && nc.hasAuthority)
            {
                curBoolValue = false;
                if (curBoolValue != prevBoolValue)
                {
                    prevBoolValue = curBoolValue;
                    if (nc) nc.UpdateInput(varName, curBoolValue);
                }
            }
            return false;
        }

        /// <summary>
        /// Get Axis like a buttonUp and if owner send results to server who will transmit it to other clients   
        /// Check if Axis is zero after press       
        /// <returns></returns>
        public bool GetAxisButtonUp()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            string varName = (seperateActions) ? this.variableName + "_axisup" : this.variableName;

            // if network version of player (none owning player)
            if (nc != null && ((NetworkClient.active && nc.hasAuthority == false) || (NetworkServer.active && !NetworkClient.active)))
            {
                return (bool)nc.GetValue(varName, typeof(bool));
            }
            else if (nc == null || (nc != null && nc.hasAuthority))
            {
                if (isAxisInUse && GetAxisRaw() == 0)
                {
                    isAxisInUse = false;
                    curBoolValue = true;
                    if (curBoolValue != prevBoolValue)
                    {
                        prevBoolValue = curBoolValue;
                        if (nc) nc.UpdateInput(varName, curBoolValue);
                    }
                    return true;
                }
                else if (!isAxisInUse && GetAxisRaw() != 0)
                {
                    isAxisInUse = true;
                }
            }
            else if (nc != null && nc.hasAuthority)
            {
                curBoolValue = false;
                if (curBoolValue != prevBoolValue)
                {
                    prevBoolValue = curBoolValue;
                    if (nc) nc.UpdateInput(varName, curBoolValue);
                }
            }
            return false;
        }

        bool IsButtonAvailable(string btnName)
        {
            if (!useInput) return false;
            try
            {
                if (isKey) return true;
                Input.GetButton(buttonName);
                return true;
            }
            catch (System.Exception exc)
            {
                Debug.LogWarning(" Failure to try access button :" + buttonName + "\n" + exc.Message);
                return false;
            }
        }
    }
}

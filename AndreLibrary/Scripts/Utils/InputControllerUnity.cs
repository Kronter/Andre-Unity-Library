using System;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.Utils
{
    public class InputButtonUnity
    {
        public string ButtonName;
        public event Action OnButtonDown;
        public event Action OnButtonUp;
        public event Action OnButton;

        public void CheckInput()
        {
            if (Input.GetButtonDown(ButtonName))
            {
                OnButtonDown?.Invoke();
            }
            if (Input.GetButtonUp(ButtonName))
            {
                OnButtonUp?.Invoke();
            }
            if (Input.GetButton(ButtonName))
            {
                OnButton?.Invoke();
            }
        }
    }

    public class InputControllerUnity : MonoBehaviour
    {
        public int playerId = 0;
        private List<InputButtonUnity> inputs;

        [System.NonSerialized, HideInInspector]
        public bool initialized;

        public void SetUp()
        {
            inputs = new List<InputButtonUnity>();
            initialized = true;
        }

        private void Update()
        {
            foreach (InputButtonUnity input in inputs)
            {
                input.CheckInput();
            }
        }

        public bool HasInputButton(string name)
        {
            if (inputs.Count == 0)
                return false;

            return GetInputButtonIndex(name) != -1;
        }

        public bool HasInputButton(int index)
        {
            if (inputs.Count == 0)
                return false;

            return inputs.Count - 1 <= index;
        }

        public int GetInputButtonIndex(string name)
        {
            if (inputs.Count == 0)
                return -1;

            return inputs.FindIndex(input => input.ButtonName == name);
        }

        public string GetInputButtonName(int index)
        {
            if (HasInputButton(index))
                return inputs[index].ButtonName;

            return "";
        }

        public InputButtonUnity GetInputButton(string name)
        {
            if (HasInputButton(name))
                return inputs[GetInputButtonIndex(name)];

            return null;
        }

        public InputButtonUnity GetInputButton(int index)
        {
            if (HasInputButton(index))
                return inputs[index];

            return null;
        }

        public void ChangeInputButtonName(string curName, string nameToChange)
        {
            if (HasInputButton(curName))
                inputs[GetInputButtonIndex(curName)].ButtonName = nameToChange;
        }

        public void ChangeInputButtonName(int index, string nameToChange)
        {
            if (HasInputButton(index))
                inputs[index].ButtonName = nameToChange;
        }

        public bool AddInputButton(string name)
        {
            if (HasInputButton(name))
                return false;

            InputButtonUnity newButton = new InputButtonUnity
            {
                ButtonName = name
            };

            inputs.Add(newButton);
            return true;
        }

        public bool RemoveInputButton(string name)
        {
            if (!HasInputButton(name))
                return false;

            inputs.Remove(GetInputButton(name));
            return true;
        }

        public bool RemoveInputButton(int index)
        {
            if (!HasInputButton(index))
                return false;

            inputs.Remove(GetInputButton(index));
            return true;
        }

        /// <summary>
        /// gets input movement in 2D
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMovement2D()
        {
            Vector2 movement = Vector2.zero;
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            return movement;
        }

        /// <summary>
        /// gets input movement in 2D
        /// </summary>
        /// <param name="horizontalAxisName"></param>
        /// <param name="verticalAxisName"></param>
        /// <returns></returns>
        public Vector2 GetMovement2D(string horizontalAxisName, string verticalAxisName)
        {
            Vector2 movement = Vector2.zero;
            movement.x = Input.GetAxis(horizontalAxisName);
            movement.y = Input.GetAxis(verticalAxisName);
            return movement;
        }

        /// <summary>
        /// gets input movement in 3D
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMovement3D()
        {
            Vector3 movement = Vector3.zero;
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
            return movement;
        }

        /// <summary>
        /// gets input movement in 3D
        /// </summary>
        /// <param name="horizontalAxisName"></param>
        /// <param name="verticalAxisName"></param>
        /// <returns></returns>
        public Vector2 GetMovement3D(string horizontalAxisName, string verticalAxisName)
        {
            Vector3 movement = Vector3.zero;
            movement.x = Input.GetAxis(horizontalAxisName);
            movement.z = Input.GetAxis(verticalAxisName);
            return movement;
        }
    }
}

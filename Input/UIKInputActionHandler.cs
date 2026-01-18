using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    public interface UIKInputActionHandler
    {
        /// <returns>Return true if the input action was consumed, false if not</returns>
        public virtual bool HandleInputAction(InputAction.CallbackContext _context)
        {
            return false;
        }
    }
} // UIKit namespace

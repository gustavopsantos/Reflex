using Reflex.Microsoft.Sample.Application;
using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
{
    internal class CollectorInputMouse : ICollectorInput
    {
        public Vector2 Get()
        {
            return new Vector2
            {
                x = Input.GetAxis("Mouse X"),
                y = Input.GetAxis("Mouse Y")
            };
        }
    }
}
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    public class CollectorInputMouse : ICollectorInput
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
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    public class CollectorInputKeyboard : ICollectorInput
    {
        public Vector2 Get()
        {
            return new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
        }
    }
}
using Reflex.Microsoft.Sample.Application;
using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
{
    internal class CollectorInputKeyboard : ICollectorInput
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
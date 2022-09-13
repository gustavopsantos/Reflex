using UnityEngine;

namespace Reflex.Scripts
{
    public abstract class Installer : MonoBehaviour
    {
        public abstract void InstallBindings(Container container);
    }
}
using Reflex.Scripts;
using UnityEngine;

public class NumberInstaller : Installer
{
    [SerializeField] private int _number;
    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton(_number);
    }
}
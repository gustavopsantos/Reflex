using System.Collections;
using System.Collections.Generic;
using Project.Code.DI;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ExampleFactory3 injectExample;
    [SerializeField] private InjectExample examplePrefab;
    
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton("Hello");
        containerBuilder.AddFactory<InjectExample, InjectExample.InjectExampleFactory>();
        containerBuilder.AddFactory<InjectExample, InjectExample.InjectExampleFactory2>();
        containerBuilder.AddFactory<InjectExample, ExampleFactory3>(examplePrefab);
        containerBuilder.AddFactoryFromPrefab<InjectExample, ExampleFactory3>(injectExample, examplePrefab);
        containerBuilder.AddFactory<InjectExample, ExampleFactory3>();
    }
}

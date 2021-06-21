using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IExampleService>().FromInstance(new ExampleService());
    }
}

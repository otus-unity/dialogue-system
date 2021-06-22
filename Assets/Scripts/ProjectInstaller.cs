using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IExampleService>().FromInstance(new ExampleService());
        Container.Bind<INotificationManager>().FromInstance(new NotificationManager());
        Container.Bind<IQuestManager>().FromInstance(GetComponent<QuestManager>());
    }
}

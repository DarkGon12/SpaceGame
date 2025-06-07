using MessagePipe;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        private MessagePipeOptions options;

        public override void InstallBindings()
        {
            options = Container.BindMessagePipe();


            BindSceneInstaller();
            BindMessages();
        }

        private void BindSceneInstaller()
        {
            Container.Bind<SceneInfo>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<Inventory>().FromComponentInHierarchy().AsSingle().NonLazy();
        }

        private void BindMessages()
        {
            Container.BindMessageBroker<StartDroneWorkEvent>(options);
            Container.BindMessageBroker<ReturnDroneWorkEvent>(options);
            Container.BindMessageBroker<StopDroneWorkEvent>(options);

            Container.BindMessageBroker<UpdateUserInventoryEvent>(options);

            Container.BindMessageBroker<BuyDroneEvent>(options);
        }
    }
}
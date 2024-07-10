using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using GMF.Saving;
using R3;
using UnityEngine;
using UnityEngine.UI;


namespace GMF.GameInitializer
{

    public static class GameInitializer
    {

        [UnityEditor.MenuItem("Test/test")]
        public static void Test()
        {
            EventManager.UnSubscribeAll();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            int lenght = 1000;
            // Запуск Stopwatch
            stopwatch.Start();
            var arr = new EventTest[lenght];

            for (int i = 0; i < lenght; i++)
            {
                var et = new EventTest();
                arr[i] = et;

            }
            stopwatch.Stop();
            var t0 = stopwatch.ElapsedMilliseconds;

            stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < lenght; i++)
            {
                var et = arr[i];

                EventManager.Subscribe(et);
            }
            stopwatch.Stop();


            var t = stopwatch.ElapsedMilliseconds;

            stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                EventManager.SendEvent(new EventInt());


            }
            stopwatch.Stop();
            var t2 = stopwatch.ElapsedMilliseconds;

            UnityEngine.Debug.Log($"Delay Test: {t0} : {t} : {t2} {EventTest.counter}");
        }
        public static void Initialize()
        {

        }

        [UnityEditor.MenuItem("Test/Save")]
        public static void SaveTest()
        {
            var sm = new SaveLoadManager(new JsonDataSerializer(), new UnityLocalStorageProvider());
            Task.Run(async () => await sm.AutoSaveAsync()).GetAwaiter().GetResult();
            //Task.Run(( () => sm.SaveAsync(new ExampleSaveMetaData { Path = "P" })));
        }

        [UnityEditor.MenuItem("Test/Load")]
        public static void LoadTest()
        {
            var sm = new SaveLoadManager(new JsonDataSerializer(), new UnityLocalStorageProvider());
            Task.Run(async () => await sm.AutoLoadAsync()).GetAwaiter().GetResult();
        }
    }



    public struct EventInt : IEvent
    {
        int i;
    }

    public struct EventString : IEvent
    {
        string s;
    }
    [System.Serializable]
    public class EventTest : IEventSubscriber<EventInt>, IEventSubscriber<EventString>
    {
        public static int counter;
        Vector2 vect;
        void IEventSubscriber<EventInt>.ReceiveEvent(EventInt data)
        {
            int t = 1;
            counter++;
            t += counter / t;
        }

        void IEventSubscriber<EventString>.ReceiveEvent(EventString data)
        {

        }


    }

    public class Enemy
    {
        public ReactiveProperty<long> CurrentHp { get; private set; }

        public ReactiveProperty<bool> IsDead { get; private set; }
        Subject<ShipPartType> subject;
        public Enemy(int initialHp)
        {
            // Declarative Property
            CurrentHp = new ReactiveProperty<long>(initialHp);

            IsDead = CurrentHp.Select(x => x <= 0).ToBindableReactiveProperty();
        }
    }

    public class GameStarter : MonoBehaviour//, IViewFor<ReactiveTest>
    {
        [SerializeField]
        int value;
        /*        [SerializeField]
                ReactiveTest2 reactiveTest;*/
        [SerializeField]
        Button b;

        Enemy enemy;

        private ICloneService _cloneService;

        public GameObject prefab;

        private void Awake()
        {
            //ObservableSystem.DefaultFrameProvider = new FrameProvider

            //reactiveTest.Mana.Subscribe(hp => Console.WriteLine($"Player HP: {hp}"));
            // Observable.EveryValueChanged(reactiveTest, x => x.Mana).Subscribe(OnManaChanged);

            //var isDead = reactiveTest.Health.Select(health => health <= 0).ToReactiveProperty();
            // IsDead.Where(isDead => isDead).Subscribe(_ => CurrentHp.Value = 0);

            enemy = new Enemy(2);

            b.onClick.AddListener(DITest);

            //  _cloneService = DependencyInjectionSetup.ServiceProvider.GetService<ICloneService>();


            // subscribe from notification model.
            //	b.OnClickAsObservable().Subscribe(_ => enemy.CurrentHp.Value -= 1);
            enemy.CurrentHp.Subscribe(x =>
                Debug.Log("HP:" + x));
            enemy.IsDead.Where(isDead => isDead == true)
            .Subscribe(_ =>
            {
                // when dead, disable button

                Debug.Log($"enemy die {_}");
            });
        }

        static void OnManaChanged(ReactiveProperty<int> mana)
        {
            Debug.Log($"Mana {mana}");
        }



        void DITest()
        {
            // _cloneService.Clone(prefab, Vector3.zero, Quaternion.identity);
            enemy.CurrentHp.Value--;


        }

    }
}


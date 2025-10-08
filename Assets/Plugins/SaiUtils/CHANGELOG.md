## [1.0.0] - 2024-09-21
### First Release
- Basically compiled the ScriptableVariables, GameEvents, and StateMachine into one project!

## [1.0.1] - 2024-09-21
### Singletons!!
- Added Generic Singletons to the mix. Now make any class a child of Singleton<T> and boom! Singleton!!

## [1.0.2] - 2024-09-21
### Transform Extensions
- Added SetPosition and SetLocalPosition to Transforms

## [1.0.3] - 2024-09-21
### SaiStaticUtils
- Added a static class for all my static function needs!!

## [1.0.4] - 2024-09-21
### Vector2Events and Trigger Controllers

## [1.0.5] - 2024-09-22
### Transform Extension Update -> GetClosestEntity
- Supply in a list of entities and GetClosestEntity will return the entity that is the closest to the player. 
  - It also helps that this is relatively more performant than using the Vector3.Distance since it does not use any sqrt functions!!

## [1.0.6] - 2024-09-22
### Improved Timers
- Credit goes to git-amend for originally showcasing this!!
- Created a CountdownTimer
  ```c#
    CountdownTimer newTimer = new CountdownTimer(timerDuration) // timerDuration is a float

    void Start() => newTimer.Start();

    // no need to use newTimer.Tick() in Update() since it is already hooked up via the PlayerLoop System

    void OnDestroy() => newTimer.Dispose(); // important to call Dispose!!
  ```

## [1.0.7] - 2024-10-01
### Optional Variables
- Credit goes to aarthificial for originally showcasing this!! [Original Post](https://gist.github.com/aarthificial/f2dbb58e4dbafd0a93713a380b9612af)
```c#
  [SerializeField] Optional<float> _interactionRange = new Optional<float>(2f);

  void Update() {
    if (_interactionRange.HasValue) DoSomething(_interactionRange.Value)
  }
```

## [1.0.8] - 2024-10-01
### Additional Extensions
- Credit goes to [Monoflauta](https://monoflauta.com/2021/07/27/11-useful-unity-c-extension-methods/)


## [1.0.9] - 2024-11-01
### ReadMe Component
- Credit goes to [neoneper](https://github.com/neoneper/UnityReadme)

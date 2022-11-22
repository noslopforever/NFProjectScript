# Purpose

This project is designed for translating ".nps" scripts (NF's prototype-script) to different project-patterns (e.g. UE4, UE5, Unity).

本项目被设计为将 “.nps脚本” （NF's prototype-script） 转换到不同的项目模板（如 UE4, UE5, Unity）。

# Example

比如一开始可以这么构思一个小塔防思路：

```
// .nps code

// components should be attached to objects/units and assign functions to them.
comp battleunit
    - hp
    - ap
    - acd
    - dp

// tower unit base
unit tower @abstract
    +battleunit
    +cellbased

// cannon tower
tower tower_cannon
    +battleunit
        - hp = 500
        - ap = 100
        - acd = 5
        - dp = 5

// electric tower
tower tower_electric
    +battleunit
        - hp = 300
        - ap = 1
        - acd = 0.1
        - dp = 1

// monster unit base
unit monster
    +battleunit
    +movable

// goblin monster
monster goblin
    +battleunit
        - hp = 20
        - ap = 10
        - dp = 1
    +movable
        - speed = 10


// a test world
world test_world
    +unit_generator_byTime
        - pos = {200, 0, 0}
        - unitclass = goblin
	- time = 10
    +towercells
        - lt = {0, 0, -50}
        - rb = {0, 0, +50}


```

开发时，先设计一个Parser，将其翻译为对应开发环境下的资源和代码，比如假设我们使用 UnrealEngine 来完成开发工作，则 goblin 可能被翻译为：

```cpp
UCLASS(Blueprintable)
class ATDMonster: public APawn
{
    GENERATED_BODY()
public:
    UPROPERTY()
    class UMovableComponent* MovableComp;

    UPROPERTY()
    class UTDBattleUnitComponent* BattleUnit;

};
```

如果在 Unity 中完成项目，则可能被翻译为：

```csharp
public class TDMoster : MonoBehaviour
{
    MovableComponent MovableComp;
    TDBattleUnitComponent BattleUnitComp;

    TDMonster()
    {
        MovableComp = this.AddComponent<MovableComponent>();
        BattleUnitComp = this.AddComponent<TDBattleUnitComponent>();
    }

}
```

由于类似 goblin， tower 这种概念都是非常抽象的，因而可以附加其他概念，比如应用于甘特图的某个 WorkItem 。

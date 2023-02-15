// Property update listener.
class PropertyUpdateListener
{
    constructor(InObj, InFn) {
        this.Obj = InObj;
        this.Fn = InFn;

        this._Pool = null;
        this._PoolHandle = null;
    }

    // invoke listener.
    Invoke() {
        this.Fn(this.Obj);
    }
}

// Listener pool
class ListenerPool
{
    constructor() {

        this.ListenerPool = [];

        this.FreeIDPool = [];

    }

    // attach
    Attach(InListener) {
        let newNum = this.ListenerPool.push(InListener);
        InListener._Pool = this;
        InListener._PoolHandle = newNum - 1;
    }

    // detach
    Detach(InHandler) {
        let killingListener = this.ListenerPool[InHandler.index];
        killingListener.Obj = null;
        killingListener.Fn = null;
        delete this.ListenerPool[InHandler.index];
        this.ListenerPool[InHandler.index] = null;
        this.FreeIDPool.push(InHandler.index);
    }

    // trigger
    TriggerAll() {
        let i = 0;
        for (; i < this.ListenerPool.length; i++) {
            let listener = this.ListenerPool[i];
            listener.Invoke();
        }
    }

}

class DataSourceComponent
{
    constructor() {

        this.PoolDict = new Map();

    }

    Attach(InName, InListener) {
        if (!this.PoolDict.has(InName)) {
            let newPool = new ListenerPool();
            this.PoolDict.set(InName, newPool);
        }
        let pool = this.PoolDict.get(InName);
        pool.Attach(InListener);
    }

    Trigger(InName) {
        let pool = this.PoolDict.get(InName);
        pool.TriggerAll();
    }

}


class DataBindingSettings
{
    constructor() {
        this.SourceType = "";
        this.SourceName = "";
        this.SourcePath = "";

        this.TargetType = "";
        this.TargetName = "";
        this.TargetPath = "";

    }

}

class DataBinding
{
    constructor() {
    }
}




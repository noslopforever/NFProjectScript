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

class DynamicDataBinding
{
    constructor(InHost, InSettings) {

        // host vm-element of the databinding.
        this.Host = InHost;

        this.Settings = InSettings;

        this.SourceObj = null;

        this.TargetObj = null;

        this.UpdateDataBinding();
    }

    static New(InHost, InSourcePath, InTargetPath) {
        let settings = new DataBindingSettings();
        settings.SourceType = "dc";
        settings.SourcePath = InSourcePath;
        settings.TargetType = "this";
        settings.TargetPath = InTargetPath;
        return new DynamicDataBinding(InHost, settings);
    }

    _Trigger(InThis)
    {
        InThis.TargetObj[InThis.Settings.TargetPath] = InThis.SourceObj[InThis.Settings.SourcePath];
    }

    UpdateDataBinding()
    {
        this.SourceObj = null;
        if (this.Settings.SourceType == "dc") {
            this.SourceObj = this.Host.dataContext;
        }

        this.TargetObj = null;
        if (this.Settings.TargetType == "this") {
            this.TargetObj = this.Host;
        }

        if (this.SourceObj.DSComp) {
            let listener = new PropertyUpdateListener(this, this._Trigger);
            this.SourceObj.DSComp.Attach(this.Settings.SourcePath, listener);
            this._Trigger(this);
        } 
        else {
            // refresh only once
            alert("Must be used with DataSource object.")
        }
    }

}




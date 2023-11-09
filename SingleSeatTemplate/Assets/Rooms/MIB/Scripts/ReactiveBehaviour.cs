using UnityEngine;
using UniRx;

public class ReactiveBehaviour : MonoBehaviour
{
    private CompositeDisposable _disposer = new CompositeDisposable();
    public CompositeDisposable Disposer
    {
        get { return _disposer; }
        private set { _disposer = value; }
    }

    public virtual void OnEnable() { }

    public virtual void OnDisable()
    {
        Disposer.Clear();
    }

    public virtual void OnDestroy()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
        Disposer.Clear();
        Disposer.Dispose();
    }

    public virtual void OnApplicationQuit()
    {
        Dispose();
    }
}

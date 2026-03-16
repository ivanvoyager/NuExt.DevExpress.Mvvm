using System.Text.Json.Serialization;

namespace DevExpress.Mvvm;

public abstract class BindableSettings : Bindable
{
    #region Internal Classes

    private class DirtySuspender : IDisposable
    {
        private readonly BindableSettings _this;

        public DirtySuspender(BindableSettings self)
        {
            _this = self;
            bool isDirtySuspended = _this.IsDirtySuspended;
            Interlocked.Increment(ref _this._isDirtySuspended);
            if (isDirtySuspended != _this.IsDirtySuspended)
            {
                _this.RaisePropertyChanged(nameof(IsDirtySuspended));
            }
        }

        public void Dispose()
        {
            bool isDirtySuspended = _this.IsDirtySuspended;
            Interlocked.Decrement(ref _this._isDirtySuspended);
            if (isDirtySuspended != _this.IsDirtySuspended)
            {
                _this.RaisePropertyChanged(nameof(IsDirtySuspended));
            }
        }
    }

    #endregion

    #region Properties

    private bool _isDirty;
    [JsonIgnore]
    public bool IsDirty
    {
        get => _isDirty;
        private set { SetProperty(ref _isDirty, value, () => IsDirty); }
    }

    private volatile int _isSuspended;
    [JsonIgnore]
    public bool IsSuspended => _isSuspended != 0;

    private volatile int _isDirtySuspended;

    private bool IsDirtySuspended => _isDirtySuspended != 0;

    #endregion

    #region Methods

    protected virtual bool IsValidPropertyValue<T>(string propertyName, T value)
    {
        return true;
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (!IsInitialized || IsDirtySuspended || 
            propertyName is nameof(IsInitialized) or nameof(IsDirty))
        {
            return;
        }
        IsDirty = true;
    }

    public void ResetDirty()
    {
        IsDirty = false;
    }

    public void ResumeChanges()
    {
        bool isSuspended = IsSuspended;
        Interlocked.Decrement(ref _isSuspended);
        if (isSuspended != IsSuspended)
        {
            RaisePropertyChanged(nameof(IsSuspended));
        }
    }

    protected override bool SetPropertyCore<T>(string propertyName, T value, out T oldValue)
    {
        if (IsSuspended || !IsValidPropertyValue(propertyName, value))
        {
            oldValue = default!;
            return false;
        }
        if (!base.SetPropertyCore(propertyName, value, out oldValue)) return false;
        OnPropertyChanged(propertyName);
        return true;
    }

    public void SuspendChanges()
    {
        bool isSuspended = IsSuspended;
        Interlocked.Increment(ref _isSuspended);
        if (isSuspended != IsSuspended)
        {
            RaisePropertyChanged(nameof(IsSuspended));
        }
    }

    public IDisposable SuspendDirty()
    {
        return new DirtySuspender(this);
    }

    #endregion
}

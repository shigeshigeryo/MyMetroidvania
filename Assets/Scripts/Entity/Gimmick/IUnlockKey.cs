using System;

public interface IUnlockKey
{
    public event Action OnUnlocked;
    public void Unlock();
}

using System;

namespace MyMetroidVania.Entity.Gimmick
{
    public interface IUnlockKey
    {
        public event Action OnUnlocked;
        public void Unlock();
    }
}

using System;


namespace Unity.DebugWatch
{

    public interface IVisualizer
    {
        IWatch GetWatch();
        void Update();
        //void Update(UnityEditor.Handles h);
        void SetHighlighted(bool on);
        void Dispose();
    }

    [Serializable]
    public abstract class Visualizer<TValue> : IVisualizer
    {
        public IWatch Watch;
        public IAccessor<TValue> Accessor;
        public bool Highlighted;

        public abstract void Update();
        public IWatch GetWatch()
        {
            return Watch;
        }
        public virtual void Dispose()
        {

        }

        public void SetHighlighted(bool on)
        {
            Highlighted = on;
        }
    }

    public interface IVisualizerFactory
    {
        string GetName();
        string GetDescription();
        IVisualizer Create(IWatch w, IAccessor a);
        void Dispose(IVisualizer v);
    }
}
namespace Game.Panels.Funnel
{
    public class Bottle
    {
        private readonly int _capacity;
        private int  _filled;
        private bool _tainted;

        public bool IsFull    => _filled >= _capacity;
        public bool IsTainted => _tainted;
        public int  FillCount => _filled;

        public Bottle(int capacity) => _capacity = capacity;

        public void AddCube(bool isPure)
        {
            if (IsFull) return;
            _filled++;
            if (!isPure) _tainted = true;
        }
    }
}

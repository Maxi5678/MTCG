

namespace Models
{
    public class PredictableRandom : Random
    {
        private readonly int _fixedResult;

        public PredictableRandom(int fixedResult)
        {
            _fixedResult = fixedResult;
        }

        public override int Next(int maxValue)
        {
            // Ignoriert maxValue und gibt immer einen festen Wert zurück, um vorhersehbares Verhalten zu erzeugen
            return _fixedResult;
        }
    }
}

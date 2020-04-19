
public class NeuralDataSet
{
    public double[] values { get; set; }
    public double[] targets { get; set; }

    public NeuralDataSet(double[] _values, double[] _targets)
    {
        values = _values;
        targets = _targets;
    }
}

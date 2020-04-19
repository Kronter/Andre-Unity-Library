using System;

public static class ActivationFunctions 
{
    //activation functions and their corrosponding derivatives
    public static double Sigmoid(double x)
    {
        double k = Math.Exp(x);
        return k / (1.0f + k);
    }
    public static double Tanh(double x)
    {
        return Math.Tanh(x);
    }
    public static double Relu(double x)
    {
        return (0 >= x) ? 0 : x;
    }
    public static double LeakyRelu(double x)
    {
        return (0 >= x) ? 0.01f * x : x;
    }
    public static double SigmoidDer(double x)
    {
        return x * (1 - x);
    }
    public static double TanhDer(double x)
    {
        return 1 - (x * x);
    }
    public static double ReluDer(double x)
    {
        return (0 >= x) ? 0 : 1;
    }
    public static double LeakyReluDer(double x)
    {
        return (0 >= x) ? 0.01f : 1;
    }
}

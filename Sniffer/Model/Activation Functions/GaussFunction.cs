using System;

namespace Sniffer.Model
{
	/// <summary>
	/// Gaussian activation function
	/// </summary>
	///
	/// <remarks>The class represents gaussian activation function with
	/// the next expression:<br />
	/// <code>									 (x - μ)^2
	///                1				   - (--------------)
	/// f(x) = -------------------- * exp ^	    2 * sigma^2
	///        sigma * sqrt(2 * pi)
	///													(x - μ)^2
	///                    x - μ			       - (-------------)
	/// f'(x) = - ------------------------ * exp ^	   2 * sigma^2
	///            sigma^3 * sqrt(2 * pi)
	/// </code>
	/// Output range of the function: <b>[0, 1]</b><br /><br />
	/// </remarks>
	public class GaussFunction : IActivationFunction
    {
		private double math_expect;
		public double Math_expect
		{
			get { return math_expect; }
			set { math_expect = value; }
		}

		private double dispersion;
		public double Dispersion
		{
			get { return dispersion; }
			set { dispersion = value; }
		}
		public GaussFunction() { }

		public GaussFunction(double math_expect, double dispersion)
		{
			this.math_expect = math_expect;
			this.dispersion = dispersion;
		}
		public double Function(double x)
		{
			return (1 / (Math.Sqrt(2 * Math.PI * dispersion)) * Math.Exp(-Math.Pow((x - math_expect), 2) / (2 * Math.Pow(dispersion, 2))));
		}
		public double Derivative(double x)
        {
            return ((x - math_expect) / (Math.Sqrt(2 * Math.PI) * Math.Pow(Math.Sqrt(dispersion), 3)) * Math.Exp(-Math.Pow((x - math_expect), 2) / (2 * Math.Pow(dispersion, 2))));
        }

    }
}

namespace Sniffer.Model
{
	public interface IActivationFunction
	{
		/// <summary>
		/// Calculates function value
		/// </summary>
		///
		/// <param name="x">Function input value</param>
		/// 
		/// <returns>Function output value, <i>f(x)</i></returns>
		///
		/// <remarks>The method calculates function value at point <b>x</b>.</remarks>
		///
		double Function(double x);

		/// <summary>
		/// Calculates function derivative
		/// </summary>
		/// 
		/// <param name="x">Function input value</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i></returns>
		/// 
		/// <remarks>The method calculates function derivative at point <b>x</b>.</remarks>
		///
		double Derivative(double x);
	}
}

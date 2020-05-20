using System;

namespace Sniffer.Model
{
	public class ActivationNeuron : Neuron
	{
		// Threshold value 
		// The value is added to inputs weighted sum
		protected double threshold = 0.0f;

		// Activation function
		// The function is applied to inputs weighted sum plus threshold value.
		protected IActivationFunction function = null;

		// Threshold value
		// The value is added to inputs weighted sum. 
		public double Threshold
		{
			get { return threshold; }
			set { threshold = value; }
		}

		// Neuron's activation function
		public IActivationFunction ActivationFunction
		{
			get { return function; }
		}

		/// Initializes a new instance of the <see cref="ActivationNeuron"/> class
		/// <param name="inputs">Neuron's inputs count</param>
		/// <param name="function">Neuron's activation function</param>
		public ActivationNeuron(int inputs, IActivationFunction function) : base(inputs)
		{
			this.function = function;
		}

		// Randomize neuron 
		/// Calls base class <see cref="Neuron.Randomize">Randomize</see> method
		/// to randomize neuron's weights and then randomize threshold's value.
		public override void Randomize()
		{
			// randomize weights
			base.Randomize();
			// randomize threshold
			threshold = rand.NextDouble() * (randRange.Length) + randRange.Min;
		}

		/// <summary>
		/// Computes output value of neuron
		/// </summary>
		/// 
		/// <param name="input">Input vector</param>
		/// 
		/// <returns>Returns neuron's output value</returns>
		/// 
		/// <remarks>The output value of activation neuron is equal to value
		/// of neuron's activation function, which parameter is weighted sum
		/// of its inputs plus threshold value. The output value is also stored
		/// in <see cref="Neuron.Output">Output</see> property.</remarks>
		/// 
		public override double Compute(double[] input)
		{
			// check for corrent input vector
			if (input.Length != inputsCount)
				throw new ArgumentException();

			// initial sum value
			double sum = 0.0;

			// compute weighted sum of inputs
			for (int i = 0; i < inputsCount; i++)
			{
				sum += weights[i] * input[i];
			}
			sum += threshold;

			return (output = function.Function(sum));
		}
	}
}
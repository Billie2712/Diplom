using Sniffer.Model;
using System;

namespace Sniffer.Model
{
	public abstract class Neuron
	{
		// Neuron's inputs count
		protected int inputsCount = 0;

		// Neuron's weights
		protected double[] weights = null;

		// Neuron's output value
		protected double output = 0;

		// Random number generator
		// The generator is used for neuron's weights randomization
		protected static Random rand = new Random((int)DateTime.Now.Ticks);

		// Sets the range of random generator. Affects initial values of neuron's weight.
		protected static DoubleRange randRange = new DoubleRange(-1.0, 1.0);

		// Random number generator
		// The property allows to initialize random generator with a custom seed. The generator is used for neuron's weights randomization.
		public static Random RandGenerator
		{
			get { return rand; }
			set
			{
				if (value != null)
				{
					rand = value;
				}
			}
		}

		// Random generator range
		public static DoubleRange RandRange
		{
			get { return randRange; }
			set
			{
				if (value != null)
				{
					randRange = value;
				}
			}
		}

		// Neuron's inputs count
		public int InputsCount
		{
			get { return inputsCount; }
		}

		// Neuron's output value
		// The calculation way of neuron's output value is determined by inherited class.
		public double Output
		{
			get { return output; }
		}


		// Neuron's weights accessor
		/// <param name="index">Weight index</param>
		// Allows to access neuron's weights.
		public double this[int index]
		{
			get { return weights[index]; }
			set { weights[index] = value; }
		}

		public Neuron(int inputs)
		{
			// allocate weights
			inputsCount = Math.Max(1, inputs);
			weights = new double[inputsCount];
			// randomize the neuron
			Randomize();
		}

		// Randomize neuron 
		/// Initialize neuron's weights with random values within the range specified by <see cref="RandRange"/>
		public virtual void Randomize()
		{
			double d = randRange.Length;

			// randomize weights
			for (int i = 0; i < inputsCount; i++)
				weights[i] = rand.NextDouble() * d + randRange.Min;
		}

		// Computes output value of neuron
		/// <param name="input">Input vector</param>
		// Returns neuron's output value
		/// The output value is also stored in <see cref="Output"/> property.</remarks>
		public abstract double Compute(double[] input);
	}
}

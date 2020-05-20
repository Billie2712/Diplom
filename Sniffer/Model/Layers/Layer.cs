using System;

namespace Sniffer.Model
{
	public abstract class Layer
	{
		// Layer's inputs count
		protected int inputsCount = 0;

		// Layer's neurons count
		protected int neuronsCount = 0;

		// Layer's neurons
		protected Neuron[] neurons;

		// Layer's output vector
		protected double[] output;

		// Layer's inputs count
		public int InputsCount
		{
			get { return inputsCount; }
		}

		// Layer's neurons count
		public int NeuronsCount
		{
			get { return neuronsCount; }
		}

		// Layer's output vector
		// The calculation way of layer's output vector is determined by inherited class
		public double[] Output
		{
			get { return output; }
		}

		// Layer's neurons accessor
		/// <param name="index">Neuron index</param>
		// Allows to access layer's neurons.
		public Neuron this[int index]
		{
			get { return neurons[index]; }
		}

		/// Initializes a new instance of the <see cref="Layer"/> class
		/// <param name="neuronsCount">Layer's neurons count</param>
		/// <param name="inputsCount">Layer's inputs count</param>
		/// 
		/// <remarks>Protected contructor, which initializes <see cref="inputsCount"/>,
		/// <see cref="neuronsCount"/>, <see cref="neurons"/> and <see cref="output"/>
		/// members.</remarks>
		/// 
		protected Layer(int neuronsCount, int inputsCount)
		{
			this.inputsCount = Math.Max(1, inputsCount);
			this.neuronsCount = Math.Max(1, neuronsCount);
			// create collection of neurons
			neurons = new Neuron[this.neuronsCount];
			// allocate output array
			output = new double[this.neuronsCount];
		}


		// Compute output vector of the layer 
		/// <param name="input">Input vector</param>
		/// <returns> Returns layer's output vector </returns>
		/// 
		/// <remarks>The actual layer's output vector is determined by inherited class and it
		/// consists of output values of layer's neurons. The output vector is also stored in
		/// <see cref="Output"/> property.</remarks>
		/// 
		public virtual double[] Compute(double[] input)
		{
			// compute each neuron
			for (int i = 0; i < neuronsCount; i++)
				output[i] = neurons[i].Compute(input);

			return output;
		}


		// Randomize neurons of the layer
		/// <remarks> Randomizes layer's neurons by calling <see cref="Neuron.Randomize"/> method
		/// of each neuron. </remarks>
		/// 
		public virtual void Randomize()
		{
			foreach (Neuron neuron in neurons)
				neuron.Randomize();
		}
	}
}
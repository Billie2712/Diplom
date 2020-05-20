using System;

namespace Sniffer.Model
{
	// Base neural network class
	/// <remarks>This is a base neural netwok class, which represents
	/// collection of neuron's layers.</remarks>
	/// 
	public abstract class NeuralNetwork
	{
		// Network's inputs count
		protected int inputsCount;

		// Network's layers count
		protected int layersCount;

		// Network's layers
		protected Layer[] layers;

		// Network's output vector
		protected double[] output;

		// Network's inputs count
		public int InputsCount
		{
			get { return inputsCount; }
		}

		// Network's layers count
		public int LayersCount
		{
			get { return layersCount; }
		}

		// Network's output vector
		// The calculation way of network's output vector is determined by inherited class. 
		public double[] Output
		{
			get { return output; }
		}

		// Network's layers accessor
		/// <param name="index">Layer index</param>
		// Allows to access network's layer.
		public Layer this[int index]
		{
			get { return layers[index]; }
		}


		/// Initializes a new instance of the <see cref="Network"/> class
		/// <param name="inputsCount">Network's inputs count</param>
		/// <param name="layersCount">Network's layers count</param>
		/// 
		/// <remarks>Protected constructor, which initializes <see cref="inputsCount"/>,
		/// <see cref="layersCount"/> and <see cref="layers"/> members.</remarks>
		/// 
		protected NeuralNetwork(int inputsCount, int layersCount)
		{
			this.inputsCount = Math.Max(1, inputsCount);
			this.layersCount = Math.Max(1, layersCount);
			// create collection of layers
			layers = new Layer[this.layersCount];
		}

		// Compute output vector of the network
		/// <param name="input">Input vector</param>
		/// 
		/// <returns> Returns network's output vector </returns>
		/// 
		/// <remarks>The actual network's output vecor is determined by inherited class and it
		/// represents an output vector of the last layer of the network. The output vector is
		/// also stored in <see cref="Output"/> property.</remarks>
		/// 
		public virtual double[] Compute(double[] input)
		{
			output = input;

			// compute each layer
			foreach (Layer layer in layers)
			{
				output = layer.Compute(output);
			}

			return output;
		}

		// Randomize layers of the network
		/// <remarks> Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
		/// of each layer. </remarks>
		/// 
		public virtual void Randomize()
		{
			foreach (Layer layer in layers)
			{
				layer.Randomize();
			}
		}
	}
}